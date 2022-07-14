using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;

namespace AL_Bot
{
    public partial class Form1 : Form
    {
        bool MapFinishTest = false, DockFullTest = false, IsExercise = false, ExerciseAsked = false; 
        int DoubleMapItemTest = 0, ExerciseWinLoseTest = 0;
        int xMax = 0, yMax = 0;
        bool SecScreenIsPos = false;
        int goalCounter = -1, goalNb;
        int goalCounterExercise = -1, goalNbExercise;
        Screen[] screens;

        //données et fonction pour faire un click
        #region
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        #endregion
        //

        //Get les pixels de l'écran
        #region
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
        #endregion
        //

        public Form1()
        {
            InitializeComponent();
            InitializeTimer();

            screens = System.Windows.Forms.Screen.AllScreens;
            if (screens.Length < 2)
            {
                button4.Enabled = false;
                checkBox3.Checked = true;
                checkBox2.Checked = false;
            }
            else
            {
                if (screens[1].Bounds.X > 0)
                {
                    SecScreenIsPos = true;
                }
                else
                {
                    SecScreenIsPos = false;
                }
            }

            if (checkBox3.Checked == true)
            {
                xMax = screens[0].Bounds.Width;
                yMax = screens[0].Bounds.Height;
            }
            else
            {
                xMax = screens[1].Bounds.Width;
                yMax = screens[1].Bounds.Height;
            }

        }

        private void InitializeTimer()
        {
            // Set to 1 second  
            myTimer.Interval = 1000;
            myTimer.Tick += new EventHandler(myTimer_Tick);

            // init timer state  
            myTimer.Enabled = false;
        }


        //Buttons Functions
        #region
        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                myTimer.Enabled = false;
                checkBox1.Checked = false;
            }
            else
            {
                myTimer.Enabled = true;
                checkBox1.Checked = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Color tmp = GetPixelColor(Cursor.Position.X, Cursor.Position.Y);
            MessageBox.Show("X :" + Cursor.Position.X + " Y :" + Cursor.Position.Y + " RGB :" + tmp.R + " " + tmp.G + " " + tmp.B);
        }


