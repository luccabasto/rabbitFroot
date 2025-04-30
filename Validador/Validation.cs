using System;
using System.Collections;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

record FruitPayload(string Timestamp, string Name, string Description);
record UserPayload(string Timestamp, string FullName, string Address, string RG, string CPF);
record ValidationResult(string Timestamp, string Code, bool IsValid);

class Validation
{
    static async Task Main()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        await using var conn = await factory.CreateConnectionAsync();
        await using var channel = await conn.CreateChannelAsync();

        channel.ExchangeDeclareAsync("fiap.exchange", ExchangeType.Topic, durable: true);

        // Filas e routing-keys
        const string QfVal = "frutas.validate";
        const string QuVal = "usuarios.validate";
        const string RkFrutaIn = "frutas.epoca";
        const string RkUserIn = "usuarios.dados";
        const string RkFrutaOut = "frutas.validated";
        const string RkUserOut = "usuarios.validated";

        // Declara e bind
        await channel.QueueDeclareAsync(QfVal, durable: true, exclusive: false, autoDelete: false);
        await channel.QueueDeclareAsync(QuVal, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBindAsync(QfVal, "fiap.exchange", RkFrutaIn);
        channel.QueueBindAsync(QuVal, "fiap.exchange", RkUserIn);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (s, ea) =>
        {
            var raw = Encoding.UTF8.GetString(ea.Body.ToArray());
            // Decide se é fruta ou usuário pelo routing-key original
            var rk = ea.RoutingKey;
            if (rk == RkFrutaIn)
            {
                var fp = JsonSerializer.Deserialize<FruitPayload>(raw)!;
                bool ok = !string.IsNullOrWhiteSpace(fp.Name) && !string.IsNullOrWhiteSpace(fp.Description);
                var vr = new ValidationResult(fp.Timestamp, fp.Name, ok);
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(vr));

                // Criação das propriedades básicas
                var properties = new BasicProperties
                {
                    Persistent = true // Example of setting a property
                };
                await channel.BasicPublishAsync("fiap.exchange", RkFrutaOut, false, properties, body);
                Console.WriteLine($"[Validation] Fruta “{fp.Name}” {(ok ? "válida" : "inválida")}");
            }
            else if (rk == RkUserIn)
            {
                var up = JsonSerializer.Deserialize<UserPayload>(raw)!;
                // Simula lista de CPF válidos
                var validCpfs = new ArrayList { up.CPF, "00000000000" };
                bool ok = validCpfs.Contains(up.CPF);
                var vr = new ValidationResult(up.Timestamp, up.FullName, ok);
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(vr));
                var properties = new BasicProperties
                {
                    Persistent = true // Example of setting a property
                };
                await channel.BasicPublishAsync("fiap.exchange", RkUserOut, false, properties, body);
                Console.WriteLine($"[Validation] Usuário “{up.FullName}” {(ok ? "válido" : "inválido")}");
            }
        };

        await channel.BasicConsumeAsync(QfVal, autoAck: true, consumer: consumer);
        await channel.BasicConsumeAsync(QuVal, autoAck: true, consumer: consumer);

        Console.WriteLine("[Validation] Aguardando mensagens... Pressione Enter para sair.");
        Console.ReadLine();
    }
}
