namespace F1_ManagerFrontEnd.Models
{
    public class Track
    {
        public int Id { get; set; }
        public string NaamTrack { get; set; } = "";
        public string LandTrack { get; set; } = "";
        public int LapsTrack { get; set; }
        public DateOnly Begindatum { get; set; }
        public DateOnly EindDatum { get; set; }
    }
}
