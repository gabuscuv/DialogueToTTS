//using System;
using System.IO;
using DialogueToTTS.Utils;
using CommandLine;
using Newtonsoft.Json.Linq;

namespace DialogueToTTS
{
    class Program
    {
        public class Options
        {
            [Option('i', "csv", Required = true, HelpText = "CSV file for Text-To-Speech")]
            public string Filename { get; set; }
            [Option('o', "output", Required = false, HelpText = "Set folder output for audiofiles")]
            public string OutputFolder { get; set; }
            [Option('c', "cuda", Required = false, HelpText = "Set true or false for use Cuda")]
            public bool UseCuda { get; set; }
        }

        static void Main(string[] args)
        {
            if (!CommandExists.ExistsOnPath("tts")){ Logs.Log("TTS is not installed or detected, Please Check if you have installed TTS or is included in PATH environment variable"); return; }

            if (args.Length != 0)
            {
                Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                //.WithNotParsed(HandleParseError)
                ;
            }
            else
            {
                Logs.Log("Arguments not passed, Trying to read default configuration file");
                ReadConfig();
            }
        }

        static void RunOptions(Options o)
        {
            if (!File.Exists(o.Filename))
            {
                Logs.Log("CSV File doesn't exists, Please check the location file or file directory");
                return;
            }

            new DialogueToTTS(o.Filename, o.OutputFolder).Run();
        }
        static void ReadConfig()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "/config.json"))
            {
                Logs.Log(("Reading config file:" + Directory.GetCurrentDirectory() + "/config.json"));
                var config = JObject.Parse(File.ReadAllText(Directory.GetCurrentDirectory() + "/config.json")).ToObject<DTO.Config>();
                if (config != null)
                {
                    Logs.Log("Config file read successfully\n" +
                    "Parameters:" +
                    "\nCSV File: " + config.DefaultCsv +
                    "\nOutput Folder: " + config.OutputPath +
                    "\nUseCuda: " + config.UseCuda);
                    new DialogueToTTS(config.DefaultCsv, config.OutputPath, config.UseCuda).Run();
                }
            }
            else
            {
                Logs.Log("It doesn't exist config file: " + Directory.GetCurrentDirectory()
                + "/config.json" +
                "\nPlease check the config file location");
            }
        }
    }
}
