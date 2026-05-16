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
            var userResponse = await response.Content.ReadFromJsonAsync<UserStateService>();

            if (userResponse is null)
            {
                await JS.InvokeVoidAsync("alert", "Onjuiste antwoord van server.");
                return;
            }

            UserState.UserId = userResponse.UserId;
            UserState.Username = userResponse.Username;


            if (response.IsSuccessStatusCode)
            {
                //create team - UPDATE THIS
                var CreateTeamUrl = $"/Create/Team?NaamTeam={Uri.EscapeDataString(registerModel.TeamName)}&NationaliteitTeam={Uri.EscapeDataString(registerModel.Nation)}&UserID={UserState.UserId}";
                var TeamResponse = await Http.PostAsync(CreateTeamUrl, null);
                var CreateTeamUrlResponse = await TeamResponse.Content.ReadFromJsonAsync<TeamModel>();

                if (CreateTeamUrlResponse is null)
                {
                    await JS.InvokeVoidAsync("alert", "Onjuiste antwoord van server.");
                    return;
                }

                UserState.TeamId = CreateTeamUrlResponse.Id;

                await CreateCalender(); 

                Navigation.NavigateTo("/dashboard");
            }
            else
            {
                await JS.InvokeVoidAsync("alert", "Onbekende fout bij register.");
            }
        }

        public async Task CreateCalender()
        {
            //seizoen aanmaken en deze koppelen aan de user
            var urlSeizon = $"create/Eerste/seizon?IDUser={UserState.UserId}";
            var responseSeizon = await Http.PostAsync(urlSeizon, null);
            //seizoenID opvragen van het aangemaakte seizoen
            var urlID = $"get/ID/from/SeizoenName?NaamSeizoen={Uri.EscapeDataString("Seizoen 2025")}";
            var ResponseID = await Http.GetAsync(urlID);
            var IDSeizon = await ResponseID.Content.ReadFromJsonAsync<int>();
            //RaceCalendar aanmaken en deze koppelen aan het aangemaakte seizoen
            var urlSeizonCreate = $"create/Eerste/calendar?IDUser={UserState.UserId}&seasonID={IDSeizon}";
            var responseSeizonCreate = await Http.PostAsync(urlSeizonCreate, null);
        }
    }
}