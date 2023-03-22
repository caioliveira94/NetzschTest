using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "user", Password = "password" }; //This would be an env variable in the future
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "clientMessages",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.QueueDeclare(queue: "serverMessages",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var receivedBody = ea.Body.ToArray();
                var receivedMessage = Encoding.UTF8.GetString(receivedBody);
                Console.WriteLine($"\n[x] Received Message: {receivedMessage}\nType a message: ");
            };

            channel.BasicConsume(queue: "clientMessages",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine("Send messages. Press CTRL+C to exit.");

            while (true)
            {
                // Enviando mensagem
                Console.Write("Type a message: ");
                string message = Console.ReadLine();
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "serverMessages",
                                     basicProperties: null,
                                     body: body);

            }
        }
    }
}
