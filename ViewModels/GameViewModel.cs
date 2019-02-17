using GeometryDash.Lib;
using GeometryDash.Models;
using GeometryDash.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GeometryDash.ViewModels
{
    public static class Constants
    {
        public const double DEF_0 = 50;
        public const double PSIZE = 30;
        public const double OBST_HT = 15;
        public const double OBSTSPEED = 2;
        public const double STARTX = 40;
        public const double JUMPV = 10;
        public const double G = -350;
        //public const double IM_RATIO = .0254;
    }

    public class GameViewModel : INotifyPropertyChanged, IDisposable
    {
        private SelectorViewModel _selectVM;
        private Canvas _obstacleContainer;
        public ContentControl PageContent = ((ContentControl)Application.Current.MainWindow.FindName("PageContent"));
        
        private DispatcherTimer _timer;
        private DispatcherTimer _spawnTimer;
        private Random _spawnTimeGenerator = new Random();

        private int _counter;
        public int Counter { get { return _counter; } set { _counter = value; OnPropertyChanged("Counter"); } }

        public Player Player { get; set; }
        public List<Obstacle> Obstacles { get; set; }

        public ICommand QuitGameCommand { get; private set; }

        public GameViewModel(SelectorViewModel selectVM, GameView gv, Dispatcher dispatcher)
        {
            _selectVM = selectVM;
            _obstacleContainer = gv.ObstacleCanvas;

            QuitGameCommand = new RelayCommand(
                (param) => QuitGame(), 
                (param) => true
            );

            Application.Current.MainWindow.KeyDown += (s, e) => 
            {
                if (e.Key == Key.Space)
                {
                    Player.BeginJump();
                }
            };

            Player = new Player(this);
            gv.MainCanvas.Children.Add(Player.Element);

            Obstacles = new List<Obstacle>();

            Obstacle startingPlatform = new Obstacle(this, true);
            startingPlatform.Initialize();
            Obstacles.Add(startingPlatform);
            _obstacleContainer.Children.Add(startingPlatform.Element);


            _timer = new DispatcherTimer();
            _timer.Tick += UpdateScreen;
            _timer.Interval = TimeSpan.FromMilliseconds(1);

            _spawnTimer = new DispatcherTimer();
            _spawnTimer.Tick += SpawnObstacle;
            _spawnTimer.Interval = GetObstacleSpawnTime();
            Player.Initialize();

            _timer.Start();
            _spawnTimer.Start();
        }

        private void UpdateScreen(object sender, EventArgs e)
        {
            Player.Update();

            var obstToRemove = new List<Obstacle>();
            Obstacles.ForEach(o =>
            {
                o.Update();
                if(o.EndX <= 0)
                {
                    Counter += 1;
                    obstToRemove.Add(o);
                } else if(o.EndX <= PageContent.ActualWidth && o.PrevEndX >= PageContent.ActualWidth)
                {
                    _spawnTimer.Start();
                }

            });
            obstToRemove.ForEach(o => Obstacles.Remove(o));

            List<bool> canJumpResult = new List<bool>();
            Obstacles.ForEach(o =>
            {
                bool result = CheckCollision(o);
                canJumpResult.Add(result);
                if(result)
                {
                    Player.CanJump = true;
                }
            });

            if(canJumpResult.All(b => !b))
            {
                Player.CanJump = false;
            }
        }

        private void SpawnObstacle(object sender, EventArgs e)
        {
            Obstacle obst = new Obstacle(this);
            obst.Initialize();
            Obstacles.Add(obst);
            _obstacleContainer.Children.Add(obst.Element);
            _spawnTimer.Interval = GetObstacleSpawnTime();
            _spawnTimer.Stop();
        }

        private TimeSpan GetObstacleSpawnTime()
        {
            return TimeSpan.FromSeconds(_spawnTimeGenerator.NextDouble() * .7 + .3);
        }

        private bool CheckCollision(Obstacle obstacle)
        {
            //If player falls below the screen, game over
            if (Player.TopY <= 0)
            {
                _selectVM.GameOver(Counter);
                Dispose();
            }

            bool canJump = false;

            //If overlapping horizontally
            if (Player.EndX >= obstacle.StartX && Player.StartX <= obstacle.EndX)
            {
                //If overlapping vertically
                if (Player.BottomY <= obstacle.TopY && Player.TopY >= obstacle.BottomY)
                {
                    //At this point the player and the obstacle are touching

                    //If the player hit the obstacle from the left side
                    if(Player.PrevEndX <= obstacle.PrevStartX && Player.EndX >= obstacle.StartX)
                    {
                        Player.HasCollidedX = true;
                        canJump = false;
                    }

                    //If the player hits the obstacle from the top
                    if(Player.PrevBottomY >= obstacle.PrevTopY && Player.BottomY <= obstacle.TopY)
                    {
                        if(!Player.CanJump)
                        {
                            Player.VelocityY = 0;
                            Player.BottomY = obstacle.TopY;
                        }
                        return true;
                    }

                    //If the player only collides from the side, stop the obstacles, but if it lands on a corner, allow it to
                    //keep going
                    if(Player.HasCollidedX && !Player.CanJump)
                    {
                        Obstacles.ForEach(o => o.VelocityX = 0);
                        _spawnTimer.Stop();
                        return false;
                    }
                    Player.HasCollidedX = false;
                    
                    return false;
                }
            }

            Player.HasCollidedX = false;
            return canJump;
        }

        private void QuitGame()
        {
            _selectVM.MainMenu();
            Dispose();
        }

        public void Dispose()
        {
            _spawnTimer.Stop();
            _timer.Stop();
            _spawnTimer.Tick -= SpawnObstacle;
            _timer.Tick -= UpdateScreen;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
