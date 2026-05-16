namespace F1_ManagerFrontEnd.Models
{
    public class DriverModel
    {
        public int IdDriver { get; set; }
        public string VoornaamDriver { get; set; } = "";
        public string AchternaamDriver { get; set; } = "";
        public string NationaliteitDriver { get; set; } = "";
        public int Rating { get; set; }
        public int Confidence { get; set; }
        public int LeeftijdDriver { get; set; }
    }
}
