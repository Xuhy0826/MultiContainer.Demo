using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Mark.Common.Helper;

namespace Mark.Common
{
    /// <summary>
    /// Xml工具类
    /// </summary>
    public class XmlHelper
    {
        private static string xmlfilePath = System.AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 将指定的xml转成实体类
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xmlName"></param>
        /// <returns></returns>
        public static object Read(Type type, string xmlName)
        {
            if (!File.Exists(xmlfilePath + @"\" + xmlName + ".xml"))
            {
                return null;
            }
            try
            {
                using (FileStream fs = new FileStream(xmlfilePath + @"\" + xmlName + ".xml", FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        string content = reader.ReadToEnd();
                        using (StringReader sr = new StringReader(content))
                        {
                            XmlSerializer xmldes = new XmlSerializer(type);
                            return xmldes.Deserialize(sr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("【读取xml文件时发生异常】", ex);
                return null;
            }
        }

        /// <summary>
        /// 将一个类型写入指定的xml
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xmlName"></param>
        /// <returns></returns>
        public static bool Write(Type type, object obj, string xmlName)
        {
            using (MemoryStream Stream = new MemoryStream())
            {
                //创建序列化对象
                XmlSerializer xml = new XmlSerializer(type);
                try
                {
                    //序列化对象
                    xml.Serialize(Stream, obj);
                    Stream.Position = 0;
                    using (StreamReader sr = new StreamReader(Stream))
                    {
                        string str = sr.ReadToEnd();
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter((xmlfilePath + @"\" + xmlName + ".xml"), false, Encoding.UTF8))
                        {
                            sw.Write(str);
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Logger.Error("【写入xml文件时发生异常】", ex);
                    return false;
                }
            }
            return true;
        }


        #region 反序列化
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns></returns>
        public static object Deserialize(Type type, string xml)
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(type);
                    return xmldes.Deserialize(sr);
                }
            }
            catch (Exception e)
            {
                Logger.Error("【读取xml文件时发生异常】", e);
                return null;
            }
        }


        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static object Deserialize(Type type, Stream stream)
        {
            XmlSerializer xmldes = new XmlSerializer(type);
            return xmldes.Deserialize(stream);
        }
        #endregion

        #region 序列化XML文件
        /// <summary>
        /// 序列化XML文件
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Serializer(Type type, object obj)
        {
            MemoryStream Stream = new MemoryStream();
            //创建序列化对象
            XmlSerializer xml = new XmlSerializer(type);
            try
            {
                //序列化对象
                xml.Serialize(Stream, obj);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            Stream.Position = 0;
            StreamReader sr = new StreamReader(Stream);
            string str = sr.ReadToEnd();
            Stream.Dispose();
            sr.Dispose();
            return str;
        }
        #endregion

        #region 获取对应XML节点的值
        /// <summary>
        /// 摘要:获取对应XML节点的值
        /// </summary>
        /// <param name="stringRoot">XML节点的标记</param>
        /// <returns>返回获取对应XML节点的值</returns>
        public static string XmlAnalysis(string stringRoot, string xml)
        {
            if (stringRoot.Equals("") == false)
            {
                try
                {
                    XmlDocument XmlLoad = new XmlDocument();
                    XmlLoad.LoadXml(xml);
                    return XmlLoad.DocumentElement.SelectSingleNode(stringRoot).InnerXml.Trim();
                }
                catch (Exception ex)
                {

                }
            }
            return "";
        }
        #endregion
    }
}
