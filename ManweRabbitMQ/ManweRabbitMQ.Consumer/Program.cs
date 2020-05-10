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
                    //queue = kuyruk ismimiz
                    //durable = eğer false olursa bizim rabbitmq instance'mız restart atarsa bu kuyruk silinir çünkü bellekte tutulur.Ancak true olursa rabbitmq bunu bir fiziksel diske yazar ve kaybolmaz.
                    //exclusive = bu kuyruğa bir tane mi kanal bağlansın yoksa birden fazla kanal bağlansın mı bunu belirtmek için kullanılır.True olursa tek kanal bağlanır.
                    //autoDelete = eğer kuyrukta bulunan son mesaj da bu kuyruktan çıkarsa bu kuyruk silinsin mi belirtmek için.True olursa otomatik olarak silinir.
                    channel.QueueDeclare("task_queue", true, false, false, null);

                    //prefetchCount: aynı anda kaç mesajın verilmek istendiği.
                    //global: true dersek kaç tane instance varsa tüm instance lar toplam prefetchCount kadar alabilir.Eğer false olursa her instance prefetchCount kadar mesaj alabilir.
                    channel.BasicQos(0, 1, false);

                    Console.WriteLine("Mesajları bekliyorum...");

                    var consumer = new EventingBasicConsumer(channel);

                    //autoAck = true olursa mesaj kuyruktan silinir işi biter bitmez.
                    channel.BasicConsume("task_queue", autoAck: false, consumer);

                    consumer.Received += (model, ea) =>
                    {
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine("Mesaj alındı: " + message);
                        int time = int.Parse(GetMessage(args));
                        Thread.Sleep(time);
                        Console.WriteLine("Mesaj işlendi.");

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
