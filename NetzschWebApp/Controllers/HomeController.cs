using Microsoft.AspNetCore.Mvc;
using NetzschWebApp.Models;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;

namespace NetzschWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SendMessage(string message)
        {
            SendToRabbitMQ(message);
            return Ok();
        }

        private void SendToRabbitMQ(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                    return;

                var factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "user", Password = "password" }; //This would be an env variable in the future
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(queue: "clientMessages",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "clientMessages",
                                     basicProperties: null,
                                     body: body);
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        private List<string> ReceiveFromRabbitMQ()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "user", Password = "password" }; //This would be an env variable in the future
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "serverMessages",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var messages = new List<string>();
            while (true)
            {
                var data = channel.BasicGet("serverMessages", true);
                if (data == null)
                {
                    break;
                }
                messages.Add(Encoding.UTF8.GetString(data.Body.ToArray()));
            }

            return messages;
        }

        [HttpGet]
        public IActionResult GetMessages()
        {
            var message = ReceiveFromRabbitMQ();
            return Json(message);
        }

    }
}