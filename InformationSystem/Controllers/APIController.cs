using ICCModule.EntityService.Service;
using InformationSystem.Models.Login;
using ICCModule.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Configuration;
using System.Web;
using ICCModule.Entity.Tables.Platform;
using ICCModule.Models.Map.Google;
using System.Net;
using System.Net.Http;
using System.Web.Http.Results;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Diagnostics;
using DocumentFormat.OpenXml.Office2013.Excel;
using System.Security.Cryptography;

namespace InformationSystem.Controllers
{
    /// <summary>
    /// API
    /// </summary>
    public class APIController : ApiController
    {
        [HttpPost]
        [Route("Api/PesticideInfo")]
        public HttpResponseMessage PesticideInfo(List<ApiModel> DATA)
        {

            List<string> AllowIpList = new List<string>();
            // 白名單設定
            string AllowIP = ConfigurationManager.AppSettings["IP_Allowlist"];
            AllowIpList = AllowIP.Split(',').ToList();

            var result = new
            {
                isSuccess = false,
                message = ""
            };

            // json轉字串
            var json = JsonConvert.SerializeObject(DATA);

            // 判斷IP
            string ip = getIP();

            try
            {
                //初始log資料
                var record = new pesticideResidueAPITestingRecord()
                {
                    SourceData = json,
                    CreatedAt = DateTime.Now,
                    IP = ip
                };
              
                // 判斷IP是否在白名單內
                if (!AllowIpList.Contains(ip))
                {
                    result = new
                    {
                        isSuccess = false,
                        message = "非可存取API之來源"
                    };

                    record.Message = "非可存取API之來源";

                    // 寫LOG紀錄
                    var check = Service_pesticideResidueAPITestingRecord.Insert(record);

                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {                   
                    var objects = DATA;
                    // 處理資料
                    foreach (var data in objects)
                    {
                        var apidata = new pesticideResidueAPITesting()
                        {
                            Year = data.Year,
                            PlanType = data.PlanType,
                            PlanTypeName = data.PlanTypeName,
                            SampleName = data.SampleName,
                            SampleDate = DateTime.Parse(data.SampleDate),
                            AnalyzeDate = DateTime.Parse(data.AnalyzeDate),
                            City = data.City,
                            District = data.District,
                            SampleID = data.SampleID,
                            FastSampleID = data.FastSampleID,
                            ProviderName = data.ProviderName,
                            ProviderAddress = data.ProviderAddress,
                            ProviderPhone = data.ProviderPhone,
                            ProvideUnit = data.ProvideUnit,
                            JointUnit = data.JointUnit,
                            SampleNum = data.SampleNum,
                            CropType = data.CropType,
                            CropName = data.CropName,
                            SampleSource = data.SampleSource,
                            SampleSourceName = data.SampleSourceName,
                            Result = (data.Result == "Y" ? true : false),
                            ProviderCode = data.ProviderCode,
                            ProviderUnit = data.ProviderUnit,
                            SampleResult = (data.SampleResult == "N" ? true : false),
                            SampleContext = (data.SampleContext == null ? "[]" : JsonConvert.SerializeObject(data.SampleContext)),
                            CreatedAt = DateTime.Now
                        };

                        // 先處理作物類別及作物名稱
                        string CropType = string.Empty;
                        string CropName = string.Empty;
                        var crops = Service_crops.GetList(x => x.Name == apidata.CropName).FirstOrDefault();
                        if (crops != null)
                        {
                            CropType = crops.TypeID.ToString();
                            CropName = crops.ID.ToString();
                        }
                        else
                        {
                            CropType = apidata.CropType;
                            CropName = apidata.CropName;
                        }

                        // 寫API資料
                        var insertAPI = Service_pesticideResidueAPITesting.Insert(apidata);
                        
                        if (insertAPI.result)
                        {
                            string INSSCD = string.Empty;
                            List<SampleContext> ChangeData = new List<SampleContext>();
                            if (apidata.SampleContext != "[]")
                            {
                                // 處理SampleContext
                                var SampleContextData = JsonConvert.DeserializeObject<List<SampleContext>>(apidata.SampleContext) ?? new List<SampleContext>();
                                foreach (var cd in SampleContextData)
                                {
                                    var SCD = new SampleContext();
                                    SCD.Name = cd.Name;
                                    SCD.EnName = cd.EnName;
                                    SCD.RemainQty = cd.RemainQty;
                                    SCD.AllowQty = cd.AllowQty;
                                    if (cd.UseWay == "Y")
                                    {
                                        SCD.UseWay = "核准使用";
                                    }
                                    else if (cd.UseWay == "N")
                                    {
                                        SCD.UseWay = "☆未核准使用";
                                    }
                                    else
                                    {
                                        SCD.UseWay = "◎已公告使用方法未公告容許量";
                                    }
                                    ChangeData.Add(SCD);
                                }
                                INSSCD = JsonConvert.SerializeObject(ChangeData);
                            }
                            else
                            {
                                INSSCD = apidata.SampleContext;
                            }

                            // 判斷是否已存在資料
                            var ExistData = Service_pesticideResidueTesting.GetList(x => x.SampleNum == apidata.SampleNum && x.AnalyzeDate == apidata.AnalyzeDate).FirstOrDefault();
                            if (ExistData != null)
                            {
                                // update
                                ExistData.Year = (int.Parse(data.Year) - 1911).ToString();
                                ExistData.PlanType = (apidata.PlanType == "LF" ? "PesticideResidueSampling" : "CampusLunch");
                                ExistData.SampleNum = apidata.SampleNum;
                                ExistData.SampleDate = apidata.SampleDate;
                                ExistData.City = apidata.City;
                                ExistData.District = apidata.District;
                                ExistData.SampleName = apidata.SampleName;
                                //insdata.CropType = data.CropType;
                                //insdata.CropName = data.CropName;
                                ExistData.ProviderName = apidata.ProviderName;
                                ExistData.ProviderCode = apidata.ProviderCode;
                                ExistData.ProviderAddress = apidata.ProviderAddress;
                                ExistData.ProviderPhone = apidata.ProviderPhone;
                                ExistData.SampleSource = (apidata.SampleSource == "F" ? "Field" : "CargoYard");
                                ExistData.ProvideUnit = (apidata.ProvideUnit == "高雄市政府農業局" ? "KcgAgriculture" : "SouthOffice");
                                ExistData.SampleResult = apidata.SampleResult;
                                ExistData.Result = apidata.Result;
                                ExistData.AnalyzeDate = apidata.AnalyzeDate;
                                ExistData.JointUnit = apidata.JointUnit;
                                ExistData.SampleID = apidata.SampleID;
                                ExistData.FastSampleID = apidata.FastSampleID;
                                ExistData.ProviderUnit = apidata.ProviderUnit;
                                ExistData.SampleContext = INSSCD;
                                ExistData.UpdatedAt = DateTime.Now;

                                var updateData = Service_pesticideResidueTesting.Update(ExistData);
                            }
                            else
                            {
                                // insert
                                var insdata = new pesticideResidueTesting();
                                insdata.Year = (int.Parse(data.Year) - 1911).ToString();
                                insdata.PlanType = (apidata.PlanType == "LF" ? "PesticideResidueSampling" : "CampusLunch");
                                insdata.SampleNum = apidata.SampleNum;
                                insdata.SampleDate = apidata.SampleDate;
                                insdata.City = apidata.City;
                                insdata.District = apidata.District;
                                insdata.SampleName = apidata.SampleName;
                                insdata.CropType = CropType;
                                insdata.CropName = CropName;
                                insdata.ProviderName = data.ProviderName;
                                insdata.ProviderCode = apidata.ProviderCode;
                                insdata.ProviderAddress = apidata.ProviderAddress;
                                insdata.ProviderPhone = apidata.ProviderPhone;
                                insdata.SampleSource = (apidata.SampleSource == "F" ? "Field" : "CargoYard");
                                insdata.ProvideUnit = (apidata.ProvideUnit == "高雄市政府農業局" ? "KcgAgriculture" : "SouthOffice");
                                insdata.SampleResult = apidata.SampleResult;
                                insdata.Result = apidata.Result;
                                insdata.AnalyzeDate = apidata.AnalyzeDate;
                                insdata.JointUnit = apidata.JointUnit;
                                insdata.SampleID = apidata.SampleID;
                                insdata.FastSampleID = apidata.FastSampleID;
                                insdata.ProviderUnit = apidata.ProviderUnit;
                                insdata.SampleContext = INSSCD;
                                insdata.CreatedAt = DateTime.Now;
                                insdata.StagingData = "[]";

                                var insertData = Service_pesticideResidueTesting.Insert(insdata);
                            }

                        }

                        result = new
                        {
                            isSuccess = true,
                            message = "成功"
                        };
                    }

                    record.Message = "成功";
                    var check = Service_pesticideResidueAPITestingRecord.Insert(record);

                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }

            }
            catch (Exception ex)
            {
                result = new
                {
                    isSuccess = false,
                    //message = ex.Message
                    message = ex.ToString()
                };

                var record = new pesticideResidueAPITestingRecord()
                {
                    SourceData = json,
                    CreatedAt = DateTime.Now,
                    IP = ip,
                    Message = ex.ToString()
                };
                var check = Service_pesticideResidueAPITestingRecord.Insert(record);
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        public string getIP()
        {
            // 取得ip
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }
            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        public class ApiModel {
            public string Year { get; set; }
            public string PlanType { get; set; }
            public string PlanTypeName { get; set; }
            public string SampleName { get; set; }
            public string SampleDate { get; set; }
            public string AnalyzeDate { get; set; }
            public string City { get; set; }
            public string District { get; set; }
            public string SampleID { get; set; }
            public string FastSampleID { get; set; }
            public string ProviderName { get; set; }
            public string ProviderPhone { get; set; }
            public string ProviderAddress { get; set; }
            public string ProvideUnit { get; set; }
            public string JointUnit { get; set; }
            public string SampleNum { get; set; }
            public string CropType { get; set; }
            public string CropName { get; set; }
            public string SampleSource { get; set; }
            public string SampleSourceName { get; set; }
            public string Result { get; set; }
            public string ProviderCode { get; set; }
            public string ProviderUnit { get; set; }
            public string SampleResult { get; set; }
            public List<SampleContext> SampleContext { get; set; }

        }

        public class SampleContext
        {
            public string Name { get; set; }
            public string EnName { get; set; }
            public string RemainQty { get; set; }
            public string AllowQty { get; set; }
            public string UseWay { get; set; }
        }
    }
}