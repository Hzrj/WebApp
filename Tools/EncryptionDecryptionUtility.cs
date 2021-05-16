using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace WebApp.Tools
{
    public class EncryptionDecryptionUtility
    {
        //ctor 快速生成构造方法
        //public static string Encryption(string encryptionStr)
        //{
        //    var bytes = Encoding.UTF8.GetBytes(encryptionStr);

        //    System.Security.Cryptography.MD5CryptoServiceProvider check;
        //    check = new System.Security.Cryptography.MD5CryptoServiceProvider();
        //    byte[] somme = check.ComputeHash(bytes);
        //    string ret = "";
        //    foreach (byte a in somme)
        //    {
        //        if (a < 16)
        //            ret += "0" + a.ToString("X");
        //        else
        //            ret += a.ToString("X");
        //    }
        //    DecryptionUtility(ret);
        //    return ret.ToLower();
        //    //原文链接：https://blog.csdn.net/pan_junbiao/article/details/19477895
        //    //return Convert.ToBase64String("");
        //    //return "";
        //}
        //public static string  DecryptionUtility(string decryptionutility)
        //{

        //    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //    byte[] inputByteArray = new byte[decryptionutility.Length / 2];
        //    for (int x = 0; x < decryptionutility.Length / 2; x++)
        //    {
        //        int i = (Convert.ToInt32(decryptionutility.Substring(x * 2, 2), 16));
        //        inputByteArray[x] = (byte)i;
        //    }

        //    //des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
        //    //des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
        //    MemoryStream ms = new MemoryStream();
        //    CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
        //    cs.Write(inputByteArray, 0, inputByteArray.Length);
        //    cs.FlushFinalBlock();
        //    StringBuilder ret = new StringBuilder();
        //    return System.Text.Encoding.Default.GetString(ms.ToArray());

        //    //原文链接：https://blog.csdn.net/z434418000z/article/details/76417671
        //}





        //--------------- 因为加密个解密都需要用到key所有在加密的后需要把key和加密码都存到数据库中



        /// <summary>
        /// 唯一加密方式
        /// </summary>
        /// <param name="texts"></param>
        /// <returns></returns>
        public static string WeiJiaMiGuid(string texts)
        {
            string Keys = GenerateKey();
            return MD5Encrypt(texts, Keys) + "=" + Keys;      //这里我把要加密的字符串和生成的key给拼接起来，这样我在调用 WeiJiaMiGuid方法是只需要传文本框text值就可以了；
        }

 

//------------------------取出加密时存在数据库的加密码和key


        /// <summary>
        /// 唯一解密方式
        /// </summary>
        /// <param name="texts"></param>
        /// <returns></returns>
        //public static string WeiYiJieMiGuid(string texts)
        //{
        //    string[] pwa = texts.Split(new char[] { '=' });   //分割一下    然后调解密
        //    //return Commonality.CommGUID.MD5Decrypt(pwa[0], pwa[1]);

        //}



        /// <summary>
        /// 创建Key
        /// </summary>
        /// <returns></returns>
        public static string GenerateKey()
        {
            DESCryptoServiceProvider desCrypto = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();
            return ASCIIEncoding.ASCII.GetString(desCrypto.Key);
        }





        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="pToEncrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string MD5Encrypt(string pToEncrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            //des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            //des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        /// <summary>
        /// MD5解密
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string MD5Decrypt(string pToDecrypt, string sKey="9")
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            //des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            //des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            //cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();

            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }
        //https://blog.csdn.net/weixin_33957648/article/details/86001732?utm_term=c#md5%E8%A7%A3%E5%AF%86%E6%96%B9%E6%B3%95&utm_medium=distribute.pc_aggpage_search_result.none-task-blog-2~all~sobaiduweb~default-1-86001732&spm=3001.4430
    }
}
