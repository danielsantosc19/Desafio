using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DesafioFront.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Runtime.Serialization.Json;

namespace DesafioFront.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index2()
        {
            var factory = new ConnectionFactory() { HostName = "35.199.98.99", Port = 5672 };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "rpc_queue",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);                                        

                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    Models.Operacao operacao = new Operacao() { Id = 1, Side = "buy", Price = 19, Quantity = 10, Symbol = "PETR4" };

                    DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(Operacao));
                    s.WriteObject(ms, operacao);
                    byte[] json = ms.ToArray();
                    ms.Close();
                    string body = Encoding.UTF8.GetString(json, 0, json.Length);

                    channel.BasicPublish(exchange: "", 
                        routingKey: "rpc_queue", 
                        basicProperties: null, 
                        body: json
                        );
                    
                }
            }
            return View();
        }

        

        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
