using Bank.Core.Features.Payments.Queries.Models;
using Bank.Core.Features.Payments.Queries.Results;
using Bank.Core.Wrappers;
using Bank.Data.Entities;
using Bank.Services.Abstracts;
using Bank.Services.AuthServices.Interfaces;
using MediatR;

namespace Bank.Core.Features.Payments.Queries.Handlers
{
    public class PaymentQueryHandler : IRequestHandler<GetAllPaymentsByUsernameQuery, PaginatedResult<GetAllPaymentsByUsernameResult>>,
                                       IRequestHandler<GetAllPaymentsQuery, PaginatedResult<GetAllPaymentsResult>>,
                                       IRequestHandler<GetAllsTransfersByUsernameQuery, PaginatedResult<GetAllsTransfersByUsernameResult>>,
                                       IRequestHandler<GetAllsTransfersQuery, PaginatedResult<GetAllsTransfersResult>>
    {
        private readonly IPaymentServices _paymentServices;
        private readonly ICurrentUserService _currentUserService;

        public PaymentQueryHandler(IPaymentServices paymentServices, ICurrentUserService currentUserService)
        {
            _paymentServices = paymentServices;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedResult<GetAllPaymentsByUsernameResult>> Handle(GetAllPaymentsByUsernameQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch Current User
                var user =  _currentUserService.GetUserNameAsync();

                // Fetch Payments By Username
                var payments = await _paymentServices.GetAllPaymentsByUsernameAsync(user);

                // Map payments to GetAllPaymentsByUsernameResult
                var mappedPayments = payments.Select(payment =>
                {
                    try
                    {
                        // Get the Status as int and determine the description
                        byte paymentStatus = payment.GetType().GetProperty("Status")?.GetValue(payment) as byte? ?? 0;
                        string statusDescription = paymentStatus == 1 ? "Completed" : "Failed"; // Convert status to string descriptio

                        return new GetAllPaymentsByUsernameResult
                        {
                            Amount = (decimal)payment.GetType().GetProperty("Amount")?.GetValue(payment),
                            Description = (string)payment.GetType().GetProperty("Description")?.GetValue(payment),
                            PaymentDate = (DateTime)payment.GetType().GetProperty("PaymentDate")?.GetValue(payment),
                            PaymentMethod = (string)payment.GetType().GetProperty("PaymentMethod")?.GetValue(payment),
                            PaymentType = (string)payment.GetType().GetProperty("PaymentType")?.GetValue(payment),
                            ReferenceNumber = (int)payment.GetType().GetProperty("ReferenceNumber")?.GetValue(payment),
                            Status = statusDescription
                        };
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error during payment mapping", ex); // Catch and throw specific exception
                    }
                }).ToList();  // Since payments is a List<object>, it's now properly mapped

                // Apply pagination
                IEnumerable<GetAllPaymentsByUsernameResult> usersQuery;

                // Check if we should use IQueryable or List based on the source
                if (mappedPayments is IQueryable<GetAllPaymentsByUsernameResult>)
                {
                    usersQuery = mappedPayments.AsQueryable(); // Use AsQueryable if it's IQueryable
                }
                else
                {
                    usersQuery = mappedPayments; // Use the List directly if it's a List
                }

                // Return paginated result
                return await usersQuery.ToPaginatedListAsync(request.PageNumber, request.PageSize);
            }
            catch (Exception ex)
            {
                // Handle exception and return error message
                throw new Exception($"An error occurred while fetching payments: {ex.Message}");
            }
        }

        public async Task<PaginatedResult<GetAllPaymentsResult>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch all payments
                var payments = await _paymentServices.GetAllPaymentsAsync();

                // Map payments to GetAllPaymentsResult
                var mappedPayments = payments.Select(payment =>
                {
                    try
                    {
                        // Get the Status as int and determine the description
                        byte paymentStatus = payment.GetType().GetProperty("Status")?.GetValue(payment) as byte? ?? 0;
                        string statusDescription = paymentStatus == 1 ? "Completed" : "Failed"; // Convert status to string description

                        return new GetAllPaymentsResult
                        {
                            Id = payment.GetType().GetProperty("Id")?.GetValue(payment) as int? ?? 0, // Safely handle nulls
                            Username = payment.GetType().GetProperty("UserName")?.GetValue(payment) as string,
                            Amount = payment.GetType().GetProperty("Amount")?.GetValue(payment) as decimal? ?? 0m,
                            Description = payment.GetType().GetProperty("Description")?.GetValue(payment) as string,
                            PaymentDate = payment.GetType().GetProperty("PaymentDate")?.GetValue(payment) as DateTime? ?? DateTime.MinValue,
                            PaymentMethod = payment.GetType().GetProperty("PaymentMethod")?.GetValue(payment) as string,
                            PaymentType = payment.GetType().GetProperty("PaymentType")?.GetValue(payment) as string,
                            Status = statusDescription,
                            ReferenceNumber = payment.GetType().GetProperty("ReferenceNumber")?.GetValue(payment) as int? ?? 0,
                            Email = payment.GetType().GetProperty("Email")?.GetValue(payment) as string // Assuming Email is part of the mapped object
                        };
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error during payment mapping", ex); // Catch and throw specific exception
                    }
                }).ToList();

                // Apply pagination
                IEnumerable<GetAllPaymentsResult> usersQuery;

                // Check if we should use IQueryable or List based on the source
                if (mappedPayments is IQueryable<GetAllPaymentsResult>)
                {
                    usersQuery = mappedPayments.AsQueryable(); // Use AsQueryable if it's IQueryable
                }
                else
                {
                    usersQuery = mappedPayments; // Use the List directly if it's a List
                }

                return await usersQuery.ToPaginatedListAsync(request.PageNumber, request.PageSize);
            }
            catch (Exception ex)
            {
                // Handle exception and return error message
                throw new Exception($"An error occurred while fetching payments: {ex.Message}");
            }
        }

        public async Task<PaginatedResult<GetAllsTransfersByUsernameResult>> Handle(GetAllsTransfersByUsernameQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch Current User
                var user = _currentUserService.GetUserNameAsync();

                // Fetch Payments By Username
                var payments = await _paymentServices.GetAllsTransfersByUsernameAsync(user);

                // Map accounts to GetAccountsPaginationReponse
                var mappedpayments = payments.Select(payment => 
                {
                    try
                    {
                        // Get the Status as int and determine the description
                        byte paymentStatus = payments.GetType().GetProperty("Status")?.GetValue(payment) as byte? ?? 0;
                        string statusDescription = paymentStatus == 1 ? "Completed" : "Failed"; // Convert status to string description

                        return new GetAllsTransfersByUsernameResult
                        {
                            Amount = (decimal)payment.GetType().GetProperty("Amount").GetValue(payment),
                            Description = (string)payment.GetType().GetProperty("Description").GetValue(payment),
                            PaymentDate = (DateTime)payment.GetType().GetProperty("PaymentDate").GetValue(payment),
                            PaymentMethod = (string)payment.GetType().GetProperty("PaymentMethod").GetValue(payment),
                            PaymentType = "Transfer",
                            ReceiverAccountId = (int)payment.GetType().GetProperty("ReceiverAccountId").GetValue(payment),
                            ReferenceNumber = (int)payment.GetType().GetProperty("ReferenceNumber").GetValue(payment),
                            ReceiverUsername = (string)payment.GetType().GetProperty("ReceiverUsername").GetValue(payment),
                            Status = statusDescription
                        };
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error during payment mapping", ex); // Catch and throw specific exception
                    }
                }).ToList();

                // Apply pagination
                IEnumerable<GetAllsTransfersByUsernameResult> usersQuery;

                // Check if we should use IQueryable or List based on the source
                if (mappedpayments is IQueryable<GetAllsTransfersByUsernameResult>)
                {
                    usersQuery = mappedpayments.AsQueryable(); // Use AsQueryable if it's IQueryable
                }
                else
                {
                    usersQuery = mappedpayments; // Use the List directly if it's a List
                }

                // Return paginated result
                return await usersQuery.ToPaginatedListAsync(request.PageNumber, request.PageSize);
            }
            catch (Exception ex)
            {
                // Handle exception and return error message
                throw new Exception($"An error occurred while fetching payments: {ex.Message}");
            }
        }

        public async Task<PaginatedResult<GetAllsTransfersResult>> Handle(GetAllsTransfersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch Payments By Username
                var payments = await _paymentServices.GetAllsTransfersAsync();

                // Map accounts to GetAccountsPaginationReponse
                var mappedPayments = payments.Select(payment =>
                {
                    try
                    {
                        // Get the Status as int and determine the description
                        byte paymentStatus = payment.GetType().GetProperty("Status")?.GetValue(payment) as byte? ?? 0;
                        string statusDescription = paymentStatus == 1 ? "Completed" : "Failed"; // Convert status to string description

                        return new GetAllsTransfersResult
                        {
                            Amount = (decimal)payment.GetType().GetProperty("Amount").GetValue(payment),
                            Description = (string)payment.GetType().GetProperty("Description").GetValue(payment),
                            PaymentDate = (DateTime)payment.GetType().GetProperty("PaymentDate").GetValue(payment),
                            PaymentMethod = (string)payment.GetType().GetProperty("PaymentMethod").GetValue(payment),
                            PaymentType = "Transfer",
                            UsernameSender = (string)payment.GetType().GetProperty("UsernameSender").GetValue(payment),
                            ReferenceNumber = (int)payment.GetType().GetProperty("ReferenceNumber").GetValue(payment),
                            ReceiverUsername = (string)payment.GetType().GetProperty("UsernameReceiver").GetValue(payment),
                            Status = statusDescription,
                            AccountNumberSender = (int)payment.GetType().GetProperty("AccountNumberSender").GetValue(payment),
                            AccountNumberReceiver = (int)payment.GetType().GetProperty("AccountNumberReceiver").GetValue(payment)
                        };
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error during payment mapping", ex); // Catch and throw specific exception
                    }
                }).ToList();

                // Apply pagination
                var paginatedResult = await mappedPayments.ToPaginatedListAsync(request.PageNumber, request.PageSize);

                return paginatedResult;
            }
            catch (Exception ex)
            {
                // Handle exception and return error message
                throw new Exception($"An error occurred while fetching payments: {ex.Message}", ex);
            }
        }
    }
}