        private void button4_Click(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                checkBox2.Checked = true;
                checkBox3.Checked = false;
            }
            else
            {
                checkBox2.Checked = false;
                checkBox3.Checked = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (button8.Enabled == false && goalCounterExercise >= 0)
            {
                myTimer.Enabled = false;
                button1.Enabled = true;
                goalCounterExercise = -1;
                ExerciseAsked = false;
                button8.Enabled = true;
                checkBox7.Checked = false;
                button11.Enabled = true;
                button10.Enabled = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Working Requirements :\n" + "-Device emulated must be the 'Samsung Galaxy S20 Ultra'\n" + "-Emulator window size must be entire window (not full screen, you must see the task bar)\n" + "-Quick retire must be configured" + " \n" + " \n" + "Text Box and 'Launch' / 'Stop' buttons are used to launch a certain number of maps by entering the number in the textbox and then clicking launch");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
            {
                checkBox4.Checked = false;
            }
            else
            {
                checkBox4.Checked = true;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            bool successfullyParsed = int.TryParse(textBox1.Text, out goalNb);

            if (successfullyParsed && goalNb > 0)
            {
                if (checkBox1.Checked == true)
                {
                    myTimer.Enabled = false;
                    checkBox1.Checked = false;
                }
                goalCounter = 0;
                button1.Enabled = false;
                myTimer.Enabled = true;
                button8.Enabled = false;
                textBox1.Enabled = false;
            }
            else
            {
                MessageBox.Show("Texte entrée non valide");
            }
        }


        private void button9_Click(object sender, EventArgs e)
        {
            if (button8.Enabled == false && goalCounter >= 0)
            {
                myTimer.Enabled = false;
                button1.Enabled = true;
                goalCounter = -1;
                button8.Enabled = true;
                textBox1.Enabled = true;
                textBox1.Text = "";
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ExerciseAsked = true;

            if (checkBox1.Checked == true)
            {
                myTimer.Enabled = false;
                checkBox1.Checked = false;
            }
            goalCounterExercise = 0;
            if(checkBox5.Checked == true)
            {
                goalNbExercise = 5;
            }
            else if(checkBox6.Checked == true)
            {
                goalNbExercise = 10;
            }
            button1.Enabled = false;
            myTimer.Enabled = true;
            button8.Enabled = false;
            button11.Enabled = false;
            button10.Enabled = false;
            checkBox7.Checked = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true)
            {
                checkBox6.Checked = true;
                checkBox5.Checked = false;
            }
            else
            {
                checkBox6.Checked = false;
                checkBox5.Checked = true;
            }
        }
        #endregion



        public void DoMouseClick(int X, int Y)
        {
            int pre_height = Cursor.Clip.Size.Height, pre_width = Cursor.Clip.Size.Width, preXRect = Cursor.Clip.X, preYRect = Cursor.Clip.Y;
            int pre_x = Cursor.Position.X, pre_y = Cursor.Position.Y;

            if (xMax != 1920 && yMax != 1080)
            {
                X = X * xMax / 1920;
                Y = Y * yMax / 1080;
            }

            if (checkBox2.Checked == true)
            {
                if (SecScreenIsPos)
                {
                    X += xMax;
                }
                else
                {
                    X -= xMax;
                }
            }

            Rectangle BoundRect = new Rectangle(X, Y, 1, 1);
            Cursor.Clip = BoundRect;

            Thread.Sleep(100);
            Cursor.Position = new Point(X, Y);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)X, (uint)Y, 0, 0);
            Thread.Sleep(100);

            Rectangle FullRect = new Rectangle(preXRect, preYRect, pre_width, pre_height);
            Cursor.Clip = FullRect;

            Cursor.Position = new Point(pre_x, pre_y);
        }

