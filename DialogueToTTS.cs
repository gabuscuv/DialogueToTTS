using DialogueToTTS.DTO; // for ArrowDialogueParserFormat
using System.Collections; // for IEnumerable
using System.Linq; // for ToList() 
using System.Diagnostics; // for ProcessStartInfo
using System.IO;


namespace DialogueToTTS
{
    public class DialogueToTTS
    {
        readonly string filename;
        readonly string path;

        public DialogueToTTS(string filename, string path = null)
        {
            this.filename = filename;
            if (path == null)
            {
                path = Directory.GetCurrentDirectory() + "/output/";
            }
            else { this.path = path; }

        }

        public bool Run()
        {
            log("Starting Process");
            ProcessStartInfo temp;
            var rows = GetDataTable(filename);
            if (rows != null)
            {
                foreach (ArrowDialogueParserFormat row in rows)
                {
                    if (row.DialogueWaveId.Contains("#"))
                    {
                        var id = row.DialogueWaveId.Split("#");
                        if (!Directory.Exists(path + "/" + id[0]))
                        {
                            Directory.CreateDirectory(path + "/" + id[0]);
                        }

                        temp = new ProcessStartInfo()
                        {
                            FileName = "tts",
                            Arguments =
                                "--text \"" + row.Text + "\" " +
                                chooseModel(row.Voice) + chooseVocoder(row.Voice) +
                                "--out_path " + path + "/" + id[0] + "/" + id[1] + ".wav "
                            //+"--use-cuda "+"true",

                        };
                        Process.Start(temp).WaitForExit();
                    }
                }
            }
            else
            {
                log("No Rows Found");
            }
            return true;
        }

        /// TODO: JSON Config load settings
        private string chooseModel(string voice)
        {
            string tmp = " --model_name ";
            switch (voice)
            {
                case "Player": tmp += "tts_models/en/ek1/tacotron2"; break;
                case "Radio": tmp += "tts_models/en/ljspeech/vits"; break;
                case "Auntie Robot": // Default Is Fine
                default: tmp = ""; break;
            }
            return tmp + " ";
        }

        /// TODO: JSON Config load settings
        private string chooseVocoder(string voice)
        {
            string tmp = "--vocoder_name ";
            switch (voice)
            {
                case "Player": tmp += "vocoder_models/en/ek1/wavegrad"; break;
                case "Radio": tmp += "vocoder_models/en/ljspeech/univnet"; break;
                case "Auntie Robot": // DEfault is Fine
                default: tmp = ""; break;
            }
            return tmp + " ";
        }

        private IEnumerable GetDataTable(string filepath)
        {
            log("Reading Datatable from: " + filepath);
            try
            {
                using (var reader = new StreamReader(filepath))
                using (var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                {
                    return csv.GetRecords<ArrowDialogueParserFormat>().ToList(); ;
                }
            }
            catch (DirectoryNotFoundException)
            {
                log("Directory not found");
            }
            catch (System.Exception ex)
            {
                log("Generic Exception:" + ex.ToString());
            }
            return null;
        }

        // TODO: make a logger
        private void log(string log, [System.Runtime.CompilerServices.CallerMemberName] string functionName = "")
        {
            System.Console.WriteLine("LOG: [" + functionName + "]: " + log);
        }

    }
}