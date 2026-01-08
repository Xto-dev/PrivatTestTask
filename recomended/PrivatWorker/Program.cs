using PrivatWorker.Application.Transactions;
using PrivatWorker.Infra.Logging;
using PrivatWorker.Infra.Persistence;
using PrivatWorker.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddHostedService<TransactionIngestionWorker>();
builder.Services.AddHostedService<TransactionProcessingWorker>();

builder.Services.AddScoped<CreateTransaction>();
builder.Services.AddScoped<UpdateTransactionsStatus>();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

builder.Services.AddScoped<ITransactionLogger, TransactionLogger>();
builder.Services.AddSingleton<ErrorLogger>();
builder.Services.AddSingleton<ILogWriter, ConsoleLogWriter>();

var host = builder.Build();
host.Run();
