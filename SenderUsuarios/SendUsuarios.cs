using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;

record UserPayload(string Timestamp, string FullName, string Address, string RG, string CPF);

class Program
{
    static async Task Main()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        await using var conn = await factory.CreateConnectionAsync();
        await using var channel = await conn.CreateChannelAsync();

        channel.ExchangeDeclare("fiap.exchange", ExchangeType.Topic, durable: true);

        const string RkSend = "usuarios.dados";
        const string QueueVal = "usuarios.validate";

        await channel.QueueDeclareAsync(QueueVal, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(QueueVal, "fiap.exchange", RkSend);

        Console.Write("Nome completo: ");
        var name = Console.ReadLine() ?? "";
        Console.Write("Endereço: ");
        var address = Console.ReadLine() ?? "";
        Console.Write("RG: ");
        var rg = Console.ReadLine() ?? "";
        Console.Write("CPF: ");
        var cpf = Console.ReadLine() ?? "";
        var payload = new UserPayload(DateTime.Now.ToString("O"), name, address, rg, cpf);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));

        await channel.BasicPublishAsync(
            exchange: "fiap.exchange",
            routingKey: RkSend,
            mandatory: false,
            basicProperties: null,
            body: body
        );

        Console.WriteLine($"[SenderUsuarios] Enviado → {JsonSerializer.Serialize(payload)}");
    }
}
