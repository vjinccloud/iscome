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
    public class Service_VW_Role_Group
    {
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<VWRoleGroup> GetList()
        {
            Expression<Func<VWRoleGroup, bool>> filter = (x => true);
            return RepositoryUtility.GetList<VWRoleGroup>(filter);
        }
    }
}
