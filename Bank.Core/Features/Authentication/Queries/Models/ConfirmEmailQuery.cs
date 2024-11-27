using Bank.Core.Bases;
using MediatR;

namespace Bank.Core.Features.Authentication.Queries.Models
{
    public class ConfirmEmailQuery : IRequest<Response<string>>
    {
        public string? Email { get; set; }
        public string? code { get; set; }
    }
}
