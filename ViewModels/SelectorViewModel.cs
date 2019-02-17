using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryDash.ViewModels
{
    public class SelectorViewModel : INotifyPropertyChanged
    {
        private int _sv;
        public int SwitchView
        {
            get { return _sv; }
            set { _sv = value; OnPropertyChanged("SwitchView"); }
        }

        private int _score;
        public int Score
        {
            get { return _score; }
            set { _score = value;  OnPropertyChanged("Score"); }
        }

        public SelectorViewModel()
        {
            SwitchView = 0;
        }

        public void StartGame()
        {
            SwitchView = 1;
        }

        public void MainMenu()
        {
            SwitchView = 0;
        }

        public void GameOver(int score)
        {
            Score = score;
            if(score > Properties.Settings.Default.HighScore)
            {
                Properties.Settings.Default.HighScore = score;
                Properties.Settings.Default.Save();
            }
            SwitchView = 2;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
