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
    public class AESCode
    {
        public string Key { get; set; }

        public string Encrypt(string val)
        {
            if (string.IsNullOrEmpty(val))
                return null;
#if CSP
            using (AesCryptoServiceProvider des = new AesCryptoServiceProvider())
#else
            using (AesManaged des = new AesManaged())
#endif
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(val);
                byte[] _key;
                byte[] _iv;
                GeneralKeyIV(this.Key, out _key, out _iv);
                des.Key = _key;
                des.IV = _iv;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        byte[] bytes = (byte[])ms.ToArray();
                        return Convert.ToBase64String(bytes);
                    }
                }
            }
        }

        public string Decrypt(string val)
        {
            if (string.IsNullOrEmpty(val))
                return null;
#if CSP
            using (AesCryptoServiceProvider des = new AesCryptoServiceProvider())
#else
            using (AesManaged des = new AesManaged())
#endif
            {
                byte[] inputByteArray = Convert.FromBase64String(val);
                byte[] _key;
                byte[] _iv;
                GeneralKeyIV(this.Key, out _key, out _iv);
                des.Key = _key;
                des.IV = _iv;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }

        public DataTable Encrypt(DataTable dt, int[] columnIndexs)
        {
            if (dt == null)
                return null;
            DataTable result = dt.Clone();
#if CSP
            using (AesCryptoServiceProvider des = new AesCryptoServiceProvider())
#else
            using (AesManaged des = new AesManaged())
#endif
            {
                byte[] _key;
                byte[] _iv;
                GeneralKeyIV(this.Key, out _key, out _iv);
                des.Key = _key;
                des.IV = _iv;
                ICryptoTransform transform = des.CreateEncryptor();
                foreach (DataRow dr in dt.Rows)
                {
                    object[] objs = dr.ItemArray;
                    foreach (int index in columnIndexs)
                    {
                        if (objs[index] != null && objs[index] != DBNull.Value)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                                {
                                    byte[] src = Encoding.UTF8.GetBytes(objs[index].ToString());
                                    if (src.Length == 0)
                                        continue;
                                    cs.Write(src, 0, src.Length);
                                    cs.FlushFinalBlock();
                                    byte[] bytes = (byte[])ms.ToArray();
                                    objs[index] = Convert.ToBase64String(bytes);
                                }
                            }
                        }
                    }
                    result.Rows.Add(objs);
                }
            }
            return result;
        }

        public DataTable Decrypt(DataTable dt, int[] columnIndexs)
        {
            if (dt == null)
                return null;
            DataTable result = dt.Clone();
#if CSP
            using (AesCryptoServiceProvider des = new AesCryptoServiceProvider())
#else
            using (AesManaged des = new AesManaged())
#endif
            {
                byte[] _key;
                byte[] _iv;
                GeneralKeyIV(this.Key, out _key, out _iv);
                des.Key = _key;
                des.IV = _iv;
                ICryptoTransform transform = des.CreateDecryptor();
                foreach (DataRow dr in dt.Rows)
                {
                    object[] objs = dr.ItemArray;
                    foreach (int index in columnIndexs)
                    {
                        if (objs[index] != null && objs[index] != DBNull.Value)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                                {
                                    byte[] src = Convert.FromBase64String(objs[index].ToString());
                                    if (src.Length == 0)
                                        continue;
                                    cs.Write(src, 0, src.Length);
                                    cs.FlushFinalBlock();
                                    objs[index] = Encoding.UTF8.GetString(ms.ToArray());
                                }
                            }
                        }
                    }
                    result.Rows.Add(objs);
                }
            }
            return result;
        }

        //public int Encrypt(string src, string dest, int[] columns, Predicate<string> action)
        //{
        //    return Encrypt(src, dest, true, columns, action);
        //}

        //public int Decrypt(string src, string dest, int[] columns, Predicate<string> action)
        //{
        //    return Decrypt(src, dest, true, columns, action);
        //}

