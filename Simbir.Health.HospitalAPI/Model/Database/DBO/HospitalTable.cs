namespace Simbir.Health.HospitalAPI.Model.Database.DBO
{
    public class HospitalTable
    {
        public int id {  get; set; }

        public string? name { get; set; }

        public string? address { get; set; }

        public string? contactPhone { get; set; }

        public List<string>? rooms { get; set; }
    }
}
