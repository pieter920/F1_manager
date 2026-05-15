using F1_ManagerFrontEnd.Models;
using F1_ManagerFrontEnd.Services;
using Microsoft.JSInterop;

namespace F1_ManagerFrontEnd.Components.Pages
{
    public partial class Register
    {

        private RegisterModel registerModel = new();

        private async Task RegisterUser()
        {
            var url = $"/user/register?username={Uri.EscapeDataString(registerModel.Username)}&password={Uri.EscapeDataString(registerModel.Password)}";
            var response = await Http.PostAsync(url, null);
            var User = await response.Content.ReadFromJsonAsync<UserStateService>();

            if (User is null)
            {
                await JS.InvokeVoidAsync("alert", "Onjuiste antwoord van server.");
                return;
            }

            if (response.IsSuccessStatusCode)
            {
                //create team - UPDATE THIS
                var CreateTeamUrl = $"/Create/Team?NaamTeam={Uri.EscapeDataString(registerModel.TeamName)}&NationaliteitTeam={Uri.EscapeDataString(registerModel.Nation)}&UserID={User.UserId}";
                var CreateTeamUrlResponse = await Http.PostAsync(CreateTeamUrl, null);
                Navigation.NavigateTo("/home");
            }
            else
            {
                await JS.InvokeVoidAsync("alert", "Onbekende fout bij register.");
            }
        }
    }
}