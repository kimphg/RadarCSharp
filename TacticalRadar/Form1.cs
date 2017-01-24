using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TacticalRadar
{
    
    public partial class RadarDisplay : Form
    {
        int mWidth = 1024;
        int mHeight = 768;
        int centerX, centerY;
        int timerInterval = 10;
        int radius = 350;
        Graphics bmG;
        double angle = 0;
        double dAngle;
        Bitmap bm;
        target t1,t2;
        public RadarDisplay()
        {
            InitializeComponent();
            //khoi tao cac gia tri 
            centerX = mWidth / 2;
            centerY = mHeight / 2;
            this.timerUpdateForm.Interval = timerInterval;
            dAngle = 6.0 * 3.1415926535 * (timerInterval / 10000.0);
            //khoi tao cua so
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Size = new Size(mWidth, mHeight);
            this.Location = new Point(0, 0);
           
            //cai dat moi truong do hoa

            bm = new Bitmap(mWidth, mHeight);
            bmG = Graphics.FromImage(bm);
            bmG.Clear(Color.Black);
            bmG.PixelOffsetMode = PixelOffsetMode.HighQuality;
            bmG.SmoothingMode = SmoothingMode.HighQuality;
            this.SetStyle(
                System.Windows.Forms.ControlStyles.UserPaint |
                System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                true);
            // khoi tao muc tieu 
            t1 = new target(150,170,-2,-3);
            t2 = new target(-190,120,3,-2);
            
        }

        private void timerUpdateForm_Tick(object sender, EventArgs e)
        {
            //cap nhat goc quet
            angle += dAngle;
            if (angle > 2.0 * 3.1415926535)
            {
                angle = 0;
                t1.isUpdated = false;
                t2.isUpdated = false;
            }
            //ve tia quet
            System.Drawing.Pen pen = new System.Drawing.Pen(Color.White);
            bmG.Clear(Color.Transparent);
            bmG.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2);
            bmG.DrawLine(pen, centerX, centerY,
                            centerX + (int)(radius * Math.Sin(angle)), centerY + (int)(radius * Math.Cos(angle)));
            //update muc tieu khi co tia quet chay qua
            if ((angle <= t1.angle) && (angle + dAngle > t1.angle)) 
                t1.Update();
            if ((angle <= t2.angle) && (angle + dAngle > t2.angle)) 
                t2.Update();
            //ve track
            pen = new System.Drawing.Pen(Color.White, 2);
            foreach (Point p in t1.track)
            {
                bmG.DrawRectangle(pen, centerX + p.X-1, centerY + p.Y-1, 2, 2);
            }
            foreach (Point p in t2.track)
            {
                bmG.DrawRectangle(pen, centerX + p.X-1, centerY + p.Y-1, 2, 2);
            }
            //ve muc tieu
            pen = new System.Drawing.Pen(Color.Red, 2);
            if(t1.track.Count>3)bmG.DrawRectangle(pen, centerX +t1.mx-2, centerY + t1.my-2, 4, 4);
            if (t2.track.Count > 3) bmG.DrawRectangle(pen, centerX + t2.mx - 2, centerY + t2.my - 2, 4, 4);
            
            this.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            pe.Graphics.DrawImage(bm, 0, 0);
            
        }
    }
    public class target
    {
        public int mx, my;
        double mvx, mvy;
        public double angle = 0;
        public List<Point> track;
        public bool isUpdated;
        public target(int x, int y, double vx, double vy)
        {
            track = new List<Point>();
            mx = x;
            my = y;
            mvx = vx;
            mvy = vy;
            if (my != 0) angle = Math.Atan((double)mx / (double)my);
            else 
            {
                if (mx > 0) angle = 3.1415926535 / 2;
                else angle = -3.1415926535 / 2;
            }
            if (my < 0) angle += 3.1415926535;
            if (angle < 0) angle += 3.1415926535 * 2.0;

            track.Add(new Point(mx, my));
        }
        public void Update()
        {
            if (isUpdated) return;
            mx += (int)mvx;
            my += (int)mvy;
            if (my != 0) angle = Math.Atan((double)mx / (double)my);
            else
            {
                if (mx > 0) angle = 3.1415926535 / 2;
                else angle = -3.1415926535 / 2;
            }
            if (my < 0) angle += 3.1415926535;
            if (angle < 0) angle += 3.1415926535*2.0;
            track.Add(new Point(mx, my));
            isUpdated = true;
        }
    }
}
