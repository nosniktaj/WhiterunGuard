using System.Text;

namespace WhiterunGuard
{
    public class ConsoleCommandHandler
    {
        #region Private Fields

        private readonly StringBuilder _consoleString = new();

        #endregion

        #region Public Proprties

        public bool Running = true;

        #endregion

        #region Constructor

        public ConsoleCommandHandler() => Task.Run(Run);

        #endregion

        #region Public Methods

        public void WriteConsoleLine(string line)
        {
            Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r> ");
            Console.WriteLine(line);
            Console.Write($"> {_consoleString}");
        }

        #endregion

        private void Run()
        {
            while (Running)
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.Write("\n> ");
                        ProcessConsoleLine(_consoleString.ToString());
                    }
                    else if (key.Key == ConsoleKey.Backspace && _consoleString.Length > 0)
                    {
                        _consoleString.Remove(_consoleString.Length - 1, 1);
                        UpdateConsoleLine();
                    }
                    else if (key.Key != ConsoleKey.Backspace)
                    {
                        _consoleString.Append(key.KeyChar);
                        Console.Write(key.KeyChar);
                    }
                }
        }

        private void UpdateConsoleLine()
        {
            Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r> ");
            Console.Write(_consoleString.ToString());
        }


        private void ProcessConsoleLine(string line)
        {
            _consoleString.Clear();
            var parts = line.Split(' ', 2);
            switch (parts[0].ToLowerInvariant())
            {
                case "end":
                    EndApplication();
                    break;
                case "ver":
                    WriteConsoleLine("Version 1.0.0");
                    break;
            }
        }

        private void EndApplication()
        {
            var confirmed = false;
            while (confirmed == false)
            {
                WriteConsoleLine("Are you sure you want to end the application? (Y/N)");
                var key = Console.ReadKey(true);
                WriteConsoleLine(key.KeyChar.ToString());
                switch (key.Key)
                {
                    case ConsoleKey.Y:
                        WriteConsoleLine("App Ending...");
                        confirmed = true;
                        Running = false;
                        break;
                    case ConsoleKey.N:
                        WriteConsoleLine("App not ending");
                        confirmed = true;
                        break;
                    default:
                        WriteConsoleLine("Invalid Key");
                        break;
                }
            }
        }
    }
}