using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Memory.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object currentView;

       
        public MainViewModel()
        {
            CurrentView = new SignInViewModel(this);
        }


        public object CurrentView
        {
            get => currentView;
            set
            {
                currentView = value;
                OnPropertyChanged();

            }
        }

        //navigare intre ferestre
        public void NavigateToGame()
        {
            CurrentView = new GameViewModel(this);
        }

        public void NavigateToSignIn()
        {
            CurrentView = new SignInViewModel(this);
        }

        public void NavigateToStatistics()
        {
            CurrentView = new StatisticsViewModel(this);
        }

        //eveniment ce anunta schimbarea
        public event PropertyChangedEventHandler PropertyChanged;

        //metoda care declanseaza evenimentul si trimite notificarea
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
