using Newtonsoft.Json;
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

            Console.WriteLine("CNC IP Adress : " + config.CncIpAddr);
            Console.WriteLine("CNC Port : " + config.CncPort);

            string path = null;
            /*
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = saveFileDialog.FileName;

                using (StreamWriter writer = new StreamWriter(path))
                {
                    CNCControler cncControler = new CNCControler(config.CncIpAddr, (ushort)config.CncPort, 1000, writer);

                    Console.WriteLine(cncControler.Connect());

                    cncControler.StartRecording();

                    Pause();

                    cncControler.StopRecording();
                }
            }
            */
            using (StreamWriter writer = new StreamWriter(Path.Combine(appPath, @"dummy.csv")))
            {
                CNCControler cncControler = new CNCControler(config.CncIpAddr, (ushort)config.CncPort, 1000, writer);

                Console.WriteLine(cncControler.Connect());

                cncControler.StartRecording();

                Pause();

                cncControler.StopRecording();
            }
            Pause();
        }
    }
}
