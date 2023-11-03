using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;

namespace ICCModule
{
    public class ICCModuleContext : DataContext
    {
        private static MappingSource mappingSource = new AttributeMappingSource();

        public ICCModuleContext()
            : base("")
        {
            //是否要記錄SQL Log
            if (isLogSQL)
            {
                this.Log = new StringWriter();
            }
        }

        internal ICCModuleContext(string connection)
            : base(connection, mappingSource)
        {

            //是否要記錄SQL Log
            if (isLogSQL)
            {
                this.Log = new StringWriter();
            }
        }

        //是否要記錄SQL Log?
        public const bool isLogSQL = true;

    }
}
