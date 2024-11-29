using Bank.Services.Abstracts;
using Bank.Services.AuthServices.Implementations;
using Bank.Services.AuthServices.Interfaces;
using Bank.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Services
{
    public static class ModuleServiceDependencies
    {
        public static IServiceCollection AdddServiceDependencies(this IServiceCollection services)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddTransient<ICurrentUserService, CurrentUserService>();

            services.AddTransient<IAuthenticationService, AuthenticationService>();

            services.AddTransient<IEmailsService, EmailsService>();

            services.AddTransient<IFileService, FileService>();

            services.AddTransient<IAdminService, AdminService>();

            services.AddTransient<IAccountServices, AccountServices>();

            return services;
        }
    }
}
