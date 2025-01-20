using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SeriesAI
{
    public class SampleValidationMethod
    {
        private readonly Sample sample;
        private readonly Stack<int> stack;
        private readonly QueuedLock queuedLock;
        private readonly CancellationTokenSource cancellationTokenSource;

        public SampleValidationMethod(Sample sample)
        {
            this.sample = sample;
            stack = new Stack<int>();
            queuedLock = new QueuedLock();
            cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task ExcecuteAsync()
        {
            LogInput();

            var pushTask = RunPushTask();
            var popTask = RunPopTask();

            await Task.WhenAll(popTask);

            CancelIfFailed(popTask.Result);

            await Task.WhenAll(pushTask);

            LogResult(popTask.Result);
        }

        private Task RunPushTask()
        {
            var numberPushMethod = new NumberPushMethod(stack, lastNumber: sample.N,
                sample.PushMillisecondsDelay, queuedLock, cancellationTokenSource.Token);

            var pushTask = Task.Run(numberPushMethod.ExecuteAsync);

            return pushTask;
        }

        private Task<bool> RunPopTask()
        {
            var numberPopMethod = new NumberPopMethod(stack, numbers: sample.Input,
                sample.PopMillisecondsDelays, queuedLock, cancellationTokenSource.Token);

            var popTask = Task.Run(numberPopMethod.ExecuteAsync);

            return popTask;
        }

        private void CancelIfFailed(bool result)
        {
            if (result == false && !cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
            }
        }

        private void LogInput()
        {
            Console.WriteLine($"{sample.Name} input:");
            Console.WriteLine(string.Join(" ", sample.Input));
            Console.WriteLine("Explanation:");
        }

        private void LogResult(bool result)
        {
            Console.WriteLine($"{sample.Name} output: {result}");

            if (result == false)
            {
                Console.WriteLine("So this is invalid.");
            }

            Console.WriteLine();
        }
    }
}
