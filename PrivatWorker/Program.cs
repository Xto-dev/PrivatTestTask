using PrivatWorker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<InsertOperationService>();
builder.Services.AddHostedService<UpdateStatusService>();

var host = builder.Build();
host.Run();
