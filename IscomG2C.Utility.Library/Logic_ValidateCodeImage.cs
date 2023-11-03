using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace IscomG2C.Utility
{
    /// <summary>圖形驗證碼模組
    /// 
    /// </summary>
    public class Logic_ValidateCodeImage
    {
        /// <summary>產生驗證碼圖片 轉換成 byte array
        /// 
        /// </summary>
        /// <param name="checkCode"></param>
        /// <returns></returns>
        public static byte[] GetBinary_ValidateCodeImage(string checkCode)
        {
            //取得圖片物件
            System.Drawing.Image image = CreateCheckCodeImage(checkCode);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] output = ms.ToArray();
            ms.Close();
            return output;
        }

        /// <summary>產生驗證碼圖片
        /// 
        /// </summary>
        /// <param name="checkCode"></param>
        /// <returns></returns>
        public static System.Drawing.Image CreateCheckCodeImage(string checkCode)
        {
            System.Drawing.Bitmap image = new System.Drawing.Bitmap((checkCode.Length * 23), 40);//產生圖片，寬20*位數，高40像素
            System.Drawing.Graphics g = Graphics.FromImage(image);


            //生成隨機生成器
            Random random = new Random(Guid.NewGuid().GetHashCode());
            //int int_Red = 103;
            //int int_Green = 237;
            //int int_Blue = 242;

            int int_Red = 255;
            int int_Green = 255;
            int int_Blue = 255;
            //int_Red = random.Next(256);//產生0~255
            //int_Green = random.Next(256);//產生0~255
            //int_Blue = (int_Red + int_Green > 400 ? 0 : 400 - int_Red - int_Green);
            //int_Blue = (int_Blue > 255 ? 255 : int_Blue);

            //清空圖片背景色
            g.Clear(Color.FromArgb(int_Red, int_Green, int_Blue));

            //畫圖片的背景噪音線
            for (int i = 0; i <= 8; i++) //24
            {

                int x1 = random.Next(image.Width);
                int x2 = random.Next(image.Width);
                int y1 = random.Next(image.Height);
                int y2 = random.Next(image.Height);

                g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                g.DrawEllipse(new Pen(Color.DarkViolet), new System.Drawing.Rectangle(x1, y1, x2, y2));
            }

            Font font = new System.Drawing.Font("Arial", 22, (System.Drawing.FontStyle.Bold));
            System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2F, true);
            g.DrawString(checkCode, font, brush, 2, 2);
            //for (int i = 0; i <= 99; i++)
            //{

            //    //畫圖片的前景噪音點
            //    int x = random.Next(image.Width);
            //    int y = random.Next(image.Height);

            //    image.SetPixel(x, y, Color.FromArgb(random.Next()));
            //}

            //畫圖片的邊框線
            g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);


            return image;

        }
        /// <summary>產生隨機驗證碼
        /// 
        /// </summary>
        /// <param name="iCodeSize"></param>
        /// <returns></returns>
        public static string GetValidateCode(int iCodeSize)
        {
            //string sCodeList = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string sCodeList = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";      // 去除相似物件
            string sCode = "";
            Random rand = new Random();
            for (int i = 0; i < iCodeSize; i++)
                sCode += sCodeList[rand.Next(sCodeList.Length)];
            return sCode;
        }
    }
}