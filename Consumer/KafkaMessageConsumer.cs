using Confluent.Kafka;

namespace Consumer;

public class KafkaMessageConsumer
{
    private readonly ILogger<KafkaMessageConsumer> _logger;

    private const string Topic = "test";
    private const int RetentionTimeMs = 7000;
    private readonly ConsumerConfig _config;
    private readonly CancellationToken _cancellationToken;

    public KafkaMessageConsumer(ILogger<KafkaMessageConsumer> logger)
    {
        _logger = logger;
        _cancellationToken = new CancellationToken();

        _config = new ConsumerConfig
        {
            GroupId = "test_group",
            BootstrapServers = "broker:29092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }

    public async Task StartConsumingMessages(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
        consumer.Subscribe(Topic);
        try
        {
            while (true)
            {
                _logger.LogInformation("Started consuming at {DateTime}", DateTime.Now);
                var consumeResult =
                    consumer.Consume(new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);

                var difference = DateTime.UtcNow - consumeResult.Message.Timestamp.UtcDateTime;
                if (difference.TotalMilliseconds >= RetentionTimeMs)
                    throw new OperationCanceledException();

                await Task.Delay(4000, stoppingToken);

                _logger.LogInformation("{Message}", consumeResult.Message.Value);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("\n MESSAGE CANNOT BE CONSUMED \n");
            consumer.Close();
        }
        finally
        {
            _logger.LogInformation("Connection closed");
        }
    }
}