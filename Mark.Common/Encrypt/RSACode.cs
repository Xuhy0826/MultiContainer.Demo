using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mark.Common.Encrypt
{
    public class RSACode
    {
        /// <summary>
        /// 创建RSA公钥和私钥
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static bool CreateKey(out string publicKey, out string privateKey)
    {
        publicKey = null;
        privateKey = null;
        try
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
#if RSAXML
                    privateKey = rsa.ToXmlString(true);
                    publicKey = rsa.ToXmlString(false);
#else
                byte[] publicKeyBytes = rsa.ExportCspBlob(false);
                byte[] privateKeyBytes = rsa.ExportCspBlob(true);
                publicKey = Convert.ToBase64String(publicKeyBytes);
                privateKey = Convert.ToBase64String(privateKeyBytes);
#endif
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// RSA加密
    /// </summary>
    /// <param name="publickey"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string Encrypt(string publickey, string content)
    {
        if (string.IsNullOrEmpty(content))
            return null;
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            byte[] cipherbytes;
#if RSAXML
                rsa.FromXmlString(publickey);
#else
            byte[] keyBytes = Convert.FromBase64String(publickey);
            rsa.ImportCspBlob(keyBytes);
#endif
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);
            return Convert.ToBase64String(cipherbytes);
        }
    }

    /// <summary>
    /// RSA加密
    /// </summary>
    /// <param name="publickey"></param>
    /// <param name="dt"></param>
    /// <param name="columnIndexs"></param>
    /// <returns></returns>
    public static DataTable Encrypt(string publickey, DataTable dt, int[] columnIndexs)
    {
        if (dt == null)
            return null;
        DataTable result = dt.Clone();
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
#if RSAXML
                rsa.FromXmlString(publickey);
#else
            byte[] keyBytes = Convert.FromBase64String(publickey);
            rsa.ImportCspBlob(keyBytes);
#endif
            foreach (DataRow dr in dt.Rows)
            {
                object[] objs = dr.ItemArray;
                foreach (int index in columnIndexs)
                {
                    if (objs[index] != null && objs[index] != DBNull.Value)
                    {
                        byte[] bytes = rsa.Encrypt(Encoding.UTF8.GetBytes(objs[index].ToString()), false);
                        objs[index] = Convert.ToBase64String(bytes);
                    }
                }
                result.Rows.Add(objs);
            }
        }
        return result;
    }

    /// <summary>
    /// RSA解密
    /// </summary>
    /// <param name="privatekey"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string Decrypt(string privatekey, string content)
    {
        if (string.IsNullOrEmpty(content))
            return null;
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            byte[] cipherbytes;
#if RSAXML
                rsa.FromXmlString(privatekey);
#else
            byte[] keyBytes = Convert.FromBase64String(privatekey);
            rsa.ImportCspBlob(keyBytes);
#endif
            cipherbytes = rsa.Decrypt(Convert.FromBase64String(content), false);
            return Encoding.UTF8.GetString(cipherbytes);
        }
    }

    /// <summary>
    ///  RSA解密
    /// </summary>
    /// <param name="privatekey"></param>
    /// <param name="dt"></param>
    /// <param name="columnIndexs"></param>
    /// <returns></returns>
    public static DataTable Decrypt(string privatekey, DataTable dt, int[] columnIndexs)
    {
        if (dt == null)
            return null;
        DataTable result = dt.Clone();
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
#if RSAXML
                rsa.FromXmlString(privatekey);
#else
            byte[] keyBytes = Convert.FromBase64String(privatekey);
            rsa.ImportCspBlob(keyBytes);
#endif
            foreach (DataRow dr in dt.Rows)
            {
                object[] objs = dr.ItemArray;
                foreach (int index in columnIndexs)
                {
                    if (objs[index] != null && objs[index] != DBNull.Value)
                    {
                        byte[] bytes = rsa.Decrypt(Convert.FromBase64String(objs[index].ToString()), false);
                        objs[index] = Encoding.UTF8.GetString(bytes);
                    }
                }
                result.Rows.Add(objs);
            }
        }
        return result;
    }

    //public static int Encrypt(string publickey, string src, string dest, int[] columns, Predicate<string> action)
    //{
    //    return Encrypt(publickey, src, dest, true, columns, action);
    //}

