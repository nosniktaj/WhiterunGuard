using System.Runtime.InteropServices;
using Python.Runtime;

namespace WhiterunGuard
{
    public class TikTokHandler : BaseTask
    {
        private dynamic _tiktok;
        public TikTokHandler()
        {
            Runtime.PythonDLL = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? @"C:\Users\JonathanA\AppData\Local\Programs\Python\Python312\python312.dll"
                : @"/usr/lib/x86_64-linux-gnu/libpython3.10.so";
            PythonEngine.Initialize();

            Delay = 60000;
            StartTime = StartTime.Now;
            Start();
        }

        protected override void PerformAction()
        {
            bool isLive = false;
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Python"));
                dynamic tiktok = Py.Import("TikTok");
                isLive = (bool)tiktok.check_live();
            }

                if (isLive)
                {
                    Console.WriteLine("Live");
                }
                else
                {
                    Console.WriteLine("Not Live");
                }
            
        }
    }
}