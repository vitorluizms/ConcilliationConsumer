using ConcilliationConsumer.Dtos;

namespace ConcilliationConsumer .DTOs;
public class PostConcilliationBody(Response outputVar)
{
    public List<BaseJson> DatabaseToFile = outputVar.DatabaseToFile.Select(pair => new BaseJson(pair.Key, pair.Value)).ToList();
    public List<BaseJson> FileToDatabase = outputVar.FileToDatabase.Select(pair => new BaseJson(pair.Key, pair.Value)).ToList();
    public List<BaseJson> DifferentStatus = outputVar.DifferentStatus.Select(pair => new BaseJson(pair.Key, pair.Value)).ToList();
}