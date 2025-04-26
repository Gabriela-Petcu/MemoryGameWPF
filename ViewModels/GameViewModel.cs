using Memory.Helpers;
using Memory.Models;
using Memory.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Memory.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        //generare jetoane
        private readonly GameService gameService = new();

        //cronometru
        private readonly DispatcherTimer timer;

        private TileModel firstSelected;
        private TileModel secondSelected;

        private bool canClick = true;

        private bool gameInProgress = false;

        //tabla de joc
        public ObservableCollection<TileModel> Tiles { get; set; } = new();
        public List<string> Categories { get; } = new() { "Italy", "Spain", "Turkey" };
        public List<int> TimeOptions { get; } = new() { 30, 60, 90 };
        public List<int> CustomOptions { get; } = new() { 2, 3, 4, 5, 6 };

        private string selectedCategory = "Italy";
        public string SelectedCategory
        {
            get => selectedCategory;
            set { selectedCategory = value; OnPropertyChanged(); }
        }

        private int selectedTime = 60;
        public int SelectedTime
        {
            get => selectedTime;
            set { selectedTime = value; OnPropertyChanged(); }
        }

        private int rows = 4;
        public int Rows
        {
            get => rows;
            set { rows = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanStart)); }
        }

        private int columns = 4;
        public int Columns
        {
            get => columns;
            set { columns = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanStart)); }
        }
        public bool CanStart => (Rows * Columns) % 2 == 0;

        private int timeLeft;
        public int TimeLeft
        {
            get => timeLeft;
            set { timeLeft = value; OnPropertyChanged(); OnPropertyChanged(nameof(TimeLeftFormatted)); }
        }

        public string TimeLeftFormatted => $"{TimeLeft / 60:D2}:{TimeLeft % 60:D2}";

        public int ElapsedSeconds { get; set; } = 0;

        public ICommand TileClickCommand { get; }
        public ICommand StartGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand LoadGameCommand { get; }
        public ICommand NewGameCommand { get; }

        private bool isCustomMode = false;
        public bool IsCustomMode
        {
            get => isCustomMode;
            set
            {
                isCustomMode = value;
                OnPropertyChanged();
                if (!isCustomMode)
                {
                    Rows = 4;
                    Columns = 4;
                }
            }
        }

        private readonly MainViewModel mainViewModel;

        public ICommand GoBackCommand { get; }
        public GameViewModel(MainViewModel mainVM)
        {
            this.mainViewModel = mainVM;

            GoBackCommand = new RelayCommand(_ => mainViewModel.NavigateToSignIn());

            TileClickCommand = new RelayCommand(TileClicked, _ => canClick);
            StartGameCommand = new RelayCommand(_ => StartGame());
            SaveGameCommand = new RelayCommand(_ => SaveGame());
            LoadGameCommand = new RelayCommand(_ => LoadGame());
            NewGameCommand = new RelayCommand(_ => StartGame());

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeLeft--;
            ElapsedSeconds++;

            if (TimeLeft <= 0)
            {
                timer.Stop();
                gameInProgress = false;
                canClick = false;
                StatisticsService.AddGameResult(AppState.CurrentUser, false);
                MessageBox.Show("Timpul a expirat! Ai pierdut jocul.", "Game Over");
            }
            else if (Tiles.All(t => t.IsMatched))
            {
                timer.Stop();
                gameInProgress = false;
                StatisticsService.AddGameResult(AppState.CurrentUser, true);
                MessageBox.Show("Felicitări! Ai câștigat jocul!", "Victory");
            }

        }

        private async void TileClicked(object param)
        {
            if (!canClick || param is not TileModel tile || tile.IsFlipped || tile.IsMatched)
                return;

            tile.IsFlipped = true;
            OnPropertyChanged(nameof(Tiles));

            if (firstSelected == null)
            {
                firstSelected = tile;
            }
            else if (secondSelected == null && tile != firstSelected)
            {
                secondSelected = tile;
                canClick = false;

                await Task.Delay(1000); // timp de asteptare pt pereche

                if (firstSelected.Id == secondSelected.Id)
                {
                    firstSelected.IsMatched = true;
                    secondSelected.IsMatched = true;
                }
                else
                {
                    firstSelected.IsFlipped = false;
                    secondSelected.IsFlipped = false;
                }

                firstSelected = null;
                secondSelected = null;
                canClick = true;

                OnPropertyChanged(nameof(Tiles));
            }
        }

        private void StartGame()
        {
            // jocul in asteptare contorizeaza pierderea
            if (gameInProgress && !Tiles.All(t => t.IsMatched))
            {
                timer.Stop();
                StatisticsService.AddGameResult(AppState.CurrentUser, false);
            }

            //nou joc
            Tiles = new ObservableCollection<TileModel>(
                gameService.GenerateTiles(SelectedCategory, Rows, Columns));

            TimeLeft = SelectedTime;
            ElapsedSeconds = 0;
            canClick = true;
            gameInProgress = true;

            OnPropertyChanged(nameof(Tiles));
            OnPropertyChanged(nameof(TimeLeftFormatted));

            timer.Start();
        }


        private void SaveGame()
        {
            //MessageBox.Show(" SaveGame() a fost apelat"); 

            var save = new SavedGameModel
            {
                Username = AppState.CurrentUser,
                Category = SelectedCategory,
                Rows = Rows,
                Columns = Columns,
                Tiles = Tiles.Select(t => new TileModel
                {
                    Id = t.Id,
                    ImagePath = t.ImagePath,
                    IsFlipped = t.IsFlipped,
                    IsMatched = t.IsMatched
                }).ToList(),
                TimeRemaining = TimeLeft,
                ElapsedSeconds = ElapsedSeconds
            };

            GameSaveService.SaveGame(save);
            MessageBox.Show("Joc salvat cu succes!", "Salvare");
        }


        private void LoadGame()
        {
            var save = GameSaveService.LoadGame(AppState.CurrentUser);

            if (save == null)
            {
                MessageBox.Show("Nu există niciun joc salvat pentru acest utilizator.", "Load");
                return;
            }

            SelectedCategory = save.Category;
            Rows = save.Rows;
            Columns = save.Columns;
            Tiles = new ObservableCollection<TileModel>(save.Tiles);
            TimeLeft = save.TimeRemaining;
            ElapsedSeconds = save.ElapsedSeconds;

            gameInProgress = true;
            canClick = true;

            OnPropertyChanged(nameof(SelectedCategory));
            OnPropertyChanged(nameof(Rows));
            OnPropertyChanged(nameof(Columns));
            OnPropertyChanged(nameof(Tiles));
            OnPropertyChanged(nameof(TimeLeftFormatted));

            timer.Start();
            MessageBox.Show("Jocul a fost încărcat cu succes!", "Load");
        }



    }
}
