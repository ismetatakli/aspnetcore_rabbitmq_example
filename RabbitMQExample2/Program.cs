using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;


ConnectionFactory connectionInfo = new ConnectionFactory()
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};

var user = Guid.NewGuid().ToString();

using (var cfConn = connectionInfo.CreateConnection())
{
    using (var channel = cfConn.CreateModel())
    {
        channel.QueueDeclare
        (
            queue: "chat_messages",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        while (true)
        {
            string userMessageText = Console.ReadLine();
            if (userMessageText == "Q")
                break;
            var message = new Message { MessageType = (int)MessageType.Message, MessageText = userMessageText, User = user };


            string strJson = JsonConvert.SerializeObject(message);
            byte[] byteMessage = Encoding.UTF8.GetBytes(strJson);

            channel.BasicPublish
            (
                exchange: "",
                routingKey: "chat_messages",
                basicProperties: null,
                body: byteMessage
            );
        }

    }
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