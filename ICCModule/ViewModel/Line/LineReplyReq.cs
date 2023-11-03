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
    public class LineReplyMessageModel
    {
        public string replyToken { get; set; }
        public List<SendMessage> messages { get; set; }
        public bool notificationDisabled { get; set; } = false;
    }
    public class LinePushMessageModel
    {
        public List<string> to { get; set; }
        public List<SendMessage> messages { get; set; }
        public bool notificationDisabled { get; set; } = false;
    }
    public class SendMessage
    {
        public string type { get; set; } = "text";
        public string text { get; set; }
        public string originalContentUrl { get; set; }
        public string previewImageUrl { get; set; }
    }
}