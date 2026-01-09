var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var services = builder.Services;
services.AddPrivatWorkerDatabase(builder.Configuration);
services.AddPrivatWorkerServices();

var host = builder.Build();
host.Run();
