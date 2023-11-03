using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Text;

namespace ICCModule.Entity.Tables
{

    [Serializable]
    [Table(Name = "VW_News")]
    public class VW_New
    {
        /// <summary> 
        /// 
        /// </summary> 
        [Column]
        public String NewsType { get; set; }
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
        public DateTime StartDate { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        [Column]
        public String Code { get; set; }
    }
}
