using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SeriesAI
{
    [Obsolete("More functional programming implementation. Check Program for more oop version.")]
    public class ProgramObsolete
    {
        private static void MainObsolete(string[] args)
        {
            SampleRunnerAsync();
            Console.Read();
        }

        private static async void SampleRunnerAsync()
        {
            await Sample1();
            await Sample2();
        }

        private static async Task Sample1()
        {
            var sample = new Sample()
            {
                Name = "Sample 1",
                N = 3,
                Input = new int[] { 0, 1, 2, 3 },
                PushMillisecondsDelay = 100,
                PopMillisecondsDelays = new int[] { 150, 100, 100, 100 },
            };

            await ValidateAsync(sample);
        }

        private static async Task Sample2()
        {
            var sample = new Sample()
            {
                Name = "Sample 2",
                N = 10,
                Input = new int[] { 0, 3, 1, 2 },
                PushMillisecondsDelay = 100,
                PopMillisecondsDelays = new int[] { 150, 300, 50, 100 },
            };

            await ValidateAsync(sample);
        }

        private static async Task ValidateAsync(Sample sample)
        {
            Console.WriteLine($"{sample.Name} input:");
            Console.WriteLine(string.Join(" ", sample.Input));
            Console.WriteLine("Explanation:");

            var stack = new Stack<int>();
            var queuedLock = new QueuedLock();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var pushTask = Task.Run(() => PushSequentialNumbersAsync(
                stack, sequenceLength: sample.N, sample.PushMillisecondsDelay,
                queuedLock, cancellationTokenSource.Token));

            var popTask = Task.Run(() => PopAsync(
                stack, numbers: sample.Input, sample.PopMillisecondsDelays,
                queuedLock, cancellationTokenSource.Token));

            await Task.WhenAll(popTask);

            if (popTask.Result == false && !cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
            }

            await Task.WhenAll(pushTask);

            Console.WriteLine($"{sample.Name} output: {popTask.Result}");
            if (popTask.Result == false) Console.WriteLine("So this is invalid.");
            Console.WriteLine();
        }

        private static async Task PushSequentialNumbersAsync(Stack<int> stack, int sequenceLength,
            int millisecondsDelay, QueuedLock queuedLock, CancellationToken cancellationToken)
        {
            if (stack == null || queuedLock == null)
            {
                throw new ArgumentNullException();
            }

            for (int i = 0; i <= sequenceLength; ++i)
            {
                try
                {
                    await Task.Delay(millisecondsDelay, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    return;
                }

                queuedLock.Enter();

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                stack.Push(i);
                Console.WriteLine($"push {i}");

                queuedLock.Exit();
            }
        }

        private static async Task<bool> PopAsync(Stack<int> stack, int[] numbers,
            int[] millisecondsDelays, QueuedLock queuedLock, CancellationToken cancellationToken)
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
                try
                {
                    await Task.Delay(millisecondsDelays[i], cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    return false;
                }

                queuedLock.Enter();

                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                if (stack.Any() && stack.Peek() == numbers[i])
                {
                    var poppedNumber = stack.Pop();
                    Console.WriteLine($"pop {poppedNumber}");
                }
                else
                {
                    Console.WriteLine($"We can't pop {numbers[i]} now because at this point the stack is:");
                    Console.WriteLine($"[{string.Join(" ", stack.Reverse())} (top)]");
                    queuedLock.Exit();
                    return false;
                }

                queuedLock.Exit();
            }

            return true;
        }
    }
}
