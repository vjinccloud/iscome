using ICCModule.Entity.Tables.Platform;
using ICCModule.Repository;
using ICCModule.Repository.Class;
using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ICCModule.EntityService.Service
{
    public class Service_QuestionManagement
    {
        /// <summary>
        /// 抓取單筆資料
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static QuestionManagement GetDetail(int ID)
        {
            return RepositoryUtility.GetDetail<QuestionManagement>(x => x.ID == ID);
        }

        /// <summary>讀取清單</summary>
        /// <param name="Type"></param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static List<QuestionManagement> GetList(string Type = "")
        {
            Expression<Func<QuestionManagement, bool>> filter = (x => true);
            if (!string.IsNullOrEmpty(Type))
            {
                filter = filter.And(x => x.Type == Type);
            }

            return RepositoryUtility.GetList<QuestionManagement>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(QuestionManagement data)
        {
            return RepositoryUtility.Insert<QuestionManagement>(data);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(int ID)
        {
            return RepositoryUtility.Insert<QuestionManagement>(GetDetail(ID));
        }

        /// <summary>
        /// 更新單筆資料
        /// </summary>
        public static BaseResult Update(QuestionManagement data)
        {
            string sErrMsg = "";

            BaseRepository Base = new BaseRepository();
            //查詢資料
            var model = Base
                .QueryData<QuestionManagement>(ref sErrMsg, x => x.ID == data.ID)
                .SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
                return new BaseResult(false, sErrMsg);
            }
            model.Type = data.Type;
            model.Question = data.Question;
            model.Answer = data.Answer;
            model.UpdatedAt = DateTime.Now;

            // TODO: 未實作，保存關聯檔案，需有 MD5

            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return new BaseResult(rslt, sErrMsg);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        public static BaseResult Delete(int ID)
        {
            return RepositoryUtility.Delete<QuestionManagement>(x => x.ID == ID);
        }
    }
}
