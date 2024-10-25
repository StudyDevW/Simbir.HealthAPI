using Simbir.Health.TimetableAPI.Model.Database.DTO;

namespace Simbir.Health.TimetableAPI.SDK.Services
{
    public interface IDatabaseService
    {
        public Task CreateRecordTimetable(Timetable_Create dtoObj);
    }
}
