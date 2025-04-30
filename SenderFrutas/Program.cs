using System;
using System.Text;
using RabbitMQ.Client;

class Program
{
    static void Main()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare("fiap.exchange", ExchangeType.Topic, true);

        Console.Write("Nome da fruta: ");
        var nome = Console.ReadLine();
        Console.Write("Descrição: ");
        var desc = Console.ReadLine();
        var timestamp = DateTime.Now.ToString("O");
        var message = $"{nome}|{desc}|{timestamp}";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish("fiap.exchange", "frutas.epoca", null, body);
        Console.WriteLine($"Enviado: {message}");
    }
}
