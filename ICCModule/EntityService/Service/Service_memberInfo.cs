using ICCModule.Entity.Tables.Platform;
using ICCModule.Helper;
using ICCModule.Repository;
using ICCModule.Repository.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.EntityService.Service
{
    public class Service_MemberInfo
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="LoginID"></param>
        /// <returns></returns>
        public static memberInfo GetDetail(long ID)
        {
            return RepositoryUtility.GetDetail<memberInfo>(x => x.ID == ID);
        }

        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="lId">帳號</param>
        /// <returns></returns>
        public static memberInfo GetAccountDetail(string lId)
        {
            return RepositoryUtility.GetDetail<memberInfo>(x => x.LoginID == lId);
        }
        
        /// <summary>
        /// 抓取第三方平台綁定帳號
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <returns></returns>
        public static memberInfo GetAccountDetail(string LoginId,int typeId)
        {
            if(typeId == 1)
            {
                return RepositoryUtility.GetDetail<memberInfo>(x => x.GoogleId == LoginId);
            }
            if(typeId == 2)
            {
                return RepositoryUtility.GetDetail<memberInfo>(x => x.FacebookId == LoginId);
            }
            if(typeId == 3)
            {
                return RepositoryUtility.GetDetail<memberInfo>(x => x.LineLoginId == LoginId);
            }
            return null;
        }

        public static memberInfo ChkAccount(string ChkAccount)
        {
            return RepositoryUtility.GetDetail<memberInfo>(x => x.LoginID == ChkAccount);
        }
        public static memberInfo ChkAccount(string ChkAccount, string SourceAccount)
        {
            return RepositoryUtility.GetDetail<memberInfo>(x => x.LoginID == ChkAccount && x.LoginID != SourceAccount);
        }

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<memberInfo> GetList()
        {
            return RepositoryUtility.GetList<memberInfo>();
        }

        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<memberInfo> GetList(Expression<Func<memberInfo, bool>> filter = null)
        {
            return RepositoryUtility.GetList<memberInfo>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(memberInfo data)
        {
            if (data.NewYear > 0) data.YearOfBirth = new DateTime(data.NewYear + 1911, 1, 1);
            data.CreatedAt = DateTime.Now;
            return RepositoryUtility.Insert<memberInfo>(data);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static BaseResult Delete(long ID)
        {
            return RepositoryUtility.Delete<memberInfo>(x => x.ID == ID);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static BaseResult DeleteByAccount(string account)
        {
            return RepositoryUtility.Delete<memberInfo>(x => x.LoginID == account);
        }

        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult Update(memberInfo data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<memberInfo>(ref sErrMsg, x => x.ID == data.ID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }

            /*不再頁面的資料會變空值，無法這樣做
            Type t = data.GetType();
            PropertyInfo[] properties = t.GetProperties();

            foreach (PropertyInfo pi in properties)
            {
                if (pi.CanWrite)
                {
                    pi.SetValue(model, pi.GetValue(data, null), null);
                }
            }
            */
            if (!String.IsNullOrEmpty(data.Status))
            {
                model.Status = data.Status;
            }

            if ((string.IsNullOrEmpty(model.LoginPass)) && model.Status == "Y" && !string.IsNullOrEmpty(data.LoginPass))
            {
                model.LoginPass = data.LoginPass;
            }

            model.Email = data.Email;
            if (!String.IsNullOrEmpty(data.VerifyMethod))
            {
                model.VerifyMethod = data.VerifyMethod;
            }
            model.Name = data.Name;
            model.Identify = data.Identify;
            model.District = data.District;
            model.Mobile = data.Mobile;
            model.SubscribeEpidemic = data.SubscribeEpidemic;

            // 更新基本資料頁面用到
            if (!String.IsNullOrEmpty(data.Sexy))
            {
                model.Sexy = data.Sexy;
            }
            // 更新基本資料頁面用到
            if (data.YearOfBirth != null)
            {
                model.YearOfBirth = data.YearOfBirth;
            }
            if (!String.IsNullOrEmpty(data.RoleCode))
            {
                model.RoleCode = data.RoleCode;
            }

            model.UpdatedAt = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }
        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult UpdateClient(memberInfo data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<memberInfo>(ref sErrMsg, x => x.ID == data.ID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }

            if (!string.IsNullOrEmpty(data.GoogleId)) CheckFastBind(1, data.GoogleId, data.ID);
            if (!string.IsNullOrEmpty(data.FacebookId)) CheckFastBind(2, data.FacebookId, data.ID);

            model.Email = data.Email;
            model.Name = data.Name;
            model.District = data.District;
            model.Mobile = data.Mobile;
            model.Sexy = data.Sexy;
            model.GoogleId = data.GoogleId;
            model.FacebookId = data.FacebookId;
            if (data.NewYear > 0) model.YearOfBirth = new DateTime(data.NewYear + 1911, 1, 1);

            model.UpdatedAt = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }
        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult UpdateCheckCode(memberInfo data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<memberInfo>(ref sErrMsg, x => x.ID == data.ID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }

            model.CheckCode = data.CheckCode;
            model.UpdatedAt = DateTime.Now;

            // TODO: 未實作，保存關聯檔案，需有 MD5

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }
        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult UpdatePwd(memberInfo data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<memberInfo>(ref sErrMsg, x => x.ID == data.ID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }

            model.LoginPass = data.LoginPass;
            model.OldPasswords = data.OldPasswords;
            model.PasswordExpiredAt = data.PasswordExpiredAt;
            if (model.VerifyMethod == "SMS" && data.SMSVerifiedAt != null)
            {
                model.SMSVerifiedAt = data.SMSVerifiedAt;
            }
            else if (model.VerifyMethod == "MAIL" && data.EmailVerifiedAt != null)
            {
                model.EmailVerifiedAt = data.EmailVerifiedAt;
            }

            model.UpdatedAt = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }
        /// <summary>
        /// LineMessageId 綁定
        /// </summary>
        public static BaseResult BindLineMessage(long id, string lineUserId)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();

            if (!string.IsNullOrEmpty(lineUserId))
            {
                var otherBind = Base.QueryData<memberInfo>(ref sErrMsg, x => x.LineMessageId == lineUserId).ToList();
                foreach (var item in otherBind)
                {
                    item.LineMessageId = "";
                    bool rslt2 = Base.Update(ref sErrMsg);
                }
            }

            //查詢資料
            var model = Base
                .QueryData<memberInfo>(ref sErrMsg, x => x.ID == id)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }

            model.LineMessageId = lineUserId;
            model.LineBindCode = "";
            model.UpdatedAt = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }
        
        /// <summary>
        /// LineMessage 綁定申請
        /// </summary>
        public static BaseResult SetLineBindCode(long id, string lineBindCode)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<memberInfo>(ref sErrMsg, x => x.ID == id)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }

            model.LineBindCode = lineBindCode;
            model.LineBindLimit = DateTime.Now.AddMinutes(15);
            model.UpdatedAt = DateTime.Now;

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 第三方登入榜定
        /// </summary>
        public static BaseResult SetFastBind(long id, int TypeId, string BindCode)
        {
            string sErrMsg = "";
            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<memberInfo>(ref sErrMsg, x => x.ID == id)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }

            var allTypeId = new List<int>() { 1, 2, 3 };
            if (allTypeId.Contains(TypeId) && !string.IsNullOrEmpty(BindCode))
            {
                CheckFastBind(TypeId, BindCode, id);

                if (TypeId == 1) model.GoogleId = BindCode;
                if (TypeId == 2) model.FacebookId = BindCode;
                if (TypeId == 3) model.LineLoginId = BindCode;
                model.UpdatedAt = DateTime.Now;

                //更新回資料庫
                bool rslt = Base.Update(ref sErrMsg);
                return new BaseResult(rslt, sErrMsg);
            }

            return new BaseResult(false, sErrMsg);
        }
        /// <summary>
        /// 第三方登入確認
        /// </summary>
        public static void CheckFastBind(int TypeId, string BindCode,long skipId)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base.QueryData<memberInfo>(ref sErrMsg, x => x.ID!= skipId);
            if (TypeId == 1)
            {
                var data = model.Where(x => x.GoogleId == BindCode).ToList();
                foreach(var item in data)
                {
                    item.GoogleId = "";
                    item.UpdatedAt = DateTime.Now;
                }
                //更新回資料庫
                bool rslt = Base.Update(ref sErrMsg);
            }
            else if (TypeId == 2)
            {
                var data = model.Where(x => x.FacebookId == BindCode).ToList();
                foreach (var item in data)
                {
                    item.FacebookId = "";
                    item.UpdatedAt = DateTime.Now;
                }
                //更新回資料庫
                bool rslt = Base.Update(ref sErrMsg);
            }
            else if (TypeId == 3)
            {
                var data = model.Where(x => x.LineLoginId == BindCode).ToList();
                foreach (var item in data)
                {
                    item.LineLoginId = "";
                    item.UpdatedAt = DateTime.Now;
                }
                //更新回資料庫
                bool rslt = Base.Update(ref sErrMsg);
            }
        }
    }
}
