namespace Simbir.Health.TimetableAPI.Model.Database.DTO
{
    public class Timetable_Create
    {
        public int hospitalId { get; set; }

        public int doctorId { get; set; }

        public DateTime from { get; set; }

        public DateTime to { get; set; }

        public string? room { get; set; }
    }
}
