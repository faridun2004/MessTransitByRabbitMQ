using ImageProccessor;
using ImageProccessor.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;


var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        Console.WriteLine(hostContext.Configuration["ConnectionString1"]);
        services.AddDbContext<PhotoContext>(con => con.UseSqlServer(hostContext.Configuration["ConnectionString1"])
                       .LogTo(Console.Write, LogLevel.Error)
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        // Configure MassTransit
        services.AddMassTransit(cfg =>
        {
            cfg.AddConsumer<ImageAddedConsumers>();

            cfg.UsingRabbitMq((context, rabbitMqConfig) =>
            {
                rabbitMqConfig.Host(new Uri(hostContext.Configuration["RabbitMq:Host"]), h =>
                {
                    h.Username(hostContext.Configuration["RabbitMq:Username"]);
                    h.Password(hostContext.Configuration["RabbitMq:Password"]);
                });

                rabbitMqConfig.ReceiveEndpoint("photo-uploaded", e =>
                {
                    e.ConfigureConsumer<ImageAddedConsumers>(context);
                });
            });
        });

        services.AddHostedService<MassTransitHostedService>();

    });

var host = builder.Build();
await host.RunAsync();