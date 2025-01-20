using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SeriesAI
{
    public class NumberPushMethod
    {
        private readonly Stack<int> stack;
        private readonly int lastNumber;
        private readonly int millisecondsDelay;
        private readonly QueuedLock queuedLock;
        private readonly CancellationToken cancellationToken;

        public NumberPushMethod(Stack<int> stack, int lastNumber, 
            int millisecondsDelay, QueuedLock queuedLock, CancellationToken cancellationToken)
        {
            this.stack = stack;
            this.lastNumber = lastNumber;
            this.millisecondsDelay = millisecondsDelay;
            this.queuedLock = queuedLock;
            this.cancellationToken = cancellationToken;
        }

        public async Task ExecuteAsync()
        {
            if (stack == null || queuedLock == null)
            {
                throw new ArgumentNullException();
            }

            for (int number = 0; number <= lastNumber; ++number)
            {
                await SilentDelay();

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                SafePush(number);
            }
        }

        private async Task SilentDelay()
        {
            try
            {
                await Task.Delay(millisecondsDelay, cancellationToken);
            }
            catch (TaskCanceledException) { }
        }

        private void SafePush(int number)
        {
            queuedLock.Enter();

            if (!cancellationToken.IsCancellationRequested)
            {
                stack.Push(number);
                LogPush(number);
            }
            
            queuedLock.Exit();
        }

        private void LogPush(int number)
        {
            Console.WriteLine($"push {number}");
        }
    }
}
