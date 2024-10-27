using Simbir.Health.TimetableAPI.Model.Database.DTO;
using Simbir.Health.TimetableAPI.Model.Database.DTO.HospitalsSelect;

namespace Simbir.Health.TimetableAPI.SDK.Services
{
    public interface IDatabaseService
    {
        public Task CreateRecordTimetable(Timetable_Create dtoObj);

        public Task UpdateRecordTimetable(int id, Timetable_Create dtoObj);

        public Task DeleteRecordTimetable(int id);

        public Task DeleteDoctorRecordTimetable(int id);

        public Task DeleteHospitalRecordTimetable(int id);

        public Hospital_GetAll GetAllHospitalsRecords(int id, DateTime _from, DateTime _to);

        public Hospital_GetAll GetAllDoctorRecords(int id, DateTime _from, DateTime _to);

        public Hospital_GetAll GetRoomRecord(int id, string room, DateTime _from, DateTime _to);

        public Task WriteAppointment(int id_timetable, int id_user, DateTime time);

        public List<Appointments_Free> FreeAppointments(int id);

        public Task DeleteAppoinment(int appointment_id, int userId);
    }
}
