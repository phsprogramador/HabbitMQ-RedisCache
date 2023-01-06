using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Timers;

namespace Messenger
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection connection;
        private readonly HttpClientRequest client;
        private IModel channel;

        private const string queueName = "notification";

        public RabbitMQConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
            connection = new ConnectionFactory() {
                HostName = _configuration.GetSection("HabbitMQSettings:HostName").Value,
                Port = Int32.Parse(_configuration.GetSection("HabbitMQSettings:Port").Value), 
                UserName = _configuration.GetSection("HabbitMQSettings:UserName").Value, 
                Password = _configuration.GetSection("HabbitMQSettings:Password").Value
            }.CreateConnection();

            channel = connection.CreateModel();
            channel.QueueDeclare(queueName, false, false, false, null);
            client = new HttpClientRequest();
        }

        private void Read(object sender, ElapsedEventArgs e)
        {
            try
            {
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (ModuleHandle, ea) =>
                {
                    ReadOnlyMemory<byte> body = ea.Body;
                    string? json = Encoding.UTF8.GetString(body.ToArray());
                    if (json != null)
                    {
                        Notification notification = JsonConvert.DeserializeObject<Notification>(json);
                        client.RequestClients(notification);
                    }
                };

                channel.BasicConsume(queueName, autoAck: true, consumer: consumer);


                //EventingBasicConsumer? consumidor = new EventingBasicConsumer(channel);

                //consumidor.Received += (ModuleHandle, ea) =>
                //{
                //    ReadOnlyMemory<byte> body = ea.Body;
                //    string? json = Encoding.UTF8.GetString(body.ToArray());
                //    if(json != null)
                //    {
                //        Notification notification = JsonConvert.DeserializeObject<Notification>(json);
                //        client.RequestClients(notification);
                //    }
                //};

                //channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumidor);
            }
            catch (Exception ex)
            {

            }
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            System.Timers.Timer timer = new(5000);

            timer.AutoReset = true;
            timer.Elapsed += new ElapsedEventHandler(Read);
            timer.Start();

            return Task.CompletedTask;
        }
    }
}
