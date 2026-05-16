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
            _rating = rng.Next(75, 81);
            var urlDriverCreate = $"Create/Driver?VoorNaamDriver={Uri.EscapeDataString(DriverModel.FirstName)}&AchterNaamDriver={Uri.EscapeDataString(DriverModel.LastName)}&NationaliteitDriver={Uri.EscapeDataString(DriverModel.Nationality)}&Leeftijd={DriverModel.Age}&ratingDriver={_rating}&TeamID={UserState.TeamId}";
            var responseDriverCreate = await Http.PostAsync(urlDriverCreate, null);
            Navigation.NavigateTo("/dashboard");
        }
    }
}