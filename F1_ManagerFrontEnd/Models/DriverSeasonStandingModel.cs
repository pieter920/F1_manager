namespace F1_ManagerFrontEnd.Models
{
    public class DriverSeasonStandingModel
    {
        public int Position { get; set; }
        public int DriverId { get; set; }

        public string Naam { get; set; } = "";

        public string Team { get; set; } = "";

        public int Punten { get; set; }
    }
}
