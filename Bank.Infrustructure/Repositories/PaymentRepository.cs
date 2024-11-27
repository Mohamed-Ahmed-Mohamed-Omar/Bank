using Bank.Data.Entities;
using Bank.InfrastructureBases;
using Bank.Infrustructure.Abstracts;
using Bank.Infrustructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrustructure.Repositories
{
    public class PaymentRepository : GenericRepositoryAsync<Payment>, IPaymentRepository
    {
        private DbSet<Payment> _paymentSet;

        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
            _paymentSet = context.Set<Payment>();
        }
    }
}
