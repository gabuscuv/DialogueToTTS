using DialogueToTTS.DTO; // for ArrowDialogueParserFormat
using DialogueToTTS.Utils;
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

        readonly bool useCuda;

        public DialogueToTTS(string filename, string path = null, bool useCuda = false)
        {
            this.filename = filename;
            if (path == null)
            {
                path = Directory.GetCurrentDirectory() + "/output/";
            }
            else { this.path = path; }
            this.useCuda = useCuda;

        }

        public bool Run()
        {
            Logs.Log("Starting Process");
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
                                + "--use_cuda " + useCuda,

                        };
                        Process.Start(temp).WaitForExit();
                    }
                }
            }
            else
            {
                Logs.Log("No Rows Found");
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
            Logs.Log("Reading Datatable from: " + filepath);
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
                Logs.Log("Directory not found");
            }
            catch (System.Exception ex)
            {
                Logs.Log("Generic Exception:" + ex.ToString());
            }
            return null;
        }

    }
}