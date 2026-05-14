using Microsoft.JSInterop;

namespace F1_ManagerFrontEnd.Components.Pages
{
    public partial class Login
    {
        private string _username = "";
        private string _password = "";
        private int _userId = 0;

        public async Task LoginUser()
        {
            var url = $"/user/check?username={Uri.EscapeDataString(_username)}&password={Uri.EscapeDataString(_password)}";
            var response = await Http.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var url2 = $"get/ID/from/username?username={Uri.EscapeDataString(_username)}";
                var response2 = await Http.GetAsync(url2);
                _userId = await response2.Content.ReadFromJsonAsync<int>();

                var url3 = $"get/empty/team/from/user?IDUser={_userId}";
                var response3 = await Http.GetAsync(url3);
                var hasNoTeam = await response3.Content.ReadFromJsonAsync<bool>();
                UserState.UserId = _userId;
                UserState.Username = _username;
                if (hasNoTeam)
                {
                    Navigation.NavigateTo("/CreateTeam");
                }
                else
                {
                    Navigation.NavigateTo("/Home");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await JS.InvokeVoidAsync("alert", "Ongeldige gebruikersnaam of wachtwoord.");
            }
            else
            {
                await JS.InvokeVoidAsync("alert", "Er is een fout opgetreden. Probeer het later opnieuw.");
            }
        }
        public async Task RegistreerUser()
        {
            Navigation.NavigateTo("/register");

        }
    }
}