using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNC_WRMACRO
{
    class StructInfosCnc
    {
        public void ReadFromJSON()
        {
            string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            StructInfosConfig infosconfig = JsonConvert.DeserializeObject<StructInfosConfig>(File.ReadAllText(Path.Combine(appPath, @"Configuration//StructDataConfig.json")));

            foreach(StructDataConfig dataConfig in infosconfig.structInfosConfig)
            {
                structInfosCnc.Add(new StructDataCnc(dataConfig));
            }
        }

        public List<StructDataCnc> structInfosCnc = new List<StructDataCnc>();
    }

    class StructDataCnc
    {
        public StructDataCnc(StructDataConfig config)
        {
            _config = config;
        }

        public StructDataConfig _config;
        public Focas1.ODBDGN diag = new Focas1.ODBDGN();
        public long readingTime;
    }

    class StructInfosConfig
    {
        public List<StructDataConfig> structInfosConfig = new List<StructDataConfig>();
    }

    class StructDataConfig
    {
        public short diagnosNumber;
        public short axis;
        public short length;
    }


}
