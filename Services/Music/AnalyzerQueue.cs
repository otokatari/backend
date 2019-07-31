using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OtokatariBackend.Model.DependencyInjection.MessageQueue;
using OtokatariBackend.Persistence.MongoDB.Model;
using RabbitMQ.Client;

namespace OtokatariBackend.Services.Music
{
    public class AnalyzerQueue : IDisposable
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        private readonly IBasicProperties properties;
        private readonly string QueueName;
        
        private readonly ILogger<AnalyzerQueue> _logger;
        public AnalyzerQueue(IOptions<RabbitMQConfiguration> options,ILogger<AnalyzerQueue> logger)
        {
            // initialize some DI instance.

            _logger = logger;
            
            // initialize RabbitMQ Client Producer instance.

            var host = options.Value.HostName;

            QueueName = options.Value.QueueName;
            var userName = options.Value.UserName;
            var password = options.Value.Password;

            var factory = new ConnectionFactory { HostName = host, UserName = userName, Password = password };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            // Persist each message.
            properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            // Persist queue to prevent losing message.
            channel.QueueDeclare(QueueName, true, false, true, null);

        }

        public void SendAnalyzeMusicMessage(SimpleMusic MusicInfo)
        {
            string MusicInfoJson = JsonConvert.SerializeObject(MusicInfo);
            var Message = Encoding.UTF8.GetBytes(MusicInfoJson);
            channel.BasicPublish("", QueueName, properties, Message);
            _logger.LogInformation($"Sent analyze request message [OK] -- {MusicInfo.Musicid} -- {MusicInfo.Name}");            
        }
        
        public void Dispose()
        {
            // Close resources.
            channel.Close();
            connection.Close();
        }
    }
}