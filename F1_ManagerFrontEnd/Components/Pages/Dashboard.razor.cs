using F1_ManagerFrontEnd.Models;
using Microsoft.JSInterop;

namespace F1_ManagerFrontEnd.Components.Pages
{
    public partial class Dashboard
    {
        TeamModel team = new();

        List<DriverModel> Drivers = new();
        Track track = new();
        AutoModel autoModel = new();

        private int CompletedRaces = 0;

        private int TotalRaces = 24;

        private double percentage => (double)CompletedRaces / 24 * 100;


        protected override async Task OnInitializedAsync()
        {
            await GetCurrentRaceCount();

            await GetTeam();

            await GetNextRace();

            await GetDrivers();

            await GetAuto();
        }

        private void OpenCreateDriver() => Navigation.NavigateTo("/CreateDriver");

        private void GoToNextPage()
        {
            Navigation.NavigateTo("/Standings");
        }

        public async Task GetTeam()
        {
            //get team
            var teamUrl = $"get/Team/from/userID?IDUser={UserState.UserId}";
            var response = await Http.GetAsync(teamUrl);

            if (response.IsSuccessStatusCode)
            {
                var responsecontent = await response.Content.ReadFromJsonAsync<TeamModel>();
                if (responsecontent is null)
                {
                    await JS.InvokeVoidAsync("alert", "Onjuiste antwoord van server.");
                    return;
                }

                team = responsecontent;
            }
        }

        public async Task GetAuto()
        {
            //get auto
            var autoUrl = $"/Auto/{UserState.TeamId}";
            var response = await Http.GetAsync(autoUrl);

            if (response.IsSuccessStatusCode)
            {
                var responsecontent = await response.Content.ReadFromJsonAsync<AutoModel>();
                if (responsecontent is null)
                {
                    await JS.InvokeVoidAsync("alert", "Onjuiste antwoord van server.");
                    return;
                }

                autoModel = responsecontent;
            }
        }

        public async Task GetNextRace()
        {
            var teamUrl = $"get/track/by/user?IDUser={UserState.UserId}";
            var response = await Http.GetAsync(teamUrl);

            if (response.IsSuccessStatusCode)
            {
                var responsecontent = await response.Content.ReadFromJsonAsync<Track>();
                if (responsecontent is null)
                {
                    await JS.InvokeVoidAsync("alert", "Onjuiste antwoord van server.");
                    return;
                }

                track = responsecontent;
            }
        }

        public async Task GetDrivers()
        {
            var url = $"/Drivers/{UserState.TeamId}";
            var response = await Http.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var drivers = await response.Content.ReadFromJsonAsync<List<DriverModel>>();
                if (drivers is null)
                {
                    await JS.InvokeVoidAsync("alert", "Onjuiste antwoord van server.");
                    return;
                }

                Drivers = drivers;
            }
        }

        private async void GoToRaceweekend()
        {
            if (Drivers.Count < 2)
            {
                await JS.InvokeVoidAsync(
                    "alert",
                    "You need at least 2 drivers before you can simulate a race weekend.");

                return;
            }

            await SimulateRace();
        }

        private async Task GetCurrentRaceCount()
        {
            var completedCountUrl = $"get/completed/raceweekends/from/user?IDUser={UserState.UserId}";
            var response = await Http.GetAsync(completedCountUrl);

            if (response.IsSuccessStatusCode)
            {
                var responsecontent = await response.Content.ReadFromJsonAsync<int>();
                CompletedRaces = responsecontent;
            }
        }

        private List<RaceResult>? _results;
        private int _userTeamId;

        private async Task SimulateRace()
        {
            var url = $"simulate/raceweekend?IDUser={UserState.UserId}";
            var response = await Http.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                _results = await response.Content.ReadFromJsonAsync<List<RaceResult>>();
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }

            Navigation.NavigateTo("/ResultRaceweekend");
        }

        private bool _showDropdown = false;

        private void ToggleDropdown()
        {
            _showDropdown = !_showDropdown;
        }

        private void Logout()
        {
            _showDropdown = false;
            UserState.UserId = 0;
            UserState.Username = "";
            Navigation.NavigateTo("/");
        }
    }
}