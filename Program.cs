using MES_ProcessSvc;
using MES_ProcessSvc.Model;



var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();


