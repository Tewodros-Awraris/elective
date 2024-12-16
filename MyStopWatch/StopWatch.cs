using System;
using System.Threading;
using System.Threading.Tasks;

public class Stopwatch
{
    public TimeSpan TimeElapsed { get; private set; } = TimeSpan.Zero;
    private bool _isRunning = false;
    private CancellationTokenSource _cancellationTokenSource;

    public delegate void StopwatchEventHandler(string message);
    public event StopwatchEventHandler OnStarted;
    public event StopwatchEventHandler OnStopped;
    public event StopwatchEventHandler OnReset;

    public delegate void TimeUpdatedHandler(TimeSpan time);
    public event TimeUpdatedHandler OnTimeUpdated;

    public void Start()
    {
        if (!_isRunning)
        {
            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();
            OnStarted?.Invoke("Stopwatch Started!");
            Task.Run(() => Tick(_cancellationTokenSource.Token));
        }
    }

    public void Stop()
    {
        if (_isRunning)
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();
            OnStopped?.Invoke("Stopwatch Stopped!");
        }
    }

    public void Reset()
    {
        Stop();
        TimeElapsed = TimeSpan.Zero;
        OnReset?.Invoke("Stopwatch Reset!");
    }

    private async Task Tick(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
                TimeElapsed = TimeElapsed.Add(TimeSpan.FromSeconds(1));
                OnTimeUpdated?.Invoke(TimeElapsed);
            }
        }
        catch (TaskCanceledException)
        {
            
        }
    }
}
