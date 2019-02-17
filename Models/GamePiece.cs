using GeometryDash.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GeometryDash.Models
{
    public class Player : GamePiece
    {
        public bool HasCollidedX = false;
        public bool HasCollidedY = false;
        public bool CanJump = false;

        public Player(GameViewModel vm) : base(vm)
        {
            Element = new Rectangle()
            {
                Fill = Brushes.Red,
                Stroke = Brushes.DarkGray,
                StrokeThickness = 2,
                Height = Constants.PSIZE,
                Width = Constants.PSIZE,
            };
        }

        public void Initialize()
        {
            BottomY = Constants.DEF_0;
            StartX = Constants.STARTX;
            PrevStartX = StartX;
            PrevBottomY = BottomY;
            VelocityY = 0;
        }

        public void BeginJump()
        {
            if(CanJump)
            {
                VelocityY = Constants.JUMPV;
                CanJump = false;
            }
        }

        public void Update()
        {
            VelocityY = CanJump ? 0 : UpdateRealVelocity(VelocityY);

            BottomY = BottomY + VelocityY; 
        }

        private double UpdateRealVelocity(double v)
        {
            //+ Constants.G/1000
            //PropertyInfo dpiYProperty = typeof(SystemParameters).GetProperty("DpiY", BindingFlags.NonPublic | BindingFlags.Static);
            //var source = PresentationSource.FromVisual(Element);
            //var matrix = source.CompositionTarget.TransformFromDevice;

            //var dpiY = matrix.M22 * 96;
            //.0254 meters per inch
            
            //double dpmY = dpiY/Constants.IM_RATIO;

            return v + Constants.G/1000;
        }
    }

    public class Obstacle : GamePiece
    {
        public bool IsFirstObstacle;

        public double VelocityX { get; set; }

        public Obstacle(GameViewModel vm, bool isFirstTime = false) : base(vm)
        {
            IsFirstObstacle = isFirstTime;

            double width = isFirstTime ? _vm.PageContent.ActualWidth : new Random().NextDouble() * 100 + Constants.PSIZE;

            Element = new Rectangle()
            {
                Fill = Brushes.Green,
                Height = Constants.OBST_HT,
                Width = width,
            };
        }

        public void Initialize()
        {
            BottomY = IsFirstObstacle ? Constants.DEF_0 - Element.Height : ((new Random()).NextDouble()*40 + 30) - Element.Height;
            PrevBottomY = BottomY;
            StartX = IsFirstObstacle ?  0 : _vm.PageContent.ActualWidth;
            PrevStartX = StartX;
            VelocityY = 0;
            VelocityX = Constants.OBSTSPEED;
        }

        public void Update()
        {
            StartX = StartX - VelocityX;
        }


    }

    public abstract class GamePiece
    {
        public Rectangle Element;

        public double PrevBottomY;
        public double PrevTopY { get { return PrevBottomY + Element.Height; } }

        public double PrevStartX;
        public double PrevEndX { get { return PrevStartX + Element.Width; } }

        public double TopY { get { return BottomY + Element.Height; } }
        public double BottomY { get { return Canvas.GetBottom(Element); } set { PrevBottomY = Canvas.GetBottom(Element); Canvas.SetBottom(Element, value); } }

        public double StartX { get { return Canvas.GetLeft(Element); } set { PrevStartX = Canvas.GetLeft(Element); Canvas.SetLeft(Element, value); } }
        public double EndX { get { return StartX + Element.Width; } }

        public double VelocityY { get; set; }

        protected GameViewModel _vm;

        protected GamePiece(GameViewModel vm)
        {
            _vm = vm;
        }
    }
}

