using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using TemplateEngine.Docx;

namespace ICCModule.Helper
{
    public static class CoordinateTransHelper
    {
        private static double a = 6378137.0;
        private static double b = 6356752.3142451;
        private  static double lon0 = 121 * Math.PI / 180;
        private  static double k0 = 0.9999;
        private  static int dx = 250000;
        private  static int dy = 0;
        private  static double e = 1 - Math.Pow(b, 2) / Math.Pow(a, 2);
        private static double e2 = (1 - Math.Pow(b, 2) / Math.Pow(a, 2)) / (Math.Pow(b, 2) / Math.Pow(a, 2));

        //給WGS84經緯度度分秒轉成TWD97坐標
        public static string lonlat_To_twd97(int lonD, int lonM, int lonS, int latD, int latM, int latS)
        {
            double RadianLon = (double)(lonD) + (double)lonM / 60 + (double)lonS / 3600;
            double RadianLat = (double)(latD) + (double)latM / 60 + (double)latS / 3600;
            return Cal_lonlat_To_twd97(RadianLon, RadianLat);
        }
        //給WGS84經緯度弧度轉成TWD97坐標
        public static string lonlat_To_twd97(double RadianLon, double RadianLat)
        {
            return Cal_lonlat_To_twd97(RadianLon, RadianLat);
        }

        //給TWD97坐標 轉成 WGS84 度分秒字串 (type1傳度分秒 2傳弧度)
        public static string TWD97_To_lonlat(double XValue, double YValue, int Type)
        {

            string lonlat = "";

            if (Type == 1)
            {
                string[] Answer = Cal_TWD97_To_lonlat(XValue, YValue).Split(',');
                int LonDValue = (int)double.Parse(Answer[0]);
                int LonMValue = (int)((double.Parse(Answer[0]) - LonDValue) * 60);
                double LonSValue = (((double.Parse(Answer[0]) - LonDValue) * 60) - LonMValue) * 60;

                int LatDValue = (int)double.Parse(Answer[1]);
                int LatMValue = (int)((double.Parse(Answer[1]) - LatDValue) * 60);
                double LatSValue = (((double.Parse(Answer[1]) - LatDValue) * 60) - LatMValue) * 60;

                lonlat = LonDValue + "度" + LonMValue + "分" + LonSValue + "秒," + LatDValue + "度" + LatMValue + "分" + LatSValue + "秒,";
            }
            else if (Type == 2)
            {
                lonlat = Cal_TWD97_To_lonlat(XValue, YValue);
            }

            return lonlat;
        }

        private static string Cal_lonlat_To_twd97(double lon, double lat)
        {
            string TWD97 = "";

            lon = (lon - Math.Floor((lon + 180) / 360) * 360) * Math.PI / 180;
            lat = lat * Math.PI / 180;

            double V = a / Math.Sqrt(1 - e * Math.Pow(Math.Sin(lat), 2));
            double T = Math.Pow(Math.Tan(lat), 2);
            double C = e2 * Math.Pow(Math.Cos(lat), 2);
            double A = Math.Cos(lat) * (lon - lon0);
            double M = a * ((1.0 - e / 4.0 - 3.0 * Math.Pow(e, 2) / 64.0 - 5.0 * Math.Pow(e, 3) / 256.0) * lat -
                  (3.0 * e / 8.0 + 3.0 * Math.Pow(e, 2) / 32.0 + 45.0 * Math.Pow(e, 3) / 1024.0) *
                  Math.Sin(2.0 * lat) + (15.0 * Math.Pow(e, 2) / 256.0 + 45.0 * Math.Pow(e, 3) / 1024.0) *
                  Math.Sin(4.0 * lat) - (35.0 * Math.Pow(e, 3) / 3072.0) * Math.Sin(6.0 * lat));
            // x
            double x = dx + k0 * V * (A + (1 - T + C) * Math.Pow(A, 3) / 6 + (5 - 18 * T + Math.Pow(T, 2) + 72 * C - 58 * e2) * Math.Pow(A, 5) / 120);
            // y
            double y = dy + k0 * (M + V * Math.Tan(lat) * (Math.Pow(A, 2) / 2 + (5 - T + 9 * C + 4 * Math.Pow(C, 2)) * Math.Pow(A, 4) / 24 + (61 - 58 * T + Math.Pow(T, 2) + 600 * C - 330 * e2) * Math.Pow(A, 6) / 720));

            TWD97 = x.ToString() + "," + y.ToString();
            return TWD97;
        }

        private static string Cal_TWD97_To_lonlat(double x, double y)
        {
            x -= dx;
            y -= dy;

            // Calculate the Meridional Arc
            double M = y / k0;

            // Calculate Footprint Latitude
            double mu = M / (a * (1.0 - e / 4.0 - 3 * Math.Pow(e, 2) / 64.0 - 5 * Math.Pow(e, 3) / 256.0));
            double e1 = (1.0 - Math.Sqrt(1.0 - e)) / (1.0 + Math.Sqrt(1.0 - e));

            double J1 = (3 * e1 / 2 - 27 * Math.Pow(e1, 3) / 32.0);
            double J2 = (21 * Math.Pow(e1, 2) / 16 - 55 * Math.Pow(e1, 4) / 32.0);
            double J3 = (151 * Math.Pow(e1, 3) / 96.0);
            double J4 = (1097 * Math.Pow(e1, 4) / 512.0);

            double fp = mu + J1 * Math.Sin(2 * mu) + J2 * Math.Sin(4 * mu) + J3 * Math.Sin(6 * mu) + J4 * Math.Sin(8 * mu);

            // Calculate Latitude and Longitude
            double C1 = e2 * Math.Pow(Math.Cos(fp), 2);
            double T1 = Math.Pow(Math.Tan(fp), 2);
            double R1 = a * (1 - e) / Math.Pow((1 - e * Math.Pow(Math.Sin(fp), 2)), (3.0 / 2.0));
            double N1 = a / Math.Pow((1 - e * Math.Pow(Math.Sin(fp), 2)), 0.5);

            double D = x / (N1 * k0);

            // 計算緯度
            double Q1 = N1 * Math.Tan(fp) / R1;
            double Q2 = (Math.Pow(D, 2) / 2.0);
            double Q3 = (5 + 3 * T1 + 10 * C1 - 4 * Math.Pow(C1, 2) - 9 * e2) * Math.Pow(D, 4) / 24.0;
            double Q4 = (61 + 90 * T1 + 298 * C1 + 45 * Math.Pow(T1, 2) - 3 * Math.Pow(C1, 2) - 252 * e2) * Math.Pow(D, 6) / 720.0;
            double lat = fp - Q1 * (Q2 - Q3 + Q4);

            // 計算經度
            double Q5 = D;
            double Q6 = (1 + 2 * T1 + C1) * Math.Pow(D, 3) / 6;
            double Q7 = (5 - 2 * C1 + 28 * T1 - 3 * Math.Pow(C1, 2) + 8 * e2 + 24 * Math.Pow(T1, 2)) * Math.Pow(D, 5) / 120.0;
            double lon = lon0 + (Q5 - Q6 + Q7) / Math.Cos(fp);

            lat = (lat * 180) / Math.PI; //緯度
            lon = (lon * 180) / Math.PI; //經度

            string lonlat = lon + "," + lat;
            return lonlat;
        }
    }
}
