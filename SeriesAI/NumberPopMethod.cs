using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SeriesAI
{
    public class NumberPopMethod
    {
        private readonly Stack<int> stack;
        private readonly int[] numbers;
        private readonly int[] millisecondsDelays;
        private readonly QueuedLock queuedLock;
        private readonly CancellationToken cancellationToken;

        public NumberPopMethod(Stack<int> stack, int[] numbers,
            int[] millisecondsDelays, QueuedLock queuedLock, CancellationToken cancellationToken)
        {
            this.stack = stack;
            this.numbers = numbers;
            this.millisecondsDelays = millisecondsDelays;
            this.queuedLock = queuedLock;
            this.cancellationToken = cancellationToken;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (stack == null || numbers == null || millisecondsDelays == null || queuedLock == null)
            {
                throw new ArgumentNullException();
            }

            if (numbers.Length != millisecondsDelays.Length)
            {
                throw new ArgumentException("Length of numbers and milliseconds delays is not equal!");
            }

            for (int i = 0; i < numbers.Length; ++i)
            {
                await SilentDelay(millisecondsDelays[i]);

                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                if (!TrySafePop(numbers[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private async Task SilentDelay(int millisecondsDelay)
        {
            try
            {
                await Task.Delay(millisecondsDelay, cancellationToken);
            }
            catch (TaskCanceledException) { }
        }

        private bool TrySafePop(int number)
        {
            queuedLock.Enter();

            var isPopValid = !cancellationToken.IsCancellationRequested && TryPop(number);

            queuedLock.Exit();

            return isPopValid;
        }

        private bool TryPop(int number)
        {
            var isPopValid = stack.Count > 0 && stack.Peek() == number;

            if (isPopValid)
            {
                stack.Pop();
            }

            LogTryPop(isPopValid, number);

            return isPopValid;
        }

        private void LogTryPop(bool isValid, int number)
        {
            if (isValid)
            {
                Console.WriteLine($"pop {number}");
            }
            else
            {
                Console.WriteLine($"We can't pop {number} now because at this point the stack is:");
                Console.WriteLine($"[{string.Join(" ", stack.Reverse())} (top)]");
            }
        }
    }
}
