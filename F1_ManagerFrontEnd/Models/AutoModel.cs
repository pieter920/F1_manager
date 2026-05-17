namespace F1_ManagerFrontEnd.Models
{
    public class AutoModel
    {

        public int Idauto { get; set; }

        public int PresatieAuto { get; set; }

        public string NaamAuto { get; set; } = null!;

        public int Fkteam { get; set; }
    }
}
