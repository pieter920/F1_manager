using F1_ManagerFrontEnd.Models;
using F1_ManagerFrontEnd.Services;
using Microsoft.JSInterop;

namespace F1_ManagerFrontEnd.Components.Pages
{
    public partial class Login
    {
        readonly LoginModel loginModel = new();
        UserStateService user = new();

        public async Task LoginUser()
        {
            var url = $"/user/check?username={Uri.EscapeDataString(loginModel.UserName)}&password={Uri.EscapeDataString(loginModel.Password)}";
            var response = await Http.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var userResponse = await response.Content.ReadFromJsonAsync<UserStateService>();
                if (userResponse is null)
                {
                    await JS.InvokeVoidAsync("alert", "Onjuiste antwoord van server.");
                    return;
                }

                user = userResponse;

                if (user.UserTeam is null)
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