namespace Audit;

public static class ConsoleHelpers
{
    public static async Task WriteLineGreen(string message)
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        await Console.Out.WriteLineAsync(message);
        Console.ForegroundColor = foregroundColor;
    }

    public static async Task WriteLineYellow(string message)
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        await Console.Out.WriteLineAsync(message);
        Console.ForegroundColor = foregroundColor;
    }

    public static async Task WriteLineRed(string message)
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        await Console.Error.WriteLineAsync(message);
        Console.ForegroundColor = foregroundColor;
    }
}
