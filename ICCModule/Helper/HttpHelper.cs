using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.Helper
{
    /// <summary>
    /// http請求類
    /// </summary>
    public class HttpHelper
    {
        private HttpClient _httpClient;
        private string _baseIPAddress;

        /// <param name="ipaddress">請求的基礎IP，例如：http://192.168.0.33:8080/ </param>
        public HttpHelper(string ipaddress = "")
        {
            this._baseIPAddress = ipaddress;
            _httpClient = new HttpClient { BaseAddress = new Uri(_baseIPAddress) };
        }

        /// <summary>
        /// 創建帶用戶信息的請求客戶端
        /// </summary>
        /// <param name="userName">用戶賬號</param>
        /// <param name="pwd">用戶密碼，當WebApi端不要求密碼驗證時，可傳空串</param>
        /// <param name="uriString">The URI string.</param>
        public HttpHelper(string userName, string pwd = "", string uriString = "")
            : this(uriString)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                _httpClient.DefaultRequestHeaders.Authorization = CreateBasicCredentials(userName, pwd);
            }
        }

        /// <summary>
        /// Get請求數據
        ///   /// <para>最終以url參數的方式提交</para>
        /// <para>yubaolee 2016-3-3 重構與post同樣異步調用</para>
        /// </summary>
        /// <param name="parameters">參數字典,可為空</param>
        /// <param name="requestUri">例如/api/Files/UploadFile</param>
        /// <returns></returns>
        public object Get(Dictionary<string, string> parameters, string requestUri, string accessToken = "")
        {
            if (parameters != null)
            {
                var strParam = string.Join("&", parameters.Select(o => o.Key + "=" + o.Value));
                requestUri = string.Concat(ConcatURL(requestUri), '?', strParam);
            }
            else
            {
                requestUri = ConcatURL(requestUri);
            }

            if (!string.IsNullOrEmpty(accessToken)) _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var result = _httpClient.GetStringAsync(requestUri);
            return result.Result;
        }

        /// <summary>
        /// Get請求數據
        ///   /// <para>最終以url參數的方式提交</para>
        /// <para>yubaolee 2016-3-3 重構與post同樣異步調用</para>
        /// </summary>
        /// <param name="parameters">參數字典,可為空</param>
        /// <param name="requestUri">例如/api/Files/UploadFile</param>
        /// <returns></returns>
        public virtual async Task<ContentStream> GetContentStream(Dictionary<string, string> parameters, string requestUri, string accessToken = "")
        {
            if (parameters != null)
            {
                var strParam = string.Join("&", parameters.Select(o => o.Key + "=" + o.Value));
                requestUri = string.Concat(ConcatURL(requestUri), '?', strParam);
            }
            else
            {
                requestUri = ConcatURL(requestUri);
            }
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            if (!string.IsNullOrEmpty(accessToken)) _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            return new ContentStream(await response.Content.ReadAsStreamAsync(), response.Content.Headers);
        }

        /// <summary>
        /// Get請求數據
        /// <para>最終以url參數的方式提交</para>
        /// </summary>
        /// <param name="parameters">參數字典</param>
        /// <param name="requestUri">例如/api/Files/UploadFile</param>
        /// <returns>實體對象</returns>
        public T Get<T>(Dictionary<string, string> parameters, string requestUri) where T : class
        {
            string jsonString = Get(parameters, requestUri).ToString();
            if (string.IsNullOrEmpty(jsonString))
                return null;

            return JsonHelper.Instance.Deserialize<T>(jsonString);
        }

        /// <summary>
        /// 以json的方式Post數據 返回string類型
        /// <para>最終以json的方式放置在http體中</para>
        /// </summary>
        /// <param name="entity">實體</param>
        /// <param name="requestUri">例如/api/Files/UploadFile</param>
        /// <returns></returns>
        public string Post(object entity, string requestUri, string accessToken = "", string contentType = "application/json")
        {
            string request = string.Empty;
            if (entity != null)
                request = JsonHelper.Instance.Serialize(entity);
            HttpContent httpContent = new StringContent(request);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return Post(requestUri, httpContent, accessToken);
        }

        /// <summary>
        /// 提交字典類型的數據
        /// <para>最終以formurlencode的方式放置在http體中</para>
        /// <para>李玉寶於2016-07-20 19:01:59</para>
        /// </summary>
        /// <returns>System.String.</returns>
        public string PostDicObj(Dictionary<string, object> para, string requestUri)
        {
            Dictionary<string, string> temp = new Dictionary<string, string>();
            foreach (var item in para)
            {
                if (item.Value != null)
                {
                    if (item.Value.GetType().Name.ToLower() != "string")
                    {
                        temp.Add(item.Key, JsonHelper.Instance.Serialize(item.Value));
                    }
                    else
                    {
                        temp.Add(item.Key, item.Value.ToString());
                    }
                }
                else
                {
                    temp.Add(item.Key, "");
                }
            }

            return PostDic(temp, requestUri);
        }

        /// <summary>
        /// Post Dic數據
        /// <para>最終以formurlencode的方式放置在http體中</para>
        /// <para>李玉寶於2016-07-15 15:28:41</para>
        /// </summary>
        /// <returns>System.String.</returns>
        public string PostDic(Dictionary<string, string> temp, string requestUri)
        {
            HttpContent httpContent = new FormUrlEncodedContent(temp);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            return Post(requestUri, httpContent, "");
        }

        public string PostByte(byte[] bytes, string requestUrl, string accessToken = "", string contentType = "application/octet-stream")
        {
            HttpContent content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return Post(requestUrl, content, accessToken);
        }

        private string Post(string requestUrl, HttpContent content, string accessToken)
        {
            if (!string.IsNullOrEmpty(accessToken)) _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
            var result = _httpClient.PostAsync(ConcatURL(requestUrl), content);
            return result.Result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 把請求的URL相對路徑組合成絕對路徑
        /// <para>李玉寶於2016-07-21 9:54:07</para>
        /// </summary>
        private string ConcatURL(string requestUrl)
        {
            return new Uri(_httpClient.BaseAddress, requestUrl).OriginalString;
        }

        private AuthenticationHeaderValue CreateBasicCredentials(string userName, string password)
        {
            string toEncode = userName + ":" + password;
            // The current HTTP specification says characters here are ISO-8859-1.
            // However, the draft specification for the next version of HTTP indicates this encoding is infrequently
            // used in practice and defines behavior only for ASCII.
            Encoding encoding = Encoding.GetEncoding("utf-8");
            byte[] toBase64 = encoding.GetBytes(toEncode);
            string parameter = Convert.ToBase64String(toBase64);

            return new AuthenticationHeaderValue("Basic", parameter);
        }
    }
    /// <summary>
    /// Stream object for content such as image, file, etc.
    /// </summary>
    public class ContentStream : Stream
    {
        protected Stream _baseStream;

        protected Stream BaseStream
        {
            get
            {
                if (_baseStream == null) { throw new ObjectDisposedException(nameof(BaseStream)); }
                return _baseStream;
            }
        }

        public HttpContentHeaders ContentHeaders { get; }

        public ContentStream(Stream baseStream, HttpContentHeaders contentHeaders)
        {
            _baseStream = baseStream;
            ContentHeaders = contentHeaders;
        }

        public override bool CanRead => _baseStream?.CanRead ?? false;

        public override bool CanSeek => _baseStream?.CanSeek ?? false;

        public override bool CanWrite => _baseStream?.CanWrite ?? false;

        public override long Length => BaseStream.Length;

        public override long Position { get => BaseStream.Position; set => BaseStream.Position = value; }

        public override void Flush() => BaseStream.Flush();

        public override int Read(byte[] buffer, int offset, int count) => BaseStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);

        public override void SetLength(long value) => BaseStream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => BaseStream.Write(buffer, offset, count);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _baseStream?.Dispose();
                _baseStream = null;
            }
        }
    }
}
