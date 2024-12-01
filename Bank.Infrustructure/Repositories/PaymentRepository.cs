using Bank.Data.Entities;
using Bank.Data.Entities.Identity;
using Bank.Infrustructure.Abstracts;
using Bank.Infrustructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection;

namespace Bank.Infrustructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentRepository(ApplicationDbContext context, IAccountRepository accountRepository, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _accountRepository = accountRepository;
            _userManager = userManager;
        }

        public async Task<Payment> PaymentAsync(Payment payment, string username)
        {
            // Start a transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Retrieve the account based on the username
                var account = await _accountRepository.GetAccountByUsernameAsync(username);

                // Check if the account exists
                if (account == null)
                {
                    throw new InvalidOperationException("Account not found.");
                }

                // Check if the account has enough balance to make the payment
                if (account.Balance < payment.Amount)
                {
                    throw new InvalidOperationException("Insufficient balance to complete the payment.");
                }

                // Deduct the payment amount from the account balance
                account.Balance -= payment.Amount;

                // Create the payment record
                payment.AccountId = account.Id;
                payment.PaymentDate = DateTime.UtcNow;
                payment.ReferenceNumber = GenerateRandomNumber();
                payment.Status = 1; // Mark as completed

                // Add the payment to the database
                _context.payments.Add(payment);

                // Save changes to the database (commit the payment addition)
                await _context.SaveChangesAsync();

                // Update the account's balance
                _context.accounts.Update(account);

                // Save changes to the database (commit the balance update)
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                return payment;
            }
            catch (Exception ex)
            {
                // In case of any exception, roll back the transaction to ensure consistency
                await transaction.RollbackAsync();

                // Re-throw the exception to be handled by the caller
                throw new InvalidOperationException($"An error occurred while processing the payment: {ex.Message}", ex);
            }
        }

        public async Task<Payment> TransferAsync(Payment payment, string username)
        {
            // Begin a database transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Retrieve the sender's account using the username
                var senderAccount = await _accountRepository.GetAccountByUsernameAsync(username);
                if (senderAccount == null)
                {
                    throw new InvalidOperationException("Sender's account not found.");
                }

                // Validate ReceiverAccountId
                if (payment.ReceiverAccountId == null)
                {
                    throw new ArgumentException("Receiver's account ID cannot be null.");
                }

                //Retrieve the receiver's account using the account number
                var receiverAccount = await _accountRepository.GetAccountByAccountNumberAsync(payment.ReceiverAccountId.Value);

                // Validate the transfer amount
                if (payment.Amount <= 0)
                {
                    throw new ArgumentException("Transfer amount must be greater than zero.");
                }

                // Check sender's balance
                if (senderAccount.Balance < payment.Amount)
                {
                    throw new InvalidOperationException("Insufficient balance for this transfer.");
                }

                // Update balances
                senderAccount.Balance -= payment.Amount;
                receiverAccount.Balance += payment.Amount;

                // Create a payment record
                payment.AccountId = senderAccount.Id;
                payment.PaymentDate = DateTime.UtcNow;
                payment.Status = 1; // Mark as completed
                payment.ReferenceNumber = GenerateRandomNumber(); // Generate reference number
                payment.PaymentType = "Transfer";

                // Save payment and update accounts
                _context.payments.Add(payment);
                _context.accounts.Update(senderAccount);
                _context.accounts.Update(receiverAccount);

                // Save changes
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return payment;
            }
            catch (Exception ex)
            {
                // Rollback transaction in case of errors
                await transaction.RollbackAsync();
                throw new InvalidOperationException($"Transfer failed: {ex.Message}", ex);
            }
        }

        public async Task<List<object>> GetAllPaymentsByUsernameAsync(string username)
        {
            var data = await _context.payments
                .Where(e => e.Account.UserName == username && e.Status != null && e.ReceiverAccountId == null)
                .Select(s => new
                {
                    Amount = s.Amount,
                    Description = s.Description,
                    PaymentDate = s.PaymentDate,
                    PaymentMethod = s.PaymentMethod,
                    PaymentType = s.PaymentType,
                    ReferenceNumber = s.ReferenceNumber,
                    Status = s.Status 
                })
                .ToListAsync();

            // Returning as List<object>
            return data.Cast<object>().ToList();
        }

        public async Task<List<object>> GetAllsTransfersByUsernameAsync(string username)
        {
            // Fetch data with LINQ query using a join to get receiver account details
            var data = await (from payment in _context.payments
                              join receiverAccount in _context.accounts
                              on payment.ReceiverAccountId equals receiverAccount.Id
                              where payment.Account.UserName == username && payment.ReceiverAccountId != null
                              select new
                              {
                                  // Conversion data
                                  Amount = payment.Amount,
                                  Description = payment.Description,
                                  ReferenceNumber = payment.ReferenceNumber,
                                  Status = payment.Status,
                                  PaymentDate = payment.PaymentDate,
                                  PaymentMethod = payment.PaymentMethod,

                                  // Receiver data
                                  ReceiverAccountId = receiverAccount.AccountNumber,
                                  ReceiverUsername = receiverAccount.UserName
                              }).ToListAsync();

            // Prepare the result list
            var result = new List<object>();

            foreach (var item in data)
            {
                // Determine status description
                string statusDescription = item.Status == 1 ? "Completed" : "Failed";

                // Add the result to the list
                result.Add(new
                {
                    item.ReceiverUsername,
                    item.Amount,
                    item.Description,
                    item.ReferenceNumber,
                    item.PaymentDate,
                    item.PaymentMethod,
                    ReceiverAccountId = item.ReceiverAccountId,
                    StatusDescription = statusDescription,
                });
            }

            return result;
        }

        public async Task<List<object>> GetAllPaymentsAsync()
        {
            // Fetch the payments from the database
            var payments = await _context.payments
                .Where(s => (s != null && s.Account != null && s.Account.UserName != null) && (s.Status != null && s.ReceiverAccountId == null))
                .Select(s => new
                {
                    s.Id,
                    Username = s.Account.UserName,
                    s.Amount,
                    s.Description,
                    s.PaymentDate,
                    s.PaymentMethod,
                    s.PaymentType,
                    s.ReferenceNumber,
                    s.Status
                })
                .ToListAsync(); // Asynchronously fetch payments

            // Check if the payment list is empty
            if (payments == null || !payments.Any())
            {
                throw new Exception("No payments found.");
            }

            // Prepare a list of results
            var result = new List<object>();

            foreach (var payment in payments)
            {
                if (payment == null)
                {
                    throw new Exception("Payment is null.");
                }

                // Fetch email asynchronously based on the username
                var email = await GetEmailByUsernameAsync(payment.Username);

                // Add the result to the list
                result.Add(new
                {
                    payment.Id,
                    payment.Username,
                    payment.Amount,
                    payment.Description,
                    payment.PaymentDate,
                    payment.PaymentMethod,
                    payment.PaymentType,
                    payment.ReferenceNumber,
                    payment.Status, 
                    Email = email
                });
            }

            // Return the final result as List<object>
            return result;
        }

        public async Task<List<object>> GetAllsTransfersAsync()
        {
            // Fetch data with LINQ query
            var data = await (from payment in _context.payments
                              join receiverAccount in _context.accounts
                              on payment.ReceiverAccountId equals receiverAccount.Id
                              where payment.ReceiverAccountId != null
                              select new
                              {
                                  // Sender data
                                  UsernameSender = payment.Account.UserName,
                                  AccountNumberSender = payment.Account.AccountNumber,

                                  // Receiver data
                                  UsernameReceiver = receiverAccount.UserName,
                                  AccountNumberReceiver = receiverAccount.AccountNumber,

                                  // Conversion data
                                  Amount = payment.Amount,
                                  Description = payment.Description,
                                  PaymentMethod = payment.PaymentMethod,
                                  ReferenceNumber = payment.ReferenceNumber,
                                  Status = payment.Status,
                                  PaymentDate = payment.PaymentDate,
                              }).ToListAsync();

            // Prepare the result list
            var result = new List<object>();

            foreach (var item in data)
            {
                // Add the result to the list
                result.Add(new
                {
                    UsernameSender = item.UsernameSender,
                    AccountNumberSender = item.AccountNumberSender,
                    UsernameReceiver = item.UsernameReceiver,
                    AccountNumberReceiver = item.AccountNumberReceiver,
                    Amount = item.Amount,
                    Description = item.Description,
                    PaymentMethod = item.PaymentMethod,
                    ReferenceNumber = item.ReferenceNumber,
                    PaymentDate = item.PaymentDate,
                    Status = item.Status
                });
            }

            return result;
        }

        private static int GenerateRandomNumber()
        {
            Random random = new Random();

            // Generate 6 random digits
            string numbers = new string(Enumerable.Range(0, 6)
                .Select(_ => random.Next(0, 10).ToString()[0])
                .ToArray());

            // Shuffle the digits
            string shuffled = new string(numbers.OrderBy(_ => random.Next()).ToArray());

            return int.Parse(shuffled);
        }

        private async Task<string> GetEmailByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username cannot be null or empty.");

            // Retrieve the user by username
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                throw new ArgumentException($"User with username '{username}' does not exist.");

            // Return the email
            return user.Email;
        }
    }
}
