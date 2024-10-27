﻿namespace Simbir.Health.DocumentAPI.Model.Database.DTO
{
    public class History_Create
    {
        public DateTime date { get; set; }

        public int pacientId { get; set; }

        public int hospitalId { get; set; }

        public int doctorId { get; set; }

        public string? room { get; set; }

        public string? data { get; set; }
    }
}