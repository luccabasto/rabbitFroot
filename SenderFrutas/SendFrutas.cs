/*
 * SendFrutas.cs
 * Producer de Frutas da Época
 * Fluxo: SendFrutas → Broker (“fiap.exchange”) → Validation
 */

using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

// Payload de fruta: timestamp, nome e descrição
record FruitPayload(string Timestamp, string Name, string Description);

public class SendFrutas
{
    public static async Task Main()
    {
        // 1) Cria factory e conexão
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // 2) Declara exchange do tipo Topic
        const string exchange = "fiap.exchange";
        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Topic, durable: true);

        // 3) Declara fila de validação e faz bind
        const string queueValidate = "frutas.validate";
        const string routingKey = "frutas.epoca";
        await channel.QueueDeclareAsync(
            queue: queueValidate,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        await channel.QueueBindAsync(
            queue: queueValidate,
            exchange: exchange,
            routingKey: routingKey,
            arguments: null
        );

        // 4) Lê dados do usuário
        Console.Write("Nome da fruta: ");
        var name = Console.ReadLine() ?? "";
        Console.Write("Descrição: ");
        var desc = Console.ReadLine() ?? "";

        // 5) Serializa payload e converte em bytes
        var payload = new FruitPayload(DateTime.Now.ToString("O"), name, desc);
        var json = JsonSerializer.Serialize(payload);
        var body = Encoding.UTF8.GetBytes(json);

        // 6) Publica no exchange com routing-key
        var basicProperties = new BasicProperties(); // Substitui o método inexistente
        await channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: basicProperties, // Usa a instância criada
            body: body
        );

        Console.WriteLine($"[SendFrutas] Enviado → {json}");
    }
}
