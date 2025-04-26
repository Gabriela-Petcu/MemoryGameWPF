using Memory.Models;
using Memory.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Memory.Helpers;

namespace Memory.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        public ICommand GoBackCommand { get; }
        private readonly MainViewModel mainViewModel;

        public ObservableCollection<UserStatistics> Statistics { get; set; }

        public StatisticsViewModel(MainViewModel mainVM)
        {
            mainViewModel = mainVM;

            var loaded = StatisticsService.LoadStatistics();
            Statistics = new ObservableCollection<UserStatistics>(loaded);

            GoBackCommand = new RelayCommand(_ => mainViewModel.NavigateToSignIn());
        }

       
    }
}
