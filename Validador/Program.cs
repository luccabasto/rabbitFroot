using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program
{
    static void Main()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare("fiap.exchange", ExchangeType.Topic, true);

        channel.QueueDeclare("frutas.validation", true, false, false);
        channel.QueueDeclare("usuarios.validation", true, false, false);
        channel.QueueBind("frutas.validation", "fiap.exchange", "frutas.epoca");
        channel.QueueBind("usuarios.validation", "fiap.exchange", "usuarios.dados");

        var frutasConsumer = new EventingBasicConsumer(channel);
        frutasConsumer.Received += (s, ea) =>
        {
            var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
            var p = msg.Split('|');
            if (p.Length == 3)
                channel.BasicPublish("fiap.exchange", "frutas.validated", null, ea.Body.ToArray());
        };
        channel.BasicConsume("frutas.validation", true, frutasConsumer);

        var usuConsumer = new EventingBasicConsumer(channel);
        usuConsumer.Received += (s, ea) =>
        {
            var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
            var p = msg.Split('|');
            if (p.Length == 5)
                channel.BasicPublish("fiap.exchange", "usuarios.validated", null, ea.Body.ToArray());
        };
        channel.BasicConsume("usuarios.validation", true, usuConsumer);

        Console.WriteLine("Validador pronto. Tecle ENTER para sair.");
        Console.ReadLine();
    }
}
