namespace Consumer.HostedService;

public class MessageDelayingService : BackgroundService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(1));
    private readonly KafkaMessageConsumer _kafkaMessageConsumer;

    public MessageDelayingService(KafkaMessageConsumer kafkaMessageConsumer)
    {
        _kafkaMessageConsumer = kafkaMessageConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            Console.WriteLine($"Background Service restarted at {DateTime.Now}");
            await _kafkaMessageConsumer.StartConsumingMessages(stoppingToken);
        } while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested);
    }
}