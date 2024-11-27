using Bank.Infrustructure.Abstracts;
using Bank.Infrustructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Infrustructure
{
    public static class ModuleInfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddTransient<IPaymentRepository, PaymentRepository>();

            services.AddTransient<IAccountRepository, AccountRepository>();

            services.AddTransient<IMessageRepository, MessageRepository>();

            services.AddTransient<IAdminRepository, AdminRepository>();

            return services;
        }
    }
}
