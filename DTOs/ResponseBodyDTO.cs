namespace ConcilliationConsumer.DTOs;

public class BaseJson(int id, string status)
{
    public int Id { get; set; } = id;
    public string Status { get; set; } = status;
}
public class Response()
{
    public Dictionary<int, string> DatabaseToFile { get; set; } = [];
    public Dictionary<int, string> FileToDatabase { get; set; } = [];
    public Dictionary<int, string> DifferentStatus { get; set; } = [];
}