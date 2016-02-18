using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Upserts.Command
{
    public class Command
    {
        public void Execute()
        {
            PrepareDatabaseFor(State.NumberOfAccountBuckets);
            var message1 = $@"Table truncated and {State.NumberOfAccountBuckets:#,##0} account(s) loaded with version 0.";
            Console.WriteLine(message1);
            Console.WriteLine();

            LoadMessages(State.NumberOfMessages, State.MaxNumber);
            var elapsed = ProcessMessages(State.NumberOfMessages, State.NumberOfWorkers);

            var message2 = $@"{State.NumberOfMessages:#,###} message(s) processed
with numbers from 0 - {State.MaxNumber:#,###}
in {elapsed.TotalSeconds:n2} second(s) with {State.NumberOfWorkers} worker(s).";

            Console.WriteLine(message2);
            Console.ReadLine();
        }

        private void PrepareDatabaseFor(int numberOfAccountBuckets)
        {
            var repository = new AccountSequencesRepository();
            repository.RemoveCounts();
            repository.LoadEmptyAccounts(numberOfAccountBuckets);
        }

        private void LoadMessages(int numberOfMessages, int maxNumber)
        {
            foreach (var count in Enumerable.Range(0, numberOfMessages))
            {
                var randomAccountNumber = GetRandomAccountNumber(maxNumber);
                State.Instance.Enqueue(randomAccountNumber);
            }
        }

        private TimeSpan ProcessMessages(int numberOfMessages, int numberOfWorkers)
        {
            var watch = new Stopwatch();
            watch.Start();

            var options = new ParallelOptions { MaxDegreeOfParallelism = numberOfWorkers };
            Parallel.Invoke(options,
                Enumerable.Range(0, numberOfMessages)
                    .Select(count => (Action)ProcessNextMessage)
                    .ToArray()
                );

            watch.Stop();
            return watch.Elapsed;
        }

        private void ProcessNextMessage()
        {
            var message = State.Instance.Dequeue();
            var processor = new MessageProcessor(new AccountSequencesRepository());
            processor.Process(message);
        }

        private int GetRandomAccountNumber(int maxNumber)
        {
            return Math.Abs(Guid.NewGuid().GetHashCode() % maxNumber);
        }
    }
}