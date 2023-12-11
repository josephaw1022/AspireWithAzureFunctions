namespace SharedClassLibraryForConsumerAndProducer
{
    public class ArbitraryMessage
    {
        public string Name { get; set; }

        public string Message { get; set; }

        public static ArbitraryMessage Create(string name, string message)
        {
            return new ArbitraryMessage
            {
                Name = name,
                Message = message
            };
        }
    }
}
