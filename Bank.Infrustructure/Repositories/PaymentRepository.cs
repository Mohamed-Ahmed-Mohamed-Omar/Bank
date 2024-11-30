using Bank.Data.Entities;
using Bank.Infrustructure.Abstracts;
using Bank.Infrustructure.Context;

namespace Bank.Infrustructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountRepository _accountRepository;

        public PaymentRepository(ApplicationDbContext context, IAccountRepository accountRepository)
        {
            _context = context;
            _accountRepository = accountRepository;
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

                // Retrieve the receiver's account using the account number
                var receiverAccount = await _accountRepository.GetAccountByAccountNumberAsync(payment.ReceiverAccountId.ToString());
                if (receiverAccount == null)
                {
                    throw new InvalidOperationException("Receiver's account not found.");
                }

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

        public Task<IQueryable<Payment>> GetAllPaymentsAsync(string? username)
        {
            throw new NotImplementedException();
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
    }
}
