using ChemicalEquationBalancer.Algorithms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChemicalEquationBalancer.Forms.Controls
{
    public class TextboxWithAssociation : TextBox
    {
        public delegate Association[] SearchEventHandler(string textToSearch);
        public SearchEventHandler onSearch;
        public ListBox associationListBox = new ListBox();
        private bool ap;
        ChemicalInputPanelControl cipc = new ChemicalInputPanelControl();
        public TextboxWithAssociation()
        {
            associationListBox.Font = new Font("微软雅黑", 11F);
            associationListBox.FormattingEnabled = true;
            associationListBox.ItemHeight = 20;
            associationListBox.Location = new Point(107, -26);
            associationListBox.Size = new Size(194, 104);
            associationListBox.TabIndex = 8;
            associationListBox.Visible = false;
            associationListBox.DoubleClick += new EventHandler(OnListDoubleClick);
            associationListBox.KeyDown += new KeyEventHandler(OnListKeyDown);
            TextChanged += new EventHandler(OnTextChanged);
            ParentChanged += new EventHandler(OnControlAdded);
            KeyDown += new KeyEventHandler(OnInput);
            Leave += new EventHandler(OnLosingFocus);
        }
        private void OnLosingFocus(object sender, EventArgs e)
        {
            if (associationListBox.Focused) return;
            associationListBox.Visible = false;
        }
        private void OnInput(object sender, KeyEventArgs e)
        {
            cipc.focusedTextbox = this;
            cipc.focusedIndex = this.SelectionStart;
            if (associationListBox.Visible)
            {
                if (e.KeyCode == Keys.Down)
                {
                    associationListBox.Focus();
                    associationListBox.SelectedIndex = 0;
                    e.Handled = true;
                }
                if (e.KeyCode == Keys.F1)
                {
                    Insert(0);
                    e.Handled = true;
                }
                if (e.KeyCode == Keys.F2)
                {
                    Insert(1);
                    e.Handled = true;
                }
                if (e.KeyCode == Keys.F3)
                {
                    Insert(2);
                    e.Handled = true;
                }
                if (e.KeyCode == Keys.F4)
                {
                    Insert(3);
                    e.Handled = true;
                }
                if (e.KeyCode == Keys.F5)
                {
                    Insert(4);
                    e.Handled = true;
                }
            }
        }
        private void OnControlAdded(object sender, EventArgs e)
        {
            this.Parent.Controls.Add(associationListBox);
            ;
        }
        private void OnTextChanged(object sender, EventArgs e)
        {
            if (!cipc.inputing)
            {
                cipc.focusedIndex = SelectionStart;
            }
            if (SelectionLength != 0 && SelectionLength != -1) return;
            if (ap) return;
            ShowList();
        }
        public void OnListDoubleClick(object sender, EventArgs e)
        {
            if (associationListBox.SelectedIndex != -1)
            {
                Insert(associationListBox.SelectedIndex);
            }
        }
        public void OnListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Up && e.KeyCode != Keys.Down && e.KeyCode != Keys.Enter)
            {
                Focus();
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Enter)
            {
                if (associationListBox.SelectedIndex != -1)
                {
                    Insert(associationListBox.SelectedIndex);
                }
            }
        }
        public List<Molecule> search = new List<Molecule>();
        public void ShowList()
        {
            ShowList(FindLastWordStartIndex(), Search.SearchMolecules(GetLastWord(), out search));
        }
        public void ShowList(int index, List<Molecule> wordList)
        {
            associationListBox.Visible = true;
            associationListBox.Items.Clear();
            string[] list = Molecule.ToWordDisplay(wordList);
            associationListBox.Items.AddRange(list);
            if (associationListBox.Items.Count == 0) associationListBox.Visible = false;
            RefreshListPos(index);
            search = wordList;
        }
        public void RefreshListPos(int index)
        {
            Graphics g = associationListBox.CreateGraphics();
            int width = 0;
            foreach (var r in associationListBox.Items)
            {
                int d = g.MeasureString((string)r, associationListBox.Font).ToSize().Width + 10;
                if (width < d)
                {
                    width = d;
                }
            }
            Point a = Location;
            Point b = GetPositionFromCharIndex(index);
            Point c = new Point(a.X + b.X, a.Y + b.Y + (int)(Font.Size) + 15);
            associationListBox.Location = c;
            associationListBox.Width = width;
            associationListBox.Height = Math.Min(associationListBox.ItemHeight * (associationListBox.Items.Count + 1), associationListBox.ItemHeight * 6);
        }
        public void Insert(int i)
        {
            ap = true;
            if (i >= 0 && i < associationListBox.Items.Count)
            {
                int a = FindLastWordStartIndex();
                int l = GetLastWord().Length;
                Text = Text.Remove(a, l);
                Text = Text.Insert(a, search[i].formula);
                Focus();
                SelectionStart = a + search[i].formula.Length;
                associationListBox.Visible = false;
            }
            ap = false;
            cipc.focusedIndex = SelectionStart;
        }
        private string GetLastWord()
        {
            int i = FindLastWordStartIndex();
            return Text.Substring(i, SelectionStart - i);
        }

        readonly List<char> s = new List<char>() { '=', '+' };
        private int FindLastWordStartIndex()
        {
            int i = SelectionStart;

            for (; i > 0; i--)
            {
                if (s.Contains(Text[i - 1])) break;
            }
            for (; i < SelectionStart; i++)
            {
                if (CharUtil.GetCharCase(Text[i]) != CharCase.number) return i;
            }
            return 0;
        }
    }
    public struct Association
    {
        public string textToInput;
        public string textToDisplay;
        public Association(string textToInput, string textToDisplay)
        {
            this.textToInput = textToInput;
            this.textToDisplay = textToDisplay;
        }
    }
}
