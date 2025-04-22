namespace MineSweeper.Models;

/// <summary>
///     Simple logging interface for debugging
/// </summary>
public interface ILogger
{
    /// <summary>
    ///     Logs an informational message
    /// </summary>
    /// <param name="message">The message to log</param>
    void Log(string message);

    /// <summary>
    ///     Logs an error message
    /// </summary>
    /// <param name="message">The error message to log</param>
    void LogError(string message);

    /// <summary>
    ///     Logs a warning message
    /// </summary>
    /// <param name="message">The warning message to log</param>
    void LogWarning(string message);
}

/// <summary>
///     Console implementation of ILogger that writes messages to the console
/// </summary>
public class ConsoleCustomDebugLogger : ILogger
{
    /// <summary>
    ///     Logs an informational message to the console
    /// </summary>
    /// <param name="message">The message to log</param>
    public void Log(string message)
    {
        Console.WriteLine($"[INFO] {message}");
    }

    /// <summary>
    ///     Logs an error message to the console
    /// </summary>
    /// <param name="message">The error message to log</param>
    public void LogError(string message)
    {
        Console.WriteLine($"[ERROR] {message}");
    }

    /// <summary>
    ///     Logs a warning message to the console
    /// </summary>
    /// <param name="message">The warning message to log</param>
    public void LogWarning(string message)
    {
        Console.WriteLine($"[WARNING] {message}");
    }
}

/// <summary>
///     Custom debug customDebugLogger implementation to demonstrate dependency injection is working
/// </summary>
/// <remarks>
///     This customDebugLogger prefixes messages with "CUSTOM" to distinguish them from the standard customDebugLogger
/// </remarks>
public class CustomDebugLogger : ILogger
{
    /// <summary>
    ///     Logs an informational message with a custom prefix
    /// </summary>
    /// <param name="message">The message to log</param>
    public void Log(string message)
    {
        Console.WriteLine($"[CUSTOM-DEBUG] {message}");
    }

    /// <summary>
    ///     Logs an error message with a custom prefix
    /// </summary>
    /// <param name="message">The error message to log</param>
    public void LogError(string message)
    {
        Console.WriteLine($"[CUSTOM-ERROR] {message}");
    }

    /// <summary>
    ///     Logs a warning message with a custom prefix
    /// </summary>
    /// <param name="message">The warning message to log</param>
    public void LogWarning(string message)
    {
        Console.WriteLine($"[CUSTOM-WARNING] {message}");
    }
}