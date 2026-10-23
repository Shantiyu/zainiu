using ChemicalEquationBalancer.Algorithms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ChemicalEquationBalancer.Forms.Controls
{
    public partial class AutoLegendControl : UserControl
    {
        public AutoLegendControl()
        {
            InitializeComponent();
        }

        bool ap = false;
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //if (!cipc.inputing)
            {
             //   cipc.focusedIndex = textBox1.SelectionStart;
            }
            if (textBox1.SelectionLength != 0 && textBox1.SelectionLength != -1) return;
            if (ap) return;
            //int s = textBox1.SelectionStart;
            ShowList();
        }
        public void ShowList()
        {
            ShowList(FindLastWordStartIndex(), Search.SearchMolecules(GetLastWord(), out search));
        }
        public void ShowList(int index, List<Molecule> wordList)
        {
            listBox1.Visible = true;
            listBox1.Items.Clear();
            string[] list = Molecule.ToWordDisplay(wordList);
            listBox1.Items.AddRange(list);
            if (listBox1.Items.Count == 0) listBox1.Visible = false;
            RefreshListPos(index);
            search = wordList;
        }
        public void RefreshListPos(int index)
        {
            Graphics g = listBox1.CreateGraphics();
            int width = 0;
            foreach (var r in listBox1.Items)
            {
                int d = g.MeasureString((string)r, listBox1.Font).ToSize().Width + 10;
                if (width < d)
                {
                    width = d;
                }
            }


            Point a = textBox1.Location;
            Point b = textBox1.GetPositionFromCharIndex(index);
            Point c = new Point(a.X + b.X, a.Y + b.Y + (int)(textBox1.Font.Size) + 15);
            listBox1.Location = c;
            listBox1.Width = width;
            listBox1.Height = Math.Min(listBox1.ItemHeight * (listBox1.Items.Count + 1), listBox1.ItemHeight * 6);
        }
        public void CloseWordList()
        {
            listBox1.Visible = false;
        }
        public List<Molecule> search = new List<Molecule>();
        public void Insert(int i)
        {
            ap = true;
            if (i >= 0 && i < listBox1.Items.Count)
            {
                int a = FindLastWordStartIndex();
                int l = GetLastWord().Length;
                textBox1.Text = textBox1.Text.Remove(a, l);
                textBox1.Text = textBox1.Text.Insert(a, search[i].formula);
                textBox1.Focus();
                textBox1.SelectionStart = a + search[i].formula.Length;
                listBox1.Visible = false;
            }
            ap = false;
            //cipc.focusedIndex = textBox1.SelectionStart;
        }
        private string GetLastWord()
        {
            int i = FindLastWordStartIndex();
            return SpecialChars.NormalToSub(textBox1.Text.Substring(i, textBox1.SelectionStart - i));
        }

        readonly List<char> s = new List<char>() { '=', '+' };
        private int FindLastWordStartIndex()
        {
            int i = textBox1.SelectionStart;

            for (; i > 0; i--)
            {
                if (s.Contains(textBox1.Text[i - 1])) break;
            }
            for (; i < textBox1.SelectionStart; i++)
            {
                if (CharUtil.GetCharCase(textBox1.Text[i]) != CharCase.number) return i;
            }
            return 0;
        }



        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Up && e.KeyCode != Keys.Down && e.KeyCode != Keys.Enter)
            {
                textBox1.Focus();
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Enter)
            {
                if (listBox1.SelectedIndex != -1)
                {
                    Insert(listBox1.SelectedIndex);
                }
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                Insert(listBox1.SelectedIndex);
            }
        }
    }
}
