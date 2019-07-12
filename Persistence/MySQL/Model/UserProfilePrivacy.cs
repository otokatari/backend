namespace OtokatariBackend.Persistence.MySQL.Model
{
    public partial class UserProfilePrivacy
    {
        public string Userid { get; set; }
        public byte Sex { get; set; }
        public byte Country { get; set; }
        public byte City { get; set; }
        public byte Birthday { get; set; }
        public byte Moment { get; set; }
        public byte Playlists { get; set; }
    }
}
