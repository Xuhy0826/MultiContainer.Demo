using System;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace Mark.Common
{
    class _Serializer
    {
        #region 常用方法

        /// <summary>
        /// Serialize data and save in the xml file
        /// </summary>
        public static void Pack(string file, object data)
        {
            TextWriter tw = null;
            try
            {
                Type dataType = data.GetType();
                XmlSerializer serializer = new XmlSerializer(dataType);
                tw = new StreamWriter(CombineFileName(file), false, Encoding.Unicode);
                serializer.Serialize(tw, data);
            }
            catch
            {
                //LogPrinter.WriteWithDate(string.Format("[Utilities][Serialize]保存为文件{0}时发生异常\r\n",
                //    CombineFileName(file)));
            }
            finally
            {
                if (tw != null)
                {
                    tw.Close();
                }
            }
        }

        /// <summary>
        /// Serialize data and save in the xml file
        /// </summary>
        public static void PackFullPathFile(string file, object data)
        {
            TextWriter tw = null;
            try
            {
                Type dataType = data.GetType();
                XmlSerializer serializer =
                    new XmlSerializer(dataType);
                tw =
                   new StreamWriter(file, false, Encoding.Unicode);
                serializer.Serialize(tw, data);
            }
            catch
            {
                //LogPrinter.WriteWithDate(string.Format("[Utilities][Serialize]保存为文件{0}时发生异常\r\n",
                //    CombineFileName(file)));
            }
            finally
            {
                if (tw != null)
                {
                    tw.Close();
                }
            }
        }

        /// <summary>
        /// Deserialize the XML file and load to data of the dataType
        /// </summary>
        public static object Unpack(string file, Type dataType)
        {
            object data = null;
            TextReader tr = null;
            try
            {
                XmlSerializer serializer =
                    new XmlSerializer(dataType);

                string full = CombineFileName(file);
                tr = new StreamReader(full);

                data = serializer.Deserialize(tr);

            }
            catch
            {
                // throw (e);
            }
            finally
            {
                if (tr != null)
                {
                    tr.Close();
                }
            }

            return data;
        }

        /// <summary>
        /// Deserialize the XML file and load to data of the dataType 
        /// by file full name
        /// </summary>
        public static object UnpackFullPathFile(string file, Type dataType)
        {
            object data = null;
            TextReader tr = null;
            try
            {
                XmlSerializer serializer =
                    new XmlSerializer(dataType);

                tr = new StreamReader(file);

                data = serializer.Deserialize(tr);
            }
            catch (Exception)
            {
                // throw (e);
            }
            finally
            {
                if (tr != null)
                {
                    tr.Close();
                }
            }
            return data;
        }

        /// <summary>
        /// Serialize data to binary and save in the xml file 
        /// 保存为二进制文件-----------------有BUG没解决，读出的东西跟版本有关系，兼容性不好
        /// Bug已解决，原因是因为做了混淆后，类名被混淆了，下次再混淆，产生的值不一样，故不能读取上次存储的值
        /// </summary>
        public static void SerializeToBinary(string file, object data)
        {
            FileStream stream = null;
            try
            {
                BinaryFormatter binFmater = new BinaryFormatter();
                stream = new FileStream(
                    CombineFileName(file), FileMode.OpenOrCreate);

                binFmater.Serialize(stream, data);
                stream.Close();
            }
            catch
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserialize the XML binary file and load to data of the dataType
        /// 从二进制文件中读出---------------有BUG没解决，读出的东西跟版本有关系，兼容性不好
        /// Bug已解决，原因是因为做了混淆后，类名被混淆了，下次再混淆，产生的值不一样，故不能读取上次存储的值
        /// 如需要再次读取，解决方法为此类所在的DLL不混淆
        /// </summary>
        public static object DeserializeFromBinary(string file, Type dataType)
        {
            object data = null;
            Stream stream = null;
            try
            {
                IFormatter seri = new BinaryFormatter();
                stream = new FileStream(
                    CombineFileName(file), FileMode.Open, FileAccess.Read);

                object c = seri.Deserialize(stream);
                stream.Close();
                return c;
            }
            catch
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
            return data;
        }

        #endregion

        /// <summary>
        /// Serialize data to buffer 
        /// </summary>
        /// <param name="source">the object to be packed</param>
        /// <returns>the buffer packed from object</returns>
        public static byte[] Pack(object source)
        {
            Type dataType = source.GetType();
            XmlSerializer serializer = new XmlSerializer(dataType);

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, source);
                return stream.GetBuffer();
            }
        }

        /// <summary>
        /// Serialize data to string 
        /// </summary>
        /// <param name="data">the object to be packed</param>
        /// <returns>string</returns>
        public static string PackToString(object data)
        {
            string returnStr = string.Empty;
            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    XmlSerializer serializer = new XmlSerializer(data.GetType());
                    XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
                    xmlns.Add(String.Empty, String.Empty);
                    serializer.Serialize(sw, data, xmlns);
                    returnStr = sw.ToString();
                }
            }
            catch
            {

            }
            return returnStr;
        }

        /// <summary>
        /// Deserialize data from the buffer
        /// </summary>
        /// <param name="source">the source buffer to unpack</param>
        /// <param name="dataType">the type of object unpacked from the buffer</param>
        /// <returns>the object unpacked from the buffer</returns>
        public static object Unpack(byte[] source, Type dataType)
        {
            object destination = null;
            XmlSerializer serializer = new XmlSerializer(dataType);
            using (MemoryStream stream = new MemoryStream(source))
            {
                try
                {
                    destination = serializer.Deserialize(stream);
                }
                catch { }
            }
            return destination;
        }

        /// <summary>
        /// Deserialize data from the string
        /// </summary>
        /// <param name="source">the source buffer to unpack</param>
        /// <param name="dataType">the type of object unpacked from the buffer</param>
        /// <returns>the object unpacked from the buffer</returns>
        public static object UnpackStr(string source, Type dataType)
        {
            //byte[] bytes = Encoding.Default.GetBytes(source);

            //object destination = Unpack(bytes, dataType);

            //return destination;
            try
            {
                using (StringReader sr = new StringReader(source))
                {
                    XmlSerializer serializer = new XmlSerializer(dataType);
                    //XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
                    //xmlns.Add(String.Empty, String.Empty);
                    //serializer.Serialize(sr, data, xmlns);
                    return serializer.Deserialize(sr);
                    //returnStr = sw.ToString();
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 把结构转成字符串并加密后保存为本地XML文件
        /// </summary>
        /// <param name="file">文件名称，不含路径。内部会存储在软件安装目录下</param>
        /// <param name="source">要存储的对象</param>
        public static void PackToEncryptFile(string file, object source)
        {
            try
            {
                string filePath = CombineFileName(file);
                string str = PackToString(source);
                //加密字符串
                str = DESEncrypt.Encrypt(str);
                //转成二进制
                byte[] strbyte = System.Text.Encoding.Default.GetBytes(str);
                //保存xml文件
                File.WriteAllBytes(filePath, strbyte);
            }
            catch
            {

            }
        }

        /// <summary>
        /// 把结构转成字符串并加密后保存为本地XML文件
        /// </summary>
        /// <param name="file">文件名称，绝对路径含文件名</param>
        /// <param name="data">要存储的对象</param>
        public static void PackToFullPathEncryptFile(string path, object source)
        {
            try
            {
                string str = PackToString(source);
                //加密字符串
                str = DESEncrypt.Encrypt(str);
                //转成二进制
                byte[] strbyte = System.Text.Encoding.Default.GetBytes(str);
                //保存xml文件
                File.WriteAllBytes(path, strbyte);
            }
            catch
            {

            }
        }

        /// <summary>
        /// 从XML文件中读出对应的对象
        /// </summary>
        /// <param name="file">文件名称，不含路径。内自动从软件安装目录下读取</param>
        /// <param name="dataType">要读取的对象类型</param>
        public static object UnpackFromEncryptFile(string file, Type dataType)
        {
            string path = CombineFileName(file);
            if (File.Exists(path))
            {
                try
                {
                    byte[] strbyte = File.ReadAllBytes(path);
                    //解密字符串
                    string str = DESEncrypt.Decrypt(System.Text.Encoding.Default.GetString(strbyte));
                    //反序列化
                    return UnpackStr(str, dataType);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 从XML文件中读出对应的对象
        /// </summary>
        /// <param name="file">文件绝对路径</param>
        /// <param name="dataType">要读取的对象类型</param>
        public static object UnpackFromFullPathEncryptFile(string path, Type dataType)
        {
            if (File.Exists(path))
            {
                try
                {
                    byte[] strbyte = File.ReadAllBytes(path);
                    //解密字符串
                    string str = DESEncrypt.Decrypt(System.Text.Encoding.Default.GetString(strbyte));
                    //反序列化
                    return UnpackStr(str, dataType);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private static string CombineFileName(string fileName)
        {
            return string.Format(XmlFileFormat, AppDomain.CurrentDomain.BaseDirectory, fileName);
        }

        const string XmlFileFormat = @"{0}{1}";
    }

    /// <summary>
    /// DES加密/解密类。
    /// Copyright (C) Maticsoft
    /// </summary>
    public class DESEncrypt
    {
        public DESEncrypt()
        {
        }

        #region ========加密========

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Encrypt(string Text)
        {
            return Encrypt(Text, "DIONEDUEF");
        }
        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Encrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            using (var md5 = MD5.Create())
            {
                des.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(sKey));
                des.IV = md5.ComputeHash(Encoding.UTF8.GetBytes(sKey));
            }
            //des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            //des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        #endregion

        #region ========解密========

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Decrypt(string Text)
        {
            return Decrypt(Text, "DIONEDUEF");
        }
        /// <summary> 
        /// 解密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Decrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }

            using (var md5 = MD5.Create())
            {
                des.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(sKey));
                des.IV = md5.ComputeHash(Encoding.UTF8.GetBytes(sKey));
            }
            //des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            //des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        #endregion
    }
}
