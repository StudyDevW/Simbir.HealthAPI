namespace Simbir.Health.TimetableAPI.Model.Database.DTO.HospitalsSelect
{
    public class Hospital_GetAll
    {
        public Hospital_SelectionSettings Settings { get; set; }

        public List<Timetable_Get> Content { get; set; }

        public void ContentFill(List<Timetable_Get> listOut)
        {
            Content = listOut;
        }
    }
}
