using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Text;

namespace ICCModule.Entity.Tables
{

    [Serializable]
    [Table(Name = "VW_Search")]
    public class VW_Search
    {
        /// <summary> 
        /// 
        /// </summary> 
        [Column]
        public String SearchType { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        [Column]
        public Int64 ID { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        [Column]
        public String Title { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        [Column]
        public String Context { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        [Column]
        public DateTime DataDt { get; set; }
    }
}
