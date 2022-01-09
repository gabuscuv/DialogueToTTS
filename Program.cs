using System;
using System.IO;
using CommandLine;
using Newtonsoft.Json.Linq;

namespace DialogueToTTS
{
    class Program
    {
        public class Options
        {
            [Option('i', "csv", Required = true, HelpText = "Set output to verbose messages.")]
            public string filename { get; set; }
            [Option('o', "output", Required = false, HelpText = "Set output to verbose messages.")]
            public string outputfolder { get; set; }
        }

        static void Main(string[] args)
        {
            if(args.Length != 0){
                Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                //.WithNotParsed(HandleParseError)
                ;
            }
            else
            {
                ReadConfig();
            }
        }

        static void RunOptions(Options o)
        {
            if(File.Exists(o.filename))
            {
            new DialogueToTTS(o.filename,o.outputfolder).Run();
            }
            else
            {
                Console.WriteLine("File doesn't Exists");
            }
}
        static void ReadConfig()
        {
            if(File.Exists(Directory.GetCurrentDirectory()+"/config.json"))
            {
                var config = JObject.Parse(File.ReadAllText(Directory.GetCurrentDirectory()+"/config.json")).ToObject<DTO.Config>();            
                if (config != null)
                {
                    new DialogueToTTS(config.defaultcsv,config.outputPath).Run();
                }
            }
        }
    }
}
