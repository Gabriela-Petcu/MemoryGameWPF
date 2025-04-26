using Memory.ViewModels;

public class TileModel : ViewModelBase 
{
    public int Id { get; set; }
    public string ImagePath { get; set; }

    private bool isFlipped;
    public bool IsFlipped
    {
        get => isFlipped;
        set { isFlipped = value; OnPropertyChanged(); }
    }

    private bool isMatched;
    public bool IsMatched
    {
        get => isMatched;
        set { isMatched = value; OnPropertyChanged(); }
    }
}
