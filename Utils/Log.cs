namespace DialogueToTTS.Utils
{
    public static class Logs
    {
        // TODO: make a logger
        public static void Log(string log, [System.Runtime.CompilerServices.CallerMemberName] string functionName = "")
        {
            System.Console.WriteLine("LOG: [" + functionName + "]: " + log);
        }

    }
}