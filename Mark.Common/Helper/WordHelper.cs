using System;
using System.IO;

namespace Mark.Common.Word
{
    public class WordHelper
    {
        /// <summary>
        /// 二进制数据转换为word文件
        /// </summary>
        /// <param name="data">二进制数据</param>
        /// <param name="fileName">word文件名</param>
        /// <returns>word保存的相对路径</returns>
        public static string ByteConvertWord(byte[] data, string fileName, string savePath)
        {
            if (!Path.IsPathRooted(savePath))
            {
                savePath = GetPath() + savePath;
            }
            if (!System.IO.Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            savePath = Path.Combine(savePath, fileName + ".doc");
            string filePath = savePath;

            FileStream fs;
            if (System.IO.File.Exists(filePath))
            {
                fs = new FileStream(filePath, FileMode.Truncate);
            }
            else
            {
                fs = new FileStream(filePath, FileMode.CreateNew);
            }
            BinaryWriter br = new BinaryWriter(fs);
            br.Write(data, 0, data.Length);
            br.Close();
            fs.Close();
            return savePath;
        }


        /// <summary>
        /// word文件转换二进制数据(用于保存数据库)
        /// </summary>
        /// <param name="wordPath">word文件路径</param>
        /// <returns>二进制</returns>
        private byte[] wordConvertByte(string wordPath)
        {
            byte[] bytContent = null;
            System.IO.FileStream fs = null;
            System.IO.BinaryReader br = null;
            try
            {
                fs = new FileStream(wordPath, FileMode.Open);
            }
            catch
            {
            }
            br = new BinaryReader((Stream)fs);
            bytContent = br.ReadBytes((Int32)fs.Length);
            return bytContent;
        }


        /// <summary>
        /// 项目所在目录
        /// </summary>
        /// <returns></returns>
        private static string GetPath()
        {
            return Environment.CurrentDirectory;
        }


        /// <summary>
        /// 格式化当前时间: 
        /// 1:yyMMddHHmmss; 2:yyyy-MM\dd\
        /// </summary>
        /// <returns></returns>
        public string FormatNowTime(int num)
        {
            if (num == 1)
            {
                return DateTime.Now.ToString("yyMMddHHmmss");
            }
            else if (num == 2)
            {
                return DateTime.Now.ToString("yyyy-MM") + @"\" + DateTime.Now.Day;
            }
            return "";
        }
    }
}
