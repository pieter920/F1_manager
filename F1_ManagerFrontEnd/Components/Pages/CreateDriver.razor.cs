namespace F1_ManagerFrontEnd.Components.Pages
{
    public partial class CreateDriver
    {

        private DriverCreateModel DriverModel = new();

        public class DriverCreateModel
        {
            public string FirstName { get; set; } = "";

            public string LastName { get; set; } = "";

            public string Nationality { get; set; } = "";

            public int Age { get; set; } = 18;
        }

        Random rng = new Random();
        private int _rating = 0;

        public async Task CreateNewDriver()
        {
            //teamID opvragen van het team van de user
            // var urlTeamID = $"get/Team/from/userID?IDUser={UserState.UserId}";
            // var responseTeamID = await Http.GetAsync(urlTeamID);
            // var team = await responseTeamID.Content.ReadFromJsonAsync<TeamModel>();
            //generate random rating tussen 75 en 80
            _rating = rng.Next(75, 81);
            //driver aanmaken en deze koppelen aan het team van de user
            var urlDriverCreate = $"Create/Driver?VoorNaamDriver={Uri.EscapeDataString(DriverModel.FirstName)}&AchterNaamDriver={Uri.EscapeDataString(DriverModel.LastName)}&NationaliteitDriver={Uri.EscapeDataString(DriverModel.Nationality)}&Leeftijd={DriverModel.Age}&ratingDriver={_rating}&TeamID={UserState.TeamId}";
            var responseDriverCreate = await Http.PostAsync(urlDriverCreate, null);
            Navigation.NavigateTo("/dashboard");
        }
    }
}