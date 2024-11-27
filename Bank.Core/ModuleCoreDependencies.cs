using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bank.Core
{
    public static class ModuleCoreDependencies
    {
        public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
        {
            //Configuration Of Mediator
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            //Configuration Of Automapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            // Get Validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
