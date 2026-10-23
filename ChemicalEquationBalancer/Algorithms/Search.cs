using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChemicalEquationBalancer.Algorithms
{
    public class Search
    {
        public const string FILE_PATH = "Molecules.txt";
        public static List<Molecule> molecules = new List<Molecule>();
        public static void Init()
        {
            string[] lines = File.ReadAllLines(Application.StartupPath + "\\" + FILE_PATH);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("//")) continue;
                string[] items = line.Split(',');
                if (items.Length < 2) molecules.Add(new Molecule(string.Empty, items[0], SpecialChars.SuperToNormal(SpecialChars.SubToNormal(items[0].ToUpper()))));
                else molecules.Add(new Molecule(items[1], items[0], SpecialChars.SuperToNormal(SpecialChars.SubToNormal(items[0].ToUpper()))));
            }
        }
        public static List<Molecule> SearchMolecules(string text, out List<Molecule> list) 
        {
            text = text.ToUpper();
            text = SpecialChars.SuperToNormal(text);
            text = SpecialChars.SubToNormal(text);
            List<Molecule> res = new List<Molecule>();
            if (string.IsNullOrWhiteSpace(text)) { list = res; return res; }
            foreach (var r in molecules)
            {
                if (r.name.Contains(text)) res.Add(r);
                else if (r.normalized.Contains(text)) res.Add(r);
            }
            list = res;
            /*List<float> similarity = new List<float>();
            foreach (var r in molecules)
            {
                similarity.Add(GetSimilarityWith(r.shantig, Molecule) + GetSimilarityWith(r.chinese, Molecule) + +GetSimilarityWith(r.english, Molecule));
            }*/
            //if (res.Count > 20) return res;
            return res;
        }
    }
    public struct Molecule
    {
        public string name;
        public string formula;
        public string normalized;
        public Molecule(string name, string formula, string normalized)
        {
            this.name = name;
            this.formula = formula;
            this.normalized = normalized;
        }

        public static string[] ToWordDisplay(List<Molecule> l)
        {
            List<string> strings = new List<string>();
            foreach (var r in l)
            {
                if (string.IsNullOrWhiteSpace(r.name)) 
                {
                    strings.Add(r.formula);
                }
                else
                {
                    strings.Add(r.formula + "（" + r.name + "）");
                }
            }
            return strings.ToArray();
        }
    }
}
