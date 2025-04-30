/*
 * SendUsuarios.cs
 * Producer de Dados de Usuários
 * Fluxo: SendUsuarios → Broker (“fiap.exchange”) → Validation
 */

using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

// Payload de usuário: timestamp, nome, endereço, RG e CPF
record UserPayload(string Timestamp, string FullName, string Address, string RG, string CPF);

public class SendUsuarios
{
    public static async Task Main()
    {
        // 1) Cria factory e conexão
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // 2) Declara exchange Topic
        const string exchange = "fiap.exchange";
        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Topic, durable: true);

        // 3) Declara fila de validação e faz bind
        const string queueValidate = "usuarios.validate";
        const string routingKey = "usuarios.dados";
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
        Console.Write("Nome completo: ");
        var name = Console.ReadLine() ?? "";
        Console.Write("Endereço: ");
        var address = Console.ReadLine() ?? "";
        Console.Write("RG: ");
        var rg = Console.ReadLine() ?? "";
        Console.Write("CPF: ");
        var cpf = Console.ReadLine() ?? "";

        // 5) Serializa payload e converte em bytes
        var payload = new UserPayload(DateTime.Now.ToString("O"), name, address, rg, cpf);
        var json = JsonSerializer.Serialize(payload);
        var body = Encoding.UTF8.GetBytes(json);

        // 6) Publica no exchange com routing-key
        var basicProperties = new BasicProperties(); // Substitui CreateBasicProperties
        await channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: basicProperties,
            body: body
        );

        Console.WriteLine($"[SendUsuarios] Enviado → {json}");
    }
}
