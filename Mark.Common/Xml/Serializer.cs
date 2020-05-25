using System;

namespace Mark.Common
{
    public class Serializer
    {
        #region 常用方法

        /// <summary>
        /// 结构存在XML文件
        /// </summary>
        /// <param name="file">文件名称，不含路径。内部会存储在软件安装目录下</param>
        /// <param name="data">要存储的对象</param>
        public static void Pack(string file, object data)
        {
            _Serializer.Pack(file, data);
        }

        /// <summary>
        /// 结构存在XML文件
        /// </summary>
        /// <param name="file">文件名称，绝对路径含文件名</param>
        /// <param name="data">要存储的对象</param>
        public static void PackFullPathFile(string file, object data)
        {
            _Serializer.PackFullPathFile(file, data);
        }

        /// <summary>
        /// 从XML文件中读出对应的对象
        /// </summary>
        /// <param name="file">文件名称，不含路径。内自动从软件安装目录下读取</param>
        /// <param name="data">要读取的对象类型</param>
        public static object Unpack(string file, Type dataType)
        {
            return _Serializer.Unpack(file, dataType);
        }

        /// <summary>
        /// 从XML文件中读出对应的对象
        /// </summary>
        /// <param name="file">文件名称，绝对路径含文件名</param>
        /// <param name="data">要读取的对象类型</param>
        public static object UnpackFullPathFile(string file, Type dataType)
        {
            return _Serializer.UnpackFullPathFile(file, dataType);
        }

        /// <summary>
        /// Serialize data to binary and save in the xml file 
        /// 保存为二进制文件-----------------有BUG没解决，读出的东西跟版本有关系，兼容性不好
        /// Bug已解决，原因是因为做了混淆后，类名被混淆了，下次再混淆，产生的值不一样，故不能读取上次存储的值
        /// </summary>
        /// /// <param name="file">文件名称，不含路径。内部会存储在软件安装目录下</param>
        public static void SerializeToBinary(string file, object data)
        {
            _Serializer.SerializeToBinary(file, data);
        }

        /// <summary>
        /// Deserialize the XML binary file and load to data of the dataType
        /// 从二进制文件中读出---------------有BUG没解决，读出的东西跟版本有关系，兼容性不好
        /// Bug已解决，原因是因为做了混淆后，类名被混淆了，下次再混淆，产生的值不一样，故不能读取上次存储的值
        /// 如需要再次读取，解决方法为此类所在的DLL不混淆
        /// </summary>
        /// <param name="file">文件名称，不含路径。内自动从软件安装目录下读取</param>
        public static object DeserializeFromBinary(string file, Type dataType)
        {
            return _Serializer.DeserializeFromBinary(file, dataType);
        }

        #endregion

        /// <summary>
        /// Serialize data to buffer 
        /// </summary>
        /// <param name="source">the object to be packed</param>
        /// <returns>the buffer packed from object</returns>
        public static byte[] Pack(object source)
        {
            return _Serializer.Pack(source);
        }

        /// <summary>
        /// Serialize data to string 
        /// </summary>
        /// <param name="data">the object to be packed</param>
        /// <returns>string</returns>
        public static string PackToString(object data)
        {
            return _Serializer.PackToString(data);
        }

        /// <summary>
        /// Deserialize data from the buffer
        /// </summary>
        /// <param name="source">the source buffer to unpack</param>
        /// <param name="dataType">the type of object unpacked from the buffer</param>
        /// <returns>the object unpacked from the buffer</returns>
        public static object Unpack(byte[] source, Type dataType)
        {
            return _Serializer.Unpack(source, dataType);
        }

        /// <summary>
        /// Deserialize data from the string
        /// </summary>
        /// <param name="source">the source buffer to unpack</param>
        /// <param name="dataType">the type of object unpacked from the buffer</param>
        /// <returns>the object unpacked from the buffer</returns>
        public static object UnpackStr(string source, Type dataType)
        {
            return _Serializer.UnpackStr(source, dataType);
        }

        #region 2015-07-21新增加密的存取方法
        /// <summary>
        /// 把结构转成字符串并加密后保存为本地XML文件
        /// </summary>
        /// <param name="file">文件名称，不含路径。内部会存储在软件安装目录下</param>
        /// <param name="source">要存储的对象</param>
        public static void PackToEncryptFile(string file, object source)
        {
            _Serializer.PackToEncryptFile(file, source);
        }

        /// <summary>
        /// 把结构转成字符串并加密后保存为本地XML文件
        /// </summary>
        /// <param name="file">文件名称，绝对路径含文件名</param>
        /// <param name="data">要存储的对象</param>
        public static void PackToFullPathEncryptFile(string filePath, object source)
        {
            _Serializer.PackToFullPathEncryptFile(filePath, source);
        }

        /// <summary>
        /// 从XML文件中读出对应的对象
        /// </summary>
        /// <param name="file">文件名称，不含路径。内自动从软件安装目录下读取</param>
        /// <param name="dataType">要读取的对象类型</param>
        public static object UnpackFromEncryptFile(string file, Type dataType)
        {
            return _Serializer.UnpackFromEncryptFile(file, dataType);
        }

        /// <summary>
        /// 从XML文件中读出对应的对象
        /// </summary>
        /// <param name="file">文件绝对路径</param>
        /// <param name="dataType">要读取的对象类型</param>
        public static object UnpackFromFullPathEncryptFile(string file, Type dataType)
        {
            return _Serializer.UnpackFromFullPathEncryptFile(file, dataType);
        }
        #endregion

    }
}
