using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Altın_Toplama_Oyunu
{
    public partial class SettingsForm : Form
    {
        Settings1 mySettings;
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mySettings.speed_A       = (int)numericUpDown1.Value;
            mySettings.targetCost_A  = (int)numericUpDown2.Value;
            mySettings.moveCost_A    = (int)numericUpDown3.Value;
            mySettings.speed_B       = (int)numericUpDown4.Value;
            mySettings.targetCost_B  = (int)numericUpDown5.Value;
            mySettings.moveCost_B    = (int)numericUpDown6.Value;
            mySettings.speed_C       = (int)numericUpDown7.Value;
            mySettings.targetCost_C  = (int)numericUpDown8.Value;
            mySettings.moveCost_C    = (int)numericUpDown9.Value;
            mySettings.speed_D       = (int)numericUpDown10.Value;
            mySettings.targetCost_D  = (int)numericUpDown11.Value;
            mySettings.moveCost_D    = (int)numericUpDown12.Value;
            mySettings.X             = (int)numericUpDown13.Value;
            mySettings.Y             = (int)numericUpDown14.Value;
            mySettings.startingGold  = (int)numericUpDown15.Value;
            mySettings.goldRate      = (int)numericUpDown16.Value;
            mySettings.invisibleRate = (int)numericUpDown17.Value;
            
            mySettings.Save();
            
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            mySettings = new Settings1();

            numericUpDown1.Value = mySettings.speed_A;
            numericUpDown2.Value = mySettings.targetCost_A;
            numericUpDown3.Value = mySettings.moveCost_A;

            numericUpDown4.Value = mySettings.speed_B;
            numericUpDown5.Value = mySettings.targetCost_B;
            numericUpDown6.Value = mySettings.moveCost_B;

            numericUpDown7.Value = mySettings.speed_C;
            numericUpDown8.Value = mySettings.targetCost_C;
            numericUpDown9.Value = mySettings.moveCost_C;

            numericUpDown10.Value = mySettings.speed_D;
            numericUpDown11.Value = mySettings.targetCost_D;
            numericUpDown12.Value = mySettings.moveCost_D;

            numericUpDown13.Value = mySettings.X;
            numericUpDown14.Value = mySettings.Y;
            numericUpDown15.Value = mySettings.startingGold;
            numericUpDown16.Value = mySettings.goldRate;
            numericUpDown17.Value = mySettings.invisibleRate;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = Settings1.Default.speed_A;
            numericUpDown2.Value = Settings1.Default.targetCost_A;
            numericUpDown3.Value = Settings1.Default.moveCost_A;
            numericUpDown4.Value = Settings1.Default.speed_B;
            numericUpDown5.Value = Settings1.Default.targetCost_B;
            numericUpDown6.Value = Settings1.Default.moveCost_B;
            numericUpDown7.Value = Settings1.Default.speed_C;
            numericUpDown8.Value = Settings1.Default.targetCost_C;
            numericUpDown9.Value = Settings1.Default.moveCost_C; 
            numericUpDown10.Value = Settings1.Default.speed_D;
            numericUpDown11.Value = Settings1.Default.targetCost_D;
            numericUpDown12.Value = Settings1.Default.moveCost_D;

            numericUpDown13.Value = Settings1.Default.X;
            numericUpDown14.Value = Settings1.Default.Y;
            numericUpDown15.Value = Settings1.Default.startingGold;
            numericUpDown16.Value = Settings1.Default.goldRate;
            numericUpDown17.Value = Settings1.Default.invisibleRate;
        }
    }
}
