using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Altın_Toplama_Oyunu
{
    public partial class Form1 : Form
    {
        GameState state = GameState.Stopped;
        Direc direction;
        bool isAnimating = false;
        int gamer = 0;
        Graphics graphics;
        Pen gridPen;
        Brush goldBrush, stringBrush;
        Font goldFont;

        //bu değişkenler tahtadaki pikselleri temsil eder
        int animateX, animateY;
        int sw; //square width
        int margin, diameter;
        int targetX, targetY;
        int lastX, lastY;

        private void NextGamer()
        {
            if (gamer == 3) gamer = 0;
            else gamer++;
        }

        public Form1()
        {
            InitializeComponent();
            this.Paint += Form1_Paint;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i <= lastX; i += sw)
                graphics.DrawLine(gridPen, i, 0, i, lastY);
            for (int j = 0; j <= lastY; j += sw)
                graphics.DrawLine(gridPen, 0, j, lastX, j);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.Restart();
            Reload();
        }
        private void Reload()
         // Açılışta veya ayarlar değiştiğinde değişkenleri tazeler
        {
            if (graphics != null)
            {
                graphics.Clear(SystemColors.Control);
                graphics.Dispose();
            }
            graphics = pictureBox1.CreateGraphics();
            gridPen = new Pen(Color.Black, 1);
            goldBrush = new SolidBrush(Color.Yellow);
            stringBrush = new SolidBrush(Color.Black);

            //kare genişliğini hesaplar
            try
            {
                int max = Program.getWidth;
                if (Program.getHeight > max) max = Program.getHeight;
                this.sw = 686 / max;
            }
            catch
            {
                MessageBox.Show("Sıfıra bölme hatası");
            }
            margin = (int) (sw * 0.2);
            diameter = (int) (sw * 0.6);

            lastX = sw * Program.getWidth;
            lastY = sw * Program.getHeight;
            isAnimating = false;
            gamer = 0;

            goldFont = new Font(FontFamily.GenericMonospace, (int)(sw*0.2));
            RedrawGolds();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var sform = new SettingsForm();
            sform.Show();
        }

        private void RedrawGolds()
        {
            foreach (Gold gold in Program.golds)
            {
                int x, y;
                x = gold.X * sw + ((int)(0.3 * sw));
                y = gold.Y * sw + ((int)(0.3 * sw));
                if (!gold.IsInvisible)
                {
                    graphics.FillEllipse(goldBrush,
                     x, y, ((int)(sw * 0.4)), ((int)(sw * 0.4)));
                }    graphics.DrawString(gold.Amount.ToString(), goldFont, stringBrush, x, y);
                
            }
        }
        private bool isArrived()
        {
            switch (direction)
            {
                case Direc.Right:
                    return animateX >= targetX;
                case Direc.Left:
                    return animateX <= targetX;
                case Direc.Up:
                    return animateY <= targetY;
                case Direc.Down:
                    return animateY >= targetY;
            }
            MessageBox.Show("Yön değeri yok");
            return false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var actGamer = Program.gamers[gamer];
            if (Program.survivors == 0)
            {
                Stop();
                return;
            }
            if (isAnimating)
            {
                if(isArrived())
                //if(animateX == targetX && animateY == targetY) //hedefe ulaşıldı
                {
                    isAnimating = false;
                }
                else //hedefe ulaşılmadı
                {
                    pictureBox1.Invalidate(new
                        Rectangle(animateX, animateY, diameter, diameter));
                    pictureBox1.Update();
                    switch(direction)
                    {
                        case Direc.Right:
                            animateX++;
                            break;
                        case Direc.Left:
                            animateX--;
                            break;
                        case Direc.Down:
                            animateY++;
                            break;
                        case Direc.Up:
                            animateY--;
                            break;
                        default:
                            MessageBox.Show("Yön değeri yanlış atanmış");
                            break;
                    }
                    graphics.FillEllipse(actGamer.Brush,
                        animateX, animateY, diameter, diameter);
                }
            }
            else //animasyon yok
            {
                label1.Text = "A : " + Program.gamers[0].Gold;
                label2.Text = "B : " + Program.gamers[1].Gold;
                label3.Text = "C : " + Program.gamers[2].Gold;
                label4.Text = "D : " + Program.gamers[3].Gold;
            //    label5.Text = targetX + " " + targetY;

                RedrawGolds();

                direction = actGamer.NextStep();
                if (direction == Direc.None) 
                    NextGamer();
                else
                {
                    //hedefi belirle
                    targetX = actGamer.X * sw + margin;
                    targetY = actGamer.Y * sw + margin;

                    switch (direction) //başlangıç noktasını belirle
                    {
                        case Direc.Right:
                            animateX = targetX - sw;
                            animateY = targetY;
                            break;
                        case Direc.Left:
                            animateX = targetX + sw;
                            animateY = targetY;
                            break;
                        case Direc.Down:
                            animateX = targetX;
                            animateY = targetY - sw;
                            break;
                        case Direc.Up:
                            animateX = targetX;
                            animateY = targetY + sw;
                            break;
                    }
                    isAnimating = true;
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch (state)
            {
                case GameState.Stopped:
                    Program.Restart();
                    Reload();
                    timer1.Start();
                    state = GameState.Running;
                    button2.Text = "Duraklat";
                    button1.Enabled = false;
                    button3.Enabled = true;
                    break;
                case GameState.Running:
                    timer1.Stop();
                    state = GameState.Paused;
                    button2.Text = "Devam et";
                    break;
                case GameState.Paused:
                    timer1.Start();
                    state = GameState.Running;
                    button2.Text = "Duraklat";
                    break;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Stop();
        }
        private void Stop()
        {
            timer1.Stop();
            state = GameState.Stopped;
            button1.Enabled = true;
            button2.Text = "Başlat";
            button3.Enabled = false;

            pictureBox1.Refresh();
            graphics.FillRectangle(
                new SolidBrush(Color.Black),
                140, 200, 400, 300);
            var brush = new SolidBrush(Color.White);
            var font = new Font(FontFamily.GenericSansSerif, 12);
            var g = Program.gamers;

            graphics.DrawString("Adım sayısı : ", font, brush, 150, 260);
            graphics.DrawString("Harcanan altın : ", font, brush, 150, 320);
            graphics.DrawString("Kalan altın : ", font, brush, 150, 380);
            graphics.DrawString("Toplanan altın : ", font, brush, 150, 440);

            for(int i=0; i<4; i++)
            {
                graphics.DrawString(g[i].Name, font, brush, 290 + i*65, 210);
                graphics.DrawString(g[i].stepCount.ToString(), 
                    font, brush, 290 + i * 65, 260);
                graphics.DrawString(g[i].goldSpent.ToString(), 
                    font, brush, 290 + i * 65, 320);
                graphics.DrawString(g[i].Gold.ToString(), 
                    font, brush, 290 + i * 65, 380);
                graphics.DrawString(g[i].goldCollected.ToString(), 
                    font, brush, 290 + i * 65, 440);

            }

        }
    }
}
