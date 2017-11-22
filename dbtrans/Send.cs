using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;

namespace dbtrans
{
    public class Send
    {
        public void EmitData(JObject confirmation, string tag)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "black-ragwort-810.bigwig.lshift.net",
                Port = 10803,
                UserName = "1doFhxuC",
                Password = "WGgk9kXy_wFIFEO0gwB_JiDuZm2-PrlO",
                VirtualHost = "SDU53lDhKShK"
              
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare("lasgetalltransconfirmation",false,false,false,null);
            channel.ExchangeDeclare("Rapid", "direct", false, false, null);
            var data = confirmation;
            var test = data.ToString();
            byte[] bytedata = Encoding.UTF8.GetBytes(test);
            
            channel.BasicPublish("Rapid", tag, null, bytedata);
        }
    }
}