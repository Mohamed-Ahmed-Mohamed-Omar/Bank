using Bank.Infrustructure;
using Bank.Infrustructure.Context;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Bank.Services;
using Bank.Core;
using Serilog;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // ConnectionStrings

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        #region Dependency injections

        builder.Services.AddInfrastructureDependencies()
                        .AdddServiceDependencies()
                        .AddCoreDependencies()
                        .AddServiceRegisteration(builder.Configuration);
        #endregion

        #region AllowCORS

        var CORS = "_cors";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: CORS,
                  policy =>
                  {
                      policy.AllowAnyHeader();
                      policy.AllowAnyMethod();
                      policy.AllowAnyOrigin();
                  }
            );
        });

        #endregion

        builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        builder.Services.AddTransient(x =>
        {
            var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
            var factory = x.GetRequiredService<IUrlHelperFactory>();
            return factory.GetUrlHelper(actionContext);
        });

        //Serilog
        Log.Logger = new LoggerConfiguration()
                      .ReadFrom.Configuration(builder.Configuration).CreateLogger();
        builder.Services.AddSerilog();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseCors(CORS);

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

#region Dependency injections

#endregion
#region AllowCORS

#endregion
