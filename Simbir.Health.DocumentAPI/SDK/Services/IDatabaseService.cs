using Simbir.Health.DocumentAPI.Model.Database.DTO;

namespace Simbir.Health.DocumentAPI.SDK.Services
{
    public interface IDatabaseService
    {
        public Task CreateHistory(int user_id, History_Create dtoObj);

        public Task UpdateHistory(int id, History_Create dtoObj);

        public History_Get? GetHistoryFromId(int id, int userId);

        public List<History_Get>? GetHistoryAccount(int id, int userId);
    }
}
