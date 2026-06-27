using NetworkAttackDetectionPlatform.MachineLearning.Testing;

namespace NetworkAttackDetectionPlatform.MachineLearning.Testing;

/// <summary>
/// Console runner for CICIDS2017 smoke test.
/// Execute this to verify the complete ML pipeline.
/// </summary>
public class SmokeTestRunner
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("CICIDS2017 Training Pipeline Smoke Test Runner");
        Console.WriteLine("============================================");
        Console.WriteLine();

        // Parse command line arguments
        var config = ParseArguments(args);

        // Display configuration
        Console.WriteLine("Test Configuration:");
        Console.WriteLine($"  Dataset Path: {config.DatasetPath}");
        Console.WriteLine($"  Sample Size: {config.SampleSize}");
        Console.WriteLine($"  Number of Trees: {config.NumberOfTrees}");
        Console.WriteLine($"  Model Output: {config.ModelOutputPath}");
        Console.WriteLine();

        Console.WriteLine("Press any key to start the smoke test...");
        Console.ReadKey();
        Console.WriteLine();

        // Create and run smoke test
        var smokeTest = new Cicids2017TrainingSmokeTest(config);
        var result = await smokeTest.RunAsync();

        // Display final result
        if (result.Success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("????????????????????????????????????????");
            Console.WriteLine("  ? SMOKE TEST PASSED");
            Console.WriteLine("????????????????????????????????????????");
            Console.ResetColor();
            Environment.Exit(0);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("????????????????????????????????????????");
            Console.WriteLine("  ? SMOKE TEST FAILED");
            Console.WriteLine("????????????????????????????????????????");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine($"Error: {result.Message}");
            Environment.Exit(1);
        }
    }

    private static Cicids2017TestConfiguration ParseArguments(string[] args)
    {
        var config = Cicids2017TestConfiguration.CreateDefault();

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "--dataset":
                case "-d":
                    if (i + 1 < args.Length)
                    {
                        config.DatasetPath = args[++i];
                    }
                    break;

                case "--samples":
                case "-s":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int samples))
                    {
                        config.SampleSize = samples;
                    }
                    break;

                case "--trees":
                case "-t":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int trees))
                    {
                        config.NumberOfTrees = trees;
                    }
                    break;

                case "--output":
                case "-o":
                    if (i + 1 < args.Length)
                    {
                        config.ModelOutputPath = args[++i];
                    }
                    break;

                case "--help":
                case "-h":
                    PrintHelp();
                    Environment.Exit(0);
                    break;
            }
        }

        return config;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("CICIDS2017 Smoke Test Runner - Command Line Arguments");
        Console.WriteLine();
        Console.WriteLine("Usage: SmokeTestRunner [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -d, --dataset <path>    Path to CICIDS2017 CSV file");
        Console.WriteLine("  -s, --samples <count>   Number of samples to use (default: 2000)");
        Console.WriteLine("  -t, --trees <count>     Number of trees for FastTree (default: 50)");
        Console.WriteLine("  -o, --output <path>     Model output path (default: ./models/test_model.zip)");
        Console.WriteLine("  -h, --help              Show this help message");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  SmokeTestRunner");
        Console.WriteLine("  SmokeTestRunner -d ./data/cicids2017.csv -s 5000");
        Console.WriteLine("  SmokeTestRunner --dataset real_data.csv --trees 100");
    }
}
