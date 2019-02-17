using GeometryDash.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeometryDash.ViewModels
{
    public class GameOverViewModel : INotifyPropertyChanged
    {
        private int _score;
        public int Score
        {
            get { return _score; }
            set { _score = value; OnPropertyChanged("Score"); }
        }

        private SelectorViewModel _vm;

        public ICommand QuitCommand { get; private set; }
        public ICommand ReplayCommand { get; private set; }
        public ICommand MainMenuCommand { get; private set; }

        public GameOverViewModel(SelectorViewModel vm)
        {
            _vm = vm;
            Score = vm.Score;
            CreateCommands();
        }

        private void CreateCommands()
        {
            QuitCommand = new RelayCommand(
                (param) => Application.Current.MainWindow.Close(),
                (param) => true
            );

            ReplayCommand = new RelayCommand(
                (param) => _vm.StartGame(),
                (param) => true
            );

            MainMenuCommand = new RelayCommand(
                (param) => _vm.MainMenu(),
                (param) => true
            );
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
