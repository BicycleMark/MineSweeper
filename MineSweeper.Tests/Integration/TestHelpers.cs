using System.Collections.ObjectModel;
using Microsoft.Maui.Dispatching;
using MineSweeper.Services.Logging;

namespace MineSweeper.Tests.Integration;

/// <summary>
/// Test implementation of ILogger for integration testing
/// </summary>
public class TestLogger : ILogger
{
    public List<string> LogMessages { get; } = new();
    public List<string> ErrorMessages { get; } = new();
    public List<string> WarningMessages { get; } = new();

    public void Log(string message)
    {
        LogMessages.Add(message);
        Console.WriteLine($"[TEST-LOG] {message}");
    }

    public void LogError(string message)
    {
        ErrorMessages.Add(message);
        Console.WriteLine($"[TEST-ERROR] {message}");
    }

    public void LogWarning(string message)
    {
        WarningMessages.Add(message);
        Console.WriteLine($"[TEST-WARNING] {message}");
    }
}

/// <summary>
/// Test implementation of IDispatcher for integration testing
/// </summary>
public class TestDispatcher : IDispatcher
{
    private readonly TestDispatcherTimer _timer = new();
    public bool IsDispatchRequired => false;

    public IDispatcherTimer CreateTimer()
    {
        return _timer;
    }

    public bool Dispatch(Action action)
    {
        action();
        return true;
    }

    public bool DispatchDelayed(TimeSpan delay, Action action)
    {
        action();
        return true;
    }
}

/// <summary>
/// Test implementation of IDispatcherTimer for integration testing
/// </summary>
public class TestDispatcherTimer : IDispatcherTimer
{
    public TimeSpan Interval { get; set; }
    public bool IsRunning { get; private set; }
    public bool IsRepeating { get; set; } = true;
    public event EventHandler? Tick;

    public void Start()
    {
        IsRunning = true;
    }

    public void Stop()
    {
        IsRunning = false;
    }

    public void SimulateTick()
    {
        Tick?.Invoke(this, EventArgs.Empty);
    }
}
