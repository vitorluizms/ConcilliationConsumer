using System.Text.Json;
using ConcilliationConsumer.DTOs;

namespace ConcilliationConsumer.Files;

public class JSONWriter
{
    public static void WriteFile(string path, BaseJson[] payments, int batchSize)
    {

        using StreamWriter file = File.CreateText(path);
        for (int i = 0; i < batchSize; i++)
        {
            try
            {
                BaseJson payment = new(payments[i].Id, payments[i].Status) { Id = payments[i].Id, Status = payments[i].Status };
                string json = JsonSerializer.Serialize(payment);
                file.WriteLine(json);
            }
            catch
            {
                Console.WriteLine("Error when serializing this line!");
            }
        }
    }
}