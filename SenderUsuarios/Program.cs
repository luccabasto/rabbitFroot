using System;
using System.Text;
using RabbitMQ.Client;

class Program
{
    static void Main()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection(); // Ensure RabbitMQ.Client is referenced
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare("fiap.exchange", ExchangeType.Topic, true);

        Console.Write("Nome completo: ");
        var nome = Console.ReadLine();
        Console.Write("Endereço: ");
        var end = Console.ReadLine();
        Console.Write("RG: ");
        var rg = Console.ReadLine();
        Console.Write("CPF: ");
        var cpf = Console.ReadLine();
        var timestamp = DateTime.Now.ToString("O");
        var message = $"{nome}|{end}|{rg}|{cpf}|{timestamp}";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish("fiap.exchange", "usuarios.dados", null, body);
        Console.WriteLine($"Enviado: {message}");
    }
}