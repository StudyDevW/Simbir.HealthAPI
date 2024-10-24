using Simbir.Health.HospitalAPI.Model.Database.DBO;

namespace Simbir.Health.HospitalAPI.Model.Database.DTO.HospitalsSelect
{
    public class Hospitals_GetAll
    {
        public Hospitals_SelectionSettings Settings { get; set; }

        public List<HospitalTable> Content { get; set; }

        public void ContentFill(List<HospitalTable> listOut)
        {
            Content = listOut;
        }
    }
}
