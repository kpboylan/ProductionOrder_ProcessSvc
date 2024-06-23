
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MES_ProcessSvc.DAL;
using MES_ProcessSvc.Model;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Channels;

namespace MES_ProcessSvc
{
    public class Worker : BackgroundService
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<Worker> _logger;
        private DAL.IProdOrderRepository _prodOrderRepository = new ProdOrderRepository();

        private const string _queueName = "ProdOrder";
        private readonly TimeSpan _stoppingCheckInterval = TimeSpan.FromSeconds(5);
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;
        private readonly string _consumerTag;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            //_channel.BasicQos(0, 100, true);

            _channel.QueueDeclare(_queueName, false, false, false, null);

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += ReceivedHandler;

            _consumerTag = _channel.BasicConsume(_queueName, false, _consumer);
        }

        private void ReceivedHandler(object? sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var po = new ProdOrder();
            po = JsonConvert.DeserializeObject<ProdOrder>(message);

            _prodOrderRepository.AddProdOrder(po);

            var tag = ea.DeliveryTag;
            //_logger.LogInformation("Received message. tag: {tag}  at: {time}", tag, DateTimeOffset.Now);
            _logger.LogInformation("Received message. PO_Number: {tag}  at: {time}", po.OrderNumber, DateTimeOffset.Now);
            _channel.BasicAck(tag, false);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (_connection)
            using (_channel)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(_stoppingCheckInterval, stoppingToken);
                }

                _logger.LogInformation("Worker STOPPING at: {time}", DateTimeOffset.Now);
                _channel.BasicCancel(_consumerTag);
            }
        }
    }
}
