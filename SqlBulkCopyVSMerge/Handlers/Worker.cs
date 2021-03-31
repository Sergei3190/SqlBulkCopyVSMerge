using NLog;
using SqlBulkCopyVSMerge.DataBase;
using SqlBulkCopyVSMerge.Helpers;
using System;
using System.Linq;

namespace SqlBulkCopyVSMerge.Handlers
{
    public class Worker
    {
        private readonly Logger _log;
        private readonly SqlCommands _sqlCommands;

        public Worker()
        {
            _log = LogManager.GetLogger(nameof(Worker));
            _sqlCommands = new SqlCommands();
        }

        public int RunWorker()
        {
            try
            {
                var sourceTestData = this.GetSourceTestData();

                var result = this.AddRequestsInDB(_sqlCommands, sourceTestData.requestsId);

                if (result != 1)
                    return -1;

                var testsResult = this.TestsMethodsRun(_sqlCommands, sourceTestData.clientTables, sourceTestData.tablesNamesDB);

                if (testsResult != 1)
                    return -1;

                var pointsToretry = _sqlCommands.GetPointsToRetry();
                _log.Info($"Количество возвращенных из БД точек = {pointsToretry?.ToList().Count ?? 0}\r\n");

                if (pointsToretry is null)
                    return -1;

                return 1;
            }
            catch (Exception ex)
            {
                _log.Error($"Ошибка при выполнении метода RunWorker\r\n{ex}");
                return -1;
            }
        }
    }
}
