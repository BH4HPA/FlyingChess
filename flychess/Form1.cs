using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace flychess
{
    public partial class Form1 : Form
    {
        //创建类的一个实例化对象
        static ResourceManager rm = new ResourceManager("flychess.Properties.Resources", Assembly.GetExecutingAssembly());
        //读取棋子资源
        public Image redChess = ((Image)(rm.GetObject("redChess")));
        public Image yellowChess = ((Image)(rm.GetObject("yellowChess")));
        public Image greenChess = ((Image)(rm.GetObject("greenChess")));
        public Image blueChess = ((Image)(rm.GetObject("blueChess")));
        public Image step1 = ((Image)(rm.GetObject("step1")));
        public Image step2 = ((Image)(rm.GetObject("step2")));
        public Image step3 = ((Image)(rm.GetObject("step3")));
        public Image step4 = ((Image)(rm.GetObject("step4")));
        public Image step5 = ((Image)(rm.GetObject("step5")));
        public Image step6 = ((Image)(rm.GetObject("step6")));
        //常量与变量
        bool startGame = true;
        int whoGo = 1;
        int whoAm = 1;
        int redStart;
        int yellowStart;
        int blueStart;
        int greenStart;
        int redInt;
        int yellowInt;
        int blueInt;
        int greenInt;
        int step = 0;
        //挂接socket线程
        Socket socketClient = new Socket(SocketType.Stream, ProtocolType.Tcp);
        public Form1()
        {
            InitializeComponent();
            timerOut.Value = 0;
            
            rs1.Image = redChess;
            rs2.Image = redChess;
            rs3.Image = redChess;
            rs4.Image = redChess;
            ys1.Image = yellowChess;
            ys2.Image = yellowChess;
            ys3.Image = yellowChess;
            ys4.Image = yellowChess;
            ls1.Image = blueChess;
            ls2.Image = blueChess;
            ls3.Image = blueChess;
            ls4.Image = blueChess;
            gs1.Image = greenChess;
            gs2.Image = greenChess;
            gs3.Image = greenChess;
            gs4.Image = greenChess;
            redInt = 1;
            yellowInt = 1;
            blueInt = 1;
            greenInt = 1;

            //挂接socket线程
            IPAddress ip = IPAddress.Parse("140.143.235.48");
            IPEndPoint point = new IPEndPoint(ip, 33351);
            //进行连接
            socketClient.Connect(point);

            //不停的接收服务器端发送的消息
            Thread thread = new Thread(Ray.Recive);
            thread.IsBackground = true;
            thread.Start(socketClient);
            var buffter = Encoding.UTF8.GetBytes($"ready");
            var temp = socketClient.Send(buffter);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void y11_Click(object sender, EventArgs e)
        {
            moveChess(y11);
        }
        public void moveChess(PictureBox pictureBox)
        {
            if (startGame == false)
            {
                return;
            }
            if (whoGo != whoAm) {
                return;
            }
            if (step == 0)
            {
                return;
            }
            Image chessSet;
            string changePic;
            int chessId;
            string flyNext;
            string picid = pictureBox.Name.Substring(1);
            Random rd = new Random();
            //MessageBox.Show(step.ToString());
            if (pictureBox.Image == redChess)
            {
                if (whoAm != 1)
                {
                    return;
                }
                chessSet = redChess;
            }
            else {
                if (pictureBox.Image == yellowChess)
                {
                    if (whoAm != 2)
                    {
                        return;
                    }
                    chessSet = yellowChess;
                }
                else
                {
                    if (pictureBox.Image == blueChess)
                    {
                        if (whoAm != 3)
                        {
                            return;
                        }
                        chessSet = blueChess;
                    }
                    else
                    {
                        if (pictureBox.Image == greenChess)
                        {
                            if (whoAm != 4)
                            {
                                return;
                            }
                            chessSet = greenChess;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            timer1.Enabled = true;
            if (picid == "s")
            {
                if(step == 2 | step == 6)
                {
                    step = step + 4;
                }
                changePic = Ray.GetLeft(pictureBox.Name, "s") + (step + 3).ToString();
                pictureBox.Image = null;
                object changeo = this.GetType().GetField(changePic, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                checkChess((PictureBox)changeo);
                ((PictureBox)changeo).Image = chessSet;
                nextMove();
                return;
            }
            if (pictureBox.Name.StartsWith("r"))
            {
                //红色区域
                if (picid.StartsWith("e")) {
                    if (int.Parse(picid.Substring(1)) + step < 6)
                    {
                        flyNext = "re" + (int.Parse(picid.Substring(1)) + step).ToString();
                    }
                    else {
                        flyNext = "re" + (int.Parse(picid.Substring(1)) + step - (int.Parse(picid.Substring(1)) + step - 5) * 2).ToString();
                    }
                    pictureBox.Image = null;
                    object changeo = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    checkChess((PictureBox)changeo);
                    ((PictureBox)changeo).Image = chessSet;
                    if (((PictureBox)changeo).Name == "re5")
                    {
                        //游戏结束
                        ((PictureBox)changeo).Image = null;
                        putEndChess("rs", chessSet);
                        redInt = redInt - 1;
                    }
                    nextMove();
                    return;
                }
                if (int.Parse(picid) == 12)
                {
                    step = step + 12;
                }
                if (int.Parse(picid) == 1)
                {
                    if (pictureBox.Image == redChess) {
                        //折跃
                        pictureBox.Image = null;
                        changePic = "re" + (step - 1).ToString();
                        object change = this.GetType().GetField(changePic, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                        checkChess((PictureBox)change);
                        ((PictureBox)change).Image = chessSet;
                        if (((PictureBox)change).Name == "re5")
                        {
                            //游戏结束
                            ((PictureBox)change).Image = null;
                            putEndChess("rs", chessSet);
                            redInt = redInt - 1;
                        }
                        nextMove();
                        return;
                    }
                }
                if (int.Parse(picid) + step < 14)
                {
                    changePic = "r" + (int.Parse(picid) + step).ToString();
                }
                else {
                    if (pictureBox.Image == yellowChess)
                    {
                        //折跃
                        pictureBox.Image = null;
                        if (int.Parse(picid) + step - 13 == 1)
                        {
                            changePic = "y1";
                        }
                        else
                        {
                            changePic = "ye" + (int.Parse(picid) + step - 14).ToString();
                        }
                        object change = this.GetType().GetField(changePic, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                        checkChess((PictureBox)change);
                        ((PictureBox)change).Image = chessSet;
                        nextMove();
                        return;
                    }
                    changePic = "y" + (int.Parse(picid) + step - 13).ToString();
                }
                chessId = int.Parse(picid) + step;
                Console.WriteLine(changePic);
                int colorid = int.Parse(picid) % 4;
                pictureBox.Image = null;
                Console.WriteLine(changePic);
                object o = this.GetType().GetField(changePic, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                if (chessSet == redChess)
                {
                    if (((PictureBox)o).BackColor == Color.Red)
                    {
                        if (chessId + 4 < 14)
                        {
                            flyNext = "r" + (chessId + 4).ToString();
                        }
                        else
                        {
                            flyNext = "y" + (chessId + 4 - 13).ToString();
                        }
                        Console.WriteLine(flyNext);
                        o = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    }
                }
                if (chessSet == yellowChess)
                {
                    if (((PictureBox)o).BackColor == Color.Yellow)
                    {
                        if (chessId + 4 < 14)
                        {
                            flyNext = "r" + (chessId + 4).ToString();
                        }
                        else
                        {
                            flyNext = "y" + (chessId + 4 - 13).ToString();
                        }
                        Console.WriteLine(flyNext);
                        o = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    }
                }
                if (chessSet == blueChess)
                {
                    if (((PictureBox)o).BackColor == Color.Aqua)
                    {
                        if (chessId + 4 < 14)
                        {
                            flyNext = "r" + (chessId + 4).ToString();
                        }
                        else
                        {
                            flyNext = "y" + (chessId + 4 - 13).ToString();
                        }
                        Console.WriteLine(flyNext);
                        o = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    }
                }
                if (chessSet == greenChess)
                {
                    if (((PictureBox)o).BackColor == Color.Lime)
                    {
                        if (chessId + 4 < 14)
                        {
                            flyNext = "r" + (chessId + 4).ToString();
                        }
                        else
                        {
                            flyNext = "y" + (chessId + 4 - 13).ToString();
                        }
                        Console.WriteLine(flyNext);
                        o = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    }
                }
                checkChess((PictureBox)o);
                ((PictureBox)o).Image = chessSet;
            }
            else
            {
                if (pictureBox.Name.StartsWith("y"))
                {
                    //黄色区域
                    if (picid.StartsWith("e"))
                    {
                        if (int.Parse(picid.Substring(1)) + step < 6)
                        {
                            flyNext = "ye" + (int.Parse(picid.Substring(1)) + step).ToString();
                        }
                        else
                        {
                            flyNext = "ye" + (int.Parse(picid.Substring(1)) + step - (int.Parse(picid.Substring(1)) + step - 5) * 2).ToString();
                        }
                        pictureBox.Image = null;
                        object changeo = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                        checkChess((PictureBox)changeo);
                        ((PictureBox)changeo).Image = chessSet;
                        if (((PictureBox)changeo).Name == "ye5")
                        {
                            //游戏结束
                            ((PictureBox)changeo).Image = null;
                            putEndChess("ys", chessSet);
                            yellowInt = yellowInt - 1;
                        }
                        nextMove();
                        return;
                    }
                    if (int.Parse(picid) == 1)
                    {
                        if (pictureBox.Image == yellowChess)
                        {
                            //折跃
                            pictureBox.Image = null;
                            changePic = "ye" + (int.Parse(picid) + step - 1).ToString();
                            object change = this.GetType().GetField(changePic, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                            checkChess((PictureBox)change);
                            ((PictureBox)change).Image = chessSet;
                            if (((PictureBox)change).Name == "ye5")
                            {
                                //游戏结束
                                ((PictureBox)change).Image = null;
                                putEndChess("ys", chessSet);
                                yellowInt = yellowInt - 1;
                            }
                            nextMove();
                            return;
                        }
                    }
                    if (int.Parse(picid) + step < 14)
                    {
                        changePic = "y" + (int.Parse(picid) + step).ToString();
                    }
                    else
                    {
                        if (pictureBox.Image == blueChess)
                        {
                            //折跃
                            pictureBox.Image = null;
                            if (int.Parse(picid) + step - 13 == 1)
                            {
                                changePic = "l1";
                            }
                            else
                            {
                                changePic = "le" + (int.Parse(picid) + step - 14).ToString();
                            }
                            object change = this.GetType().GetField(changePic, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                            checkChess((PictureBox)change);
                            ((PictureBox)change).Image = chessSet;
                            nextMove();
                            return;
                        }
                        changePic = "l" + (int.Parse(picid) + step - 13).ToString();
                    }
                    chessId = int.Parse(picid) + step;
                    Console.WriteLine("change" + changePic);
                    pictureBox.Image = null;
                    object o = this.GetType().GetField(changePic, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                    if (chessSet == redChess)
                    {
                        if (((PictureBox)o).BackColor == Color.Red)
                        {
                            if (chessId + 4 < 14)
                            {
                                flyNext = "y" + (chessId + 4).ToString();
                            }
                            else
                            {
                                flyNext = "l" + (chessId + 4 - 13).ToString();
                            }
                            Console.WriteLine(flyNext);
                            o = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                        }
                    }
                    if (chessSet == yellowChess)
                    {
                        if (((PictureBox)o).BackColor == Color.Yellow)
                        {
                            if (chessId + 4 < 14)
                            {
                                flyNext = "y" + (chessId + 4).ToString();
                            }
                            else
                            {
                                flyNext = "l" + (chessId + 4 - 13).ToString();
                            }
                            Console.WriteLine(flyNext);
                            o = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                        }
                    }
                    if (chessSet == blueChess)
                    {
                        if (((PictureBox)o).BackColor == Color.Aqua)
                        {
                            if (chessId + 4 < 14)
                            {
                                flyNext = "y" + (chessId + 4).ToString();
                            }
                            else
                            {
                                flyNext = "l" + (chessId + 4 - 13).ToString();
                            }
                            Console.WriteLine(flyNext);
                            o = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                        }
                    }
                    if (chessSet == greenChess)
                    {
                        if (((PictureBox)o).BackColor == Color.Lime)
                        {
                            if (chessId + 4 < 14)
                            {
                                flyNext = "y" + (chessId + 4).ToString();
                            }
                            else
                            {
                                flyNext = "l" + (chessId + 4 - 13).ToString();
                            }
                            Console.WriteLine(flyNext);
                            o = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                        }
                    }
                    checkChess((PictureBox)o);
                    ((PictureBox)o).Image = chessSet;
                }
                else
                {
                    if (pictureBox.Name.StartsWith("l"))
                    {
                        //蓝色区域
                        if (picid.StartsWith("e"))
                        {
                            if (int.Parse(picid.Substring(1)) + step < 6)
                            {
                                flyNext = "le" + (int.Parse(picid.Substring(1)) + step).ToString();
                            }
                            else
                            {
                                flyNext = "le" + (int.Parse(picid.Substring(1)) + step - (int.Parse(picid.Substring(1)) + step - 5) * 2).ToString();
                            }
                            pictureBox.Image = null;
                            object changeo = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                            checkChess((PictureBox)changeo);
                            ((PictureBox)changeo).Image = chessSet;
                            if (((PictureBox)changeo).Name == "le5")
                            {
                                //游戏结束
                                ((PictureBox)changeo).Image = null;
                                putEndChess("ls", chessSet);
                                blueInt = blueInt - 1;
                            }
                            nextMove();
                            return;
                        }
                        if (int.Parse(picid) == 1)
                        {
                            if (pictureBox.Image == blueChess)
                            {
                                //折跃
                                pictureBox.Image = null;
                                changePic = "le" + (int.Parse(picid) + step - 1).ToString();
                                object change = this.GetType().GetField(changePic, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                                checkChess((PictureBox)change);
                                ((PictureBox)change).Image = chessSet;
                                if (((PictureBox)change).Name == "le5")
                                {
                                    //游戏结束
                                    ((PictureBox)change).Image = null;
                                    putEndChess("ls", chessSet);
                                    blueInt = blueInt - 1;
                                }
                                nextMove();
                                return;
                            }
                        }
                        if (int.Parse(picid) + step < 14)
                        {
                            changePic = "l" + (int.Parse(picid) + step).ToString();
                        }
                        else
                        {
                            if (pictureBox.Image == greenChess)
                            {
                                //折跃
                                pictureBox.Image = null;
                                if (int.Parse(picid) + step - 13 == 1)
                                {
                                    changePic = "g1";
                                }
                                else
                                {
                                    changePic = "ge" + (int.Parse(picid) + step - 14).ToString();
                                }
                                object change = this.GetType().GetField(changePic, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                                checkChess((PictureBox)change);
                                ((PictureBox)change).Image = chessSet;
                                nextMove();
                                return;
                            }
                            changePic = "g" + (int.Parse(picid) + step - 13).ToString();
                        }
                        chessId = int.Parse(picid) + step;
                        Console.WriteLine(changePic);
                        pictureBox.Image = null;
                        object o = this.GetType().GetField(changePic, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                        if (chessSet == redChess)
                        {
                            if (((PictureBox)o).BackColor == Color.Red)
                            {
                                if (chessId + 4 < 14)
                                {
                                    flyNext = "l" + (chessId + 4).ToString();
                                }
                                else
                                {
                                    flyNext = "g" + (chessId + 4 - 13).ToString();
                                }
                                Console.WriteLine(flyNext);
                                o = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                            }
                        }
                        if (chessSet == yellowChess)
                        {
                            if (((PictureBox)o).BackColor == Color.Yellow)
                            {
                                if (chessId + 4 < 14)
                                {
                                    flyNext = "l" + (chessId + 4).ToString();
                                }
                                else
                                {
                                    flyNext = "g" + (chessId + 4 - 13).ToString();
                                }
                                Console.WriteLine(flyNext);
                                o = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                            }
                        }
                        if (chessSet == blueChess)
                        {
                            if (((PictureBox)o).BackColor == Color.Aqua)
                            {
                                if (chessId + 4 < 14)
                                {
                                    flyNext = "l" + (chessId + 4).ToString();
                                }
                                else
                                {
                                    flyNext = "g" + (chessId + 4 - 13).ToString();
                                }
                                Console.WriteLine(flyNext);
                                o = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                            }
                        }
                        if (chessSet == greenChess)
                        {
                            if (((PictureBox)o).BackColor == Color.Lime)
                            {
                                if (chessId + 4 < 14)
                                {
                                    flyNext = "l" + (chessId + 4).ToString();
                                }
                                else
                                {
                                    flyNext = "g" + (chessId + 4 - 13).ToString();
                                }
                                Console.WriteLine(flyNext);
                                o = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                            }
                        }
                        checkChess((PictureBox)o);
                        ((PictureBox)o).Image = chessSet;
                    }
                    else
                    {
                        if (pictureBox.Name.StartsWith("g"))
                        {
                            //绿色区域
                            if (picid.StartsWith("e"))
                            {
                                if (int.Parse(picid.Substring(1)) + step < 6)
                                {
                                    flyNext = "ge" + (int.Parse(picid.Substring(1)) + step).ToString();
                                }
                                else
                                {
                                    flyNext = "ge" + (int.Parse(picid.Substring(1)) + step - (int.Parse(picid.Substring(1)) + step - 5) * 2).ToString();
                                }
                                pictureBox.Image = null;
                                object changeo = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                                checkChess((PictureBox)changeo);
                                ((PictureBox)changeo).Image = chessSet;
                                if (((PictureBox)changeo).Name == "ge5")
                                {
                                    //游戏结束
                                    ((PictureBox)changeo).Image = null;
                                    putEndChess("gs", chessSet);
                                    greenInt = greenInt - 1;
                                }
                                nextMove();
                                return;
                            }
                            if (int.Parse(picid) == 1)
                            {
                                if (pictureBox.Image == greenChess)
                                {
                                    //折跃
                                    pictureBox.Image = null;
                                    changePic = "ge" + (int.Parse(picid) + step - 1).ToString();
                                    object change = this.GetType().GetField(changePic, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                                    checkChess((PictureBox)change);
                                    ((PictureBox)change).Image = chessSet;
                                    if (((PictureBox)change).Name == "ge5")
                                    {
                                        //游戏结束
                                        ((PictureBox)change).Image = null;
                                        putEndChess("gs", chessSet);
                                        greenInt = greenInt - 1;
                                    }
                                    nextMove();
                                    return;
                                }
                            }
                            if (int.Parse(picid) + step < 14)
                            {
                                changePic = "g" + (int.Parse(picid) + step).ToString();
                            }
                            else
                            {
                                if (pictureBox.Image == redChess)
                                {
                                    //折跃
                                    pictureBox.Image = null;
                                    if (int.Parse(picid) + step - 13 == 1)
                                    {
                                        changePic = "r1";
                                    }
                                    else
                                    {
                                        changePic = "re" + (int.Parse(picid) + step - 14).ToString();
                                    }
                                    object change = this.GetType().GetField(changePic, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                                    checkChess((PictureBox)change);
                                    ((PictureBox)change).Image = chessSet;
                                    nextMove();
                                    return;
                                }
                                changePic = "r" + (int.Parse(picid) + step - 13).ToString();
                            }
                            chessId = int.Parse(picid) + step;
                            Console.WriteLine(changePic);
                            pictureBox.Image = null;
                            object o = this.GetType().GetField(changePic, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                            if (chessSet == redChess)
                            {
                                if (((PictureBox)o).BackColor == Color.Red)
                                {
                                    if (chessId + 4 < 14)
                                    {
                                        flyNext = "g" + (chessId + 4).ToString();
                                    }
                                    else
                                    {
                                        flyNext = "r" + (chessId + 4 - 13).ToString();
                                    }
                                    Console.WriteLine(flyNext);
                                    o = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                                }
                            }
                            if (chessSet == yellowChess)
                            {
                                if (((PictureBox)o).BackColor == Color.Yellow)
                                {
                                    if (chessId + 4 < 14)
                                    {
                                        flyNext = "g" + (chessId + 4).ToString();
                                    }
                                    else
                                    {
                                        flyNext = "r" + (chessId + 4 - 13).ToString();
                                    }
                                    Console.WriteLine(flyNext);
                                    o = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                                }
                            }
                            if (chessSet == blueChess)
                            {
                                if (((PictureBox)o).BackColor == Color.Aqua)
                                {
                                    if (chessId + 4 < 14)
                                    {
                                        flyNext = "g" + (chessId + 4).ToString();
                                    }
                                    else
                                    {
                                        flyNext = "r" + (chessId + 4 - 13).ToString();
                                    }
                                    Console.WriteLine(flyNext);
                                    o = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                                }
                            }
                            if (chessSet == greenChess)
                            {
                                if (((PictureBox)o).BackColor == Color.Lime)
                                {
                                    if (chessId + 4 < 14)
                                    {
                                        flyNext = "g" + (chessId + 4).ToString();
                                    }
                                    else
                                    {
                                        flyNext = "r" + (chessId + 4 - 13).ToString();
                                    }
                                    Console.WriteLine(flyNext);
                                    o = this.GetType().GetField(flyNext, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                                }
                            }
                            checkChess((PictureBox)o);
                            ((PictureBox)o).Image = chessSet;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            nextMove();
        }
        void nextMove()
        {
            timer1.Enabled = false;
            timerOut.Value = 0;
            step = 0;
            whoGo = whoGo + 1;
            if (whoGo == 5)
            {
                whoGo = 1;
            }
            whoAm = whoGo;
            if(whoGo == 1 & redInt == 0)
            {
                whoGo = whoGo + 1;
                if (whoGo == 5)
                {
                    whoGo = 1;
                }
                whoAm = whoGo;
            }
            if (whoGo == 2 & yellowInt == 0)
            {
                whoGo = whoGo + 1;
                if (whoGo == 5)
                {
                    whoGo = 1;
                }
                whoAm = whoGo;
            }
            if (whoGo == 3 & blueInt == 0)
            {
                whoGo = whoGo + 1;
                if (whoGo == 5)
                {
                    whoGo = 1;
                }
                whoAm = whoGo;
            }
            if (whoGo == 4 & greenInt == 0)
            {
                whoGo = whoGo + 1;
                if (whoGo == 5)
                {
                    whoGo = 1;
                }
                whoAm = whoGo;
            }
            if(redInt == 0 & yellowInt == 0 & blueInt == 0 & greenInt == 0)
            {
                MessageBox.Show ("游戏结束。");
                startGame = false;
                whoGo = 0;
            }
            timer1.Enabled = true;
        }
        void putEndChess(string startStr,Image chessImage)
        {
            object put = this.GetType().GetField(startStr + "1", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
            if(((PictureBox) put).Image == null)
            {
                ((PictureBox)put).Image = chessImage;
                ((PictureBox)put).Enabled = false;
                return;
            }
            put = this.GetType().GetField(startStr + "2", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
            if (((PictureBox)put).Image == null)
            {
                ((PictureBox)put).Image = chessImage;
                ((PictureBox)put).Enabled = false;
                return;
            }
            put = this.GetType().GetField(startStr + "3", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
            if (((PictureBox)put).Image == null)
            {
                ((PictureBox)put).Image = chessImage;
                ((PictureBox)put).Enabled = false;
                return;
            }
            put = this.GetType().GetField(startStr + "4", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
            if (((PictureBox)put).Image == null)
            {
                ((PictureBox)put).Image = chessImage;
                ((PictureBox)put).Enabled = false;
                return;
            }
        }
        void goChess(PictureBox pictureBox)
        {
            if (startGame == false)
            {
                return;
            }
            if (whoGo != whoAm)
            {
                return;
            }
            if (step == 0)
            {
                return;
            }
            object change = this.GetType().GetField(pictureBox.Name.Substring(0,2), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
            if (pictureBox.Image != null & pictureBox.Enabled == true & ((PictureBox)change).Image == null & step > 4)
            {
                ((PictureBox)change).Image = pictureBox.Image;
                pictureBox.Image = null;
                nextMove();
            }
            if(pictureBox.Name.Substring(0,1) == "r")
            {
                redStart += 1;
            }
            else {
                if (pictureBox.Name.Substring(0, 1) == "y")
                {
                    yellowStart += 1;
                }
                else {
                    if (pictureBox.Name.Substring(0, 1) == "l")
                    {
                        blueStart += 1;
                    }
                    else {
                        greenStart += 1;
                    }
                }
            }
        }
        void checkChess(PictureBox pictureBox)
        {
            if (((PictureBox)pictureBox).Image != null)
            {
                if (pictureBox.Image == redChess)
                {
                    restartChess("rs", redChess);
                    redStart -= 1;
                }
                else
                {
                    if (pictureBox.Image == yellowChess)
                    {
                        restartChess("ys", yellowChess);
                        yellowStart -= 1;
                    }
                    else
                    {
                        if (pictureBox.Image == blueChess)
                        {
                            restartChess("ls", blueChess);
                            blueStart -= 1;
                        }
                        else
                        {
                            if (pictureBox.Image == greenChess)
                            {
                                restartChess("gs", greenChess);
                                greenStart -= 1;
                            }
                        }
                    }
                }
            }
        }
        void restartChess(string startStr, Image chessImage)
        {
            object put = this.GetType().GetField(startStr + "1", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
            if (((PictureBox)put).Image == null)
            {
                ((PictureBox)put).Image = chessImage;
                return;
            }
            put = this.GetType().GetField(startStr + "2", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
            if (((PictureBox)put).Image == null)
            {
                ((PictureBox)put).Image = chessImage;
                return;
            }
            put = this.GetType().GetField(startStr + "3", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
            if (((PictureBox)put).Image == null)
            {
                ((PictureBox)put).Image = chessImage;
                return;
            }
            put = this.GetType().GetField(startStr + "4", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
            if (((PictureBox)put).Image == null)
            {
                ((PictureBox)put).Image = chessImage;
                return;
            }
        }
        private void y12_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void y13_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void l1_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void l2_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void l3_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void l4_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void l5_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void l6_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void l7_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void l8_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void l9_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void l10_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void l11_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void l12_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void l13_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void g1_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void g2_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void g3_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void g4_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void g5_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void g6_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void g7_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void g8_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void g9_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void g10_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void g11_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void g12_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void g13_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void r1_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void r2_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void r3_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void r4_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void r5_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void r6_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void r7_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void r8_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void r9_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void r10_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void r11_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void r12_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void r13_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void y1_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void y2_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void y3_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void y4_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void y5_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void y6_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void y7_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void y8_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void y9_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void y10_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void re1_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void re2_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void re3_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void re4_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void re5_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void ye1_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void ye2_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void ye3_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void ye4_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void ye5_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void le1_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void le2_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void le3_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void le4_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void le5_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void ge1_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void ge2_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void ge3_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void ge4_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void ge5_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void ys1_Click(object sender, EventArgs e)
        {
            goChess((PictureBox)sender);
        }

        private void ys2_Click(object sender, EventArgs e)
        {
            goChess((PictureBox)sender);
        }

        private void ys3_Click(object sender, EventArgs e)
        {
            goChess((PictureBox)sender);
        }

        private void ys4_Click(object sender, EventArgs e)
        {
            goChess((PictureBox)sender);
        }

        private void ls1_Click(object sender, EventArgs e)
        {
            goChess((PictureBox)sender);
        }

        private void ls3_Click(object sender, EventArgs e)
        {
            goChess((PictureBox)sender);
        }

        private void ls4_Click(object sender, EventArgs e)
        {
            goChess((PictureBox)sender);
        }

        private void ls2_Click(object sender, EventArgs e)
        {
            goChess((PictureBox)sender);
        }

        private void rs1_Click(object sender, EventArgs e)
        {
            goChess((PictureBox)sender);
        }

        private void rs2_Click(object sender, EventArgs e)
        {
            goChess((PictureBox)sender);
        }

        private void rs3_Click(object sender, EventArgs e)
        {
            goChess((PictureBox)sender);
        }

        private void rs4_Click(object sender, EventArgs e)
        {
            goChess((PictureBox)sender);
        }

        private void gs1_Click(object sender, EventArgs e)
        {
            goChess((PictureBox)sender);
        }

        private void gs2_Click(object sender, EventArgs e)
        {
            goChess((PictureBox)sender);
        }

        private void gs3_Click(object sender, EventArgs e)
        {
            goChess((PictureBox)sender);
        }

        private void gs4_Click(object sender, EventArgs e)
        {
            goChess((PictureBox)sender);
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (startGame == false)
            {
                return;
            }
            if (whoGo != whoAm)
            {
                return;
            }
            if (step != 0)
            {
                return;
            }
            Random rd = new Random();
            step = rd.Next(1, 6);
            if (step == 1)
            {
                stepBox.Image = step1;
            }
            else
            {
                if (step == 2)
                {
                    stepBox.Image = step2;
                }
                else
                {
                    if (step == 3)
                    {
                        stepBox.Image = step3;
                    }
                    else
                    {
                        if (step == 4)
                        {
                            stepBox.Image = step4;
                        }
                        else
                        {
                            if (step == 5)
                            {
                                stepBox.Image = step5;
                            }
                            else
                            {
                                stepBox.Image = step6;
                            }
                        }
                    }
                }
            }
            if (whoAm == 1)
            {
                stepBox.BackColor = Color.Red;
            }
            if (whoAm == 2)
            {
                stepBox.BackColor = Color.Yellow;
            }
            if (whoAm == 3)
            {
                stepBox.BackColor = Color.Aqua;
            }
            if (whoAm == 4)
            {
                stepBox.BackColor = Color.Lime;
            }
            if (whoAm == 1 & redStart == 4 & step < 5) {
                nextMove();
                return;
            }
            if (whoAm == 2 & yellowStart == 4 & step < 5)
            {
                nextMove();
                return;
            }
            if (whoAm == 3 & blueStart == 4 & step < 5)
            {
                nextMove();
                return;
            }
            if (whoAm == 4 & greenStart == 4 & step < 5)
            {
                nextMove();
                return;
            }
        }

        private void rs_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void ys_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void ls_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void gs_Click(object sender, EventArgs e)
        {
            moveChess((PictureBox)sender);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timerOut.Value = timerOut.Value + 1;
            if (timerOut.Value == timerOut.Maximum)
            {
                timer1.Enabled = false;
                timerOut.Value = 0;
                nextMove();
            }
        }
    }

    public class Ray
    {
        /// <summary>
        /// 取文本左边内容
        /// </summary>
        /// <param name="str">文本</param>
        /// <param name="s">标识符</param>
        /// <returns>左边内容</returns>
        public static string GetLeft(string str, string s)
        {
            string temp = str.Substring(0, str.IndexOf(s));
            return temp;
        }


        /// <summary>
        /// 取文本右边内容
        /// </summary>
        /// <param name="str">文本</param>
        /// <param name="s">标识符</param>
        /// <returns>右边内容</returns>
        public static string GetRight(string str, string s)
        {
            string temp = str.Substring(str.IndexOf(s), str.Length - str.Substring(0, str.IndexOf(s)).Length);
            return temp;
        }

        /// <summary>
        /// 取文本中间内容
        /// </summary>
        /// <param name="str">原文本</param>
        /// <param name="leftstr">左边文本</param>
        /// <param name="rightstr">右边文本</param>
        /// <returns>返回中间文本内容</returns>
        public static string Between(string str, string leftstr, string rightstr)
        {
            int i = str.IndexOf(leftstr) + leftstr.Length;
            string temp = str.Substring(i, str.IndexOf(rightstr, i) - i);
            return temp;
        }


        /// <summary>
        /// 取文本中间到List集合
        /// </summary>
        /// <param name="str">文本字符串</param>
        /// <param name="leftstr">左边文本</param>
        /// <param name="rightstr">右边文本</param>
        /// <returns>List集合</returns>
        public List<string> BetweenArr(string str, string leftstr, string rightstr)
        {
            List<string> list = new List<string>();
            int leftIndex = str.IndexOf(leftstr);//左文本起始位置
            int leftlength = leftstr.Length;//左文本长度
            int rightIndex = 0;
            string temp = "";
            while (leftIndex != -1)
            {
                rightIndex = str.IndexOf(rightstr, leftIndex + leftlength);
                if (rightIndex == -1)
                {
                    break;
                }
                temp = str.Substring(leftIndex + leftlength, rightIndex - leftIndex - leftlength);
                list.Add(temp);
                leftIndex = str.IndexOf(leftstr, rightIndex + 1);
            }
            return list;
        }


        /// <summary>
        /// 指定文本倒序
        /// </summary>
        /// <param name="str">文本</param>
        /// <returns>倒序文本</returns>
        public static string StrReverse(string str)
        {
            char[] chars = str.ToCharArray();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < chars.Length; i++)
            {
                sb.Append(chars[chars.Length - 1 - i]);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 通过FileStream 来打开文件，这样就可以实现不锁定Image文件，到时可以让多用户同时访问Image文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Bitmap ReadImageFile(string path)
        {
            FileStream fs = File.OpenRead(path); //OpenRead
            int filelength = 0;
            filelength = (int)fs.Length; //获得文件长度 
            Byte[] image = new Byte[filelength]; //建立一个字节数组 
            fs.Read(image, 0, filelength); //按字节流读取 
            System.Drawing.Image result = System.Drawing.Image.FromStream(fs);
            fs.Close();
            Bitmap bit = new Bitmap(result);
            return bit;
        }
        /// 延迟
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)//毫秒
            {
                Application.DoEvents();//可执行某无聊的操作
            }
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="o"></param>
        public static void Recive(object o)
        {
            var send = o as Socket;
            while (true)
            {
                //获取发送过来的消息
                byte[] buffer = new byte[1024 * 1024 * 2];
                var effective = send.Receive(buffer);
                if (effective == 0)
                {
                    break;
                }
                var str = Encoding.UTF8.GetString(buffer, 0, effective);
                Console.WriteLine("socket返回：" + str);
                //处理消息

            }
        }
        /// <summary>
        /// 监听连接
        /// </summary>
        /// <param name="o"></param>
        public static void Listen(object o)
        {
            var serverSocket = o as Socket;
            while (true)
            {
                //等待连接并且创建一个负责通讯的socket
                var send = serverSocket.Accept();
                //获取链接的IP地址
                var sendIpoint = send.RemoteEndPoint.ToString();
                Console.WriteLine($"{sendIpoint}Connection");
                //开启一个新线程不停接收消息
                Thread thread = new Thread(Recive);
                thread.IsBackground = true;
                thread.Start(send);
            }
        }
    }
}
