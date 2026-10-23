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

namespace ChemicalEquationBalancer.Forms
{
    public partial class Replacer : Form
    {
        public Replacer()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = richTextBox1.Text.Replace('0', SpecialChars.SUB_0);
            richTextBox1.Text = richTextBox1.Text.Replace('1', SpecialChars.SUB_1);
            richTextBox1.Text = richTextBox1.Text.Replace('2', SpecialChars.SUB_2);
            richTextBox1.Text = richTextBox1.Text.Replace('3', SpecialChars.SUB_3);
            richTextBox1.Text = richTextBox1.Text.Replace('4', SpecialChars.SUB_4);
            richTextBox1.Text = richTextBox1.Text.Replace('5', SpecialChars.SUB_5);
            richTextBox1.Text = richTextBox1.Text.Replace('6', SpecialChars.SUB_6);
            richTextBox1.Text = richTextBox1.Text.Replace('7', SpecialChars.SUB_7);
            richTextBox1.Text = richTextBox1.Text.Replace('8', SpecialChars.SUB_8);
            richTextBox1.Text = richTextBox1.Text.Replace('9', SpecialChars.SUB_9);
        }
    }
}
