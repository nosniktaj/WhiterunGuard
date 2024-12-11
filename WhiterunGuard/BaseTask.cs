using System.Runtime.CompilerServices;

namespace WhiterunGuard;

public enum StartTime
{
    Now,
    NextMinute,
    NextHour
}
public class BaseTask : IDisposable
{ 
    #region Protected Properties
    protected int Delay { get; set; } = 5000;
    
    protected StartTime StartTime { get; set; } = StartTime.Now;
    
    #endregion
    
    #region Private Fields
    
    private Task _task = null!;
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    
    #endregion

    #region Public Methods
    public void Start()
    {
        _task = Task.Run(() => RunTask(_cancellationTokenSource.Token));
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel(); // Signal the task to stop
        try
        {
            _task.Wait(); // Wait for the task to complete
        }
        catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
        {
            // Ignore TaskCanceledException
        }
        finally
        {
            _cancellationTokenSource.Dispose();
        }
    }
    
    #endregion

    #region Protected Methods
    protected virtual void PerformAction()
    {
        
    }
    
    #endregion
    
    #region Private Methods
    private async Task RunTask(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                switch (this.StartTime)
                {
                    case StartTime.NextMinute:
                        while (DateTime.UtcNow.Second != 0)
                        {
                            await Task.Delay(100, token);
                        }

                        break;
                    case StartTime.NextHour:
                        while (DateTime.UtcNow.Minute != 0)
                        {
                            await Task.Delay(1000, token);
                        }
                        break;
                }
                PerformAction();
                await Task.Delay(Delay, token); // Use token to allow cancellation
            }
        }
        catch (TaskCanceledException)
        {
        }
    }
    
    #endregion
    

}