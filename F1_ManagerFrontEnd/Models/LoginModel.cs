namespace F1_ManagerFrontEnd.Models
{
    public class LoginModel
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } // Implement or remove this
    }
}
