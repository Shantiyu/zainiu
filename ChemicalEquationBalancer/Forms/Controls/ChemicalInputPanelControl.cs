using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChemicalEquationBalancer.Forms
{
    public partial class ChemicalInputPanelControl : UserControl
    {
        public TextBox focusedTextbox;
        public int focusedIndex;
        public Action onInput;
        public ChemicalInputPanelControl()
        {
            InitializeComponent();
        }
        public struct Tip
        {
            public string display, tip;
        }
        public void DrawList(GroupBox g, Control box, int width, int height, Tip[] texts, int space)
        {
            int bx = 8; int by = 0;
            //int x = box.Width / (width + space);
            //int y = (height + space) * (((texts.Length - 1) / x) + 1);
            int totalWidth = 0;
            int row = 0;
            for (int i = 0; i < texts.Length; i++)
            {
                int l = Math.Max(TextRenderer.MeasureText(texts[i].display, new Font("Times New Roman", 12F)).Width + 15, 40);
                if (bx + totalWidth + space + l > g.Width - 7)
                {
                    totalWidth = 0;
                    row++;
                }
                Button b = new Button();
                b.Font = new Font("Times New Roman", 12F);
                b.Location = new Point(bx + totalWidth + space, by + (height + space) * row);
                totalWidth += space + l;
                b.Size = new Size(l, height);
                b.Text = texts[i].display;
                b.Tag = texts[i].tip;
                b.Margin = new Padding(4);
                b.TabIndex = i;
                b.UseVisualStyleBackColor = true;
                /*b.MouseEnter += new EventHandler((object sender, EventArgs e) =>
                {
                    if (!string.IsNullOrWhiteSpace((string)b.Tag))
                    {
                        toolTip1.SetToolTip(box, b.Tag.ToString());
                        toolTip1.Show(b.Tag.ToString(),box);
                    }
                });
                b.MouseLeave += new EventHandler((object sender, EventArgs e) =>
                {
                    toolTip1.Hide(box);
                });
                */
                b.Click += new EventHandler((object sender, EventArgs e) => { OnInput(b.Text); });
                box.Controls.Add(b);
            }
            g.Height = (row + 1) * (height + space) + 25;
        }
        public List<GroupBox> boxes = new List<GroupBox>();
        public void DrawBox(string name, Tip[] texts)
        {
            GroupBox g = new GroupBox();
            Panel p = new Panel();
            
            g.Location = new Point(3, 3);
            g.Size = new Size(Width + 60, 61);
            g.TabStop = false;
            g.Text = name;
            //g.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            p.Dock = DockStyle.Fill;
            g.Controls.Add(p);

            boxes.Add(g);

            DrawList(g, p, 40, 35, texts, 5);

            Controls.Add(g);
        }
        public void Set()
        {
            int s = 20;
            for(int i = 0;i < boxes.Count;i++)
            {
                boxes[i].Location = new Point(boxes[i].Location.X, s);
                s += boxes[i].Height + 10;
            }
        }
        public bool inputing = false;
        public void OnInput(string input)
        {
            try
            {
                if (focusedTextbox != null && focusedIndex != -1)
                {
                    inputing = true;
                    focusedTextbox.Text = focusedTextbox.Text.Insert(focusedIndex, input);
                    focusedIndex += input.Length;
                    int p = focusedIndex;
                    focusedTextbox.Focus();
                    focusedTextbox.Select(p, 0);
                    focusedIndex = p;
                    onInput();
                    inputing = false;
                }
            }
            catch (Exception)
            {
            }
        }
        public const string FILE_PATH = "InputPanel.txt";
        private void ChemicalInputPanelControl_Load(object sender, EventArgs e)
        {
            string[] lines = File.ReadAllLines(Application.StartupPath + "\\" + FILE_PATH);
            foreach (string line in lines)
            {
                if (line.StartsWith("//")) continue;
                string[] items = line.Split(',');
                List<Tip> a = new List<Tip>(); 
                for (int i = 1; i < items.Length; i++)
                {
                    string[] c = items[i].Split('/');
                    a.Add(new Tip() { display = c[0], tip = c.Length > 1 ? c[1] : "" }) ;
                }
                DrawBox(items[0], a.ToArray());
            }
            Set();
        }

        private void toolTip1_Draw(object sender, DrawToolTipEventArgs e)//该事件重绘，我的理解是在初始化的时候修改了颜色，然后它就变成了系统色，用于重新绘制（大概理解）
        {
            Font f = new Font("微软雅黑", 9.5f);
            e.DrawBackground();
            e.DrawBorder();
            e.Graphics.DrawString(e.ToolTipText, f, Brushes.White, new PointF(2, 2));
        }
        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
            e.ToolTipSize = TextRenderer.MeasureText(toolTip1.ToolTipTitle, new Font("微软雅黑", 9.5f));
        }

    }
}
