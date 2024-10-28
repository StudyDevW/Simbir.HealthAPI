using Microsoft.EntityFrameworkCore;
using Simbir.Health.DocumentAPI.Model.Database;
using Simbir.Health.DocumentAPI.Model.Database.DBO;
using Simbir.Health.DocumentAPI.Model.Database.DTO;
using Simbir.Health.DocumentAPI.SDK.Services;

namespace Simbir.Health.DocumentAPI.SDK
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

        public async Task CreateHistory(int user_id, History_Create dtoObj/*, HttpClient httpClient*/)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                if (dtoObj != null)
                {
                    HistoryTable historyTable = new HistoryTable()
                    {
                        date = dtoObj.date,
                        pacientId = dtoObj.pacientId,
                        doctorId = dtoObj.doctorId,
                        hospitalId = dtoObj.hospitalId,
                        room = dtoObj.room,
                        data = dtoObj.data,
                    };

                    db.historyTableObj.Add(historyTable);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateHistory(int id, History_Create dtoObj)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                if (dtoObj != null)
                {
                    var historyUpdate = db.historyTableObj.Where(c => c.Id == id).FirstOrDefault();

                    if (historyUpdate != null)
                    {
                        if (!historyUpdate.date.Equals(dtoObj.date))
                        {
                            historyUpdate.date = dtoObj.date;
                            await db.SaveChangesAsync();
                        }

                        if (historyUpdate.pacientId != dtoObj.pacientId)
                        {
                            //нельзя изменить пациента
              
                        }

                        if (historyUpdate.hospitalId != dtoObj.hospitalId)
                        {
                            historyUpdate.hospitalId = dtoObj.hospitalId;
                            await db.SaveChangesAsync();
                        }

                        if (historyUpdate.doctorId != dtoObj.doctorId)
                        {
                            historyUpdate.doctorId = dtoObj.doctorId;
                            await db.SaveChangesAsync();
                        }

                        if (historyUpdate.room != dtoObj.room)
                        {
                            historyUpdate.room = dtoObj.room;
                            await db.SaveChangesAsync();
                        }

                        if (historyUpdate.data  != dtoObj.data)
                        {
                            historyUpdate.data = dtoObj.data;
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        public History_Get? GetHistoryFromId(int id, int userId)
        {
            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            {
                var historyCollect = db.historyTableObj.Where(c => c.Id == id).FirstOrDefault();


                if (historyCollect != null)
                {
                    if (userId != -1)
                    {
                        if (userId != historyCollect.pacientId)
                        {
                            return null;
                        }
                    }

                    History_Get history = new History_Get()
                    {
                        Id = id,
                        pacientId = historyCollect.pacientId,
                        hospitalId = historyCollect.hospitalId,
                        doctorId = historyCollect.doctorId,
                        date = historyCollect.date,
                        room = historyCollect.room,
                        data = historyCollect.data
                    };

                    return history;
                }

            }

            return null;
        }

        public List<History_Get>? GetHistoryAccount(int id, int userId)
        {
            List<History_Get>? historyAccounts = new List<History_Get>();

            using (DataContext db = new DataContext(_conf.GetConnectionString("ServerConn")))
            { 
                var historyAll = db.historyTableObj.Where(c => c.pacientId == id);

                if (historyAll != null)
                {
                    //Возвращает записи где {pacientId}={id} 
                    foreach (var historyAccount in historyAll)
                    {
                        if (userId != -1)
                        {
                            if (userId != historyAccount.pacientId)
                                return null;
                        }

                        History_Get history = new History_Get()
                        {
                            Id = historyAccount.Id,
                            pacientId = historyAccount.pacientId,
                            doctorId = historyAccount.doctorId,
                            hospitalId = historyAccount.hospitalId,
                            date = historyAccount.date,
                            room = historyAccount.room,
                            data = historyAccount.data
                        };

                        historyAccounts.Add(history);
                    }
                }
            }

            if (historyAccounts != null)
                return historyAccounts;

            return null;
        }


    }
}
