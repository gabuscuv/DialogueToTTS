// Source: https://stackoverflow.com/a/3856090 // License: CC BY-SA 4.0  https://creativecommons.org/licenses/by-sa/4.0/
using System;
using System.IO;
namespace DialogueToTTS.Utils
{
    public static class CommandExists
    {
        public static bool ExistsOnPath(string fileName)
        {
            return GetFullPath(fileName) != null;
        }

        public static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }
    }
}