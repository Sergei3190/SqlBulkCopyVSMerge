using NLog;
using System;
using System.Collections.Generic;
using System.Data;

namespace SqlBulkCopyVSMerge.Helpers
{
    public static class SourceTestDataBuilderExtensions
    {
        private static readonly Logger _log = LogManager.GetLogger(nameof(SourceTestDataBuilderExtensions));

        public static (List<int> points, List<long> requestsId, Dictionary<string, string> tablesNamesDB) GetObjectsToFill(this SourceTestDataBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            return (builder.GetPoints(), builder.GetRequestsId(), builder.GetTablesNamesDB());
        }

        public static Dictionary<string, DataTable> GetFilledObjects(this SourceTestDataBuilder builder, in List<int> points, in List<long> requestsId)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            if (points is null)
                throw new ArgumentNullException(nameof(points));

            if (requestsId is null)
                throw new ArgumentNullException(nameof(requestsId));

            return builder.GetCompletedClientTables(points, requestsId);
        }
    }
}
