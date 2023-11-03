using ICCModule.Entity.Tables;
using ICCModule.Repository;
using ICCModule.Repository.Class;
using ICCModule.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.EntityService.Service
{
    public class Service_sysUserInfo
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static sysUserInfo GetDetail(string LoginID)
        {
            return RepositoryUtility.GetDetail<sysUserInfo>(x => x.LoginID == LoginID);
        }

        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static sysUserInfo GetDetailByEmail(string Email)
        {
            return RepositoryUtility.GetDetail<sysUserInfo>(x => x.EmailAddress == Email);
        }

        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public static sysUserInfo GetDetailByToken(string Token)
        {
            return RepositoryUtility.GetDetail<sysUserInfo>(x => x.ResetTokenID == Token);
        }

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<sysUserInfo> GetList(Expression<Func<sysUserInfo, bool>> filter = null)
        {
            return RepositoryUtility.GetList<sysUserInfo>(filter);
        }
        
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<sysUserSelect> GetEnableDoctorList(Expression<Func<sysUserInfo, bool>> filter = null)
        {
            if (filter == null) filter = x => true;
            filter = filter.And(x => x.State == "Y" && (x.RoleID == "R05" || x.RoleID == "R08"));

            BaseRepository _db = new BaseRepository();
            var res = _db.QueryData<sysUserInfo>(filter).Select(x => new sysUserSelect
            {
                LoginID = x.LoginID,
                RoleID = x.RoleID,
                UserName = x.UserName,
                District = x.District,
            }).ToList();
            return res;
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(sysUserInfo data)
        {
            return RepositoryUtility.Insert<sysUserInfo>(data);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static BaseResult Delete(string LoginID)
        {
            //return RepositoryUtility.Delete<sysUserInfo>(x => x.LoginID == LoginID);
            string sErrMsg = "";
            BaseRepository Base = new BaseRepository();
            var model = Base
                .QueryData<sysUserInfo>(ref sErrMsg, x => x.LoginID == LoginID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.State = "D";
            model.EditDate = DateTime.Now;
            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }
        public static sysUserInfo ChkAccount(string ChkAccount)
        {
            return RepositoryUtility.GetDetail<sysUserInfo>(x => x.LoginID == ChkAccount);
        }
        public static sysUserInfo ChkAccount(string ChkAccount, string SourceAccount)
        {
            return RepositoryUtility.GetDetail<sysUserInfo>(x => x.LoginID == ChkAccount && x.LoginID != SourceAccount);
        }
        public static BaseResult Update(sysUserInfo data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<sysUserInfo>(ref sErrMsg, x => x.LoginID == data.LoginID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            if (!String.IsNullOrEmpty(data.LoginPass))
            {
                model.LoginPass = data.LoginPass;
            }
            model.State = data.State;
            model.EmailAddress = data.EmailAddress;
            model.UserName = data.UserName;
            model.UnitName = data.UnitName;
            if (model.RoleID != data.RoleID)
            {
                Service_sysUserSpecialPermission.Delete(model.LoginID);
                data.Special = "N";
            }
            model.RoleID = data.RoleID;
            if (!String.IsNullOrEmpty(data.Identify))
            {
                model.Identify = data.Identify;
            }
            
            if (!String.IsNullOrEmpty(data.Mobile))
            {
                model.Mobile = data.Mobile;
            }

            model.Sexy = data.Sexy;
            model.YearOfBirth = data.YearOfBirth;
            model.District = data.District;
            if (data.LastChangePWDate != null)
            {
                model.LastChangePWDate = data.LastChangePWDate;
            }
            model.ErrorCount = data.ErrorCount;
           
            model.LastErrorSum = data.LastErrorSum;
            model.LastSessionID = data.LastSessionID;
            if (!String.IsNullOrEmpty(data.VerifyMode))
            {
                model.VerifyMode = data.VerifyMode;
            }
            
            model.Special = data.Special;
            
            model.EditDate = DateTime.Now;


            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 忘記密碼用
        /// </summary>
        /// <param name="LoginID"></param>
        /// <param name="ResetTokenID"></param>
        /// <returns></returns>
        public static BaseResult ForgetPassword(string LoginID, string ResetTokenID)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<sysUserInfo>(ref sErrMsg, x => x.LoginID == LoginID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.ResetTokenID = ResetTokenID;
            model.EditDate = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 重設密碼用
        /// </summary>
        /// <param name="LoginID"></param>
        /// <param name="NewPass"></param>
        /// <returns></returns>
        public static BaseResult ResetPassword(string LoginID, string NewPass)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<sysUserInfo>(ref sErrMsg, x => x.LoginID == LoginID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.LoginPass = NewPass;
            model.LastChangePWDate = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 重設忘記密碼Token，並更新密碼紀錄
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static BaseResult ResetToken(string LoginID)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<sysUserInfo>(ref sErrMsg, x => x.LoginID == LoginID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.ResetTokenID = String.Empty;
            BaseResult baseResult;
            // 現有密碼加入密碼歷史內
            List <sysUserPass_log> userPass_Logs = Service_sysUserPass_log.GetList(LoginID);
            userPass_Logs.OrderByDescending(l => l.created_at);
            // 移除最舊的
            if (userPass_Logs.Count >= 3)
            {
                sysUserPass_log old = userPass_Logs.First();
                if (old != null)
                {
                    baseResult = Service_sysUserPass_log.Delete(old.id);
                    if (!baseResult.result)
                    {
                        return baseResult;
                    }
                }
            }
            // 建立新的
            baseResult = Service_sysUserPass_log.Insert(new sysUserPass_log() 
            {
                LoginID = LoginID,
                LoginPW = model.LoginPass,
                created_at = DateTime.Now,
            });
            if (!baseResult.result)
            {
                return baseResult;
            }

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        public static List<sysUserInfo> GetDoctorList()
        {
            var list = RepositoryUtility.GetList<sysUserInfo>(a => a.RoleID == "R05" || a.RoleID == "R08");
            return list;
        }

        public static sysUserInfo GetDoctorByName(string name)
        {
            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<sysUserInfo>( x => x.UserName == name)
                .SingleOrDefault();

            return model;
        }
    }
}