//    public static int Encrypt(string publickey, string src, string dest, bool hasHeaders, int[] columns, Predicate<string> action)
//    {
//        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
//        {
//#if RSAXML
//                rsa.FromXmlString(publickey);
//#else
//            byte[] keyBytes = Convert.FromBase64String(publickey);
//            rsa.ImportCspBlob(keyBytes);
//#endif
//            using (TextReader reader = new StreamReader(src, Encoding.Default))
//            {
//                using (TextWriter writer = new StreamWriter(dest, false, Encoding.Default))
//                {
//                    CsvReader _reader = new CsvReader(reader, hasHeaders);
//                    if (hasHeaders)
//                        writer.WriteLine(string.Join(",", _reader.GetFieldHeaders()));
//                    int rowIndex = 0;
//                    while (_reader.ReadNextRecord())
//                    {
//                        if (rowIndex > 0 && rowIndex % 100 == 0 && action != null)
//                        {
//                            if (!action(string.Format("正在处理第{0}行...", rowIndex)))
//                                break;
//                        }
//                        string[] objs = new string[_reader.FieldCount];
//                        for (int index = 0; index < objs.Length; index++)
//                        {
//                            if (_reader[index] != null && Array.Exists(columns, (column) => { return Convert.ToInt32(column) == index; }))
//                            {
//                                byte[] bytes = rsa.Encrypt(Encoding.UTF8.GetBytes(_reader[index].ToString()), false);
//                                objs[index] = Convert.ToBase64String(bytes);
//                            }
//                            else
//                                objs[index] = _reader[index];
//                        }
//                        writer.WriteLine(string.Join(",", objs));
//                        rowIndex++;
//                    }
//                    reader.Close();
//                    writer.Close();
//                    return rowIndex;
//                }
//            }
//        }
//    }

    //public static void Encrypt(string publickey, string src, string dest, int[] columns)
    //{
    //    Encrypt(publickey, src, dest, columns, null);
    //}

    //public static int Decrypt(string privatekey, string src, string dest, int[] columns, Predicate<string> action)
    //{
    //    return Decrypt(privatekey, src, dest, true, columns, action);
    //}

//    public static int Decrypt(string privatekey, string src, string dest, bool hasHeaders, int[] columns, Predicate<string> action)
//    {
//        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
//        {
//#if RSAXML
//                rsa.FromXmlString(privatekey);
//#else
//            byte[] keyBytes = Convert.FromBase64String(privatekey);
//            rsa.ImportCspBlob(keyBytes);
//#endif
//            using (TextReader reader = new StreamReader(src, Encoding.Default))
//            {
//                using (TextWriter writer = new StreamWriter(dest, false, Encoding.Default))
//                {
//                    CsvReader _reader = new CsvReader(reader, hasHeaders);
//                    if (hasHeaders)
//                        writer.WriteLine(string.Join(",", _reader.GetFieldHeaders()));
//                    int rowIndex = 0;
//                    while (_reader.ReadNextRecord())
//                    {
//                        if (rowIndex > 0 && rowIndex % 100 == 0 && action != null)
//                        {
//                            if (!action(string.Format("正在处理第{0}行...", rowIndex)))
//                                break;
//                        }
//                        string[] objs = new string[_reader.FieldCount];
//                        for (int index = 0; index < objs.Length; index++)
//                        {
//                            if (_reader[index] != null && Array.Exists(columns, (column) => { return Convert.ToInt32(column) == index; }))
//                            {
//                                byte[] bytes = rsa.Decrypt(Convert.FromBase64String(_reader[index].ToString()), false);
//                                objs[index] = Encoding.UTF8.GetString(bytes);
//                            }
//                            else
//                                objs[index] = _reader[index];
//                        }
//                        writer.WriteLine(string.Join(",", objs));
//                        rowIndex++;
//                    }
//                    reader.Close();
//                    writer.Close();
//                    return rowIndex;
//                }
//            }
//        }
//    }

    //public static void Decrypt(string privatekey, string src, string dest, int[] columns)
    //{
    //    Decrypt(privatekey, src, dest, columns, null);
    //}
}
}
