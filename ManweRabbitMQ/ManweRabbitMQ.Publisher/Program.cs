using RabbitMQ.Client;
using System;
using System.Text;

namespace ManweRabbitMQ.Publisher
{
    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Info = 3,
        Warning = 4
    }

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
                    channel.ExchangeDeclare("direct-exchange", durable: true, type: ExchangeType.Direct);
                    Array log_name_array = Enum.GetValues(typeof(LogNames));
                    for (int i = 1; i < 11; i++)
                    {
                        Random rnd = new Random();
                        LogNames log = (LogNames) log_name_array.GetValue(rnd.Next(log_name_array.Length));
                        var bodyByte = Encoding.UTF8.GetBytes($"log: {log.ToString()}");
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true; //mesajı korumak için, silinmesini engellemek için kullanılır. Yukardaki kuyruğu sağlama aldığımız gibi(durable:false)
                        channel.BasicPublish("direct-exchange", routingKey: log.ToString(), properties, body: bodyByte);
                        Console.WriteLine($"Log mesajı gönderilmiştir: {log.ToString()}");
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
