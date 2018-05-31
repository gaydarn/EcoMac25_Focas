using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNC_WRMACRO
{
    class Program
    {

        // FUNCTION WAIT KEY PRESSING 

        public static void Pause()
        {
            Console.WriteLine("Press any key to continue/quit . . . ");
            Console.ReadKey(true);
        }


        // MAIN FUNCTION 
        [STAThread]
        static void Main(string[] args)
        {
            // read file into a string and deserialize JSON to a type
            string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine(appPath, @"Configuration//default.json")));
            StructInfosCnc structInfo = new StructInfosCnc();
            structInfo.ReadFromJSON();

            /*long time1 = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            long time2 = time1;
            int i = 0;
            while (time1 == time2)
            {
                time2 = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                i++;
            }
             


            Console.WriteLine("iterations needed to detect different time: " + i);*/

            /*StructInfosConfig structInfo = new StructInfosConfig();
            StructDataConfig structData = new StructDataConfig();
            structData.axis = 1;
            structData.diagnosNumber = 2;
            structData.length = 3;
            structInfo.structInfosConfig.Add(structData);
            structInfo.structInfosConfig.Add(structData);
            structInfo.structInfosConfig.Add(structData);
            structInfo.structInfosConfig.Add(structData);

            string json = JsonConvert.SerializeObject(
           structInfo, Formatting.Indented,
           new JsonConverter[] { new StringEnumConverter() });

            System.IO.File.WriteAllText(Path.Combine(appPath, @"Configuration//StructDataConfig.json"), json);
            */
            Console.WriteLine("CNC IP Adress : " + config.CncIpAddr);
            Console.WriteLine("CNC Port : " + config.CncPort);

            string path = null;
            
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = saveFileDialog.FileName;

                using (StreamWriter writer = new StreamWriter(path))
                {
                    CNCControler cncControler = new CNCControler(config.CncIpAddr, (ushort)config.CncPort, structInfo, 1000, writer);

                    cncControler.StartRecording();

                    Pause();

                    cncControler.StopRecording();
                }
            }
            
            Pause();
        }
    }
}
