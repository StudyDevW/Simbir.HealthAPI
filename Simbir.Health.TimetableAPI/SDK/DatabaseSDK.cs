using Microsoft.EntityFrameworkCore;
using Simbir.Health.TimetableAPI.Model.Database;
using Simbir.Health.TimetableAPI.Model.Database.DBO;
using Simbir.Health.TimetableAPI.Model.Database.DTO;
using Simbir.Health.TimetableAPI.Model.Database.DTO.HospitalsSelect;
using Simbir.Health.TimetableAPI.SDK.Services;
using System.Collections.Generic;

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

        public async Task UpdateRecordTimetable(int id, Timetable_Create dtoObj)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                if (dtoObj != null && TimeValid(dtoObj.from, dtoObj.to))
                {
                    var changeRecord = db.timeTableObj.Where(c => c.Id == id).FirstOrDefault();

                    if (changeRecord != null)
                    {
                        if (changeRecord.hospitalId != dtoObj.hospitalId)
                        {
                            changeRecord.hospitalId = dtoObj.hospitalId;
                            await db.SaveChangesAsync();
                        }

                        if (changeRecord.doctorId != dtoObj.doctorId)
                        {
                            changeRecord.doctorId = dtoObj.doctorId;
                            await db.SaveChangesAsync();
                        }

                        if (changeRecord.from != dtoObj.from)
                        {
                            changeRecord.from = dtoObj.from;
                            await db.SaveChangesAsync();
                        }

                        if (changeRecord.to != dtoObj.to)
                        {
                            changeRecord.to = dtoObj.to;
                            await db.SaveChangesAsync();
                        }

                        if (changeRecord.room != changeRecord.room)
                        {
                            changeRecord.room = changeRecord.room;
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        public async Task DeleteRecordTimetable(int id)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var deleteRecord = db.timeTableObj.Where(c => c.Id == id).FirstOrDefault();

                if (deleteRecord != null)
                {
                    db.Remove(deleteRecord);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteDoctorRecordTimetable(int id)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var deleteRecord = db.timeTableObj.Where(c => c.doctorId == id);

                if (deleteRecord != null)
                {
                    while (deleteRecord.Count() > 0)
                    {
                        db.timeTableObj.Remove(deleteRecord.FirstOrDefault());
                        await db.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task DeleteHospitalRecordTimetable(int id)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var deleteRecord = db.timeTableObj.Where(c => c.hospitalId == id);

                if (deleteRecord != null)
                {
                    while (deleteRecord.Count() > 0)
                    {
                        db.timeTableObj.Remove(deleteRecord.FirstOrDefault());
                        await db.SaveChangesAsync();
                    }
                }
            }
        }

        public Hospital_GetAll GetAllHospitalsRecords(int id, DateTime _from, DateTime _to)
        {
            Hospital_GetAll allRecords = new Hospital_GetAll();

            allRecords.Settings = new Hospital_SelectionSettings { from = _from, to = _to };

            List<Timetable_Get> records = new List<Timetable_Get>();

            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var hospital = db.timeTableObj.Where(c => c.hospitalId == id);

                foreach (var hospitalRecord in hospital)
                {
                    var hospital_from_access = false;

                    var hospital_to_access = false;

                    if (hospitalRecord.from > _from)
                        hospital_from_access = true;

                    if (hospitalRecord.to < _to) 
                        hospital_to_access = true;

                    if (hospital_from_access && hospital_to_access)
                    {
                        Timetable_Get timetable_get = new Timetable_Get()
                        {
                            Id = hospitalRecord.Id,
                            hospitalId = hospitalRecord.hospitalId,
                            doctorId = hospitalRecord.doctorId,
                            from = hospitalRecord.from,
                            to = hospitalRecord.to,
                            room = hospitalRecord.room
                        };

                        records.Add(timetable_get);
                    }
                }
            }

            allRecords.ContentFill(records);

            return allRecords;
        }

        public Hospital_GetAll GetAllDoctorRecords(int id, DateTime _from, DateTime _to)
        {
            Hospital_GetAll allRecords = new Hospital_GetAll();

            allRecords.Settings = new Hospital_SelectionSettings { from = _from, to = _to };

            List<Timetable_Get> records = new List<Timetable_Get>();

            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var hospital = db.timeTableObj.Where(c => c.doctorId == id);

                foreach (var hospitalRecord in hospital)
                {
                    var hospital_from_access = false;

                    var hospital_to_access = false;

                    if (hospitalRecord.from > _from)
                        hospital_from_access = true;

                    if (hospitalRecord.to < _to)
                        hospital_to_access = true;

                    if (hospital_from_access && hospital_to_access)
                    {
                        Timetable_Get timetable_get = new Timetable_Get()
                        {
                            Id = hospitalRecord.Id,
                            hospitalId = hospitalRecord.hospitalId,
                            doctorId = hospitalRecord.doctorId,
                            from = hospitalRecord.from,
                            to = hospitalRecord.to,
                            room = hospitalRecord.room
                        };

                        records.Add(timetable_get);
                    }
                }
            }

            allRecords.ContentFill(records);

            return allRecords;
        }

        public Hospital_GetAll GetRoomRecord(int id, string room, DateTime _from, DateTime _to)
        {
            Hospital_GetAll allRecords = new Hospital_GetAll();

            allRecords.Settings = new Hospital_SelectionSettings { from = _from, to = _to };

            List<Timetable_Get> records = new List<Timetable_Get>();

            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var hospital = db.timeTableObj.Where(c => c.doctorId == id && c.room == room);

                foreach (var hospitalRecord in hospital)
                {
                    var hospital_from_access = false;

                    var hospital_to_access = false;

                    if (hospitalRecord.from > _from)
                        hospital_from_access = true;

                    if (hospitalRecord.to < _to)
                        hospital_to_access = true;

                    if (hospital_from_access && hospital_to_access)
                    {

                        Timetable_Get timetable_get = new Timetable_Get()
                        {
                            Id = hospitalRecord.Id,
                            hospitalId = hospitalRecord.hospitalId,
                            doctorId = hospitalRecord.doctorId,
                            from = hospitalRecord.from,
                            to = hospitalRecord.to,
                            room = hospitalRecord.room
                        };

                        records.Add(timetable_get);
                    }
                }
            }

            allRecords.ContentFill(records);

            return allRecords;
        }

        private Timetable_Get GetTimeTableRecord(DataContext _db, int id)
        {
            var actual_timetable = _db.timeTableObj.Where(c => c.Id == id).FirstOrDefault();

            if (actual_timetable != null)
            {

                Timetable_Get timetable = new Timetable_Get()
                {
                    Id = actual_timetable.Id,
                    doctorId = actual_timetable.doctorId,
                    hospitalId = actual_timetable.hospitalId,
                    from = actual_timetable.from,
                    to = actual_timetable.to,
                    room = actual_timetable.room
                };

                return timetable;
            }

            return new Timetable_Get();
        }

        public async Task WriteAppointment(int id_timetable, int id_user, DateTime time)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var actualRecord = GetTimeTableRecord(db, id_timetable);

                AppointmentsTable appointmentsTable = new AppointmentsTable()
                {
                    timetableId = actualRecord.Id,
                    doctorId = actualRecord.doctorId,
                    hospitalId = actualRecord.hospitalId,
                    userId = id_user,
                    time = time,
                    room = actualRecord.room
                };  

                db.appointmentsTableObj.Add(appointmentsTable);
                await db.SaveChangesAsync();
            }
        }

        public bool AppointmentUsing(DataContext _db, DateTime time)
        {
            foreach (var appointment in _db.appointmentsTableObj)
            {
                if (appointment.time == time)
                    return true;
            }

            return false;
        }

        public List<Appointments_Free> FreeAppointments(int id)
        {
            List<Appointments_Free> freeAppointments = new List<Appointments_Free>();   

            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var actualRecord = GetTimeTableRecord(db, id);

                var from = actualRecord.from;

                var to = actualRecord.to;

                var interval = TimeSpan.FromMinutes(30);

                for (var i = from; i < to; i += interval)
                {
                    if (!AppointmentUsing(db, i))
                    {
                        Appointments_Free appointmentsFree = new Appointments_Free()
                        {
                            doctorId = actualRecord.doctorId,
                            hospitalId = actualRecord.hospitalId,
                            time = i
                        };

                        freeAppointments.Add(appointmentsFree);
                    }
                }
            }

            return freeAppointments;
        }

        public async Task DeleteAppoinment(int appointment_id, int userId)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                if (db.appointmentsTableObj.Where(c => c.userId == userId).FirstOrDefault() != null)
                {
                    var getAppointment = db.appointmentsTableObj.Where(c => c.Id == appointment_id).FirstOrDefault();
                    db.appointmentsTableObj.Remove(getAppointment);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
