﻿namespace Simbir.Health.AccountAPI.Model.Database.DTO
{
    public class Accounts_Info
    {
        public int id {  get; set; }

        public string? lastName {  get; set; }

        public string? firstName { get; set; }

        public List<string>? roles { get; set; }
    }
}
