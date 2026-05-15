namespace F1_ManagerFrontEnd.Services
{
    public class UserStateService
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public int? UserTeam { get; set; } = null;

    }
}
