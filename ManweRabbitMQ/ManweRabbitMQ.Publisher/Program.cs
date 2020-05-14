using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManweRabbitMQ.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory(); //Rabbitmq servisleri ile bağlantı kurmak için oluşturulur.
            factory.Uri = new Uri("amqp://uiibvwim:YkWdsj19hOXbf1WbY79Ts9dn77zlka-5@orangutan.rmq.cloudamqp.com/uiibvwim");
            //factory.HostName = "localhost"; //Local rabbitmq servisine bağlanma için

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

                    var properties = channel.CreateBasicProperties();
                    Dictionary<string, object> headers = new Dictionary<string, object>();
                    headers.Add("format", "excel");
                    headers.Add("shape", "a4");

                    properties.Headers = headers;
                    Console.WriteLine("mesaj gönderildi.");
                    channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes("header mesajım"));
                }

                Console.WriteLine("Çıkış yapmak için tıklayınız.");
                Console.ReadLine();
            }
        }
    }
}
