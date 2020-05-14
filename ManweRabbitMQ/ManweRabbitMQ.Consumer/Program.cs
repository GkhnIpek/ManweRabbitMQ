using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

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
                    channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);
                    channel.QueueDeclare("kuyruk1", false, false, false, null);
                    Dictionary<string, object> headers = new Dictionary<string, object>();
                    headers.Add("format", "pdf");
                    headers.Add("shape", "a4");
                    //headers.Add("x-match", "all"); //all => tüm header ların aynı olması lazım yoksa mesajı yakalayamaz. Buradaki örnekte formatı pdf olan ile shape a4 olan varsa mesaj gelir.
                    headers.Add("x-match", "any"); //all => herhangi bir header aynı olması mesajın yakalanması için yeterlidir.Buradaki örnekte formatı pdf veya shape a4 olan varsa mesaj gelir.

                    channel.QueueBind("kuyruk1", "header-exchange", string.Empty, headers);
                    var consumer = new EventingBasicConsumer(channel);

                    channel.BasicConsume("kuyruk1", false, consumer);

                    consumer.Received += (model, ea) =>
                    {
                        string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        if (!string.IsNullOrEmpty(message))
                        {
                            User user = JsonConvert.DeserializeObject<User>(message);
                            Console.WriteLine($"gelen mesaj: {user.Id.ToString()}-{user.Name}-{user.Email}-{user.Password}");

                            //autoAck konusunda false dediğimiz için(autoack false mesajın silme işlemi yapılmıyor otomatikman) burada mesajın başarılı bir şekilde işlendi mesajı silebilirsin demek istedik.
                            channel.BasicAck(ea.DeliveryTag, multiple: false);
                        }
                    };
                    Console.WriteLine("Çıkış yapmak için tıklayınız.");
                    Console.ReadLine();
                }
            }
        }
    }
}
