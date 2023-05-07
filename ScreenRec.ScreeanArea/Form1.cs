using Microsoft.VisualBasic.Devices;
using ScreenRec.Settings;

namespace ScreenRec.ScreeanArea
{
    public delegate void ScreeanAreaFormClosing(MonitorNum monitorNum);

    public partial class Form1 : Form
    {
        public ScreeanAreaFormClosing AreaFormClosing { get; set; }
        public MonitorNum MonitorNum { get; set; }

        Pen redpen = new Pen(Color.Red, 3);
        Pen bluepen = new Pen(Color.Blue, 3);
        bool startdraw = false;
        bool addrec = true;
        Rectangle lastrect;
        //List<Rectangle> ListRect = new List<Rectangle>();
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Добавить арию
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            addrec = !addrec;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ScreenRec.Settings.Sett.Save();
            redpen.Dispose();
            bluepen.Dispose();
            AreaFormClosing?.Invoke(MonitorNum);
        }
        Point startpoint;
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!addrec) return;
            startdraw = !startdraw;
            if (!startdraw) { return; }
            startpoint = e.Location;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!addrec) return;
            if (startdraw && e.Button == MouseButtons.Left)
            {
                DrawRects();
                using var g = this.CreateGraphics();
                var s = new Size(e.Location.X - startpoint.X, e.Location.Y - startpoint.Y);
                lastrect = new Rectangle(startpoint, s);
                g.DrawRectangle(bluepen, lastrect);
            }
        }

        public void DrawRects()
        {
            using var g = this.CreateGraphics();
            g.Clear(Color.White);
            foreach (Rectangle rect in MonitorNum?.Rectangles)
            {
                g.DrawRectangle(redpen, rect);
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!addrec) return;
            if (startdraw) MonitorNum.Rectangles.Add(lastrect);
            startdraw = !startdraw;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MonitorNum.Rectangles.Clear();
            using var g = this.CreateGraphics();
            g.Clear(Color.White);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // DrawRects();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            DrawRects();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawRects();
        }
    }
}