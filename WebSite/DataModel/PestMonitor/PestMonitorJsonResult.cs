using ICCModule.EntityService.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Website.DataModel.PestMonitor
{
    /// <summary>
    /// 資料存取模組 病蟲害監測資料
    /// </summary>
    [Serializable]
    public class PestMonitorJsonResult
    {
        public PestMonitorJsonResult(List<VW_PestMonitorDetail> rawData)
        {
            PestMonitorDataList = RunStatistic(rawData ?? new List<VW_PestMonitorDetail>());
        }


        public List<PestMonitorData> PestMonitorDataList { get; set; }

        private List<PestMonitorData> RunStatistic(List<VW_PestMonitorDetail> rawData)
        {
            List<PestMonitorData> result = new List<PestMonitorData>();

            var pestGroup = rawData.GroupBy(x => x.ProjName);

            foreach (var gp in pestGroup)
            {
                var data = new PestMonitorData
                {
                    ProjName = gp.Key,
                    Points = gp.ToList(),
                    Stats =
                        gp.GroupBy(x => x.Village)
                            .Select(x =>
                                new Stat()
                                {
                                    Village = x.Key,
                                    PestNumbers = x.Sum(y => y.PestNumbers)
                                }).ToList()
                };

                result.Add(data);

            }

            return result;
        }
    }


}