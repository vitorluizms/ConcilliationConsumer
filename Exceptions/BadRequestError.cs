namespace ConcilliationConsumer.Exceptions;

public class BadRequestError(string message) : Exception(message)
{
}