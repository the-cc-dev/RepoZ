using System;

namespace SKConsole
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            int pid = -1;
            int wait = 0;
            string keysToSend = "";

            var validArguments = args?.Length == 2 || args?.Length == 3;

            if (validArguments)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    int pidIndex = args[i].IndexOf("pid:", StringComparison.OrdinalIgnoreCase);
                    int waitIndex = args[i].IndexOf("wait:", StringComparison.OrdinalIgnoreCase);
                    if (pidIndex > -1)
                    {
                        var pidString = args[i].Substring(pidIndex + "pid:".Length);
                        int.TryParse(pidString, out pid);
                    }
                    else if (waitIndex > -1)
                    {
                        var waitString = args[i].Substring(waitIndex + "wait:".Length);
                        int.TryParse(waitString, out wait);
                    }
                    else
                    {
                        keysToSend = args[i].Replace("'", "\"");
                    }
                }
            }

            if (!validArguments)
            {
                throw new ArgumentException("Invalid arguments. Please define a process id and the string value to send as keys." +
                    "\n  Example:  SendKeys -pid:4711 \"Keys to send{Enter}\"" +
                    "\n  Optional: Add -wait:100 to add a delay of 100 milliseconds, for example.");
            }

            SendKeys.Send(keysToSend);
        }
    }
}
