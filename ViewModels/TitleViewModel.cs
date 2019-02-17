using GeometryDash.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeometryDash.ViewModels
{
    public class TitleViewModel
    {
        SelectorViewModel _selectVM;

        public ICommand PlayCommand { get; private set; }
        public ICommand QuitCommand { get; private set; }

        public TitleViewModel(SelectorViewModel selectVM)
        {
            _selectVM = selectVM;

            PlayCommand = new RelayCommand(
                (param) => _selectVM.StartGame(),
                (param) => true
            );

            QuitCommand = new RelayCommand(
                (param) => Application.Current.MainWindow.Close(),
                (param) => true
            );
        }
    }
}
