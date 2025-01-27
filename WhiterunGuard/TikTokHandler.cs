using System.Diagnostics;
using System.Runtime.InteropServices;
using Python.Runtime;

namespace WhiterunGuard
{
    public sealed class TikTokHandler : BaseThread
    {
        private readonly IntPtr _allowThread;

        private readonly dynamic _tiktok;
        private bool _isOnline;

        public EventHandler<bool> LiveStarted;

        public TikTokHandler()
        {
            Runtime.PythonDLL = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? @"C:\Users\JonathanA\AppData\Local\Programs\Python\Python312\python312.dll"
                : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                    ? @"/Library/Frameworks/Python.framework/Versions/3.10/Python"
                    : GetLinuxPath();

            PythonEngine.Initialize();
            _allowThread = PythonEngine.BeginAllowThreads();
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Python"));
                _tiktok = Py.Import("TikTok");
            }

            NextTime = DateTime.UtcNow.TimeAccurateToMinutes().AddMinutes(1);
            SleepPeriod = 60000;
            Start();
        }

        public override void Stop()
        {
            PythonEngine.EndAllowThreads(_allowThread);
            base.Stop();
        }

        protected override void PerformTask()
        {
            var live = false;
            using (Py.GIL())
            {
                try
                {
                    var client = _tiktok.client;
                    live = (bool)_tiktok.check_live(client);
                }
                catch (Exception e)
                {
                    Console.Write(e);
                }

                if (live && !_isOnline)
                {
                    _isOnline = true;
                    LiveStarted?.Invoke(this, true);
                }
                else if (!live && _isOnline)
                {
                    _isOnline = false;
                    LiveStarted?.Invoke(this, false);
                }
            }
        }

        private static string GetLinuxPath()
        {
            var libpath = Path.Combine("/usr", "lib",
                $"{(RuntimeInformation.ProcessArchitecture == Architecture.Arm ? "aarch64" : "x86_64")}-linux-gnu");


            var p = new Process();
            p.StartInfo.FileName = "/bin/bash";
            p.StartInfo.Arguments = $"-c \"python3 --version\"";
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();

            var output = p.StandardOutput.ReadToEnd().Replace("\r", "").Replace("\n", "").Split(' ')[1].Split('.');
            p.WaitForExit();

            return Path.Combine(libpath, $"libpython{output[0]}.{output[1]}.so");
        }
    }
}