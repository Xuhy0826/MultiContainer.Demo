using System;
using System.Collections;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;

namespace Mark.Common.Xml
{
    /// <summary>
    /// 压缩工具类
    /// </summary>
    public class CompressUtils
    {
        #region Bytes

        /// <summary>
        /// 字节数组压缩
        /// </summary>
        /// <param name="input">字节数组</param>
        /// <returns>压缩后的字节数组</returns>
        public static byte[] CompressBytes(byte[] input)
        {
            byte[] rb = null;
            using (var ms = new MemoryStream())
            {
                using (var gZip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    gZip.Write(input, 0, input.Length);
                    gZip.Flush();
                    gZip.Close();
                }
                ms.Position = 0;
                rb = new byte[ms.Length];
                ms.Read(rb, 0, (int)ms.Length);
                ms.Close();
            }
            return rb;
        }

        /// <summary>
        /// 字节数组解压缩
        /// </summary>
        /// <param name="input">字节数组</param>
        /// <returns>解压缩后的字节数组</returns>
        public static byte[] DecompressBytes(byte[] input)
        {
            try
            {
                long totalLength = 0;
                int size = 0;
                byte[] db;
                using (var ms = new MemoryStream())
                {
                    using (var msD = new MemoryStream())
                    {
                        ms.Write(input, 0, input.Length);
                        ms.Seek(0, SeekOrigin.Begin);
                        using (var zip = new GZipStream(ms, CompressionMode.Decompress))
                        {
                            bool readed = true;
                            while (true)
                            {
                                size = zip.ReadByte();
                                if (size != -1)
                                {
                                    if (!readed) readed = true;
                                    totalLength++;
                                    msD.WriteByte((byte)size);
                                }
                                else
                                {
                                    if (readed) break;
                                }
                            }
                            zip.Close();
                        }
                        db = msD.ToArray();
                        msD.Close();
                    }
                }
                return db;
            }
            catch
            {
                return SharpZipLibUtils.DecompressBytes(input);
            }
        }

        #endregion

        #region Folder

        /// <summary>
        /// 文件夹压缩
        /// </summary>
        /// <param name="folderPath">目标文件夹</param>
        /// <returns>压缩后的字节数组</returns>
        public static byte[] CompressFolder(string folderPath)
        {
            ArrayList list = new ArrayList();
            BuildDir(Path.GetDirectoryName(folderPath), folderPath, list);
            IFormatter formatter = new BinaryFormatter();
            using (Stream s = new MemoryStream())
            {
                formatter.Serialize(s, list);
                s.Position = 0;
                return CreateCompressFile(s);
            }
        }

