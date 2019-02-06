using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
            conf.Resolution = Constants.fullhd;

            return conf;
        }
    }

    public class Cards
    {
        [XmlArray("Scores"), XmlArrayItem(typeof(CardConfig), ElementName = "Card")]
        public List<CardConfig> listOfCards { get; set; }
    }
    public class CardConfig
    {
        public string Name { get; set; }
        public string SkinPath { get; set; }

        public CardConfig()
        {

        }

        public CardConfig(string name, string skinPath)
        {
            Name = name;
            SkinPath = skinPath;
        }
    }

    public class Payments
    {
        [XmlArray("Payments"), XmlArrayItem(typeof(GamePayment), ElementName = "GamePayment")]
        public List<GamePayment> listOfPayments { get; set; }
    }

    public class GamePayment
    {
        public string Name;
        public string Id;
        public bool approved;

        public GamePayment()
        {
            approved = false;
        }
    }

}
