namespace Memory.Models
{
    public class UserModel
    {
        public string Username { get; set; }
        public string AvatarPath { get; set; }





        public string FullAvatarPath => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AvatarPath);
    
    }
}
