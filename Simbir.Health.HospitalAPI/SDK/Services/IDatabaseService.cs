using Simbir.Health.HospitalAPI.Model.Database.DBO;
using Simbir.Health.HospitalAPI.Model.Database.DTO;
using Simbir.Health.HospitalAPI.Model.Database.DTO.HospitalsSelect;

namespace Simbir.Health.HospitalAPI.SDK.Services
{
    public interface IDatabaseService
    {
        public Task CreateHospital(Hospitals_Create dtoObj);

        public Hospitals_GetAll GetAllHospitals(int _from, int _count);

        public HospitalTable GetHospitalFromId(int id);

        public Hospitals_Rooms GetHospitalRoomsFromId(int id);

        public Task UpdateHospital(Hospitals_Create dtoObj, int id);

        public Task DeleteHospitalWithAdmin(int id);
    }
}
