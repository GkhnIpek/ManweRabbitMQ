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
                    //queue = kuyruk ismimiz
                    //durable = eğer false olursa bizim rabbitmq instance'mız restart atarsa bu kuyruk silinir çünkü bellekte tutulur.Ancak true olursa rabbitmq bunu bir fiziksel diske yazar ve kaybolmaz.
                    //exclusive = bu kuyruğa bir tane mi kanal bağlansın yoksa birden fazla kanal bağlansın mı bunu belirtmek için kullanılır.True olursa tek kanal bağlanır.
                    //autoDelete = eğer kuyrukta bulunan son mesaj da bu kuyruktan çıkarsa bu kuyruk silinsin mi belirtmek için.True olursa otomatik olarak silinir.
                    channel.QueueDeclare("task_queue", durable: true, false, false, null);

                    string message = GetMessage(args);
                    for (int i = 1; i < 11; i++)
                    {
                        var bodyByte = Encoding.UTF8.GetBytes($"{message}-{i}");
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true; //mesajı korumak için, silinmesini engellemek için kullanılır. Yukardaki kuyruğu sağlama aldığımız gibi(durable:false)


                        channel.BasicPublish("", routingKey: "task_queue", properties, body: bodyByte);
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
