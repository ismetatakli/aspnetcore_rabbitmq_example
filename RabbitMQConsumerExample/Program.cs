using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

// RabbitMQ bağlantı bilgilerini belirleyin
var factory = new ConnectionFactory()
{
    HostName = "localhost",
    Port = 5672, // Varsayılan RabbitMQ portu
    UserName = "guest", // Varsayılan kullanıcı adı
    Password = "guest" // Varsayılan şifre
};

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    // Kuyruğu oluştur veya mevcutsa kullan
    channel.QueueDeclare(queue: "chat_messages",
                         durable: false,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var dMessage = JsonConvert.DeserializeObject<Message>(message);
        Console.WriteLine($"{dMessage.User}: {dMessage.MessageText}");
    };
    channel.BasicConsume(queue: "chat_messages",
                         autoAck: true,
                         consumer: consumer);

    Console.WriteLine("Messages:");
    Console.ReadLine();
}


public class Message
{
    public int MessageType { get; set; }
    public string MessageText { get; set; }
    public string? User { get; set; }
}

public enum MessageType
{
    Info = 1,
    Message = 2
}