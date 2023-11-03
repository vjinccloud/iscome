using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICCModule.Repository.Interface
{
    public interface IPagerInfo
    {
        List<T> GetRange<T>(ref string sErrMsg, IQueryable<T> _modules, PagerInfo pager) where T : class;
    }
}
