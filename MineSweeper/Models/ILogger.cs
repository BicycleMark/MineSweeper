namespace MineSweeper.Models;

/// <summary>
/// Simple logging interface for debugging
/// </summary>
public interface ILogger
{
    void Log(string message);
    void LogError(string message);
    void LogWarning(string message);
}

/// <summary>
/// Console implementation of ILogger
/// </summary>
public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[INFO] {message}");
    }

    public void LogError(string message)
    {
        Console.WriteLine($"[ERROR] {message}");
    }

    public void LogWarning(string message)
    {
        Console.WriteLine($"[WARNING] {message}");
    }
}