//        public int Encrypt(string src, string dest, bool hasHeaders, int[] columns, Predicate<string> action)
//        {
//#if CSP
//            using (AesCryptoServiceProvider des = new AesCryptoServiceProvider())
//#else
//            using (AesManaged des = new AesManaged())
//#endif
//            {
//                byte[] _key;
//                byte[] _iv;
//                GeneralKeyIV(this.Key, out _key, out _iv);
//                des.Key = _key;
//                des.IV = _iv;
//                ICryptoTransform transform = des.CreateEncryptor();
//                using (TextReader reader = new StreamReader(src, Encoding.Default))
//                {
//                    using (TextWriter writer = new StreamWriter(dest, false, Encoding.Default))
//                    {
//                        CsvReader _reader = new CsvReader(reader, hasHeaders);
//                        if (hasHeaders)
//                            writer.WriteLine(string.Join(",", _reader.GetFieldHeaders()));
//                        int rowIndex = 0;
//                        while (_reader.ReadNextRecord())
//                        {
//                            if (rowIndex > 0 && rowIndex % 100 == 0 && action != null)
//                            {
//                                if (!action(string.Format("正在处理第{0}行...", rowIndex)))
//                                    break;
//                            }
//                            string[] objs = new string[_reader.FieldCount];
//                            for (int index = 0; index < objs.Length; index++)
//                            {
//                                if (_reader[index] != null && Array.Exists(columns, (column) => { return Convert.ToInt32(column) == index; }))
//                                {
//                                    using (MemoryStream ms = new MemoryStream())
//                                    {
//                                        using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
//                                        {
//                                            byte[] _bytes = Encoding.UTF8.GetBytes(_reader[index].ToString());
//                                            if (_bytes.Length == 0)
//                                                continue;
//                                            cs.Write(_bytes, 0, _bytes.Length);
//                                            cs.FlushFinalBlock();
//                                            byte[] bytes = (byte[])ms.ToArray();
//                                            objs[index] = Convert.ToBase64String(bytes);
//                                        }
//                                    }
//                                }
//                                else
//                                    objs[index] = _reader[index];
//                            }
//                            writer.WriteLine(string.Join(",", objs));
//                            rowIndex++;
//                        }
//                        reader.Close();
//                        writer.Close();
//                        return rowIndex;
//                    }
//                }
//            }
//        }

//        public int Decrypt(string src, string dest, bool hasHeaders, int[] columns, Predicate<string> action)
//        {

//#if CSP
//            using (AesCryptoServiceProvider des = new AesCryptoServiceProvider())
//#else
//            using (AesManaged des = new AesManaged())
//#endif
//            {
//                byte[] _key;
//                byte[] _iv;
//                GeneralKeyIV(this.Key, out _key, out _iv);
//                des.Key = _key;
//                des.IV = _iv;
//                ICryptoTransform transform = des.CreateDecryptor();
//                using (TextReader reader = new StreamReader(src, Encoding.Default))
//                {
//                    using (TextWriter writer = new StreamWriter(dest, false, Encoding.Default))
//                    {
//                        CsvReader _reader = new CsvReader(reader, hasHeaders);
//                        if (hasHeaders)
//                            writer.WriteLine(string.Join(",", _reader.GetFieldHeaders()));
//                        int rowIndex = 0;
//                        while (_reader.ReadNextRecord())
//                        {
//                            if (rowIndex > 0 && rowIndex % 100 == 0 && action != null)
//                            {
//                                if (!action(string.Format("正在处理第{0}行...", rowIndex)))
//                                    break;
//                            }
//                            string[] objs = new string[_reader.FieldCount];
//                            for (int index = 0; index < objs.Length; index++)
//                            {
//                                if (_reader[index] != null && Array.Exists(columns, (column) => { return Convert.ToInt32(column) == index; }))
//                                {
//                                    using (MemoryStream ms = new MemoryStream())
//                                    {
//                                        using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
//                                        {
//                                            byte[] _bytes = Convert.FromBase64String(_reader[index].ToString());
//                                            if (_bytes.Length == 0)
//                                                continue;
//                                            cs.Write(_bytes, 0, _bytes.Length);
//                                            cs.FlushFinalBlock();
//                                            objs[index] = Encoding.UTF8.GetString(ms.ToArray());
//                                        }
//                                    }
//                                }
//                                else
//                                    objs[index] = _reader[index];
//                            }
//                            writer.WriteLine(string.Join(",", objs));
//                            rowIndex++;
//                        }
//                        reader.Close();
//                        writer.Close();
//                        return rowIndex;
//                    }
//                }
//            }
//        }

        public void GeneralKeyIV(string keyStr, out byte[] key, out byte[] iv)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(keyStr);
            key = SHA256Managed.Create().ComputeHash(bytes);
            iv = MD5.Create().ComputeHash(bytes);
        }
    }
}
