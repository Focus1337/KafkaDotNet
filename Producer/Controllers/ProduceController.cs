using System.Net;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Producer.Controllers;

[ApiController]
[Route("[controller]")]
public class ProduceController : ControllerBase
{
    private readonly string _topic;
    private readonly ProducerConfig _config;

    public ProduceController()
    {
        _topic = "test";
        _config = new ProducerConfig
        {
            BootstrapServers = "broker:29092",
            Acks = Acks.All,
            ClientId = Dns.GetHostName()
        };
    }

    [HttpPost("CreateTopic")]
    public async Task<IActionResult> ProduceTopic()
    {
        var adminConfig = new AdminClientConfig { BootstrapServers = "broker:29092" };
        using (var adminClient = new AdminClientBuilder(adminConfig).Build())
        {
            var topicSpecification = new TopicSpecification
            {
                Name = _topic,
                Configs = new Dictionary<string, string>
                {
                    {
                        "message.timestamp.difference.max.ms", "7000"
                    }, // максимальное время между временем отправки и временем записи в лог
                    { "segment.ms", "7000" }, // максимальное время жизни сегмента
                    { "retention.ms", "7000" }, // максимальное время хранения сообщения
                    { "delete.retention.ms", "7000" }, // максимальное время до удаления сообщения
                }
            };
            try
            {
                await adminClient.CreateTopicsAsync(new List<TopicSpecification> { topicSpecification });
            }
            catch (CreateTopicsException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveTopic()
    {
        var adminConfig = new AdminClientConfig { BootstrapServers = "broker:29092" };
        using (var adminClient = new AdminClientBuilder(adminConfig).Build())
        {
            try
            {
                await adminClient.DeleteTopicsAsync(new List<string> { _topic });
            }
            catch (CreateTopicsException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> ProduceMessages([FromQuery] int count)
    {
        var results = new List<Message<Null, string>>();
        using var producer = new ProducerBuilder<Null, string>(_config).Build();

        for (var i = 0; i < count; i++)
        {
            var result = await producer.ProduceAsync(_topic, new Message<Null, string> { Value = $"Message ({i})" });
            results.Add(result.Message);
        }

        return Ok(results);
    }
}