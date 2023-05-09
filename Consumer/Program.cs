using Consumer;
using Consumer.HostedService;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services => services.AddSingleton<KafkaMessageConsumer>());
builder.ConfigureServices(services => { services.AddHostedService<MessageDelayingService>(); });

var host = builder.Build();
host.Run();