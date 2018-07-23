using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesafioFront.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;
using System.Text;
using RabbitMQ.Client;


namespace DesafioFront.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class RabbitController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "OK" });
        }

        [HttpPost]
        public IActionResult Post([FromServices]RabbitConfig rabbitConfig, [FromBody] Operacao operacao)//Operacao operacao
        {
            if (operacao == null)
            {
                return BadRequest(new { id = 0, status = false, msgs = "Sem parâmetros" });
            }

            try
            {
                var factory = new ConnectionFactory() { HostName = rabbitConfig.HostName, Port = rabbitConfig.Port };

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "rpc_queue",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                        byte[] json = Serialize(operacao);
                        
                        channel.BasicPublish(exchange: "",
                            routingKey: "rpc_queue",
                            basicProperties: null,
                            body: json
                            );

                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new { id = operacao.Id, status = false, msgs = ex.Message });
            }

            return Ok(new {id = operacao.Id, status = true, msgs = "" });
           
        }

        private static byte[] Serialize(Operacao operacao)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Operacao));
                serializer.WriteObject(ms, operacao);
                byte[] json = ms.ToArray();
                ms.Close();
                return json;
            }
            
        }
    }
}