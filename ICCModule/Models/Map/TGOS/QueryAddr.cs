using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.Models.Map.TGOS
{
    public class QueryAddr
    {
        public List<info> Info { get; set; }

        public List<address> AddressList { get; set; }
    }

    public class info
    {
        public string InAddress { get; set; }
        public string InSRS { get; set; }
        public string InFuzzyType { get; set; }
        public int InFuzzyBuffer { get; set; }
        public bool InIsOnlyFullMatch { get; set; }
        public bool InIsLockCounty { get; set; }
        public bool InIsLockTown { get; set; }
        public bool InIsLockVillage { get; set; }
        public bool InIsLockRoadSection { get; set; }
        public bool InIsLockLane { get; set; }
        public bool InIsLockAlley { get; set; }
        public bool InIsLockArea { get; set; }
        public bool InIsSameNumber_SubNumber { get; set; }
        public bool InCanIgnoreVillage { get; set; }
        public bool InCanIgnoreNeighborhood { get; set; }
        public int InReturnMaxCount { get; set; }
        public int OutTotal { get; set; }
        public string OutMatchType { get; set; }
        public string OutMatchCode { get; set; }
        public string OutTraceInfo { get; set; }
    }

    public class address
    {
        public string FULL_ADDR { get; set; }
        public string COUNTY { get; set; }
        public string TOWN { get; set; }
        public string VILLAGE { get; set; }
        public string NEIGHBORHOOD { get; set; }
        public string ROAD { get; set; }
        public string SECTION { get; set; }
        public string LANE { get; set; }
        public string ALLEY { get; set; }
        public string SUB_ALLEY { get; set; }
        public string TONG { get; set; }
        public string NUMBER { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