        static public System.Drawing.Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF),
                         (int)(pixel & 0x0000FF00) >> 8,
                         (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }

        private bool CheckMapFinish()
        {
            if (xMax != 1920 && yMax != 1080)
            {
                int PixelToTest_X = 1300, PixelToTest_Y = 900;

                Color[] PixelColor = new Color[4]; // dans cet ordre : x, x+1, y, y+1

                for (int i = 0; i < 4; i++)
                {
                    int tmpPixel_X = PixelToTest_X, tmpPixel_Y = PixelToTest_Y;

                    tmpPixel_X = tmpPixel_X * xMax / 1920;
                    tmpPixel_Y = tmpPixel_Y * yMax / 1080;


                    if (i == 1 || i == 3)
                    {
                        tmpPixel_X += 1;
                    }
                    if (i == 2 || i == 3)
                    {
                        tmpPixel_Y += 1;
                    }

                    if (checkBox3.Checked == true)
                    {
                        PixelColor[i] = GetPixelColor(tmpPixel_X, tmpPixel_Y);
                    }
                    else
                    {
                        if (SecScreenIsPos)
                        {
                            PixelColor[i] = GetPixelColor(tmpPixel_X + xMax, tmpPixel_Y);
                        }
                        else
                        {
                            PixelColor[i] = GetPixelColor(tmpPixel_X - xMax, tmpPixel_Y);
                        }
                    }

                    if (/*Avec meta*/PixelColor[i].R >= 166 && PixelColor[i].G >= 186 && PixelColor[i].B >= 212 && PixelColor[i].R <= 186 && PixelColor[i].G <= 206 && PixelColor[i].B <= 232 || /*Avec meta n°2*/PixelColor[i].R >= 166 && PixelColor[i].G >= 186 && PixelColor[i].B >= 219 && PixelColor[i].R <= 186 && PixelColor[i].G <= 206 && PixelColor[i].B <= 239 || /*Sans meta*/ PixelColor[i].R >= 80 && PixelColor[i].G >= 132 && PixelColor[i].B >= 204 && PixelColor[i].R <= 100 && PixelColor[i].G <= 152 && PixelColor[i].B <= 224)
                    {
                        return true;
                    }
                }
                return false;
            }

            Color PixelColor_;

            if (checkBox3.Checked == true)
            {
                PixelColor_ = GetPixelColor(1300, 900);
            }
            else
            {
                if (SecScreenIsPos)
                {
                    PixelColor_ = GetPixelColor(3220, 900);
                }
                else
                {
                    PixelColor_ = GetPixelColor(-620, 900);
                }
            }

            if (/*Avec meta*/PixelColor_.R >= 166 && PixelColor_.G >= 186 && PixelColor_.B >= 212 && PixelColor_.R <= 186 && PixelColor_.G <= 206 && PixelColor_.B <= 232 || /*Avec meta n°2*/PixelColor_.R >= 166 && PixelColor_.G >= 186 && PixelColor_.B >= 219 && PixelColor_.R <= 186 && PixelColor_.G <= 206 && PixelColor_.B <= 239 || /*Sans meta*/ PixelColor_.R >= 80 && PixelColor_.G >= 132 && PixelColor_.B >= 204 && PixelColor_.R <= 100 && PixelColor_.G <= 152 && PixelColor_.B <= 224)
            {
                return true;
            }
            return false;
        }

        private void RelaunchMap()
        {
            DoMouseClick(1300, 900);
            Thread.Sleep(1000);

            if (button8.Enabled == false)
            {
                goalCounter++;
            }
        }

        private void ActivateDoubleMapItem()
        {
            DoubleMapItemTest = CheckDoubleItem();
            if (DoubleMapItemTest == 1)//si item double map voulu mais pas meta
            {
                DoMouseClick(1089, 899);
                Thread.Sleep(1000);
                
                DoubleMapItemTest = 0;
            }
            else if (DoubleMapItemTest == 2)//si item double map voulu mais avec meta
            {
                DoMouseClick(1188, 906);
                Thread.Sleep(1000);
                
                DoubleMapItemTest = 0;
            }
        }

        private void SortDock()
        {
                DoMouseClick(730, 750);
                Thread.Sleep(4000);
                DoMouseClick(1050, 970);
                Thread.Sleep(1500);
                DoMouseClick(1450, 930);
                Thread.Sleep(1500);
                DoMouseClick(1150, 720);
                Thread.Sleep(1500);
                DoMouseClick(1150, 720);
                Thread.Sleep(1500);
                DoMouseClick(1370, 790);
                Thread.Sleep(1500);
                DoMouseClick(1170, 830);
                Thread.Sleep(1500);
                DoMouseClick(1170, 830);
                Thread.Sleep(1500);
                DoMouseClick(1340, 960);
                Thread.Sleep(4000);
                DoMouseClick(1750, 800);
        }

        private int CheckDoubleItem()
        {
            if (xMax != 1920 && yMax != 1080)
            {
                int[] PixelToTest_X = new int[2];
                PixelToTest_X[0] = 815;
                PixelToTest_X[1] = 959;
                int[] PixelToTest_Y = new int[2];
                PixelToTest_Y[0] = 923;
                PixelToTest_Y[1] = 875;

                PixelToTest_X[0] = PixelToTest_X[0] * xMax / 1920;
                PixelToTest_X[1] = PixelToTest_X[1] * xMax / 1920;
                PixelToTest_Y[0] = PixelToTest_Y[0] * yMax / 1080;
                PixelToTest_Y[1] = PixelToTest_Y[1] * yMax / 1080;

                Color[][] PixelColor_ = new Color[2][]; // dans cet ordre : x, x+1, y, y+1
                PixelColor_[0] = new Color[4];
                PixelColor_[1] = new Color[4];


                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int tmpPixel_X = PixelToTest_X[i], tmpPixel_Y = PixelToTest_Y[i];

                        if (j == 1 || j == 3)
                        {
                            tmpPixel_X += 1;
                        }
                        if (j == 2 || j == 3)
                        {
                            tmpPixel_Y += 1;
                        }

                        if (checkBox3.Checked == true)
                        {
                            PixelColor_[i][j] = GetPixelColor(tmpPixel_X, tmpPixel_Y);
                        }
                        else
                        {
                            if (SecScreenIsPos)
                            {
                                PixelColor_[i][j] = GetPixelColor(tmpPixel_X + xMax, tmpPixel_Y);
                            }
                            else
                            {
                                PixelColor_[i][j] = GetPixelColor(tmpPixel_X - xMax, tmpPixel_Y);
                            }
                        }

                        if (/*Sans meta*/PixelColor_[i][j].R >= 173 && PixelColor_[i][j].G >= 150 && PixelColor_[i][j].B >= 212 && PixelColor_[i][j].R <= 193 && PixelColor_[i][j].G <= 170 && PixelColor_[i][j].B <= 232)
                        {
                            return 1;
                        }
                        else if (/*Avec meta*/PixelColor_[i][j].R >= 97 && PixelColor_[i][j].G >= 91 && PixelColor_[i][j].B >= 163 && PixelColor_[i][j].R <= 117 && PixelColor_[i][j].G <= 111 && PixelColor_[i][j].B <= 183)
                        {
                            return 2;
                        }
                    }
                }
                return 0;
            }

            Color PixelColor;
            Color PixelColor2;

            if (checkBox3.Checked == true)
            {
                PixelColor = GetPixelColor(815, 923);
                PixelColor2 = GetPixelColor(959, 875);
            }
            else
            {
                if (SecScreenIsPos)
                {
                    PixelColor = GetPixelColor(2735, 923);
                    PixelColor2 = GetPixelColor(2879, 875);
                }
                else
                {
                    PixelColor = GetPixelColor(-1105, 923);
                    PixelColor2 = GetPixelColor(-961, 875);
                }
            }

            if (/*Sans meta*/PixelColor.R == 183 && PixelColor.G == 160 && PixelColor.B == 222)                
            {
                return 1;
            }
            else if (/*Avec meta*/ PixelColor2.R == 107 && PixelColor2.G == 101 && PixelColor2.B == 173)
            {
                return 2;
            }
            return 0;
        }

        private bool CheckDockFull()
        {
            if (xMax != 1920 && yMax != 1080)
            {
                int[] PixelToTest_X = new int[2];
                PixelToTest_X[0] = 1347;
                PixelToTest_X[1] = 562;
                int[] PixelToTest_Y = new int[2];
                PixelToTest_Y[0] = 338;
                PixelToTest_Y[1] = 712;

                PixelToTest_X[0] = PixelToTest_X[0] * xMax / 1920;
                PixelToTest_X[1] = PixelToTest_X[1] * xMax / 1920;
                PixelToTest_Y[0] = PixelToTest_Y[0] * yMax / 1080;
                PixelToTest_Y[1] = PixelToTest_Y[1] * yMax / 1080;

                Color[][] PixelColor_ = new Color[2][]; // dans cet ordre : x, x+1, y, y+1
                PixelColor_[0] = new Color[4];
                PixelColor_[1] = new Color[4];
                

                for(int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int tmpPixel_X = PixelToTest_X[i], tmpPixel_Y = PixelToTest_Y[i];

                        if (j == 1 || j == 3)
                        {
                            tmpPixel_X += 1;
                        }
                        if (j == 2 || j == 3)
                        {
                            tmpPixel_Y += 1;
                        }

                        if (checkBox3.Checked == true)
                        {
                            PixelColor_[i][j] = GetPixelColor(tmpPixel_X, tmpPixel_Y);
                        }
                        else
                        {
                            if (SecScreenIsPos)
                            {
                                PixelColor_[i][j] = GetPixelColor(tmpPixel_X + xMax, tmpPixel_Y);
                            }
                            else
                            {
                                PixelColor_[i][j] = GetPixelColor(tmpPixel_X - xMax, tmpPixel_Y);
                            }
                        }

                        if (/*croix rouge*/PixelColor_[i][j].R >= 172 && PixelColor_[i][j].G >= 137 && PixelColor_[i][j].B >= 148 && PixelColor_[i][j].R <= 196 && PixelColor_[i][j].G <= 159 && PixelColor_[i][j].B <= 168 || PixelColor_[i][j].R >= 47 && PixelColor_[i][j].G >= 90 && PixelColor_[i][j].B >= 146 && PixelColor_[i][j].R <= 67 && PixelColor_[i][j].G <= 110 && PixelColor_[i][j].B <= 166)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            Color PixelColor;
            Color PixelColor2;

            if (checkBox3.Checked == true)
            {
                PixelColor = GetPixelColor(1347, 338);
                PixelColor2 = GetPixelColor(562, 712);
            }
            else
            {
                if (SecScreenIsPos)
                {
                    PixelColor = GetPixelColor(3267, 338);
                    PixelColor2 = GetPixelColor(2482, 712);
                }
                else
                {
                    PixelColor = GetPixelColor(-573, 338);
                    PixelColor2 = GetPixelColor(-1358, 712);
                }
            }
            
            if (PixelColor.R >= 177 && PixelColor.R <= 191 && PixelColor.G >= 142 && PixelColor.G <= 154 && PixelColor.B >= 153 && PixelColor.B <= 163 || PixelColor2.R == 57 && PixelColor2.G == 100 && PixelColor2.B == 156)
            {
                return true;
            }
            return false;
        }

        private bool CheckIfExercise()
        {
            if (xMax != 1920 && yMax != 1080)
            {
                int[] PixelToTest_X = new int[2];
                PixelToTest_X[0] = 457;
                PixelToTest_X[1] = 1331;
                int[] PixelToTest_Y = new int[2];
                PixelToTest_Y[0] = 567;
                PixelToTest_Y[1] = 567;

                PixelToTest_X[0] = PixelToTest_X[0] * xMax / 1920;
                PixelToTest_X[1] = PixelToTest_X[1] * xMax / 1920;
                PixelToTest_Y[0] = PixelToTest_Y[0] * yMax / 1080;
                PixelToTest_Y[1] = PixelToTest_Y[1] * yMax / 1080;

                Color[][] PixelColor_ = new Color[2][]; // dans cet ordre : x, x+1, y, y+1
                PixelColor_[0] = new Color[4];
                PixelColor_[1] = new Color[4];

                int count = 0; //need both pixels to be detected


                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int tmpPixel_X = PixelToTest_X[i], tmpPixel_Y = PixelToTest_Y[i];

                        if (j == 1 || j == 3)
                        {
                            tmpPixel_X += 1;
                        }
                        if (j == 2 || j == 3)
                        {
                            tmpPixel_Y += 1;
                        }

                        if (checkBox3.Checked == true)
                        {
                            PixelColor_[i][j] = GetPixelColor(tmpPixel_X, tmpPixel_Y);
                        }
                        else
                        {
                            if (SecScreenIsPos)
                            {
                                PixelColor_[i][j] = GetPixelColor(tmpPixel_X + xMax, tmpPixel_Y);
                            }
                            else
                            {
                                PixelColor_[i][j] = GetPixelColor(tmpPixel_X - xMax, tmpPixel_Y);
                            }
                        }

                        if (/*premier joueur*/PixelColor_[i][j].R >= 183 && PixelColor_[i][j].G >= 187 && PixelColor_[i][j].B >= 190 && PixelColor_[i][j].R <= 203 && PixelColor_[i][j].G <= 207 && PixelColor_[i][j].B <= 210  || /*quatrième joueur*/PixelColor_[i][j].R >= 188 && PixelColor_[i][j].G >= 193 && PixelColor_[i][j].B >= 196 && PixelColor_[i][j].R <= 208 && PixelColor_[i][j].G <= 213 && PixelColor_[i][j].B <= 216)
                        {
                            count++;
                        }
                    }
                }

                if (count >= 2)
                {
                    return true;
                }
                return false;
            }

            Color PixelColor;
            Color PixelColor2;

            if (checkBox3.Checked == true)
            {
                PixelColor = GetPixelColor(457, 567);
                PixelColor2 = GetPixelColor(1331, 567);
            }
            else
            {
                if (SecScreenIsPos)
                {
                    PixelColor = GetPixelColor(2377, 567);
                    PixelColor2 = GetPixelColor(3251, 567);
                }
                else
                {
                    PixelColor = GetPixelColor(-1463, 567);
                    PixelColor2 = GetPixelColor(-589, 567);
                }
            }

            if (PixelColor.R == 193 && PixelColor.R <= 197 && PixelColor.G >= 200 || PixelColor2.R == 198 && PixelColor2.G == 203 && PixelColor2.B == 206)
            {
                return true;
            }
            return false;
        }

        private void LaunchExerciseAttack()
        {
            IsExercise = CheckIfExercise();
            if(IsExercise == true)
            {
                DoMouseClick(337, 405);
                Thread.Sleep(1000);
                DoMouseClick(941, 828);
                Thread.Sleep(2000);
                DoMouseClick(1629, 912);
                Thread.Sleep(1000);
            }
        }

        private int CheckExerciseWinOrLose()
        {
            if (xMax != 1920 && yMax != 1080)
            {
                int[] PixelToTest_X = new int[2];
                PixelToTest_X[0] = 988;
                PixelToTest_X[1] = 1198;
                int[] PixelToTest_Y = new int[2];
                PixelToTest_Y[0] = 386;
                PixelToTest_Y[1] = 557;

                PixelToTest_X[0] = PixelToTest_X[0] * xMax / 1920;
                PixelToTest_X[1] = PixelToTest_X[1] * xMax / 1920;
                PixelToTest_Y[0] = PixelToTest_Y[0] * yMax / 1080;
                PixelToTest_Y[1] = PixelToTest_Y[1] * yMax / 1080;

                Color[][] PixelColor_ = new Color[2][]; // dans cet ordre : x, x+1, y, y+1
                PixelColor_[0] = new Color[4];
                PixelColor_[1] = new Color[4];


                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int tmpPixel_X = PixelToTest_X[i], tmpPixel_Y = PixelToTest_Y[i];

                        if (j == 1 || j == 3)
                        {
                            tmpPixel_X += 1;
                        }
                        if (j == 2 || j == 3)
                        {
                            tmpPixel_Y += 1;
                        }

                        if (checkBox3.Checked == true)
                        {
                            PixelColor_[i][j] = GetPixelColor(tmpPixel_X, tmpPixel_Y);
                        }
                        else
                        {
                            if (SecScreenIsPos)
                            {
                                PixelColor_[i][j] = GetPixelColor(tmpPixel_X + xMax, tmpPixel_Y);
                            }
                            else
                            {
                                PixelColor_[i][j] = GetPixelColor(tmpPixel_X - xMax, tmpPixel_Y);
                            }
                        }

                        if (/*victoire*/PixelColor_[i][j].R >= 215 && PixelColor_[i][j].G >= 198 && PixelColor_[i][j].B >= 97 && PixelColor_[i][j].R <= 235 && PixelColor_[i][j].G <= 218 && PixelColor_[i][j].B <= 117)
                        {
                            return 1;
                        }
                        else if (/*défaite*/ PixelColor_[i][j].R >= 216 && PixelColor_[i][j].G >= 96 && PixelColor_[i][j].B >= 98 && PixelColor_[i][j].R <= 236 && PixelColor_[i][j].G <= 116 && PixelColor_[i][j].B <= 118)
                        {
                            return 2;
                        }
                    }
                }
                return 0;
            }

            Color PixelColor;
            Color PixelColor2;
            

            if (checkBox3.Checked == true)
            {
                PixelColor = GetPixelColor(988, 386);
                PixelColor2 = GetPixelColor(1198, 557);
            }
            else
            {
                if (SecScreenIsPos)
                {
                    PixelColor = GetPixelColor(2908, 386);
                    PixelColor2 = GetPixelColor(3118, 557);
                }
                else
                {
                    PixelColor = GetPixelColor(-932, 386);
                    PixelColor2 = GetPixelColor(-722, 557);
                }
            }

            if (/*victoire*/PixelColor.R == 225 && PixelColor.G == 208 && PixelColor.B == 107)
            {
                return 1;
            }
            else if (/*défaite*/ PixelColor2.R == 226 && PixelColor2.G == 106 && PixelColor2.B == 108)
            {
                return 2;
            }
            return 0;
        }

        private void FinishExerciseAttack()
        {
            ExerciseWinLoseTest = CheckExerciseWinOrLose();
            if (ExerciseWinLoseTest == 1)//si win
            {
                DoMouseClick(1558, 962);
                Thread.Sleep(1000);
                DoMouseClick(1558, 962);
                Thread.Sleep(2000);
                DoMouseClick(1558, 962);
                Thread.Sleep(2000);
                
                ExerciseWinLoseTest = 0;
                goalCounterExercise++;
            }
            else if (ExerciseWinLoseTest == 2)//si lost
            {
                DoMouseClick(1558, 962);
                Thread.Sleep(1000);
                DoMouseClick(1558, 962);
                Thread.Sleep(2000);
                DoMouseClick(1558, 962);
                Thread.Sleep(2000);
                DoMouseClick(926, 887);
                Thread.Sleep(2000);
                
                ExerciseWinLoseTest = 0;
                goalCounterExercise++;
            }
        }



        private void myTimer_Tick(object sender, EventArgs e)
        {
            if(checkBox3.Checked == true)
            {
                xMax = screens[0].Bounds.Width;
                yMax = screens[0].Bounds.Height;
            }
            else
            {
                xMax = screens[1].Bounds.Width;
                yMax = screens[1].Bounds.Height;
            }
            

            DockFullTest = CheckDockFull();
            if (DockFullTest)
            {
                SortDock();
            }

            MapFinishTest = CheckMapFinish();
            if (MapFinishTest)
            {
                if(goalCounter >= 0)//si mode compteur
                {
                    if (checkBox4.Checked == true)//If double map item wanted
                    {
                        ActivateDoubleMapItem();
                    }

                    RelaunchMap();

                    if (goalCounter >= goalNb)
                    {
                        myTimer.Enabled = false;
                        button1.Enabled = true;
                        goalCounter = -1;
                        button8.Enabled = true;
                        textBox1.Enabled = true;
                        textBox1.Text = "";
                    }
                }
                else
                {
                    if (checkBox4.Checked == true)//If double map item wanted
                    {
                        ActivateDoubleMapItem();
                    }

                    RelaunchMap();
                }
            }

            if(ExerciseAsked == true)
            {
                LaunchExerciseAttack();
                FinishExerciseAttack();

                if (goalCounterExercise >= goalNbExercise)
                {
                    myTimer.Enabled = false;
                    button1.Enabled = true;
                    goalCounterExercise = -1;
                    ExerciseAsked = false;
                    button8.Enabled = true;
                    checkBox7.Checked = false;
                    button11.Enabled = true;
                    button10.Enabled = true;
                }
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}