using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

record ValidationResult(string Timestamp, string Code, bool IsValid);

class ReceiverUsuarios
{
    static async Task Main()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        await using var conn = await factory.CreateConnectionAsync();
        await using var channel = await conn.CreateChannelAsync();

        const string QueueOut = "usuarios.receiver";
        const string RkUserOut = "usuarios.validated";

        channel.ExchangeDeclare("fiap.exchange", ExchangeType.Topic, durable: true);
        await channel.QueueDeclareAsync(QueueOut, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(QueueOut, "fiap.exchange", RkUserOut);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (s, ea) =>
        {
            var raw = Encoding.UTF8.GetString(ea.Body.ToArray());
            var vr = JsonSerializer.Deserialize<ValidationResult>(raw)!;
            Console.WriteLine($"[ReceiverUsuarios] {vr.Code} → {(vr.IsValid ? "✔️" : "❌")} ({vr.Timestamp})");
            await Task.Yield();
        };

        await channel.BasicConsumeAsync(QueueOut, autoAck: true, consumer: consumer);

        Console.WriteLine("[ReceiverUsuarios] Aguardando mensagens... Enter para sair.");
        Console.ReadLine();
    }
}
