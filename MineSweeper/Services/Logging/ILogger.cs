namespace MineSweeper.Services.Logging;

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
