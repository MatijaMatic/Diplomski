namespace NetworkAttackDetectionPlatform.MachineLearning.Testing;

/// <summary>
/// Entry point for CICIDS2017 ML pipeline validation.
/// </summary>
public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var result = await ValidationRunner.RunValidationAsync();
            return result.Success ? 0 : 1;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Fatal error: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            Console.ResetColor();
            return 1;
        }
    }
}
