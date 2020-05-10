using RabbitMQ.Client;
using System;
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
                    channel.ExchangeDeclare("logs", durable: true, type: ExchangeType.Fanout);

                    string message = GetMessage(args);
                    for (int i = 1; i < 11; i++)
                    {
                        var bodyByte = Encoding.UTF8.GetBytes($"{message}-{i}");
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true; //mesajı korumak için, silinmesini engellemek için kullanılır. Yukardaki kuyruğu sağlama aldığımız gibi(durable:false)
                        channel.BasicPublish("logs", routingKey: "", properties, body: bodyByte);
                        Console.WriteLine($"Mesajınız gönderilmiştir: {message}-{i}");
                    }
                }

                Console.WriteLine("Çıkış yapmak için tıklayınız.");
                Console.ReadLine();
            }
        }

        private static string GetMessage(string[] args)
        {
            return args[0].ToString();
        }
    }
}
