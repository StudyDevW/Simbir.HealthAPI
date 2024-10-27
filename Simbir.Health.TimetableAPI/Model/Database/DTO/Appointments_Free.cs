namespace Simbir.Health.TimetableAPI.Model.Database.DTO
{
    public class Appointments_Free
    {
        public int hospitalId { get; set; }

        public int doctorId { get; set; }

        public DateTime? time { get; set; }
    }
}
