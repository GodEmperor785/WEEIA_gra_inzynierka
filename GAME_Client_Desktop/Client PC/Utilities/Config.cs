using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_PC.Utilities
{
    public class Config
    {
        public string Resolution { get; set; }

        public Config()
        {

        }

        public static Config Default()
        {
            Config conf = new Config();
            conf.Resolution = Constants.hd;

            return conf;
        }
    }
}
