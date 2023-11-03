using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.ViewModel.Member
{
    public class ResetPwdModel
    {
        public string ValidateMethod { get; set; }
        public string ValidateData { get; set; }
        public string ValidateCode { get; set; }
    }
}