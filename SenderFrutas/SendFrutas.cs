using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;

record FruitPayload(string Timestamp, string Name, string Description);

class Program
{
    static async Task Main()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        await using var conn = await factory.CreateConnectionAsync();
        await using var channel = await conn.CreateChannelAsync();

        // Declara o exchange do tipo Topic
        channel.ExchangeDeclare("fiap.exchange", ExchangeType.Topic, durable: true);

        // Constantes de routing
        const string RkSend = "frutas.epoca";
        const string QueueVal = "frutas.validate";

        // Declara fila de validação e faz bind
        await channel.QueueDeclareAsync(QueueVal, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind("frutas.validate", "fiap.exchange", RkSend);

        Console.Write("Nome da fruta: ");
        var name = Console.ReadLine() ?? "";
        Console.Write("Descrição: ");
        var desc = Console.ReadLine() ?? "";
        var payload = new FruitPayload(DateTime.Now.ToString("O"), name, desc);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));

        // Publica no exchange com routing-key → chegará em “frutas.validate”
        await channel.BasicPublishAsync(
            exchange: "fiap.exchange",
            routingKey: RkSend,
            mandatory: false,
            basicProperties: null,
            body: body
        );

        Console.WriteLine($"[SenderFrutas] Enviado → {JsonSerializer.Serialize(payload)}");
    }
}
