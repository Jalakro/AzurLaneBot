using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;

namespace AL_Bot
{
    public partial class Form1 : Form
    {
        bool MapFinishTest = false, DockFullTest = false; 
        int DoubleMapItemTest = 0;
        int xMax = 0, yMax = 0;
        bool SecScreenIsPos = false;
        int goalCounter = -1, goalNb;
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

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Working Requirements :\n" + "-Device emulated must be the 'Samsung Galaxy S20 Ultra'\n" + "-Emulator window size must be entire window (not full screen, you must see the task bar)\n" + "-Quick retire must be configured" + " \n" + " \n" + "Text Box and 'Launch' / 'Stop' buttons are used to laucnh a certain number of maps by entering the number in the textbox and then clicking launch");
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
                            PixelColor[i] = GetPixelColor(tmpPixel_X + 1920, tmpPixel_Y);
                        }
                        else
                        {
                            PixelColor[i] = GetPixelColor(tmpPixel_X - 1920, tmpPixel_Y);
                        }
                    }

                    if (/*Avec meta*/PixelColor[i].R == 176 && PixelColor[i].G == 196 && PixelColor[i].B == 222 || /*Avec meta n°2*/PixelColor[i].R == 176 && PixelColor[i].G == 196 && PixelColor[i].B == 229 || /*Sans meta*/ PixelColor[i].R == 90 && PixelColor[i].G == 142 && PixelColor[i].B == 214)
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

            if (/*Avec meta*/PixelColor_.R == 176 && PixelColor_.G == 196 && PixelColor_.B == 222 || /*Avec meta n°2*/PixelColor_.R == 176 && PixelColor_.G == 196 && PixelColor_.B == 229 || /*Sans meta*/ PixelColor_.R == 90 && PixelColor_.G == 142 && PixelColor_.B == 214)
            {
                return true;
            }
            return false;
        }

        private void RelaunchMap()
        {
            if (checkBox3.Checked == true)
            {
                DoMouseClick(1300, 900);
                Thread.Sleep(1000);
            }
            if (checkBox2.Checked == true)
            {
                if (SecScreenIsPos)
                {
                    DoMouseClick(3220, 900);
                    Thread.Sleep(1000);
                }
                else
                {
                    DoMouseClick(-620, 900);
                    Thread.Sleep(1000);
                }
            }
        }

        private void ActivateDoubleMapItem()
        {
            DoubleMapItemTest = CheckDoubleItem();
            if (DoubleMapItemTest == 1)//si item double map voulu mais pas meta
            {
                if (checkBox3.Checked == true)
                {
                    DoMouseClick(1089, 899);
                    Thread.Sleep(1000);
                }
                if (checkBox2.Checked == true)
                {
                    if (SecScreenIsPos)
                    {
                        DoMouseClick(3009, 899);
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        DoMouseClick(-831, 899);
                        Thread.Sleep(1000);
                    }
                }
                DoubleMapItemTest = 0;
            }
            else if (DoubleMapItemTest == 2)//si item double map voulu mais avec meta
            {
                if (checkBox3.Checked == true)
                {
                    DoMouseClick(1188, 906);
                    Thread.Sleep(1000);
                }
                if (checkBox2.Checked == true)
                {
                    if (SecScreenIsPos)
                    {
                        DoMouseClick(3108, 906);
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        DoMouseClick(-732, 906);
                        Thread.Sleep(1000);
                    }
                }
                DoubleMapItemTest = 0;
            }
        }

        private void SortDock()
        {
            if (checkBox3.Checked == true)
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
            if (checkBox2.Checked == true)
            {
                if (SecScreenIsPos)
                {
                    DoMouseClick(2650, 750);
                    Thread.Sleep(4000);
                    DoMouseClick(2970, 970);
                    Thread.Sleep(1500);
                    DoMouseClick(3370, 930);
                    Thread.Sleep(1500);
                    DoMouseClick(3070, 720);
                    Thread.Sleep(1500);
                    DoMouseClick(3070, 720);
                    Thread.Sleep(1500);
                    DoMouseClick(3290, 790);
                    Thread.Sleep(1500);
                    DoMouseClick(3090, 830);
                    Thread.Sleep(1500);
                    DoMouseClick(3090, 830);
                    Thread.Sleep(1500);
                    DoMouseClick(3260, 960);
                    Thread.Sleep(4000);
                    DoMouseClick(3670, 800);
                }
                else
                {
                    DoMouseClick(-1190, 750);
                    Thread.Sleep(4000);
                    DoMouseClick(-870, 970);
                    Thread.Sleep(1500);
                    DoMouseClick(-470, 930);
                    Thread.Sleep(1500);
                    DoMouseClick(-770, 720);
                    Thread.Sleep(1500);
                    DoMouseClick(-770, 720);
                    Thread.Sleep(1500);
                    DoMouseClick(-550, 790);
                    Thread.Sleep(1500);
                    DoMouseClick(-750, 830);
                    Thread.Sleep(1500);
                    DoMouseClick(-750, 830);
                    Thread.Sleep(1500);
                    DoMouseClick(-580, 960);
                    Thread.Sleep(4000);
                    DoMouseClick(-170, 800);
                }
            }
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
                                PixelColor_[i][j] = GetPixelColor(tmpPixel_X + 1920, tmpPixel_Y);
                            }
                            else
                            {
                                PixelColor_[i][j] = GetPixelColor(tmpPixel_X - 1920, tmpPixel_Y);
                            }
                        }

                        if (/*Sans meta*/PixelColor_[i][j].R == 183 && PixelColor_[i][j].G == 160 && PixelColor_[i][j].B == 222)
                        {
                            return 1;
                        }
                        else if (/*Avec meta*/ PixelColor_[i][j].R == 107 && PixelColor_[i][j].G == 101 && PixelColor_[i][j].B == 173)
                        {
                            return 2;
                        }
                    }
                    return 0;
                }
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
                                PixelColor_[i][j] = GetPixelColor(tmpPixel_X + 1920, tmpPixel_Y);
                            }
                            else
                            {
                                PixelColor_[i][j] = GetPixelColor(tmpPixel_X - 1920, tmpPixel_Y);
                            }
                        }

                        if (/*Avec meta*/PixelColor_[i][j].R == 176 && PixelColor_[i][j].G == 196 && PixelColor_[i][j].B == 222 || /*Avec meta n°2*/PixelColor_[i][j].R == 176 && PixelColor_[i][j].G == 196 && PixelColor_[i][j].B == 229 || /*Sans meta*/ PixelColor_[i][j].R == 90 && PixelColor_[i][j].G == 142 && PixelColor_[i][j].B == 214)
                        {
                            return true;
                        }
                    }
                    return false;
                }
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

                    goalCounter++;
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
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}