using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace CommonModule.Logic
{
    public class DecryptAndEncryptLogic
    {
        #region DES加密字串
        /// <summary> DES 加密字串
        /// 
        /// </summary>   
        /// <span  name="original" class="mceItemParam"></span>原始字串</param>   
        /// <span  name="key" class="mceItemParam"></span>Key，長度必須為 8 個 ASCII 字元</param>   
        /// <span  name="iv" class="mceItemParam"></span>IV，長度必須為 8 個 ASCII 字元</param>   
        /// <returns></returns>   
        public string EncryptDES(string original, string key, string iv)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Key = Encoding.ASCII.GetBytes(key);
                des.IV = Encoding.ASCII.GetBytes(iv);
                byte[] s = Encoding.ASCII.GetBytes(original);
                ICryptoTransform desencrypt = des.CreateEncryptor();
                return BitConverter.ToString(desencrypt.TransformFinalBlock(s, 0, s.Length)).Replace("-", string.Empty);
            }
            catch { return original; }
        }
        #endregion

        #region DES 解密字串
        /// <summary> DES 解密字串
        /// 
        /// </summary>   
        /// <span  name="hexString" class="mceItemParam"></span>加密後 Hex String</param>   
        /// <span  name="key" class="mceItemParam"></span>Key，長度必須為 8 個 ASCII 字元</param>   
        /// <span  name="iv" class="mceItemParam"></span>IV，長度必須為 8 個 ASCII 字元</param>   
        /// <returns></returns>   
        public string DecryptDES(string hexString, string key, string iv)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Key = Encoding.ASCII.GetBytes(key);
                des.IV = Encoding.ASCII.GetBytes(iv);

                byte[] s = new byte[hexString.Length / 2];
                int j = 0;
                for (int i = 0; i < hexString.Length / 2; i++)
                {
                    s[i] = Byte.Parse(hexString[j].ToString() + hexString[j + 1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    j += 2;
                }
                ICryptoTransform desencrypt = des.CreateDecryptor();
                return Encoding.ASCII.GetString(desencrypt.TransformFinalBlock(s, 0, s.Length));
            }
            catch { return hexString; }
        }
        #endregion
    }
}
