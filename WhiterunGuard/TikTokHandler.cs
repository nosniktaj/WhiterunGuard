using System.Net.Mime;
using System.Runtime.InteropServices;
using Python.Runtime;

namespace WhiterunGuard
{
    public class TikTokHandler : BaseThread
    {

        public EventHandler? LiveStarted; 
        
        private dynamic _tiktok;
        private IntPtr _allowThread;
        private bool _isOnline = false; 
        
        public TikTokHandler()
        {
            
            Runtime.PythonDLL = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? @"C:\Users\JonathanA\AppData\Local\Programs\Python\Python312\python312.dll"
                : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                    ? @"/Library/Frameworks/Python.framework/Versions/3.10/Python"
                    : @"/usr/lib/x86_64-linux-gnu/libpython3.10.so";
            
            PythonEngine.Initialize();
            _allowThread = PythonEngine.BeginAllowThreads();
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Python"));
                _tiktok = Py.Import("TikTok");
            }

            NextTime = DateTime.UtcNow;
            SleepPeriod = 10000;
            Start();
        }
        public override void Stop()
        {
            PythonEngine.EndAllowThreads(_allowThread);
            base.Stop();
        }

        protected override void PerformTask()
        {
            using (Py.GIL())
            {
                var live = false;
                try
                {
                     live = (bool)_tiktok.check_live();
                }
                catch
                {
                    
                }
                LiveStarted?.Invoke(this, EventArgs.Empty);
                if (live && !_isOnline)
                {
                    Console.WriteLine("Online");
                    _isOnline = true;
                    LiveStarted?.Invoke(this, EventArgs.Empty);
                }
                else if (!live && _isOnline)
                {
                    _isOnline = false;
                }
            }
        }
    }
}