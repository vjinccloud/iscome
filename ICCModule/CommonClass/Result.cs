using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICCModule
{
    /// <summary>回傳結果-用來取代bool值回傳結果 包含成功/失敗 和 錯誤訊息
    /// 
    /// </summary>
    [Serializable]
    public class BaseResult
    {
        public BaseResult()
        {
            this.Msg = "";
        }

        /// <summary>建構式-指定結果與訊息
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="Msg"></param>
        public BaseResult(bool result, string Msg = "")
        {
            this.result = result;
            this.Msg = Msg;
        }

        /// <summary>建構式-將例外狀況作為回傳結果
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public BaseResult(Exception ex)
        {
            //不會有人例外狀況還給我設true ...
            this.result = false;
            this.Msg = ex.ToString();
        }

        /// <summary>建構式-疊加訊息
        /// 
        /// </summary>
        /// <param name="input"></param>
        public BaseResult(BaseResult input)
        {
            this.Msg = "";

            this.Append(input);
        }

        /// <summary>結果 1:成功 2:失敗
        /// 
        /// </summary>
        public bool result { get; set; }

        /// <summary>錯誤訊息 成功的話通常就不會給了
        /// 
        /// </summary>
        public string Msg { get; set; }

        /// <summary>累加結果 通常可以應用在多個回傳結果要組合的時候
        /// 
        /// </summary>
        /// <param name="input"></param>
        public void Append(BaseResult input)
        {
            this.result = this.result && input.result;
            //疊加回傳訊息
            //如果輸入結果正確 此次不用疊加
            if (input.result)
            {
                return;
            }
            //如果目前沒有訊息 用輸入者為主
            if (string.IsNullOrWhiteSpace(this.Msg))
            {
                this.Msg = input.Msg;
                return;
            }
            //訊息一致 也不用
            if (this.Msg == input.Msg)
            {
                return;
            }
            //特殊:如果結果Msg不一致 才會疊加
            this.Msg = this.Msg + ";" + input.Msg;
        }
    }

    [Serializable]
    public class JsonResponse: BaseResult
    {
        public JToken Data { get; set; }
    }

    /// <summary>單元測試回傳結果-用來取代bool值回傳結果
    /// 
    /// </summary>
    [Serializable]
    public class UnitTestResult : BaseResult
    {
        public UnitTestResult()
        {
            this.Msg = "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ClassName">類別名稱</param>
        public UnitTestResult(string ClassName)
        {
            this.Msg = "";
            this.ClassName = ClassName;
        }

        /// <summary>類別名稱
        /// 
        /// </summary>
        public string ClassName { get; set; }
    }
}
