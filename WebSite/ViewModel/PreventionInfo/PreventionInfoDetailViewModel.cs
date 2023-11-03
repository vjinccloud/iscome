using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Website.Models.ExternalAPI;

namespace Website.ViewModel.PreventionInfo
{
    public class PreventionInfoDetailViewModel
    {
        public List<PestNotice> List { get; set; }

        public string KeyWord { get; set; }
    }
}