using F1_ManagerFrontEnd.Models;
using Microsoft.JSInterop;

namespace F1_ManagerFrontEnd.Components.Pages
{
    public partial class Standings
    {

        List<DriverSeasonStandingModel> DriverStandings = new();
        List<ConstructorSeasonStandingModel> ConstructorStandings = new();
        string ActiveTab = "drivers";

        protected override async Task OnInitializedAsync()
        {
            await GetDriverSeasonStandings();

            await GetConstructorSeasonStandings();
        }


        public async Task GetDriverSeasonStandings()
        {
            var standingUrl = $"get/driver/standings?IDUser={UserState.UserId}";
            var response = await Http.GetAsync(standingUrl);

            if (response.IsSuccessStatusCode)
            {
                var responsecontent = await response.Content.ReadFromJsonAsync<List<DriverSeasonStandingModel>>();
                if (responsecontent is null)
                {
                    await JS.InvokeVoidAsync("alert", "Onjuiste antwoord van server.");
                    return;
                }

                DriverStandings = responsecontent;
            }
        }

        public async Task GetConstructorSeasonStandings()
        {
            var standingUrl = $"get/constructor/standings?IDUser={UserState.UserId}";
            var response = await Http.GetAsync(standingUrl);

            if (response.IsSuccessStatusCode)
            {
                var responsecontent = await response.Content.ReadFromJsonAsync<List<ConstructorSeasonStandingModel>>();
                if (responsecontent is null)
                {
                    await JS.InvokeVoidAsync("alert", "Onjuiste antwoord van server.");
                    return;
                }

                ConstructorStandings = responsecontent;
            }
        }

        void GoToDashboard()
        {
            Navigation.NavigateTo("/dashboard");
        }
    }
}