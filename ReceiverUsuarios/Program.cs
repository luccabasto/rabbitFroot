using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program
{
    static void Main()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnectionAsync;
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare("fiap.exchange", ExchangeType.Topic, true);
        channel.QueueDeclare("usuarios.receiver", true, false, false);
        channel.QueueBind("usuarios.receiver", "fiap.exchange", "usuarios.validated");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (s, ea) =>
        {
            var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
            Console.WriteLine($"ReceiverUsuarios recebeu: {msg}");
        };
        channel.BasicConsume("usuarios.receiver", true, consumer);

        Console.WriteLine("ReceiverUsuarios aguardando. ENTER sai.");
        Console.ReadLine();
    }
}
