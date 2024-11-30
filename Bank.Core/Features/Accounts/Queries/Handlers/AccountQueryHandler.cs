using AutoMapper;
using Bank.Core.Bases;
using Bank.Core.Features.Accounts.Queries.Models;
using Bank.Core.Features.Accounts.Queries.Results;
using Bank.Core.Wrappers;
using Bank.Services.Abstracts;
using Bank.Services.AuthServices.Interfaces;
using MediatR;

namespace Bank.Core.Features.Accounts.Queries.Handlers
{
    public class AccountQueryHandler : IRequestHandler<GetAccountsPaginationQuery, PaginatedResult<GetAccountsPaginationReponse>>,
                                       IRequestHandler<GetAccountByNameQuery, Response<GetAccountByNameResponse>>,
                                       IRequestHandler<GetAccountByIdQuery, Response<GetAccountByIdResponse>>
    {
        private readonly IAccountServices _accountServices;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public AccountQueryHandler(IAccountServices accountServices, IMapper mapper, ICurrentUserService currentUserService)
        {
            _accountServices = accountServices;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedResult<GetAccountsPaginationReponse>> Handle(GetAccountsPaginationQuery request, CancellationToken cancellationToken)
        {
            // Fetch Accounts
            var accounts = await _accountServices.GetAllAccountsAsync();

            // Map accounts to GetAccountsPaginationReponse
            var mappedAccounts = accounts.Select(account => new GetAccountsPaginationReponse
            {
                Id = (int)account.GetType().GetProperty("Id").GetValue(account),
                AccountNumber = (int)account.GetType().GetProperty("AccountNumber").GetValue(account),
                Balance = (decimal)account.GetType().GetProperty("Balance").GetValue(account),
                CreatedAt = (DateTime)account.GetType().GetProperty("CreatedAt").GetValue(account),
                UserName = (string)account.GetType().GetProperty("UserName").GetValue(account),
                Email = (string)account.GetType().GetProperty("Email").GetValue(account),
                Phone = (string)account.GetType().GetProperty("Phone").GetValue(account)
            }).AsQueryable();

            // Apply pagination
            IEnumerable<GetAccountsPaginationReponse> usersQuery;

            // Check if we should use IQueryable or List based on the source
            if (mappedAccounts is IQueryable<GetAccountsPaginationReponse>)
            {
                usersQuery = mappedAccounts.AsQueryable(); // Use AsQueryable if it's IQueryable
            }
            else
            {
                usersQuery = mappedAccounts; // Use the List directly if it's a List
            }

            // Return paginated result
            return await usersQuery.ToPaginatedListAsync(request.PageNumber, request.PageSize);
        }

        public async Task<Response<GetAccountByNameResponse>> Handle(GetAccountByNameQuery request, CancellationToken cancellationToken)
        {
            // Retrieve the username from the current user context
            var username = _currentUserService.GetUserNameAsync();

            if (string.IsNullOrEmpty(username))
            {
                return new Response<GetAccountByNameResponse>
                {
                    Success = false,
                    Message = "Failed to retrieve the username from the current context."
                };
            }

            // Fetch the account using the service
            var account = await _accountServices.GetAccountAsync(username);

            if (account == null)
            {
                return new Response<GetAccountByNameResponse>
                {
                    Success = false,
                    Message = $"Account with username '{username}' was not found."
                };
            }

            // Map the account data to the response DTO
            var response = new GetAccountByNameResponse
            {
                AccountNumber = (int)account.GetType().GetProperty("AccountNumber").GetValue(account),
                Balance = (decimal)account.GetType().GetProperty("Balance").GetValue(account),
                CreatedAt = (DateTime)account.GetType().GetProperty("CreatedAt").GetValue(account),
                Email = (string)account.GetType().GetProperty("Email").GetValue(account),
                Phone = (string)account.GetType().GetProperty("Phone").GetValue(account)
            };

            return new Response<GetAccountByNameResponse>
            {
                Success = true,
                Message = "Account retrieved successfully.",
                Data = response
            };
        }

        public async Task<Response<GetAccountByIdResponse>> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            // Fetch the account using the repository
            var account = await _accountServices.GetAccountByIdAsync(request.Id);

            if (account == null)
            {
                return new Response<GetAccountByIdResponse>
                {
                    Success = false,
                    Message = $"Account with username '{request.Id}' was not found."
                };
            }

            // Map accounts to GetAccountByIdResponse
            var mappedAccounts = new GetAccountByIdResponse
            {
                AccountNumber = (int)account.GetType().GetProperty("AccountNumber").GetValue(account),
                Balance = (decimal)account.GetType().GetProperty("Balance").GetValue(account),
                CreatedAt = (DateTime)account.GetType().GetProperty("CreatedAt").GetValue(account),
                UserName = (string)account.GetType().GetProperty("UserName").GetValue(account),
                Email = (string)account.GetType().GetProperty("Email").GetValue(account),
                Phone = (string)account.GetType().GetProperty("Phone").GetValue(account)
            };

            return new Response<GetAccountByIdResponse>
            {
                Success = true,
                Message = "Account retrieved successfully.",
                Data = mappedAccounts
            }; ;
        }
    }
}
