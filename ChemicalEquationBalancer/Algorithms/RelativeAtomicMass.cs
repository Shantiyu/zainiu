using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChemicalEquationBalancer.Algorithms
{
    public class RelativeAtomicMass
    {
        private static Dictionary<string, double> relativeDoubleAtomicMassList = new Dictionary<string, double>();
        private static Dictionary<string, double> relativeSimpleAtomicMassList = new Dictionary<string, double>();
        public static double GetSimpleMassOf(string elementName)
        {
            if (!relativeSimpleAtomicMassList.ContainsKey(elementName)) throw new Exception(string.Format("给定的元素名{0}不在数据库中。", elementName));
            return relativeSimpleAtomicMassList[elementName];
        }
        public static double GetExactMassOf(string elementName)
        {
            if (!relativeDoubleAtomicMassList.ContainsKey(elementName)) throw new Exception(string.Format("给定的元素名{0}不在数据库中。", elementName));
            return relativeDoubleAtomicMassList[elementName];
        }
        public const string FILE_PATH = "AtomicMass.txt";
        public static void Init()
        {
            string[] lines = File.ReadAllLines(Application.StartupPath + "\\" + FILE_PATH);
            foreach (string line in lines) 
            {
                if (line.StartsWith("//")) continue;
                string[] items = line.Split(' ');
                items[3] = items[3].Trim('[', ']');
                relativeDoubleAtomicMassList.Add(items[2], double.Parse(items[3]));
                relativeSimpleAtomicMassList.Add(items[2], Round(double.Parse(items[3])));
            }
        }
        private static double Round(double value)
        {
            double v = value - (int)value;
            if (v < 0.4) return (int)value;
            if (v <= 0.6) return (int)value + 0.5;
            else return (int)value + 1;
        }
    }
}
