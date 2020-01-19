using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using CommandLine;
using Morr.Core.CLI.Commands.ValidateConfig;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Events.Queue.Strategy;

namespace MORR.Core.CLI
{
    /// <summary>
    /// Command line tool entry point
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            // return Parser.Default.ParseArguments<ValidateConfigOptions>(args)
            //            .MapResult(
            //                (ValidateConfigOptions opts) => new ValidateConfigCommand().Execute(opts),
            //                errs => 1);

            try
            {
                Listen().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an exception: {ex.ToString()}");
            }
        }

        private static async Task Listen()
        {
            var queue = new EventQueue<TestEvent>(new RefCountedUnboundedStrategy<TestEvent>());

            var producers = new List<Producer>();
            var consumers = new List<Consumer>();

            var consumingTasks = new List<Task>();
            var producingTasks = new List<Task>();

            for (int i = 0; i < 50; i++)
            {
                producers.Add(new Producer(queue, i, 10));
                consumers.Add(new Consumer(queue, i, 0));
            }

            consumers.ForEach(consumer => consumingTasks.Add(consumer.ConsumeData()));
            producers.ForEach(producer => producingTasks.Add(producer.BeginProducing()));

            await Task.WhenAll(consumingTasks);
        }
    }

    public class TestEvent : Event
    {
        public readonly int _identifier;
        public readonly string _msg;
        public readonly int _num;

        public TestEvent(int identifier, string msg, int num)
        {
            _identifier = identifier;
            _msg = msg;
            _num = num;
        }

        public override void Deserialize(string serialized)
        {
            throw new System.NotImplementedException();
        }

        public override string Serialize()
        {
            throw new System.NotImplementedException();
        }
    }

    internal class Producer
    {
        private readonly EventQueue<TestEvent> _queue;
        private readonly int _identifier;
        private readonly int _delay;

        public Producer(EventQueue<TestEvent> queue, int identifier, int delay)
        {
            _queue = queue;
            _identifier = identifier;
            _delay = delay;
        }

        public async Task BeginProducing()
        {
            Console.WriteLine($"PRODUCER ({_identifier}): Starting");

            for (var i = 0; i < 1000; i++)
            {
                await Task.Delay(_delay); // simulate producer building/fetching some data

                var msg = $"P{_identifier} - {DateTime.UtcNow:G}";

                // Console.WriteLine($"PRODUCER ({_identifier}): Creating {msg}");

                _queue.Enqueue(new TestEvent(_identifier, msg, i));
            }

            Console.WriteLine($"PRODUCER ({_identifier}): Completed");
        }
    }

    internal class Consumer
    {
        private readonly EventQueue<TestEvent> _queue;
        private readonly int _identifier;
        private readonly int _delay;

        public Consumer(EventQueue<TestEvent> queue, int identifier, int delay)
        {
            _queue = queue;
            _identifier = identifier;
            _delay = delay;
        }

        public async Task ConsumeData()
        {
            Console.WriteLine($"CONSUMER ({_identifier}): Starting");

            var i = 0;

            await foreach (var @event in _queue.GetEvents())
            {
                await Task.Delay(_delay); // simulate processing time
                i++;
                // Console.WriteLine($"CONSUMER ({_identifier}, processed: {i}): Consuming {@event._msg} from {@event._identifier} num: {@event._num}");
            }

            Console.WriteLine($"CONSUMER ({_identifier}): Completed");
        }
    }
}
