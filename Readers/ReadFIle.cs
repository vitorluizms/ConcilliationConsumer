using System.Data.Common;
using System.Text.Json;
using ConcilliationConsumer.Dtos;
using ConcilliationConsumer.DTOs;
using ConcilliationConsumer.Exceptions;

namespace ConcilliationConsumer.Files;

public class LineFile
{
    public required int id { get; set; }
    public required string status { get; set; }
}

public class JSONReader
{
    public static Response ReadFile(string path, Dictionary<int, string> paymentsDatabase)
    {
        Response response = new();
        using StreamReader fileReader = new(path);
        string? line;
        Dictionary<int, string> hashPaymentsFile = new();

        while ((line = fileReader.ReadLine()) != null)
        {
            Console.WriteLine(line);
            LineFile paymentFile = JsonSerializer.Deserialize<LineFile>(line) ?? throw new BadRequestError("Content body is not valid! Concilliation faild.");
            ValidateValueInDatabase(paymentFile, response, paymentsDatabase);

        }

        foreach (KeyValuePair<int, string> payment in paymentsDatabase)
        {
            if (!hashPaymentsFile.TryGetValue(payment.Key, out string? status))
            {
                response.DatabaseToFile.Add(payment.Key, payment.Value);
            }
        }

        fileReader.Close();
        return response;
    }

    static void ValidateValueInDatabase(LineFile paymentFile, Response response, Dictionary<int, string> paymentsDatabase)
    {
        paymentsDatabase.TryGetValue(paymentFile.id, out string? status);
        if (status == null)
        {
            Console.WriteLine($"PaymentFile: {paymentFile.id} - {paymentFile.status}");
            response.FileToDatabase.Add(paymentFile.id, paymentFile.status);
        }
        else if (status != paymentFile.status)
        {
            Console.WriteLine($"PaymentFile: {paymentFile.id} - {paymentFile.status}");
            response.DifferentStatus.Add(paymentFile.id, paymentFile.status);
        }
    }

}

