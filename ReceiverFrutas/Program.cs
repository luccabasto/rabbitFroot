using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program
{
    static void Main()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "[ip da máquina aqui]",
            Port = AmqpTcpEndpoint.UseDefaultPort,
            UserName = "meu usuario",
            Password = "minha senha"
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare("fiap.exchange", ExchangeType.Topic, true);
        channel.QueueDeclare("frutas.receiver", true, false, false);
        channel.QueueBind("frutas.receiver", "fiap.exchange", "frutas.validated");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (s, ea) =>
        {
            var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
            Console.WriteLine($"ReceiverFrutas recebeu: {msg}");
        };
        channel.BasicConsume("frutas.receiver", true, consumer);

        Console.WriteLine("ReceiverFrutas aguardando. ENTER sai.");
        Console.ReadLine();
    }
}
