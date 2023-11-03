using ICCModule.Entity.Tables;
using ICCModule.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.EntityService.Service
{
    public class Service_VW_Member
    {


        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<VW_Member> GetList(string Status, string Identify, string Special)
        {
            Expression<Func<VW_Member, bool>> filter = (x => true && x.Status != "D");
            if (!string.IsNullOrEmpty(Identify))
            {
                filter = filter.And(x => x.Identify == Identify);
            }
            if (!string.IsNullOrEmpty(Status))
            {
                filter = filter.And(x => x.Status == Status);
            }

            if (!string.IsNullOrEmpty(Special))
            {
                filter = filter.And(x => x.Special == Special);
            }
            return RepositoryUtility.GetList<VW_Member>(filter);
        }
    }
}
