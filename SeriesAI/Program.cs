using System;

namespace SeriesAI
{
    public class Program
    {
        private static void Main(string[] args)
        {
            SampleRunnerAsync();
            Console.Read();
        }

        private static async void SampleRunnerAsync()
        {
            var samples = SampleGeneratorMethods.CreateSamples();

            foreach (var sample in samples)
            {
                var sampleValidationMethod = new SampleValidationMethod(sample);
                await sampleValidationMethod.ExcecuteAsync();
            }
        }
    }
}
