using System.Collections.Generic;

namespace SeriesAI
{
    public class SampleGeneratorMethods
    {
        public static IEnumerable<Sample> CreateSamples() => new[]
        {
            new Sample()
            {
                Name = "Sample 1",
                N = 3,
                Input = new int[] { 0, 1, 2, 3 },
                PushMillisecondsDelay = 100,
                PopMillisecondsDelays = new int[] { 150, 100, 100, 100 },
            },
            new Sample()
            {
                Name = "Sample 2",
                N = 10,
                Input = new int[] { 0, 3, 1, 2 },
                PushMillisecondsDelay = 100,
                PopMillisecondsDelays = new int[] { 150, 300, 50, 100 },
            },
        };
    }
}
