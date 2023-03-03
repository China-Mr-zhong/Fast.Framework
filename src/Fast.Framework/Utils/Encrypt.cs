using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Fast.Framework.Utils
{

    /// <summary>
    /// 加密工具类
    /// </summary>
    public static class Encrypt
    {

        /// <summary>
        /// DES 加密
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string DESEncrypt(string str, string key = "!@#$%^&*")
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (key.Length < 8)
            {
                throw new ArgumentException("key length 8");
            }
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(key.Substring(0, 8));
                byte[] rgbIV = rgbKey;
                byte[] strByte = Encoding.UTF8.GetBytes(str);
                DES provider = DES.Create();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, provider.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cryptoStream.Write(strByte, 0, strByte.Length);
                cryptoStream.FlushFinalBlock();
                return Convert.ToBase64String(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception("EncryptException:" + ex.Message);
            }
        }

        /// <summary>
        /// DES 解密
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string DESDecrypt(string str, string key = "!@#$%^&*")
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (key.Length < 8)
            {
                throw new ArgumentException("key length 8");
            }
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(key.Substring(0, 8));
                byte[] rgbIV = rgbKey;
                byte[] strByte = Convert.FromBase64String(str);
                DES provider = DES.Create();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, provider.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cryptoStream.Write(strByte, 0, strByte.Length);
                cryptoStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception("DecryptException:" + ex.Message);
            }
        }

        /// <summary>
        /// MD5 文本加密
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="encoding">字符编码</param>
        /// <returns>密文</returns>
        public static string MD5Encrypt(string str, Encoding encoding = null)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            MD5 provider = MD5.Create();
            byte[] data = provider.ComputeHash(encoding.GetBytes(str));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// MD5 文件流加密
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <returns>密文</returns>
        public static string MD5Encrypt(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            MD5 provider = MD5.Create();
            byte[] data = provider.ComputeHash(stream);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// 创建RSA公钥和私钥
        /// </summary>
        /// <returns>公钥和密钥</returns>
        public static Dictionary<string, string> CreateRSAKey()
        {
            RSA rsa = RSA.Create();
            return new Dictionary<string, string> { ["public"] = rsa.ToXmlString(false), ["private"] = rsa.ToXmlString(true) };
        }

        /// <summary>
        /// 写入RSA密钥
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <param name="directory">目录</param>
        /// <param name="encoding">编码</param>
        public static void WriteRSAKey(Dictionary<string, string> keyValues, string directory, Encoding encoding = null)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            if (encoding == null)
            {
                encoding = encoding ?? Encoding.UTF8;
            }
            using (StreamWriter sw = new StreamWriter(Path.Combine(directory, "Public.xml"), false, encoding))
            {
                sw.Write(keyValues["public"]);
            }
            using (StreamWriter sw = new StreamWriter(Path.Combine(directory, "Private.xml"), false, encoding))
            {
                sw.Write(keyValues["private"]);
            }
        }

        /// <summary>
        /// RAS 加密
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public static string RSAEncrypt(string str, string publicKey)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            if (string.IsNullOrWhiteSpace(publicKey))
            {
                throw new ArgumentNullException(nameof(publicKey));
            }
            try
            {
                RSA rsa = RSA.Create();
                rsa.FromXmlString(publicKey);
                var data = rsa.Encrypt(Encoding.UTF8.GetBytes(str), RSAEncryptionPadding.OaepSHA256);
                return Convert.ToBase64String(data);
            }
            catch (Exception ex)
            {
                throw new Exception("EncryptException:" + ex.Message);
            }
        }

        /// <summary>
        /// RSA 解密
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static string RSADecrypt(string str, string privateKey)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            if (string.IsNullOrWhiteSpace(privateKey))
            {
                throw new ArgumentNullException(nameof(privateKey));
            }
            try
            {
                RSA rsa = RSA.Create();
                rsa.FromXmlString(privateKey);
                var data = rsa.Decrypt(Convert.FromBase64String(str), RSAEncryptionPadding.OaepSHA256);
                return Encoding.UTF8.GetString(data);
            }
            catch (Exception ex)
            {
                throw new Exception("DecryptException:" + ex.Message);
            }
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static string RSASign(string data, string privateKey)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (string.IsNullOrWhiteSpace(privateKey))
            {
                throw new ArgumentNullException(nameof(privateKey));
            }
            try
            {
                RSA rsa = RSA.Create(2048);
                rsa.FromXmlString(privateKey);
                byte[] sign = rsa.SignData(Encoding.UTF8.GetBytes(data), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(sign);
            }
            catch (Exception ex)
            {
                throw new Exception("DecryptException:" + ex.Message);
            }
        }
    }
}
