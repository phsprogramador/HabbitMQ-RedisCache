using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Messenger
{
    public class RabbitMQPublish
    {
        private readonly IConfiguration configuration;
        private readonly IConnection connection;
        private readonly IModel channel;
        const string queueName = "notification";
        public RabbitMQPublish()
        {

            configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                    .Build();

            connection = new ConnectionFactory()
            {
                HostName = configuration.GetSection("HabbitMQSettings:HostName").Value,
                Port = Int32.Parse(configuration.GetSection("HabbitMQSettings:Port").Value),
                UserName = configuration.GetSection("HabbitMQSettings:UserName").Value,
                Password = configuration.GetSection("HabbitMQSettings:Password").Value
            }.CreateConnection();

            channel = connection.CreateModel();
            channel.QueueDeclare(queueName, false, false, false, null);
        }

        public bool Add(Notification notification)
        {
            try
            {
            	string value = JsonConvert.SerializeObject(notification);
            	byte[] body = Encoding.UTF8.GetBytes(value);
            	channel.BasicPublish(string.Empty, queueName, null, body);
            }
            catch (Exception)
            {
            	return false;
            }
            return true;
        }
    }
}
