using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace 贪吃蛇
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        const int size = 15;
        int gameSpan;
        int score;
        bool initflag,appleFlag;
        int rightLimit , bottomLimit;
        
        Point[] dir = new Point[4];//0 right,1 up,2 left,3 down  
        Random ran;
        SolidColorBrush backcolor = Brushes.White, snakecolor = Brushes.Silver, sHcolor = Brushes.BlueViolet, appleColor = Brushes.Red;
        Button[][] blocks;
        snakeMap nx;
        DispatcherTimer mTime = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            initflag = true;
            appleFlag = false;
            stopItem.Tag = 1;
            dir[3].Y = dir[0].X =  1;
            dir[1].Y = dir[2].X = -1;
            dir[1].X = dir[3].X = dir[0].Y = dir[2].Y = 0;
            Canvas a = MainGame;
            gameSpan = 300;
            mTime.Interval = new TimeSpan(0, 0, 0, 0, gameSpan);
            mTime.Tick += playingGame;
            ran = new Random();
        }
        public class snakeMap
        {
            public Point Head, Last;
            public Queue<Point> q = new Queue<Point>();
            public int nowDir;
            public snakeMap(Point A,Point B)
            {
                Head = A;
                Last = B;
                nowDir = 0;
                for(int i=(int)B.X;i<=(int)A.X;i++)
                {
                    q.Enqueue(B);
                    B.X++;
                }
            }
        }
        private void creatApple()
        {
            if (appleFlag)
                return;
            Point applePos = new Point();
            while(true)
            {
                applePos.X = ran.Next(0, rightLimit - 1);
                applePos.Y = ran.Next(0, bottomLimit - 1);
                if (!nx.q.Contains(applePos))
                    break;
            }
            blocks[(int)applePos.X][(int)applePos.Y].Background = appleColor;
            appleFlag = true;

        }
        private void playingGame(object sender, EventArgs e)
        {
            textScore.Text = score.ToString();
            creatApple();
            nx.Head.X += dir[nx.nowDir].X;
            nx.Head.Y += dir[nx.nowDir].Y;
            Console.WriteLine((int)nx.Head.X);
            
            if(blocks[(int)nx.Head.X][(int)nx.Head.Y].Background == appleColor)
            {
                appleFlag = false;
                nx.q.Enqueue(nx.Head);
                blocks[(int)nx.Head.X][(int)nx.Head.Y].Background = sHcolor;
                score++;
                return;
            }
            nx.Last = nx.q.Dequeue();
            if (nx.q.Contains(nx.Head) || nx.Head.X < 0 || nx.Head.Y < 0 || nx.Head.Y >= bottomLimit || nx.Head.X >= rightLimit)
            {
                MessageBox.Show("游戏失败，您的得分为： " + score);
                mTime.Stop();
                return;
            }
            nx.q.Enqueue(nx.Head);
            blocks[(int)nx.Head.X][(int)nx.Head.Y].Background = sHcolor;
            blocks[(int)nx.Last.X][(int)nx.Last.Y].Background = backcolor;
        }
        private void dirChange(object sender, KeyEventArgs e)
        {
            int temp=-1;
            if (e.Key == Key.Right)
                temp = 0;
            if (e.Key == Key.Up)
                temp = 1;
            if (e.Key == Key.Left)
                temp = 2;
            if (e.Key == Key.Down)
                temp = 3;
            if (temp == -1)
                return;
            if ((temp + nx.nowDir) % 2 == 0)
                return;
            nx.nowDir = temp;
        }

        private void startGame(object sender, RoutedEventArgs e)
        {
            
            score = 0;
            textScore.Text = score.ToString();
            //Console.WriteLine(rightLimit);
            //Console.WriteLine("1 "+rightLimit);
            appleFlag = false;
            if(initflag)
            {
                rightLimit = (int)MainGame.ActualWidth / size;
                bottomLimit = (int)MainGame.ActualHeight / size;
                //Console.WriteLine(bottomLimit);
                blocks = new Button[rightLimit][];
                initflag = false;
                for (int i = 0; i < rightLimit; i++)
                {
                    blocks[i] = new Button[bottomLimit];
                    //Console.WriteLine(blocks[i].Length);
                    for (int j = 0; j < bottomLimit; j++)
                    {
                        blocks[i][j] = new Button();
                        Button temp = blocks[i][j];
                        if (blocks[i][j] == null)
                            //Console.WriteLine(rightLimit);
                        temp.IsEnabled = false;
                        temp.Height = temp.Width = size;
                        temp.Margin = new Thickness(i * size, j * size, 0, 0);
                        temp.Content = temp.Name;
                        MainGame.Children.Add(temp);
                    }
                }
            }
            for (int i = 0; i < rightLimit; i++)
                for (int j = 0; j < bottomLimit; j++)
                {
                    Button temp = blocks[i][j];
                    temp.Background = backcolor;
                }
            Point A= new Point(4, bottomLimit / 2-1), B;
            B = A;
            B.X += 3;
            nx = new snakeMap(B,A);
            for(int i=(int)nx.Head.X;i>=(int)nx.Last.X;i--)
                for(int j=(int)nx.Head.Y;j>=(int)nx.Last.Y;j--)
                {
                    Button temp = blocks[i][j];
                    if (j == (int)nx.Head.Y)
                        temp.Background = sHcolor;
                    else
                        temp.Background = snakecolor;
                }
            
            mTime.Stop();
            
            mTime.Start();
        }
        private void pauseGame(object sender, RoutedEventArgs e)
        {
            if((int)stopItem.Tag==1)
            {
                stopItem.Tag = 0;
                stopItem.Header = "继续";
                mTime.Stop();
            }
            else
            {
                stopItem.Tag = 1;
                stopItem.Header = "暂停";
                mTime.Start();
            }
        }
        private void stopGame(object sender, RoutedEventArgs e)
        {
            mTime.Stop();

        }
    }
}
