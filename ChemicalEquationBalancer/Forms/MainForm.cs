using ChemicalEquationBalancer.Algorithms;
using ChemicalEquationBalancer.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChemicalEquationBalancer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        ChemicalInputPanelControl cipc;
        private void MainForm_Load(object sender, EventArgs e)
        {
            ShowTip();
            //inputPanel = new ChemicalInputPanel(OnInput);
            RelativeAtomicMass.Init();

            cipc = new ChemicalInputPanelControl();//用户控件
            cipc.Dock = DockStyle.Fill;//填充panel
            cipc.onInput += new Action(ShowList);
            groupBox3.Controls.Add(cipc);//向panel添加用户控件

            Search.Init();

            SetFocus(tabControl1.SelectedIndex);
        }

        public TextBox currentFocused;
        public int currentSelectionIndex;

        #region 按键
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text += SpecialChars.SUB_1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text += SpecialChars.SUB_2;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text += SpecialChars.SUB_3;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text += SpecialChars.SUB_4;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text += SpecialChars.SUB_5;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox1.Text += SpecialChars.SUB_6;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox1.Text += SpecialChars.SUB_7;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox1.Text += SpecialChars.SUB_8;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox1.Text += SpecialChars.SUB_9;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            textBox1.Text += SpecialChars.SUB_0;
        }

        #endregion

        #region 输入
        //ChemicalInputPanel inputPanel;
        public void OnInput(string input)
        {
            if (currentFocused != null)
            {
                currentFocused.Text = currentFocused.Text.Insert(currentSelectionIndex, input);
            }
        }
        private void button12_Click(object sender, EventArgs e)
        {
            //if (inputPanel.IsDisposed) inputPanel = new ChemicalInputPanel(OnInput);
            //inputPanel.Show();
        }
        #endregion 

        #region 相对分子质量计算
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ChemicalFormula c = ChemicalFormula.Parse(textBox1.Text);
                ElementIngredients p = c.GetElementIngredients();

                double m = 0;
                if (radioButton1.Checked)
                    m = p.CalculateExactRelativeAtomicMass();
                else
                    m = p.CalculateSimpleRelativeAtomicMass();

                StringBuilder result = new StringBuilder("");
                if (p.elements.Count == 1 && p.elements.ElementAt(0).Value == 1)
                    result.Append(string.Format("相对原子质量：{0}", m));
                else result.Append(string.Format("相对分子质量：{0}", m));
                if (p.elements.Count > 1)
                {
                    result.AppendLine("\n————————————");
                    result.AppendLine("其中，");
                    foreach (var r in p.elements.Keys)
                    {
                        int ec = p[r];
                        double em = radioButton1.Checked ?
                            ec * RelativeAtomicMass.GetExactMassOf(r) :
                            ec * RelativeAtomicMass.GetSimpleMassOf(r);
                        double rs = em / m;
                        rs *= 100;
                        result.AppendLine(string.Format("{0}的质量分数为：({1} / {2}) × 100% = {3}%", r, em, m, rs));
                    }
                }
                richTextBox1.Text = result.ToString();
            }
            catch(Exception ex)
            {
                richTextBox1.Text = ex.Message;
            }
        }
        #endregion

        #region 配平
        FormulaBalancing balancing;
        private void button13_Click(object sender, EventArgs e)
        {
            try
            {
                ReactionFormula r = ReactionFormula.Parse(textBox3.Text);
                bool b = r.CheckBalanced();
                if (b)
                {
                    balancing = null;
                    richTextBox2.Text = "此方程式已经配平";
                }
                else
                {
                    balancing = new FormulaBalancing(r);
                    balancing.box = richTextBox2;
                    var p = balancing.Balance();
                    if (p == BalanceResult.infinite)
                    {
                        richTextBox2.Text = "无法配平此方程式：此方程式有无穷解。\n" +
                            "——————————————————\n" +
                            "可能的原因如下：\n" +
                            "① 存在输入错误\n" +
                            "② 此方程式不存在且无法被配平\n" +
                            "③ 程序错误\n" + 
                            "④ 部分氧化还原反应方程式无法通过元素守恒和电荷守恒求解，请尝试自己用化合价升降法配平！";
                        balancing = null;
                    }
                    else if (p == BalanceResult.unsolvable)
                    {
                        richTextBox2.Text = "无法配平此方程式：此方程式无解。\n" +
                            "——————————————————\n" +
                            "可能的原因如下：\n" +
                            "① 存在输入错误\n" +
                            "② 此方程式不存在且无法被配平\n" +
                            "③ 程序错误\n";
                        balancing = null;
                    }
                    if (radioButton3.Checked)
                        ShowResultInFormula();
                    else
                        ShowResultInNumbers();
                }
            }
            catch(Exception ex)
            {
                richTextBox2.Text = "错误：" + ex.Message;
            }
            
        }
        public void ShowResultInNumbers()
        {
            if (balancing == null) return;
            int[] solution = balancing.result;
            StringBuilder sb = new StringBuilder("");
            for (int i = 0; i < solution.Length; i++)
            {
                sb.Append(solution[i]);
                sb.Append(" ");
            }
            richTextBox2.Text = sb.ToString();
        }
        public void ShowResultInFormula()
        {
            if(balancing == null) return;
            richTextBox2.Text = balancing.formula.ToString();
        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
                ShowResultInFormula();
            else
                ShowResultInNumbers();
        }
        #endregion

        #region 输入框

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
            SetFocus(tabControl1.SelectedIndex);
        }

        public void SetFocus(int i)
        {
            if (i == 0) 
            {
                cipc.focusedTextbox = textBox3;
                
            }
            else if(i == 1)
            {
                cipc.focusedTextbox = textBox1;
            }
            else
            {
                cipc.focusedTextbox = null;
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            cipc.focusedTextbox = textBox3;
            cipc.focusedIndex = textBox3.SelectionStart;
            if (listBox1.Visible)
            {
                if (e.KeyCode == Keys.Down)
                {
                    listBox1.Focus();
                    listBox1.SelectedIndex = 0;
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

        private void textBox3_Click(object sender, EventArgs e)
        {
            cipc.focusedTextbox = textBox3;
            cipc.focusedIndex = textBox3.SelectionStart;
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            cipc.focusedTextbox = textBox1;
            cipc.focusedIndex = textBox1.SelectionStart;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            cipc.focusedTextbox = textBox1;
            cipc.focusedIndex = textBox1.SelectionStart;
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            cipc.focusedTextbox = textBox1;
            cipc.focusedIndex = textBox1.SelectionStart;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            //cipc.focusedTextbox = null;
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (listBox1.Focused) return;
            //cipc.focusedTextbox = null;
            listBox1.Visible = false;
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            cipc.focusedTextbox = textBox3;
            cipc.focusedIndex = textBox3.SelectionStart;
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            cipc.focusedTextbox = textBox1;
        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {
            cipc.focusedTextbox = textBox3;
        }
        #endregion

        #region 格式转换
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            { 
                textBox2.ReadOnly = false;
                textBox4.ReadOnly = true;
                textBox5.ReadOnly = true;
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked) 
            { 
                textBox2.ReadOnly = true;
                textBox4.ReadOnly = false;
                textBox5.ReadOnly = true;
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked)
            { 
                textBox2.ReadOnly = true;
                textBox4.ReadOnly = true;
                textBox5.ReadOnly = false;
            }
        }
        
        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                ChemicalFormula r = ChemicalFormula.Parse(textBox2.Text);
                textBox5.Text = r.ToStringC();
            }
            catch (FormatException)
            {
                try
                {
                    ReactionFormula d = ReactionFormula.Parse(textBox2.Text);
                    textBox5.Text = d.ToStringC();
                }
                catch (FormatException)
                {
                    textBox5.Text = "请输入正确的化学方程式或化学式";
                }
            }
            /*if (selected == 0) 
            {
                try
                {
                    ChemicalFormula r = ChemicalFormula.Parse(textBox2.Text);
                    textBox4.Text = r.ToStringB();
                }
                catch(FormatException ex)
                {
                    try
                    {
                        ReactionFormula d = ReactionFormula.Parse(textBox2.Text);
                        textBox4.Text = d.ToStringB();
                    }
                    catch (FormatException ey)
                    {

                    }
                }
            }*/
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!cipc.inputing)
            {
                cipc.focusedIndex = textBox1.SelectionStart;
            }
        }

        #endregion

        #region 辅助输入
        bool ap = false;
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (!cipc.inputing)
            {
                cipc.focusedIndex = textBox3.SelectionStart;
            }
            if (textBox3.SelectionLength != 0 && textBox3.SelectionLength != -1) return;
            if (ap) return;
            //int s = textBox3.SelectionStart;
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
            

            Point a = textBox3.Location;
            Point b = textBox3.GetPositionFromCharIndex(index);
            Point c = new Point(a.X + b.X, a.Y + b.Y + (int)(textBox3.Font.Size) + 15);
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
                textBox3.Text = textBox3.Text.Remove(a, l);
                textBox3.Text = textBox3.Text.Insert(a, search[i].formula);
                textBox3.Focus();
                textBox3.SelectionStart = a + search[i].formula.Length;
                listBox1.Visible = false;
            }
            ap = false;
            cipc.focusedIndex = textBox3.SelectionStart;
        }
        private string GetLastWord()
        {
            int i = FindLastWordStartIndex();
            return textBox3.Text.Substring(i, textBox3.SelectionStart - i);//SpecialChars.Normal_SubTranslate();
        }

        readonly List<char> s = new List<char>() { '=', '+' };
        private int FindLastWordStartIndex()
        {
            int i = textBox3.SelectionStart;

            for (; i > 0; i--)
            {
                if (s.Contains(textBox3.Text[i - 1])) break ;
            }
            for (; i < textBox3.SelectionStart; i++)
            {
                if (CharUtil.GetCharCase(textBox3.Text[i]) != CharCase.number) return i;
            }
            return 0;
        }
        #endregion

        #region 菜蛋
        public string[] tips = new string[]
        {
            "杀鸡焉用宰牛刀啊",
            "v（伟）= 3个/分",
            "v（生）= 1/3 个/分",
            "清明踏春日，柳笛忆少时！",
            "多喝六个核桃",
            "你这脑袋瓜子不够用啊",
            "今天晚上别吃饭了昂",
            "今天晚上请我吃饭昂",
            
        };
        public void ShowTip()
        {
            Random r = new Random();
            label6.Text = tips[r.Next(0, tips.Length)];
        }
        #endregion

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Up && e.KeyCode != Keys.Down && e.KeyCode != Keys.Enter)
            {
                textBox3.Focus();
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

        private void button3_Click_1(object sender, EventArgs e)
        {
            Replacer r = new Replacer();
            r.Show();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            numericUpDown3.Value = 22.4M;
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.DecimalPlaces = (int)numericUpDown7.Value;
            numericUpDown2.DecimalPlaces = (int)numericUpDown7.Value;
            numericUpDown3.DecimalPlaces = (int)numericUpDown7.Value;
            numericUpDown4.DecimalPlaces = (int)numericUpDown7.Value;
            numericUpDown5.DecimalPlaces = (int)numericUpDown7.Value;
            numericUpDown6.DecimalPlaces = (int)numericUpDown7.Value;
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.Value = numericUpDown6.Value;
            numericUpDown2.Value = numericUpDown1.Value * numericUpDown3.Value;
            numericUpDown5.Value = numericUpDown1.Value * numericUpDown8.Value;
            numericUpDown4.Value = numericUpDown1.Value / numericUpDown9.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown6.Value = numericUpDown1.Value;
            numericUpDown2.Value = numericUpDown1.Value * numericUpDown3.Value;
            numericUpDown5.Value = numericUpDown1.Value * numericUpDown8.Value;
            numericUpDown4.Value = numericUpDown1.Value / numericUpDown9.Value;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.Value = numericUpDown5.Value / numericUpDown8.Value;
            numericUpDown6.Value = numericUpDown1.Value;
            numericUpDown2.Value = numericUpDown1.Value * numericUpDown3.Value;
            numericUpDown4.Value = numericUpDown1.Value / numericUpDown9.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.Value = numericUpDown2.Value / numericUpDown3.Value;
            numericUpDown6.Value = numericUpDown1.Value;
            numericUpDown5.Value = numericUpDown1.Value * numericUpDown8.Value;
            numericUpDown4.Value = numericUpDown1.Value / numericUpDown9.Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.Value = numericUpDown4.Value * numericUpDown9.Value;
            numericUpDown2.Value = numericUpDown1.Value * numericUpDown3.Value;
            numericUpDown6.Value = numericUpDown1.Value;
            numericUpDown5.Value = numericUpDown1.Value * numericUpDown8.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown2.Value = numericUpDown1.Value * numericUpDown3.Value;
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown4.Value = numericUpDown1.Value / numericUpDown9.Value;
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown5.Value = numericUpDown1.Value * numericUpDown8.Value;
        }

        private void textBox6_Enter(object sender, EventArgs e)
        {
            cipc.focusedTextbox = textBox6;
            cipc.focusedIndex = textBox6.SelectionStart;
        }

        private void textBox6_MouseDown(object sender, MouseEventArgs e)
        {
            cipc.focusedTextbox = textBox6;
            cipc.focusedIndex = textBox6.SelectionStart;
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            try
            {
                ChemicalFormula c = ChemicalFormula.Parse(textBox6.Text);
                ElementIngredients p = c.GetElementIngredients();

                double m = 0;
                if (radioButton9.Checked)
                    m = p.CalculateExactRelativeAtomicMass();
                else
                    m = p.CalculateSimpleRelativeAtomicMass();

                numericUpDown8.Value = (decimal)m;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误");
            }
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            string t = textBox7.Text;
            if (radioButton10.Checked)
            {
                if (radioButton15.Checked)
                {

                }
                else if (radioButton14.Checked)
                {
                    t = SpecialChars.SuperToSub(t);
                }
                else
                {
                    t = SpecialChars.SuperToNormal(t);
                }
            }
            else if (radioButton11.Checked)
            {
                if (radioButton15.Checked)
                {
                    t = SpecialChars.SubToSuper(t);
                }
                else if (radioButton14.Checked)
                {
                    
                }
                else
                {
                    t = SpecialChars.SubToNormal(t);
                }
            }
            else
            {
                if (radioButton15.Checked)
                {
                    t = SpecialChars.NormalToSuper(t);
                }
                else if (radioButton14.Checked)
                {
                    t = SpecialChars.NormalToSub(t);
                }
                else
                {
                }
            }
            richTextBox4.Text = t;
           
        }

        private void textBox7_Enter(object sender, EventArgs e)
        {
            cipc.focusedTextbox = textBox7;
            cipc.focusedIndex = textBox7.SelectionStart;
        }

        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            cipc.focusedTextbox = textBox7;
            cipc.focusedIndex = textBox7.SelectionStart;
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox4.Text);
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }
    }
}
