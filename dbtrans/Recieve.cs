using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading;

namespace dbtrans
{
    public class Recieve
    {
        public Recieve()
        {
            GetRequest();
        }

        public static void GetRequest()
        {

            db database = new db();
            var factory = new ConnectionFactory()
            {
                HostName = "black-ragwort-810.bigwig.lshift.net",
                Port = 10803,
                UserName = "1doFhxuC",
                Password = "WGgk9kXy_wFIFEO0gwB_JiDuZm2-PrlO",
                VirtualHost = "SDU53lDhKShK"
                //"amqp://1doFhxuC:WGgk9kXy_wFIFEO0gwB_JiDuZm2-PrlO@black-ragwort-810.bigwig.lshift.net:10803/SDU53lDhKShK"
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare("Rapid", "direct", false, false, null);
            channel.QueueDeclare("Queue1", false, true, false, null);
            channel.QueueBind("Queue1", "Rapid", "lasgetalltrans");
            channel.QueueBind("Queue1", "Rapid", "lasgettransaction");
            channel.QueueBind("Queue1", "Rapid", "lascreatetransaction");
            channel.QueueBind("Queue1", "Rapid", "lasupdatetransaction");
            channel.QueueBind("Queue1", "Rapid", "lasgetallcategories");
            channel.QueueBind("Queue1", "Rapid", "lasgetcategory");
            channel.QueueBind("Queue1", "Rapid", "lasdeletetransaction");

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
               
                if (ea.RoutingKey == "lasgetalltrans")
                {
                    database.GetAllTransactions();
                    Console.WriteLine(ea.RoutingKey);
                }
                else if (ea.RoutingKey == "lasgettransaction")
                {
                    database.GetSingleTransactionById(ea.Body.ToString());
                    Console.WriteLine(ea.RoutingKey);
                }
                else if (ea.RoutingKey == "lascreatetransaction")
                {
                    database.CreateTransaction(ea.Body.ToString());
                    Console.WriteLine(ea.RoutingKey);
                }
                else if (ea.RoutingKey == "lasupdatetransaction")
                {
                    database.UpdateTransaction(ea.Body.ToString());
                    Console.WriteLine(ea.RoutingKey);
                }
                else if (ea.RoutingKey == "lasgetallcategories")
                {
                    database.GetAllCategories();
                    Console.WriteLine(ea.RoutingKey);
                }
                else if (ea.RoutingKey == "lasgetcategory")
                {
                    database.GetSingleCategoryById(ea.Body.ToString());
                    Console.WriteLine(ea.RoutingKey);
                }
                else if (ea.RoutingKey == "lasdeletetransaction")
                {
                    database.DeleteTransactionById(ea.Body.ToString());
                    Console.WriteLine(ea.RoutingKey);
                }


            };

            channel.BasicConsume(queue: "Queue1", autoAck: true, consumer: consumer);
        }
    }
}