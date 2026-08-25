using CoConnect.Infrastructure;
using CoConnect.Infrastructure.Auth;
using CoConnect.Infrastructure.QueryRouting;
using CoConnect.Infrastructure.Queue;
using CoConnect.Infrastructure.Service;
using CoConnect.Messaging;
using CoConnect.Messaging.Contacts.Handlers;
using CoConnect.Messaging.Users.Handlers;
using CoConnect.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SimpleBus;

namespace CoConnect
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();

            builder.Services.AddDbContextFactory<UnitOfWorkInMemory>(options => options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));
            builder.Services.AddSingleton<IQueryContextFactory, QueryContextFactory<UnitOfWorkInMemory>>();
            builder.Services.AddSingleton<IDataContextFactory, DataContextFactory<UnitOfWorkInMemory>>();
            builder.Services.AddSingleton<QueryRouteRegistry>();
            builder.Services.AddSingleton<QueryRouteExecutor>();
            builder.Services.AddSingleton<CookiePrincipalValidator>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/Login";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnValidatePrincipal = async context =>
                        {
                            var validator = context.HttpContext.RequestServices.GetRequiredService<CookiePrincipalValidator>();
                            await validator.ValidateAsync(context);
                        }
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddSingleton<MessageQueue>();
            builder.Services.AddHostedService<MessageQueueWorker>();

            builder.Services.AddSingleton<IServiceBus, ServiceBus>();
            builder.Services.AddSingleton<IServiceContext, ServiceBus>();
            builder.Services.AddSingleton<IServiceProcessor, ServiceProcessor>();

            builder.Services.AddSingleton<IMessageHandler, ContactCreateHandler>();
            builder.Services.AddSingleton<IMessageHandler, ContactCreatedHandler>();
            builder.Services.AddSingleton<IMessageHandler, ContactUpdateHandler>();
            builder.Services.AddSingleton<IMessageHandler, ContactUpdatedHandler>();
            builder.Services.AddSingleton<IMessageHandler, ContactDeleteHandler>();
            builder.Services.AddSingleton<IMessageHandler, ContactDeletedHandler>();

            builder.Services.AddSingleton<IMessageHandler, UserCreateHandler>();
            builder.Services.AddSingleton<IMessageHandler, UserCreatedHandler>();
            builder.Services.AddSingleton<IMessageHandler, UserUpdateHandler>();
            builder.Services.AddSingleton<IMessageHandler, UserUpdatedHandler>();
            builder.Services.AddSingleton<IMessageHandler, UserDisableHandler>();
            builder.Services.AddSingleton<IMessageHandler, UserDisabledHandler>();
            builder.Services.AddSingleton<IMessageHandler, UserDeleteHandler>();
            builder.Services.AddSingleton<IMessageHandler, UserDeletedHandler>();

            builder.Services.AddSingleton<INotificationDispatcher, SignalRNotificationDispatcher>();

            var app = builder.Build();

            var serviceScopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                var dbContextFactory = serviceScope.ServiceProvider.GetRequiredService<IDbContextFactory<UnitOfWorkInMemory>>();
                using var dbContext = dbContextFactory.CreateDbContext();
                dbContext.Database.EnsureCreated();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseMiddleware<QueryMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ServiceBusMiddleware>();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapHub<MessageHub>("/messagehub");

            app.Run();
        }
    }
}
