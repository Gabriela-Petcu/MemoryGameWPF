using Memory.Models;
using Memory.Services;
using Memory.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows;
using Memory.Views;
using System.Diagnostics;

namespace Memory.ViewModels
{
    public class SignInViewModel : ViewModelBase
    {
        private readonly UserService userService;
        private readonly MainViewModel mainViewModel;

        private UserModel _selectedUser;
        public ObservableCollection<UserModel> Users { get; set; }

        public UserModel SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                OnPropertyChanged(nameof(IsUserSelected));

                if (_selectedUser != null)
                    Debug.WriteLine($"calea avatarului selectat: {_selectedUser.AvatarPath}");
            }
        }


        public bool IsUserSelected => SelectedUser != null;

        public ICommand NewUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand ShowStatisticsCommand { get; }

        public SignInViewModel(MainViewModel mainVM)
        {

            mainViewModel = mainVM;
            userService = new UserService();

            Users = new ObservableCollection<UserModel>(userService.LoadUsers());

            NewUserCommand = new RelayCommand(_ => AddNewUser());
            DeleteUserCommand = new RelayCommand(_ => DeleteUser(), _ => IsUserSelected);
            PlayCommand = new RelayCommand(_ => PlayGame(), _ => IsUserSelected);
            ExitCommand = new RelayCommand(_ => System.Windows.Application.Current.Shutdown());
            ShowStatisticsCommand = new RelayCommand(_ => mainViewModel.CurrentView = new StatisticsViewModel(mainViewModel));

        }

        private void AddNewUser()
        {

            string username = Microsoft.VisualBasic.Interaction.InputBox(
                "Introduceți numele utilizatorului:", "Nume utilizator", "");

            if (string.IsNullOrWhiteSpace(username))
                return;

            var avatarWindow = new AvatarSelectionView();
            if (avatarWindow.ShowDialog() == true)
            {
                var fileName = System.IO.Path.GetFileName(avatarWindow.SelectedAvatar);
                var avatarPath = System.IO.Path.Combine("Images", "Avatars", fileName);

                var user = new UserModel
                {
                    Username = username,
                    AvatarPath = avatarPath
                };

                Users.Add(user);
                userService.SaveUsers(new List<UserModel>(Users));
            }
        }



        private void DeleteUser()
        {
            if (SelectedUser != null)
            {
                string username = SelectedUser.Username;

                Users.Remove(SelectedUser);

                userService.SaveUsers(new List<UserModel>(Users));

                StatisticsService.DeleteStatisticsForUser(username);

                GameSaveService.DeleteSavedGame(username);

                MessageBox.Show($"Utilizatorul '{username}' a fost șters complet.", "Confirmare");
            }
        }

        private void PlayGame()
        {
            if (SelectedUser != null)
            {
                AppState.CurrentUser = SelectedUser.Username;
                mainViewModel.CurrentView = new GameViewModel(mainViewModel);

            }
        }
    }
}
