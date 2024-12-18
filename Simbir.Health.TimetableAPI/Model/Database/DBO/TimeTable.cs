﻿namespace Simbir.Health.TimetableAPI.Model.Database.DBO
{
    public class Timetable
    {
        public int Id { get; set; }

        public int hospitalId { get; set; }

        public int doctorId { get; set; }

        public DateTime from { get; set; }

        public DateTime to { get; set; }

        public string? room { get; set; }
    }
}
