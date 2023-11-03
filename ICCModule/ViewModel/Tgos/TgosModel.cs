using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.ViewModel
{

    public class TgosModel
    {
        public List<Info> Info { get; set; }
        public List<Addresslist> AddressList { get; set; }
    }

    public class Info
    {
        public string IsSuccess { get; set; }
        public string InAddress { get; set; }
        public string InSRS { get; set; }
        public string InFuzzyType { get; set; }
        public string InFuzzyBuffer { get; set; }
        public string InIsOnlyFullMatch { get; set; }
        public string InIsSupportPast { get; set; }
        public string InIsShowCodeBase { get; set; }
        public string InIsLockCounty { get; set; }
        public string InIsLockTown { get; set; }
        public string InIsLockVillage { get; set; }
        public string InIsLockRoadSection { get; set; }
        public string InIsLockLane { get; set; }
        public string InIsLockAlley { get; set; }
        public string InIsLockArea { get; set; }
        public string InIsSameNumber_SubNumber { get; set; }
        public string InCanIgnoreVillage { get; set; }
        public string InCanIgnoreNeighborhood { get; set; }
        public string InReturnMaxCount { get; set; }
        public string OutTotal { get; set; }
        public string OutMatchType { get; set; }
        public string OutMatchCode { get; set; }
        public string OutTraceInfo { get; set; }
    }

    public class Addresslist
    {
        public string PERIOD { get; set; }
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
        public string AREA { get; set; }
        public string NUMBER { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public string CODEBASE { get; set; }
        public string CODE1 { get; set; }
        public string CODE2 { get; set; }
    }
}
