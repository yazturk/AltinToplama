
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Altın_Toplama_Oyunu
{
    enum GameState { Stopped, Running, Paused }
    enum Direc { None, Up, Down, Right, Left }

    static class Program
    {
        public static Gamer[] gamers;
        static Settings1 s;
        private static int Width, Height, GoldCount, 
            InvisibleRate, StartingGold;

        public static Gold[] golds;
        static Random random;

        public static int getWidth
        {
            get { return Width; }
        }
        public static int getHeight
        {
            get { return Height; }
        }
        public static int survivors = 4;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
        }

        public static void Restart()
        //Başlangıçta veya ayarlar değiştiğinde değişkenleri yeniler
        {
            survivors = 4;
            int count = 0, x, y;
            random = new Random();

            s = new Settings1();
            gamers = new Gamer[4];
            gamers[0] = new Gamer("A", 0, 0, s.targetCost_A, s.moveCost_A,
                s.speed_A, s.startingGold, "Blue");
            gamers[1] = new Gamer("B", s.X - 1, 0, s.targetCost_B, s.moveCost_B,
                s.speed_B, s.startingGold, "Green");
            gamers[2] = new Gamer("C", 0, s.Y - 1, s.targetCost_C, s.moveCost_C,
                s.speed_C, s.startingGold, "Red");
            gamers[3] = new Gamer("D", s.X - 1, s.Y - 1, s.targetCost_D, s.moveCost_D,
                s.speed_D, s.startingGold, "Purple");

            Width = s.X;  Height = s.Y;
            GoldCount = (int) (Width*Height*s.goldRate)/100;
            InvisibleRate = s.invisibleRate;
            StartingGold = s.startingGold;
            
            golds = new Gold[GoldCount];

            while (count < GoldCount)
            {
                x = random.Next(0, Width-1); y = random.Next(0, Height-1);
                int amount = random.Next(5, 24);
                amount = 5 * (int)(amount / 5);
                if (isFull(x, y)) continue;
                golds[count] = new 
                    Gold(x, y, amount, random.Next(0, 100) <= InvisibleRate);
                count++;
            }
        }
        public static bool isFull(int x, int y)
        {
            foreach(Gold gold in golds)
            {
                if (gold != null)
                {
                    if (gold.X == x && gold.Y == y) return true;
                }
            }
            for(int i=0; i<4; i++)
            {
                if (gamers[i].X == x && gamers[i].Y == y) return true;
            }
            return false;
        }
        public static void ResetGold(int i)
        {
            int x, y, amount;
            golds[i] = null;
            do
            {
                x = random.Next(0, Width);
                y = random.Next(0, Height);
            } while (isFull(x, y));
            amount = 5 * ((int) (random.Next(5, 24))/5);
            golds[i] = new Gold(x, y, amount, random.Next(0, 100) <= InvisibleRate);
        }
    }
    class Gamer
    {
        public string Name { get; }
        private int x, y;
        private int tcost, mcost, speed, gold, steps;
        public int TargetX, TargetY, targetGold;
        public System.Drawing.SolidBrush Brush { get; }
        private bool isAlive, isMoving;
        private string fileName;

        public int stepCount=0, goldSpent=0, goldCollected=0;

        public bool IsAlive
        {
            get { return isAlive; }
        }
        public int X
        {
            get { return x; }
        }
        public int Y
        {
            get { return y; }
        }
        public int Gold
        {
            get { return gold; }
        }
        public Gamer(string n, int x, int y, int tcost, int mcost, int speed,
            int gold, string color)
        {
            Name = n;
            this.x = x; this.y = y;
            this.tcost = tcost; this.mcost = mcost;
            this.speed = speed; this.gold = gold;
            steps = speed;
            Brush = new
                System.Drawing.SolidBrush(System.Drawing.Color.FromName(color));
            isAlive = true;
            isMoving = false;

            fileName = Name + ".txt";
            string[] dateString = { "\n" + DateTime.Now.ToString() + " Tarihli Oyun\n",
                x.ToString() + ", " + y.ToString() };
            File.AppendAllLines(fileName, dateString);
            // TargetX = 5; TargetY = 5;
        }

        public Direc NextStep()
        {
            if (!isAlive) return Direc.None;
            if (isMoving) //hedef varsa
            {
                if (TargetX != Program.golds[targetGold].X
                    || TargetY != Program.golds[targetGold].Y) //hedef kaybolmuşsa
                {
                    setTarget();
                    return NextStep();
                }
                else //hedef kaybolmamışsa
                {
                    if (x == TargetX && y == TargetY) // hedefe ulaşılmışsa
                    {
                        gold += Program.golds[targetGold].Amount;
                        goldCollected += Program.golds[targetGold].Amount;
                        Program.ResetGold(targetGold);
                        steps = speed;
                        isMoving = false;
                        return Direc.None;
                    }
                    else //hedefe ulaşılmamışsa
                    {
                        lookForGold();
                        if (steps == 0)
                        {
                            steps = speed;
                            return Direc.None;
                        }
                        else //adımlar sıfır değilse
                        {
                            if (x > TargetX)
                                if (tryCell(x - 1, y))
                                    return Direc.Left;
                            if (y > TargetY)
                                if (tryCell(x, y - 1))
                                    return Direc.Up;
                            if (x < TargetX)
                                if (tryCell(x + 1, y))
                                    return Direc.Right;
                            if (y < TargetY)
                                if (tryCell(x, y + 1))
                                    return Direc.Down;


                            if (tryCell(x + 1, y)) return Direc.Right;
                            if (tryCell(x, y + 1)) return Direc.Down;
                            if (tryCell(x - 1, y)) return Direc.Left;
                            if (tryCell(x, y - 1)) return Direc.Up;

                            //tüm yollar kapalıysa
                            steps = speed;
                            return Direc.None;
                        }
                    }
                }
            }
            else //hedef yoksa
            {
                setTarget();
                return NextStep();
            }
        }
        private bool tryCell(int _x, int _y)
        {
            if (_x < 0 || _y < 0)
                return false;
            if (_x >= Program.getWidth || _y >= Program.getHeight)
                return false;
            foreach (Gamer gamer in Program.gamers)
            {
                if (gamer.isAlive && gamer.X == _x && gamer.Y == _y)
                    return false;
            }

            //Sonraki adıma geçiş;
            this.x = _x; this.y = _y;
            steps--;
            stepCount++;
            goldReduce(mcost);
            string[] s = { x.ToString() + ", " + _y.ToString()};
            File.AppendAllLines(fileName, s);
            return true;
        }
        private void goldReduce(int i)
        {
            gold -= i;
            goldSpent += i;
            if (gold <= 0)
            {
                isAlive = false;
                Program.survivors--;
            }
        }
        private void lookForGold()
        {
            for (int i = 0; i < Program.golds.Length; i++)
            {
                if (Program.golds[i].X == x && Program.golds[i].Y == y)
                {
                    if (Program.golds[i].IsInvisible)
                        Program.golds[i].makeVisible();
                    else
                    {
                        this.gold += Program.golds[i].Amount;
                        goldCollected += Program.golds[i].Amount;
                        Program.ResetGold(i);
                    }
                }
            }
        }
        private void setTarget()
        {

            goldReduce(tcost);
            steps = speed;
            switch (Name)
            {
                case "A":
                    targetClosest();
                    break;
                case "B":
                    targetBiggest();
                    break;
                case "C":
                    // isMoving = false;
                    targetAfterReveal();
                    break;
                case "D":
                    targetSmartest();
                    break;
            }
        }
        private int distance(int i) //i, altının indeksi
        {
            return System.Math.Abs(Program.golds[i].X - x)
                + System.Math.Abs(Program.golds[i].Y - y);
        }
        private void target(int index)
        {
            targetGold = index;
            TargetX = Program.golds[index].X;
            TargetY = Program.golds[index].Y;
            isMoving = true;
        }
        private void targetClosest()
        {
            int index = -1;
            int dist = Program.getWidth + Program.getHeight + 1;
            int dist2;
            for (int i = 0; i < Program.golds.Length; i++)
            {
                if (Program.golds[i].IsInvisible) continue;
                dist2 = distance(i);
                if (dist2 < dist)
                {
                    dist = dist2;
                    index = i;
                }
            }
            if (index == -1)
            {
                isMoving = false;
            }
            else target(index);

        }
        private void targetBiggest()
        {
            int index = -1,
                dist = Program.getWidth + Program.getHeight + 1, dist2;
            int amount = 0;

            void update(int i)
            {
                amount = Program.golds[i].Amount;
                dist = distance(i);
                index = i;
            }
            for (int i = 0; i < Program.golds.Length; i++)
            {
                if (Program.golds[i].IsInvisible)
                    continue; //gizli altınlar hedef değil
                if (Program.golds[i].Amount > amount) update(i);
                if (Program.golds[i].Amount == amount)
                {
                    dist2 = distance(i);
                    if (dist2 < dist) update(i);
                }
            }
            if (index == -1) isMoving = false;
            else target(index);
        }
        private void targetAfterReveal()
        {
            int first = -1, second = -1,
                dist1 = Program.getWidth + Program.getHeight + 1,
                dist2 = Program.getWidth + Program.getHeight + 1,
                dist3;

            for (int i = 0; i < Program.golds.Length; i++)
            {
                if (Program.golds[i].IsInvisible)
                {
                    dist3 = distance(i);
                    if (dist3 < dist1)
                    {
                        second = first;
                        first = i;
                        dist2 = dist1;
                        dist1 = dist3;
                    }
                    else if (dist3 < dist2)
                    {
                        second = i;
                        dist2 = dist3;
                    }
                }
            }
            if (first >= 0)
            {
                Program.golds[first].makeVisible();
                if (second >= 0) Program.golds[second].makeVisible();
            }
            targetBiggest();
        }
        private void targetSmartest()
        {

            int index = -1,
                dist = Program.getWidth + Program.getHeight + 1, dist2;
            int amount = 0;
            bool isLost;

            int distBetween(Gamer gamer, Gold gold)
            {
                return System.Math.Abs(gamer.X - gold.X)
                    + System.Math.Abs(gamer.Y - gold.Y);
            }

            void update(int i)
            {
                amount = Program.golds[i].Amount;
                dist = distance(i);
                index = i;
            }
            for (int i = 0; i < Program.golds.Length; i++)
            {
                if (Program.golds[i].IsInvisible)
                    continue; //gizli altınlar hedef değil

                dist2 = distance(i);
                isLost = false;
                for (int g = 0; g < 3; g++)
                {
                    if (Program.gamers[g].TargetX == Program.golds[i].X
                        && Program.gamers[g].TargetY == Program.golds[i].Y
                        && dist2 >= distBetween(Program.gamers[g], Program.golds[i]))
                    {
                        isLost = true;
                        continue;
                    }
                }
                if (isLost) continue;

                if (Program.golds[i].Amount > amount) update(i);
                if (Program.golds[i].Amount == amount)
                {
                    if (dist2 < dist) update(i);
                }
            }
            if (index == -1) isMoving = false;
            else target(index);
        }
    }
    class Gold
    {
        int x, y, amount;
        bool isInvisible;

        public int X
        {
            get { return x; }
        }
        public int Y
        {
            get { return y; }
        }
        public int Amount
        {
            get { return amount; }
        }
        public bool IsInvisible
        {
            get { return isInvisible; }
        }
        public void makeVisible()
        {
            this.isInvisible = false;
        }
        public Gold(int _x, int _y, int _amount, bool _isInvisible)
        {
            x = _x; y = _y; 
            amount = _amount;
            isInvisible = _isInvisible;
        }
    }
}