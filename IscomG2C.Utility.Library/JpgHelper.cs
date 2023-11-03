using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace IscomG2C.Utility
{
    /// <summary>
    /// 20161226 - JAY 由JDA般移過來，圖片壓縮功能
    /// </summary>
    public class JpgHelper
    {
        /// <summary>
        ///  預設圖檔寬度為 800 px
        /// </summary>
        static readonly int DEFAULT_IMAGE_WIDTH = 800;
        /// <summary>
        ///  預設圖檔高度為 600 px
        /// </summary>
        static readonly int DEFAULT_IMAGE_HEIGHT = 600;


        /// <summary>
        ///  處理 JPEG 檔 (串流輸入) 縮放、調整品質、漸近式格式
        ///  
        ///  2014.10.28 Jason
        /// </summary>
        /// <param name="fromFile">來源JPEG檔串流</param>
        /// <param name="toSaveUrl">寫入 JPEG 檔位置，不指定則傳回串流</param>
        /// <param name="width">輸出寬度，-1表示不指定。若寬、高均指定，則不進行比例換算</param>
        /// <param name="height">輸出高度，-1表示不指定</param>
        /// <param name="maxSide">最大邊長，若width或height被設定，則此值忽略</param>
        /// <param name="quality">品質，預設100</param>
        /// <param name="interlaced">是否存為漸近式JPEG，預設為 true</param>
        ///  <returns>Stream</returns>
        public static Stream ProcessJPEG(System.IO.Stream fromFile, int width = -1, int height = -1, int maxSide = -1, int quality = 100, string toSaveUrl = null, bool interlaced = true)
        {
            MemoryStream msOut = null;

            // 產生原始 Image 物件 (含色彩資訊)
            using (System.Drawing.Image imageSource = System.Drawing.Image.FromStream(fromFile, true))
            {
                int iWidth, iHeight;
                //   計算長寬
                calcSide(imageSource, out iWidth, out iHeight, width, height, maxSide);


                #region 設定編碼參數

                //  設定編碼參數
                EncoderParameters ep = null;
                //  是否設為漸進式 JPEG
                if (interlaced)
                {
                    ep = new EncoderParameters(3);
                    ep.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                    ep.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.ScanMethod, (int)EncoderValue.ScanMethodInterlaced);
                    ep.Param[2] = new EncoderParameter(System.Drawing.Imaging.Encoder.RenderMethod, (int)EncoderValue.RenderProgressive);

                }
                else
                {
                    ep = new EncoderParameters(1);
                    ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);
                }

                #endregion



                #region 存入圖檔

                // 擷取已安裝之影像編碼器和解碼器 (稱為轉碼器) 的所有相關資訊
                ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders().First(c => c.MimeType == "image/jpeg");


                Bitmap imageOutput = new Bitmap(imageSource, iWidth, iHeight);



                //  是否指定輸出路徑，若是則建立資料夾
                if (!string.IsNullOrEmpty(toSaveUrl))
                {
                    //  建立輸出路徑或資料夾
                    string dir = Path.GetDirectoryName(toSaveUrl);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    imageOutput.Save(toSaveUrl, codec, ep);

                }
                else
                {
                    // 否，以串流方式傳回
                    msOut = new MemoryStream();

                    imageOutput.Save(msOut, codec, ep);

                    msOut.Seek(0, SeekOrigin.Begin);
                }

                imageOutput.Dispose();



                #endregion

            }   //  end using

            return msOut;

        }

        /// <summary>
        ///  處理 JPEG 檔 (路徑輸入)  縮放、調整品質、漸近式格式
        ///  
        ///  2014.10.28 Jason
        /// <param name="fromFile">來源JPEG檔路徑</param>
        /// <param name="toSaveUrl">寫入 JPEG 檔位置，不指定則傳回串流</param>
        /// <param name="width">輸出寬度，-1表示不指定</param>
        /// <param name="height">輸出高度，-1表示不指定</param>
        /// <param name="quality">品質，預設100</param>
        /// <param name="interlaced">是否存為漸近式JPEG，預設為 true</param>
        ///  <returns>Stream</returns>
        public static Stream ProcessJPEG(string fromFile, int width = -1, int height = -1, int maxSide = -1, int quality = 100, string toSaveUrl = null, bool interlaced = true)
        {
            FileStream fs = new FileStream(fromFile, FileMode.Open, FileAccess.Read);
            //  轉呼叫另一版本
            return ProcessJPEG(fs, width, height, quality, maxSide, toSaveUrl, interlaced);

        }


        #region 內部函式


        /// <summary>
        /// 計算並調整長寬
        /// 2014.11.4 重構 by Jason
        /// </summary>
        /// <param name="imageSource">原始圖檔 Image </param>
        /// <param name="iWidth">out : 最後計算出的寬度</param>
        /// <param name="iHeight">out: 最後計算出的高度</param>
        /// <param name="width">輸出寬度，-1表示不指定。若寬、高均指定，則不進行比例換算</param>
        /// <param name="height">輸出高度，-1表示不指定</param>
        /// <param name="maxSide">最大邊長，若width或height被設定，則此值忽略</param>
        private static void calcSide(Image imageSource, out int iWidth, out int iHeight, int width = -1, int height = -1, int maxSide = -1)
        {
            //原始圖片的寬、高，先設為傳回值
            iWidth = imageSource.Width;
            iHeight = imageSource.Height;


            //  寬高均指定，則不進行比例換算
            if (width != -1 && height != -1)
            {
                iWidth = width;
                iHeight = height;
            }
            else  //  否則，檢查是否圖片長寬超過其值
                if ((imageSource.Width > width) || (imageSource.Height > height))
                {

                    //  長寬均未指定
                    if (width == -1 && height == -1)
                    {
                        //  最大邊長若也未指定，則採用預設值
                        if (maxSide == -1)
                        {
                            width = DEFAULT_IMAGE_WIDTH;
                            height = DEFAULT_IMAGE_HEIGHT;
                        }
                        else
                        {
                            //  有指定最大邊長，則進行調整
                            if (iWidth > iHeight)   //  橫圖
                            {
                                if (iWidth > maxSide)
                                    width = maxSide;
                            }
                            else
                                if (iHeight > iWidth) // 直圖
                                {
                                    if (iHeight > maxSide)
                                        height = maxSide;
                                }

                        }
                    }

                    // 若 width 已指定值
                    if (width != -1)
                    {
                        //  設定修改後的圖寬，未指定則採用預設值
                        iWidth = width;

                        // 計算欲調整的高度值
                        iHeight = Convert.ToInt32((Convert.ToDouble(iWidth) / Convert.ToDouble(imageSource.Width)) * Convert.ToDouble(imageSource.Height));

                    }
                    else
                        if (height != -1)
                        {
                            //設定修改後的圖高，未指定則採用預設值
                            iHeight = height;
                            // 計算欲調整的寬度值
                            iWidth = Convert.ToInt32((Convert.ToDouble(iHeight) / Convert.ToDouble(imageSource.Height)) * Convert.ToDouble(imageSource.Width));

                        }

                }
                else      // 若圖片沒有超過設定值，不執行縮圖
                {

                    iHeight = imageSource.Height;
                    iWidth = imageSource.Width;
                }
        }

        #endregion
    }
}
