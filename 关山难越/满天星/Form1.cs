using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace 满天星
{
    public partial class Form1 : Form
    {
        private readonly Graphics _graphics;
        private readonly List<PointF> _pointFs;
        private readonly Random _random;
        private readonly ConcurrentDictionary<PointF, byte> _alphaDic;
        public Form1()
        {
            InitializeComponent();
            _graphics = this.panel1.CreateGraphics();
            _pointFs = new List<PointF>();
            _random = new Random();
            _alphaDic = new ConcurrentDictionary<PointF, byte>();
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer |ControlStyles.ResizeRedraw |ControlStyles.AllPaintingInWmPaint, true);

            // 控制闪烁
            Task.Factory.StartNew(() =>
            {
                var back = false;
                while (true)
                {
                    Thread.Sleep(1000);
                    foreach (var item in _alphaDic.Keys)
                    {
                        if (_alphaDic[item] > 0)
                            _alphaDic[item] = 0;
                        else
                            _alphaDic[item] = 255;

                    }
                    this.panel1.Invoke(new Action(() => {
                        this.panel1.Invalidate();
                    }));
                }
            });
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var item in _pointFs)
            {
                var color = Color.FromArgb(_alphaDic[item], 255,0, 0);
                var brush = new SolidBrush(color);
                //var font = SystemFonts.DefaultFont;
                var fontFamily = new FontFamily(SystemFonts.DefaultFont.Name);
                var font = new Font(fontFamily, 48);
                    
                _graphics.DrawString("*",font,brush,item);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if(int.TryParse(textBox1.Text, out int num))
            {
                for (int i = 0; i < num; i++)
                {
                    var maxWidth = this.panel1.Width;
                    var maxHeight = this.panel1.Height;
                    var point = new PointF(_random.Next(0, maxWidth), _random.Next(0, maxHeight));
                    _pointFs.Add(point);
                    _alphaDic.TryAdd(point, 255);
                }
                this.panel1.Invalidate();
            }
            else
            {
                MessageBox.Show("请输入整数!");
            }
            textBox1.Text = "";
            textBox1.Focus();
            
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            _pointFs.Clear();
            _alphaDic.Clear();
            this.panel1.Invalidate();
            _graphics.Clear(this.panel1.BackColor);
        }
    }
}
