using System;
using System.Threading;

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

            DoWork();

            _repository.Increment(message.AccountNumber);
            Interlocked.Increment(ref State.MessageCount);
            Console.Write($"\r{State.MessageCount,10:N0} message(s) processed");

            return true;
        }

        private static void DoWork()
        {
            Thread.Sleep(TimeSpan.FromSeconds(State.DelayInSeconds));
        }
    }
}