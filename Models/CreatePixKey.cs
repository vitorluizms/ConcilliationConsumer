namespace ConcilliationConsumer.Entities
{
    public class PixKeyCreationData
    {
        public required string Type { get; set; }
        public required string Value { get; set; }
        public required string CPF { get; set; }
        public required int Number { get; set; }
        public required int Agency { get; set; }
    }

    public class PixKeyGetByValueData
    {
        public required string Type { get; set; }
        public required string Value { get; set; }
    }
}