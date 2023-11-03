using ICCModule.EntityService.Views;
using ICCModule.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using ICCModule.Repository;

namespace ICCModule.EntityService.Service
{
    public static class Service_VW_PestMonitorDetail
    {
        public static IQueryable<VW_PestMonitorDetail> GetList(IBaseRepository repository, Expression<Func<VW_PestMonitorDetail, bool>> filter = null)
        {
            string dummy = "";
            return repository.QueryData(ref dummy, filter);
        }

        public static List<VW_PestMonitorDetail> GetList(Expression<Func<VW_PestMonitorDetail, bool>> filter = null)
        {
            return RepositoryUtility.GetList(filter);
        }
    }
}
