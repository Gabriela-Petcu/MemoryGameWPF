using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Memory.Views
{
    public partial class AvatarSelectionView : Window
    {
        public ObservableCollection<string> AvatarPaths { get; set; }
        public string SelectedAvatar { get; private set; }

        public ICommand SelectAvatarCommand { get; }

        public AvatarSelectionView()
        {
            InitializeComponent();
            DataContext = this;

            SelectAvatarCommand = new RelayCommand(param =>
            {
                SelectedAvatar = param as string;
                DialogResult = true;
                Close();
            });

            //incarca avatrs
            var avatarsPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Images", "Avatars");
            var files = Directory.GetFiles(avatarsPath, "*.*").Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".gif"));
            AvatarPaths = new ObservableCollection<string>(files);
        }
    }
}
