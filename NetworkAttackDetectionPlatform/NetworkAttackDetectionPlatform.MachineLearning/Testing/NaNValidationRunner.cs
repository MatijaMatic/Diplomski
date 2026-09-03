using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetworkAttackDetectionPlatform.MachineLearning.Testing;

namespace NetworkAttackDetectionPlatform.MachineLearning
{
    /// <summary>
    /// Simple runner to execute the NaN handling validation.
    /// Can be used to verify the prediction pipeline handles missing features correctly.
    /// </summary>
    public class NaNValidationRunner
    {
        public static async Task Main(string[] args)
        {
            // Create a simple console logger
            var logger = new ConsoleLogger();

            var validator = new PredictionNaNHandlingValidation(logger);

            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine("ML Prediction Pipeline - NaN Validation");
            Console.WriteLine("========================================");
            Console.WriteLine();

            var result = await validator.ValidateNaNHandlingAsync(
                modelPath: string.Empty, // Not needed for this validation
                modelExists: false
            );

            Console.WriteLine();
            Console.WriteLine("Result Summary:");
            Console.WriteLine($"  Success: {result.Success}");
            Console.WriteLine($"  Duration: {result.Duration?.TotalMilliseconds:F0}ms");
            if (result.Errors.Count > 0)
            {
                Console.WriteLine($"  Errors ({result.Errors.Count}):");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"    - {error}");
                }
            }
            if (result.Warnings.Count > 0)
            {
                Console.WriteLine($"  Warnings ({result.Warnings.Count}):");
                foreach (var warning in result.Warnings)
                {
                    Console.WriteLine($"    - {warning}");
                }
            }

            Console.WriteLine();
            Environment.Exit(result.Success ? 0 : 1);
        }
    }

    /// <summary>
    /// Simple console-based logger implementation.
    /// </summary>
    public class ConsoleLogger : ILogger<PredictionNaNHandlingValidation>
    {
        public IDisposable BeginScope<TState>(TState state) => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Console.WriteLine($"[{logLevel}] {formatter(state, exception)}");
        }
    }
}
