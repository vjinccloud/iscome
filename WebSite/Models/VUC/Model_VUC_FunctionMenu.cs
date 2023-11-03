using ICCModule.Entity.Tables;
using ICCModule.EntityService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.Models.VUC
{
    /// <summary>左側選單功能模組
    /// 
    /// </summary>
    public class Model_VUC_FunctionMenu
    {
        public Model_VUC_FunctionMenu()
        {
            this.funcList = new List<MainMenu>();
        }
        public List<MainMenu> funcList { get; set; }
        /// <summary>大項目是否要展開?
        /// 
        /// </summary>
        public bool ShowInfo { get; set; }

        /// <summary>讀取資料(提示:如果要全撈 UserID=1)
        /// 
        /// </summary>
        /// <param name="UserID">使用者ID</param>
        public void LoadData(string UserID, string MenuID)
        {
            List<VW_FuntionMenu_LV2> list_Sub = GetList_menuSub(UserID);
            //大選單
            var list_Main = Service_sysMenu.GetList(1);
            //篩選一下 如果子選單的MenuParentID有對應到的大選單才保留
            list_Main = list_Main.Where(main =>
                list_Sub.Exists(sub => sub.MenuParentID == main.MenuID)
            )
            .OrderBy(x => x.Sort)
            .ToList();
            //根據上面兩種選單資料 拼湊出MainMenu物件清單
            this.funcList = new List<MainMenu>();
            foreach (var main in list_Main)
            {
                MainMenu mainMenu = new MainMenu();
                //綁定主選單
                mainMenu.main = main;
                //抓子選單 綁定之
                mainMenu.list_detail = list_Sub.Where(x => x.MenuParentID == main.MenuID).ToList();
                this.funcList.Add(mainMenu);
            }

            //根據MenuID設定ShowInfo(讓大項目可以展開)
            Set_ShowInfo(MenuID);
        }

        /// <summary>
        /// 從資料庫讀取 lv2選單 並且篩選排除重複的選單
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        private static List<VW_FuntionMenu_LV2> GetList_menuSub(string UserID)
        {
            //讀取資料
            //子選單
            List<VW_FuntionMenu_LV2> list_Sub = Service_VW_FuntionMenu_LV2.GetList(UserID);
            //篩選一下 排除掉重複的選單 但是保留可存取列
            List<VW_FuntionMenu_LV2> selectList_Sub = new List<VW_FuntionMenu_LV2>();
            foreach (var item in list_Sub)
            {
                //不存在嗎? 加入
                var current = selectList_Sub.Find(x => x.MenuID == item.MenuID);
                if (current == null)
                {
                    selectList_Sub.Add(item);
                }
                else
                {   //把授權列拼接上去 這不妨礙等一下判斷授權
                    current.AllowList += item.AllowList;
                }
            }

            return selectList_Sub;
        }

        /// <summary>設定選單展開標記
        /// 
        /// </summary>
        public void Set_ShowInfo(string MenuID)
        {
            //沒資料? 後面不處理(應該不會啦)
            if (this.funcList.Count == 0)
                return;
            //先全部關閉
            this.funcList.ForEach(x => x.ShowInfo = false);
            //根據MenuID設定ShowInfo(讓大項目可以展開)
            var temp = this.funcList.Find(x => x.main.MenuID == MenuID);
            //如果沒有找到指定ID 那就讓第一項展開
            if (temp == null)
            {
                this.funcList[0].ShowInfo = true;
                return;
            }
            //不然就 標記為開啟
            temp.ShowInfo = true;
        }
    }

    /// <summary>左側選單-一個大項裡面的內容
    /// 
    /// </summary>
    public class MainMenu
    {
        public MainMenu()
        {
            this.main = new sysMenu();
            this.list_detail = new List<VW_FuntionMenu_LV2>();
        }


        /// <summary>大項目是否要展開?
        /// 
        /// </summary>
        public bool ShowInfo { get; set; }

        /// <summary>主選單
        /// 
        /// </summary>
        public sysMenu main { get; set; }

        /// <summary>次選單列
        /// 
        /// </summary>
        public List<VW_FuntionMenu_LV2> list_detail { get; set; }
    }
}