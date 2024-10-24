namespace Simbir.Health.HospitalAPI.Model.Database.DTO
{
    public class Hospitals_Create
    {
        public string? name { get; set; }

        public string? address { get; set; }

        public string? contactPhone { get; set; }

        public List<string>? rooms { get; set; }
    }
}
