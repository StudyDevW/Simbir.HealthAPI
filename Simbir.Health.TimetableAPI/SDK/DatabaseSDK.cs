using Simbir.Health.TimetableAPI.Model.Database;
using Simbir.Health.TimetableAPI.Model.Database.DBO;
using Simbir.Health.TimetableAPI.Model.Database.DTO;
using Simbir.Health.TimetableAPI.SDK.Services;

namespace Simbir.Health.TimetableAPI.SDK
{
    public class DatabaseSDK : IDatabaseService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _conf;

        public DatabaseSDK(IConfiguration configuration) {
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(string.Empty);
            _conf = configuration;
        }

        private bool TimeValid(DateTime from, DateTime to)
        {
            bool seconds_access = false;
            bool minutes_access = false;
            bool difference_access = false;

            if ((from.Minute % 30 == 0) && (to.Minute % 30 == 0))
                minutes_access = true;

            if ((from.Second == 0) && (to.Second == 0))
                seconds_access = true;

            if ((from - to).TotalHours <= 12)
                difference_access = true;

            if (seconds_access && minutes_access && difference_access)
                return true;

            return false;
        }

        public async Task CreateRecordTimetable(Timetable_Create dtoObj)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                if (dtoObj != null && TimeValid(dtoObj.from, dtoObj.to))
                {
                    Timetable timetable = new Timetable()
                    {
                        hospitalId = dtoObj.hospitalId,
                        doctorId = dtoObj.doctorId,
                        from = dtoObj.from,
                        to = dtoObj.to, 
                        room = dtoObj.room
                    };

                    db.timeTableObj.Add(timetable);
                    await db.SaveChangesAsync();
                }
            }
        }



    }
}
