using System;
using System.Collections.Concurrent;
using System.Configuration;

namespace Demo.Upserts.Command
{
    public class State
    {
        public static readonly int MaxNumber = GetConfigValue<int>("maxNumber");
        public static readonly int NumberOfAccountBuckets = GetConfigValue<int>("numberOfAccountBuckets");
        public static readonly int NumberOfMessages = GetConfigValue<int>("numberOfMessages");
        public static readonly int NumberOfWorkers = GetConfigValue<int>("numberOfWorkers");
        public static readonly int CommandTimeoutInSeconds = GetConfigValue<int>("commandTimeoutInSeconds");
        public static readonly int DelayInSeconds = GetConfigValue<int>("delayInSeconds");
        public static readonly int NumberOfChaosPoints = GetConfigValue<int>("numberOfChaosPoints");
        public static int MessageCount = 0;

        public static State Instance { get; }
        private readonly ConcurrentQueue<Message> _queue;
        private readonly ConcurrentDictionary<string, int> _dictionary; 
        private readonly int[] _counts; 
      
        static State()
        {
            Instance = new State();
        }

        private static T GetConfigValue<T>(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            return (T)Convert.ChangeType(value, typeof(T));
        }

        private State()
        {
            _queue = new ConcurrentQueue<Message>();
            _dictionary = new ConcurrentDictionary<string, int>();
            _counts = new int[MaxNumber];
        }

        public void Enqueue(int accountNumber, bool introduceChaos)
        {
            _counts[accountNumber] = _counts[accountNumber] + 1;

            if (introduceChaos == false)
            {
                var message = new Message { AccountNumber = accountNumber, Version = (Version)_counts[accountNumber] };
                _queue.Enqueue(message);
            }
            else
            {
                Console.WriteLine($"Introduce chaos point for account number {accountNumber}.");
            }
        }

        public void Requeue(Message message)
        {
            _queue.Enqueue(message);
        }

        public Message Dequeue()
        {
            Message message;
            return _queue.TryDequeue(out message) ? message : null;
        }

        const string ChaosPointsCounterKey = "ChaosPointsCounterKey";

        public int ChaosPointsCounter
        {
            get
            {
                int value;
                _dictionary.TryGetValue(ChaosPointsCounterKey, out value);
                return value;
            }
            set
            {
                _dictionary.AddOrUpdate(ChaosPointsCounterKey, value, (key, existing) => value);
            }
        }
    }
}