        /// <summary>
        /// 文件夹解压缩
        /// </summary>
        /// <param name="input">字节数组</param>
        /// <param name="folderPath">解压缩目标文件夹</param>
        public static void DeCompressFolder(byte[] input, string folderPath)
        {
            try
            {
                using (MemoryStream source = new MemoryStream(input))
                {
                    using (Stream destination = new MemoryStream())
                    {
                        using (GZipStream gzipStream = new GZipStream(source, CompressionMode.Decompress, true))
                        {
                            byte[] bytes = new byte[4096];
                            int n;
                            while ((n = gzipStream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                destination.Write(bytes, 0, n);
                            }
                        }
                        destination.Flush();
                        destination.Position = 0;
                        DeSerializeFiles(destination, folderPath);
                    }
                }
            }
            catch
            {
                SharpZipLibUtils.DecompressFolder(input, folderPath);
            }
        }

        #endregion

        #region DataSet

        /// <summary>
        /// DataSet压缩
        /// </summary>
        /// <param name="input">DataSet</param>
        /// <returns>压缩后的字节数组</returns>
        public static byte[] CompressDataSet(DataSet input)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, input);
                return CompressBytes(ms.GetBuffer());
            }
        }

        /// <summary>
        /// DataSet解压缩
        /// </summary>
        /// <param name="input">字节数组</param>
        /// <returns>DataSet</returns>
        public static DataSet DecompressDataSet(byte[] input)
        {
            var bf = new BinaryFormatter();
            byte[] buffer = DecompressBytes(input);
            using (var ms = new MemoryStream(buffer))
            {
                return (DataSet)bf.Deserialize(ms);
            }
        }

        #endregion

        #region File

        /// <summary>
        /// 文件压缩
        /// </summary>
        /// <param name="filePath">目标文件路径</param>
        /// <returns>压缩后的字节数组</returns>
        public static byte[] CompressFile(string filePath)
        {
            using (var fs = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read))
            using (var r = new BinaryReader(fs))
            {
                var buf = new byte[fs.Length];
                r.Read(buf, 0, (int)fs.Length);

                return CompressBytes(buf);
            }
        }

        /// <summary>
        /// 文件解压缩
        /// </summary>
        /// <param name="input">字节数组</param>
        /// <param name="filePath">目标文件路径</param>
        public static void DecompressFile(byte[] input, string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var fs = new FileStream(
                filePath,
                FileMode.CreateNew,
                FileAccess.Write))
            using (var w = new BinaryWriter(fs))
            {
                byte[] buf = DecompressBytes(input);
                w.Write(buf);
            }
        }

        #endregion

        #region Files

        /// <summary>
        /// 多文件压缩
        /// </summary>
        /// <param name="filePaths">多个文件路径</param>
        /// <returns>压缩后的字节数组</returns>
        public static byte[] CompressFiles(string[] filePaths)
        {
            ArrayList list = new ArrayList();
            foreach (var filePath in filePaths)
            {
                byte[] destBuffer = File.ReadAllBytes(filePath);
                SerializeFileInfo sfi = new SerializeFileInfo(Path.GetFileName(filePath), destBuffer);
                list.Add(sfi);
            }
            IFormatter formatter = new BinaryFormatter();
            using (Stream s = new MemoryStream())
            {
                formatter.Serialize(s, list);
                s.Position = 0;
                return CreateCompressFile(s);
            }
        }

        /// <summary>
        /// 多文件解压缩
        /// </summary>
        /// <param name="input">字节数组</param>
        /// <param name="folderPath">解压缩目标文件夹</param>
        public static void DecompressFiles(byte[] input, string folderPath)
        {
            DeCompressFolder(input, folderPath);
        }

        #endregion

        #region String

        /// <summary>
        /// 字符串压缩
        /// </summary>
        /// <param name="input">待压缩字符串</param>
        /// <returns>字节数组</returns>
        public static byte[] CompressString(string input)
        {
            return CompressBytes(Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        /// 字符串解压缩
        /// </summary>
        /// <param name="input">字节数组</param>
        /// <returns>字符串</returns>
        public static string DecompressString(byte[] input)
        {
            return Encoding.UTF8.GetString(DecompressBytes(input));
        }

        #endregion

        #region Xml Document

        /// <summary>
        /// XML文档压缩
        /// </summary>
        /// <param name="input">XML文档对象</param>
        /// <returns>字节数组</returns>
        public static byte[] CompressXmlDocument(XmlDocument input)
        {
            return CompressString(input.InnerXml);
        }

        /// <summary>
        /// XML文档解压缩
        /// </summary>
        /// <param name="input">字节数组</param>
        /// <returns>XML文档对象</returns>
        public static XmlDocument DecompressXmlDocument(byte[] input)
        {
            var doc = new XmlDocument();
            doc.LoadXml(DecompressString(input));
            return doc;
        }

        #endregion

        #region Private Methods

        private static void BuildDir(string rootPath, string dirPath, ArrayList list)
        {
            SerializeFileInfo sdi = new SerializeFileInfo(GetRelativeDir(rootPath, dirPath));
            list.Add(sdi);
            foreach (string f in Directory.GetFiles(dirPath))
            {
                byte[] destBuffer = File.ReadAllBytes(f);
                SerializeFileInfo sfi = new SerializeFileInfo(GetRelativeDir(rootPath, f), destBuffer);
                list.Add(sfi);
            }
            foreach (string d in Directory.GetDirectories(dirPath))
            {
                BuildDir(rootPath, d, list);
            }
        }

        private static string GetRelativeDir(string rootPath, string dirPath)
        {
            return dirPath.Replace(rootPath, string.Empty);
        }

        private static void DeSerializeFiles(Stream s, string dirPath)
        {
            BinaryFormatter b = new BinaryFormatter();
            ArrayList list = (ArrayList)b.Deserialize(s);

            foreach (SerializeFileInfo f in list)
            {
                if (f.FileBuffer == null)
                {
                    string tarDir = Path.Combine(dirPath, f.Name);
                    if (!Directory.Exists(tarDir))
                    {
                        Directory.CreateDirectory(tarDir);
                    }
                }
                else
                {
                    string newName = Path.Combine(dirPath, f.Name);
                    using (FileStream fs = new FileStream(newName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(f.FileBuffer, 0, f.FileBuffer.Length);
                        fs.Close();
                    }
                }
            }
        }

        private static byte[] CreateCompressFile(Stream source)
        {
            using (MemoryStream destination = new MemoryStream())
            {
                using (GZipStream output = new GZipStream(destination, CompressionMode.Compress))
                {
                    byte[] bytes = new byte[4096];
                    int n;
                    while ((n = source.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        output.Write(bytes, 0, n);
                    }
                }
                return destination.ToArray();
            }
        }

        #endregion

        #region SerializeFileInfo

        [Serializable]
        class SerializeFileInfo
        {
            private string name;
            private byte[] fileBuffer;

            public SerializeFileInfo(string name)
            {
                this.name = name;
            }

            public SerializeFileInfo(string name, byte[] buffer)
            {
                this.name = name;
                this.fileBuffer = buffer;
            }

            public string Name
            {
                get
                {
                    return name;
                }
            }

            public byte[] FileBuffer
            {
                get
                {
                    return fileBuffer;
                }
            }
        }

        #endregion
    }
}