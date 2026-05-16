using F1_ManagerFrontEnd.Models;

namespace F1_ManagerFrontEnd.Components.Pages
{
    public partial class ResultsRaceweekend
    {
        private List<RaceResult>? _results;
        private int _userTeamId;
        private TrackInfo? _track;
        protected override async Task OnInitializedAsync()
        {
            var trackResponse = await Http.GetAsync($"get/previous/track/by/user?IDUser={UserState.UserId}");
            if (trackResponse.IsSuccessStatusCode)
            {
                _track = await trackResponse.Content.ReadFromJsonAsync<TrackInfo>();
            }

            await GetResult();
        }

        private async Task GetResult()
        {
            var url = $"get/raceweekend/result/by/user?IDUser={UserState.UserId}";
            var response = await Http.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                _results = await response.Content.ReadFromJsonAsync<List<RaceResult>>();
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }

        private void GoHome()
        {
            Navigation.NavigateTo("/dashboard");
        }

        public class TrackInfo
        {
            public string NaamTrack { get; set; } = "";
            public string LandTrack { get; set; } = "";
            public int LapsTrack { get; set; }
        }

    }
}