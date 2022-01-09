using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using DialogueToTTS.DTO;

namespace DialogueToTTS
{
    public class DialogueToTTS
    {
        readonly string filename;
        readonly string path;

        public DialogueToTTS(string filename, string path = null)
        {
            this.filename = filename;
            if(path == null)
            {
                path = Directory.GetCurrentDirectory() + "/output/";
            }else{this.path = path ;}
            
        }

        public bool Run()
        {
            ProcessStartInfo temp;
            var rows = GetDataTable(filename);
            if(rows != null)
                foreach(ArrowDialogueParserFormat row in rows)
                {
                    if (row.DialogueWaveId.Contains("#")){
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
                                "--out_path "+ path + "/" + id[0]+"/"+id[1]+".wav "
                                //+"--use-cuda "+"true",
                            
                        };
                        Process.Start(temp).WaitForExit();
                    }
                }
            return true;
        }

        IEnumerable GetDataTable(string filepath)
        {
            try
            {
                using (var reader = new StreamReader(filepath))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                    return csv.GetRecords<ArrowDialogueParserFormat>().ToList();;
                    }
            }
            catch
            {
                return null;
            }
            
        }

        
    }
}