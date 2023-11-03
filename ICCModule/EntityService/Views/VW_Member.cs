using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Text;

namespace ICCModule.Entity.Tables
{

    [Serializable]
    [Table(Name = "VW_Member")]
    public class VW_Member
    {
        /// <summary> 
        /// 
        /// </summary> 
        [Column]
        public String Type { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        [Column]
        public Int64 ID { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        [Column]
        public String LoginID { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        [Column]
        public String UserName { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        [Column]
        public String Email { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        [Column]
        public String Identify { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        [Column]
        public String Special { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        [Column]
        public String Status { get; set; }
    }
}
