using System.Diagnostics;

namespace MineSweeper.Services.Logging;

/// <summary>
///     Console implementation of ILogger that writes messages to the console
/// </summary>
public class ConsoleLogger : ILogger
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
///     Debug logger implementation that writes to Debug output
/// </summary>
public class DebugLogger : ILogger
{
    /// <summary>
    ///     Logs an informational message
    /// </summary>
    /// <param name="message">The message to log</param>
    public void Log(string message)
    {
        Debug.WriteLine($"[INFO] {message}");
    }

    /// <summary>
    ///     Logs an error message
    /// </summary>
    /// <param name="message">The error message to log</param>
    public void LogError(string message)
    {
        Debug.WriteLine($"[ERROR] {message}");
    }

    /// <summary>
    ///     Logs a warning message
    /// </summary>
    /// <param name="message">The warning message to log</param>
    public void LogWarning(string message)
    {
        Debug.WriteLine($"[WARNING] {message}");
    }
}

/// <summary>
///     Custom debug logger implementation to demonstrate dependency injection is working
/// </summary>
/// <remarks>
///     This logger prefixes messages with "CUSTOM" to distinguish them from the standard logger
/// </remarks>
public class CustomDebugLogger : ILogger
{
    /// <summary>
    ///     Logs an informational message with a custom prefix
    /// </summary>
    /// <param name="message">The message to log</param>
    public void Log(string message)
    {
        Debug.WriteLine($"[CUSTOM-DEBUG] {message}");
    }

    /// <summary>
    ///     Logs an error message with a custom prefix
    /// </summary>
    /// <param name="message">The error message to log</param>
    public void LogError(string message)
    {
        Debug.WriteLine($"[CUSTOM-ERROR] {message}");
    }

    /// <summary>
    ///     Logs a warning message with a custom prefix
    /// </summary>
    /// <param name="message">The warning message to log</param>
    public void LogWarning(string message)
    {
        Debug.WriteLine($"[CUSTOM-WARNING] {message}");
    }
}
