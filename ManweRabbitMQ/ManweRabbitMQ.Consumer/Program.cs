using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace ManweRabbitMQ.Consumer
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

                    var queueName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(queue: queueName, exchange: "logs", routingKey: "");

                    //prefetchCount: aynı anda kaç mesajın verilmek istendiği.
                    //global: true dersek kaç tane instance varsa tüm instance lar toplam prefetchCount kadar alabilir.Eğer false olursa her instance prefetchCount kadar mesaj alabilir.
                    channel.BasicQos(0, 1, false);

                    Console.WriteLine("Logları bekliyorum...");

                    var consumer = new EventingBasicConsumer(channel);

                    //autoAck = true olursa mesaj kuyruktan silinir işi biter bitmez.
                    channel.BasicConsume(queue: queueName, autoAck: false, consumer);

                    consumer.Received += (model, ea) =>
                    {
                        var log = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine("Log alındı: " + log);
                        int time = int.Parse(GetMessage(args));
                        Thread.Sleep(time);
                        Console.WriteLine("Loglama bitti.");

                        //autoAck konusunda false dediğimiz için(autoack false mesajın silme işlemi yapılmıyor otomatikman) burada mesajın başarılı bir şekilde işlendi mesajı silebilirsin demek istedik.
                        channel.BasicAck(ea.DeliveryTag, multiple: false);
                    };
                    Console.WriteLine("Çıkış yapmak için tıklayınız.");
                    Console.ReadLine();
                }
            }
        }

        private static string GetMessage(string[] args)
        {
            return args[0].ToString();
        }
    }
}
