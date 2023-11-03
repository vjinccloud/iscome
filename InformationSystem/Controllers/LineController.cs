using ICCModule.EntityService.Service;
using InformationSystem.Models.Login;
using ICCModule.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ICCModule.Helper;

namespace InformationSystem.Controllers
{
    /// <summary>
    /// Line API
    /// </summary>
    public class LineController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        public void WebHook([FromBody] LineModel req)
        {
            try
            {
                Service_LineMessageLog.ReceiveMsg(req);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}