using ICCModule.EntityService.Service;
using InformationSystem.Models.Login;
using InformationSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InformationSystem.Controllers
{
    /// <summary>
    /// 開放API
    /// </summary>
    public class ServiceController : ApiController
    {
        /// <summary>
        /// 獲取監測數據
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResponseModel GetMonitorData(ServiceQueryModel req)
        {
            var _res = new ResponseModel();
            try
            {
                var login = new Model_Login_Index()
                {
                    LoginID = req.Account,
                    LoginPass = req.LoginPass
                };
                //登入驗證
                string msg = "";
                LoginState state = login.LoginCheck(false, ref msg);
                if (state != LoginState.OK) throw new Exception(msg);

                if (req.StartDate.HasValue && req.EndDate.HasValue)
                {
                    if (req.StartDate > req.EndDate) throw new Exception("起始日勿大於結束日");
                    if (req.StartDate.Value.AddYears(1) < req.EndDate) throw new Exception("查詢區間不得超過一年");
                }
                var sDate = DateTime.Now.Day >= 15 ? (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddYears(-1) : (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddMonths(-1).AddYears(-1);
                var eDate = DateTime.Now.Day >= 15 ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) : (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddMonths(-1);
                var absDate = DateTime.Now.Day >= 15 ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) : (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddMonths(-1);
                if (req.StartDate.HasValue && req.EndDate.HasValue)
                {
                    sDate = req.StartDate.Value;
                    eDate = req.EndDate.Value;
                }
                else if (req.StartDate.HasValue)
                {
                    sDate = req.StartDate.Value;
                    eDate = req.StartDate.Value.AddYears(1);
                }
                else if (req.EndDate.HasValue)
                {
                    eDate = req.EndDate.Value;
                    sDate = req.EndDate.Value.AddYears(-1);
                }
                var key = "小黃薊馬";
                if (req.SearchType == 2) key = "瓜實蠅";
                else if (req.SearchType == 3) key = "東方果實蠅";

                var data = Service_monitorProject.GetSelectList(x => x.Name == key ,x =>x.Date < absDate && x.Date >= sDate && x.Date < eDate).Select(x => new 
                {
                    地區 = x.Distist + x.Village,
                    編號 = x.SerialNum,
                    蟲數 = x.Pests,
                    調查起日 = x.StartDate.ToString("yyyy-MM-dd"),
                    調查迄日 = x.EndDate.ToString("yyyy-MM-dd"),
                    週遭作物 = x.Crops,
                });

                _res.Success = true;
                _res.Data = data;
            }
            catch (Exception ex)
            {
                _res.Msg = ex.Message;
            }
            return _res;
        }
    }
}