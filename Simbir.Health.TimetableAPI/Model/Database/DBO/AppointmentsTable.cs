namespace Simbir.Health.TimetableAPI.Model.Database.DBO
{
    public class AppointmentsTable
    {
        public int Id { get; set; }

        public int hospitalId { get; set; }

        public int doctorId { get; set; }

        public int timetableId { get; set; }

        public int userId { get; set; }

        public DateTime? time { get; set; }

        public string? room { get; set; }
    }
}
