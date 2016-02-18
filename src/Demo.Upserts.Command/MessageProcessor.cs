namespace Demo.Upserts.Command
{
    public class MessageProcessor
    {
        private readonly IAccountSequencesRepository _repository;

        public MessageProcessor(IAccountSequencesRepository repository)
        {
            _repository = repository;
        }

        public bool Process(Message message)
        {
            while (!message.IsNextToBeProcessed(_repository.GetVersionFor(message.AccountNumber)))
            {}

            _repository.Increment(message.AccountNumber);
            return true;
        }
    }
}