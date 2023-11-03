using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ICCModule.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class LineModel
    {
        public string destination { get; set; }
        public List<Event> events { get; set; }
    }
    /// <summary>
    /// 事件
    /// </summary>
    public class Event
    {
        /// <summary>
        /// 回覆的Token
        /// </summary>
        public string replyToken { get; set; }
        /// <summary>
        /// 事件類型[follow(加入)、unfollow(刪除/封鎖)、message(訊息)、postback(回覆)]
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// Line機器人狀態(active：處於活動狀態，standby：正在開發中)
        /// </summary>
        public string mode { get; set; }
        /// <summary>
        /// 事件時間
        /// </summary>
        public long timestamp { get; set; }
        /// <summary>
        /// 事件來源
        /// </summary>
        public Source source { get; set; }
        /// <summary>
        /// 訊息內容
        /// </summary>
        public Message message { get; set; }
        /// <summary>
        /// 回覆
        /// </summary>
        public Postback postback { get; set; }
        public Link Link { get; set; }
    }
    public class Link
    {
        public string result { get; set; }
        public string nonce { get; set; }
    }
    /// <summary>
    /// 事件來源
    /// </summary>
    public class Source
    {
        /// <summary>
        /// 來源類型[user(個人), group(群組),room(聊天室)]
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 來源序號
        /// </summary>
        public string userId { get; set; }
    }
    /// <summary>
    /// 訊息內容
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 序號
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 訊息類型[Text(文字)、Image(圖片)、Video(影片)、Audio(音訊)、File(檔案)、Location(位置)、Sticker(貼圖)]
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 時長(影片、音檔，單位毫秒)
        /// </summary>
        public int duration { get; set; }
        /// <summary>
        /// 訊息內容(文字)
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 檔案名稱(檔案)
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// 檔案大小(單位bytes)(檔案)
        /// </summary>
        public string fileSize { get; set; }
        /// <summary>
        /// 標題(位置訊息)
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 地址(位置訊息)
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 經度(位置訊息)
        /// </summary>
        public string latitude { get; set; }
        /// <summary>
        /// 緯度(位置訊息)
        /// </summary>
        public string longitude { get; set; }
        /// <summary>
        /// 表情貼(文字)
        /// </summary>
        public List<Emoji> emojis { get; set; }
        /// <summary>
        /// 被標記的用戶(文字)
        /// </summary>
        public Mention mention { get; set; }
        /// <summary>
        /// 多媒體內容(圖、影、音)
        /// </summary>
        public ContentProvider contentProvider { get; set; }
        /// <summary>
        /// 傳送多張圖片時,夾帶的更多資訊
        /// </summary>
        public ImageSet imageSet { get; set; }
    }
    /// <summary>
    /// 傳送多張圖片時,夾帶的更多資訊
    /// </summary>
    public class ImageSet
    {
        public string id { get; set; }
        public int index { get; set; }
        public int total { get; set; }
    }
    #region 文字訊息內容
    /// <summary>
    /// 被標記的用戶
    /// </summary>
    public class Mention
    {
        public List<Mentionee> mentionees { get; set; }
    }
    /// <summary>
    /// 被標記的用戶資訊
    /// </summary>
    public class Mentionee
    {
        /// <summary>
        /// 文字起始處
        /// </summary>
        public int index { get; set; }
        /// <summary>
        /// 文字長度
        /// </summary>
        public int length { get; set; }
        /// <summary>
        /// 被標註的用戶序號
        /// </summary>
        public string userId { get; set; }
    }
    /// <summary>
    /// 表情貼
    /// </summary>
    public class Emoji
    {
        /// <summary>
        /// 文字起始處
        /// </summary>
        public int index { get; set; }
        /// <summary>
        /// 文字長度
        /// </summary>
        public int length { get; set; }
        /// <summary>
        /// 表情貼序號
        /// </summary>
        public string productId { get; set; }
        /// <summary>
        /// 表情貼內容序號
        /// </summary>
        public string emojiId { get; set; }
    }
    #endregion
    #region 多媒體訊息內容
    /// <summary>
    /// 多媒體內容(圖、影、音)
    /// </summary>
    public class ContentProvider
    {
        /// <summary>
        /// 內容來源[line(一般使用者)、external(LIFF)]
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 檔案連結(僅來源為external時)(圖、影、音)
        /// </summary>
        public string originalContentUrl { get; set; }
        /// <summary>
        /// 預覽檔案連結(僅來源為external時)(圖、影)
        /// </summary>
        public string previewImageUrl { get; set; }
    }
    #endregion
    #region
    public class Postback
    {
        public string data { get; set; }
    }
    #endregion

    /// <summary>
    /// 使用者資訊
    /// </summary>
    public class LineUserProfileModel
    {
        public string userId { get; set; }
        public string displayName { get; set; }
        public string pictureUrl { get; set; }
        public string statusMessage { get; set; }
        public string language { get; set; }
    }

}