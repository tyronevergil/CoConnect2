using CoConnect.Domain;
using CoConnect.Domain.Handlers;
using CoConnect.Infrastructure;
using CoConnect.Infrastructure.Queue;
using CoConnect.Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using SimpleBus;
using Persistence;

namespace CoConnect
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();

            builder.Services.AddDbContextFactory<UnitOfWorkInMemory>(options => options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));
            builder.Services.AddSingleton<IQueryContextFactory, QueryContextFactory<UnitOfWorkInMemory>>();
            builder.Services.AddSingleton<IDataContextFactory, DataContextFactory<UnitOfWorkInMemory>>();

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

            builder.Services.AddSingleton<INotificationDispatcher, SignalRNotificationDispatcher>();

            var app = builder.Build();

            var serviceScopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                var dbContextFactory = serviceScope.ServiceProvider.GetRequiredService<IDbContextFactory<UnitOfWorkInMemory>>();
                using (var dbContext = dbContextFactory.CreateDbContext())
                {
                    dbContext.Database.EnsureCreated();
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

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
