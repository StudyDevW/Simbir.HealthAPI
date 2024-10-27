using Simbir.Health.HospitalAPI.Model;
using Simbir.Health.HospitalAPI.Model.Database.DBO;
using Simbir.Health.HospitalAPI.Model.Database.DTO;
using Simbir.Health.HospitalAPI.Model.Database.DTO.HospitalsSelect;
using Simbir.Health.HospitalAPI.SDK.Services;

namespace Simbir.Health.HospitalAPI.SDK
{
    public class DatabaseSDK : IDatabaseService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _conf;

        public DatabaseSDK(IConfiguration configuration)
        {
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(string.Empty);
            _conf = configuration;
        }

        public async Task CreateHospital(Hospitals_Create dtoObj) 
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                if (dtoObj != null)
                {
                    HospitalTable hospital = new HospitalTable() {
                        name = dtoObj.name,
                        address = dtoObj.address,
                        contactPhone = dtoObj.contactPhone,
                        rooms = dtoObj.rooms
                    };

                    db.hospitalTableObj.Add(hospital);
                    await db.SaveChangesAsync();
                }
            }
        }

        public Hospitals_GetAll GetAllHospitals(int _from, int _count)
        {
            Hospitals_GetAll allHospitals = new Hospitals_GetAll();

            allHospitals.Settings = new Hospitals_SelectionSettings { from = _from, count = _count };
            //allAccounts.Content.Add(new Accounts_Info() { id = 0 });

            List<HospitalTable> hospitals = new List<HospitalTable>();

            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                if (_count != 0)
                {
                    var filtered_query = db.hospitalTableObj.Skip(_from).Take(_count);

                    foreach (var hospitalInfo in filtered_query)
                    {
                        hospitals.Add(hospitalInfo);
                    }
                }
                else
                {
                    var filtered_query = db.hospitalTableObj.Skip(_from);

                    foreach (var hospitalInfo in filtered_query)
                    {
                        hospitals.Add(hospitalInfo);
                    }
                }
            }

            allHospitals.ContentFill(hospitals);

            return allHospitals;
        }

        public HospitalTable GetHospitalFromId(int id)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var filtered_query = db.hospitalTableObj.Where(o => o.id == id);

                foreach (var hospitalInfo in filtered_query)
                {
                    return new HospitalTable() { 
                        id = hospitalInfo.id,
                        address = hospitalInfo.address,
                        contactPhone = hospitalInfo.contactPhone,
                        name = hospitalInfo.name,
                        rooms = hospitalInfo.rooms
                    };
                }
            }

            return new HospitalTable();
        }

        public Hospitals_Rooms GetHospitalRoomsFromId(int id)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var filtered_query = db.hospitalTableObj.Where(o => o.id == id).FirstOrDefault();

                if (filtered_query != null)
                {
                    return new Hospitals_Rooms()
                    {
                        rooms = filtered_query.rooms
                    };
                }
            }

            return new Hospitals_Rooms();
        }

        public async Task UpdateHospital(Hospitals_Create dtoObj, int id)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var hospitalToChange = db.hospitalTableObj.Where(c => c.id == id).FirstOrDefault();

                if (hospitalToChange != null)
                {
                    if (hospitalToChange.name != dtoObj.name)
                    {
                        hospitalToChange.name = dtoObj.name;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogInformation("UpdateHospital: name not changed");
                    }

                    if (hospitalToChange.address != dtoObj.address)
                    {
                        hospitalToChange.address = dtoObj.address;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogInformation("UpdateHospital: address not changed");
                    }

                    if (hospitalToChange.contactPhone != dtoObj.contactPhone)
                    {
                        hospitalToChange.contactPhone = dtoObj.contactPhone;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogInformation("UpdateHospital: contactPhone not changed");
                    }


                    if (hospitalToChange.rooms != dtoObj.rooms)
                    {
                        hospitalToChange.rooms = dtoObj.rooms;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogInformation("UpdateHospital: rooms not changed");
                    }
                }
            }
        }

        public async Task DeleteHospitalWithAdmin(int id)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var hospitalToDelete = db.hospitalTableObj.Where(c => c.id == id).FirstOrDefault();

                if (hospitalToDelete != null)
                {
                    db.hospitalTableObj.Remove(hospitalToDelete);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
