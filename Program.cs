using System.Text;
using System.Text.Json;
using ConcilliationConsumer.Data;
using ConcilliationConsumer.Dtos;
using ConcilliationConsumer.DTOs;
using ConcilliationConsumer.Files;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var connString = "Host=localhost;Username=postgres;Password=postgres;Database=postgres";
var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseNpgsql(connString)
    .Options;

string queueName = "concilliation";
ConnectionFactory factory = new()
{
    HostName = "localhost",
    UserName = "admin",
    Password = "admin"
};
IConnection connection = factory.CreateConnection();
IModel channel = connection.CreateModel();

channel.QueueDeclare(
  queue: queueName,
  durable: true,
  exclusive: false,
  autoDelete: false,
  arguments: null
);

Console.WriteLine("[*] Waiting for messages...");

EventingBasicConsumer consumer = new(channel);
consumer.Received += (model, ea) =>
{

    HttpClient httpClient = new();
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    ConcilliationBodyDTO? dto = System.Text.Json.JsonSerializer.Deserialize<ConcilliationBodyDTO>(message);

    if (dto is null)
    {
        Console.WriteLine("Content body is not valid! Concilliation faild.");
        channel.BasicReject(ea.DeliveryTag, false);
        return;
    }

    using var db = new AppDbContext(dbOptions);
    var start = DateTime.Now;

    try
    {

        Console.WriteLine("Started processing concilliation...");

        Count count = GetPaymentsCountByPSP(db, dto);

        int chunk = 10000;
        int chunks = (int)Math.Ceiling((double)count.count / chunk);
        List<BaseJson> paymentsList = new();

        for (int i = 0; i < chunks; i++)
        {
            int offset = i * chunk;

            paymentsList.AddRange(GetPaymentsByOffSet(db, dto, chunk, offset));
        };

        if (!File.Exists(dto.File))
        {
            Console.WriteLine("File not found!");
            channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: false);
            return;
        }

        Dictionary<int, string> paymentsDatabase = paymentsList.ToDictionary(p => p.Id, p => p.Status);
        Response response = JSONReader.ReadFile(dto.File, paymentsDatabase);

        postConcilliationToPSP(dto, response);
        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: false);
    }
};

channel.BasicConsume(
    queue: "concilliation",
    autoAck: false,
    consumer: consumer
);

Console.ReadLine();

static Count GetPaymentsCountByPSP(AppDbContext db, ConcilliationBodyDTO dto)
{
    string sqlString = @"
        SELECT COUNT(p.""Id"")
            FROM ""Payments"" AS p
            INNER JOIN ""Accounts"" AS paymentOrigin
                ON paymentOrigin.""Id"" = p.""PaymentProviderAccountId""
            INNER JOIN ""PixKeys"" AS pk
                ON pk.""Id"" = p.""PixKeyId""
            INNER JOIN ""Accounts"" AS paymentDestiny
                ON paymentDestiny.""Id"" = pk.""AccountId""
            WHERE date_trunc('day', p.""CreatedAt"") = to_timestamp({0}, 'YYYY-MM-DD') AND (
                paymentOrigin.""PaymentProviderId"" = {1} OR
                paymentDestiny.""PaymentProviderId"" = {1}
        )";
    var count = db.Database.SqlQueryRaw<Count>(sqlString, dto.Date, dto.PaymentProviderId).First();

    return count;
}

static IQueryable<BaseJson> GetPaymentsByOffSet(AppDbContext db, ConcilliationBodyDTO concilliation, int chunk, int offset)
{
    string sqlString = @"
        SELECT p.""Id"", p.""Status""
            FROM ""Payments"" AS p
            INNER JOIN ""Accounts"" AS originPayment
                ON originPayment.""Id"" = p.""PaymentProviderAccountId""
            INNER JOIN ""PixKeys"" AS pk
                ON pk.""Id"" = p.""PixKeyId""
            INNER JOIN ""Accounts"" AS destinyPayment
                ON destinyPayment.""Id"" = pk.""AccountId""
            WHERE date_trunc('day', p.""CreatedAt"") = to_timestamp({0}, 'YYYY-MM-DD') AND (
                originPayment.""PaymentProviderId"" = {1} OR
                destinyPayment.""PaymentProviderId"" = {1}
                )
            ORDER BY p.""Id""
            LIMIT {2}
            OFFSET {3}";

    var paymentsChunk = db.Database.SqlQueryRaw<BaseJson>(sqlString, concilliation.Date, concilliation.PaymentProviderId, chunk, offset);

    return paymentsChunk;
}

void postConcilliationToPSP(ConcilliationBodyDTO dto, Response response)
{
    HttpClient httpClient = new();
    PostConcilliationBody postConcilliationBody = new(response);
    string json = JsonConvert.SerializeObject(postConcilliationBody);
    HttpContent data = new StringContent(json, Encoding.UTF8, "application/json");
    var responseVariable = httpClient.PostAsync(dto.Postback, data);
    Console.WriteLine(json);
    Console.WriteLine("Concilliation processed successfully!");
}

public class Count
{
    public int count { get; set; }
}

