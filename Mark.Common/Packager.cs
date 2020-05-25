using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;

namespace Mark.Common
{
    public class Packager
    {
        /// <summary>
        /// 把文件集合打包压缩，并增加解压密码
        /// </summary>
        /// <param name="outputPath">生成的压缩包的绝对路径</param>
        /// <param name="filesPath">要打包进来的文件集合</param>
        /// <param name="password">压缩包密码，不设置密码传入null即可</param>
        /// <param name="opLogs">操作日志，包含错误信息等</param>
        /// <returns></returns>
        public static bool PackFilesWithPassword(string outputPath, List<string> filesPath, string password,
            out string opLogs)
        {
            string dir;
            try
            {
                dir = Path.GetDirectoryName(outputPath);
            }
            catch (Exception ex)
            {
                opLogs = "非法的文件路径";
                return false;
            }

            if (!Directory.Exists(dir))
            {
                opLogs = "非法的文件路径";
                return false;
            }
            List<string> filesOnErr = new List<string>();
            opLogs = "操作成功";
            StringBuilder logSb = new StringBuilder();
            var tempName = Path.GetFileNameWithoutExtension(outputPath) + "temp";
            var tempFilePath = Path.Combine(dir, tempName);     //最终生成的压缩包的路径
            try
            {
                Crc32 crc = new Crc32();
                ZipOutputStream zipStream = new ZipOutputStream(File.Create(tempFilePath));
                zipStream.SetLevel(6);  // 压缩级别 0-9
                if (!string.IsNullOrEmpty(password))
                {
                    zipStream.Password = password;
                }
                for (int i = 0; i < filesPath.Count; i++)
                {
                    if (string.IsNullOrEmpty(filesPath[i]))
                    {
                        continue;
                    }
                    //如果文件不存在就跳过
                    if (!File.Exists(filesPath[i]))
                    {
                        filesOnErr.Add(Path.GetFileName(filesPath[i]));
                        filesPath.RemoveAt(i);
                        i--;
                        continue;
                    }
                    //压缩进文件

                    FileStream fileStream = new FileStream(filesPath[i], FileMode.Open);
                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    ZipEntry entry = new ZipEntry(Path.GetFileName(filesPath[i]));
                    entry.DateTime = DateTime.Now;
                    entry.Size = fileStream.Length;
                    fileStream.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipStream.PutNextEntry(entry);
                    zipStream.Write(buffer, 0, buffer.Length);
                }
                zipStream.Finish();
                zipStream.Close();
                //报告不存在的文件
                if (filesOnErr.Count > 0)
                {
                    logSb.Append("文件");
                    filesOnErr.ForEach(file => { logSb.Append($"{file}、"); });
                    logSb.Append("不存在");
                    opLogs = logSb.ToString();
                }
                //改名临时文件
                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }
                File.Move(tempFilePath, outputPath);
                return true;
            }
            catch (Exception ex)
            {
                opLogs = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 压缩包解压，解压出来的文件放在同级目录下创建的同名文件夹内
        /// </summary>
        /// <param name="filePath">压缩包的路径</param>
        /// <param name="password">密码，没有密码传空值即可</param>
        /// <param name="opLogs">操作记录（如果成功，则记录解压后文件夹的路径，如果失败，则记录失败原因）</param>
        /// <returns></returns>
        public static bool UnPackFilesWithPassword(string filePath, string password, out string opLogs, bool isOverWrite = false)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                opLogs = "传入的文件路径不存在";
                return false;
            }
            opLogs = "操作成功";
            var tempName = Path.GetFileNameWithoutExtension(filePath) + "temp";
            var dir = Path.GetDirectoryName(filePath);
            var tempDirectory = Path.Combine(dir, tempName);

            try
            {
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, true);
                }

                Directory.CreateDirectory(tempDirectory);

                using (ZipInputStream s = new ZipInputStream(File.OpenRead(filePath)))
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        s.Password = password;
                    }

                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        //var directorName = Path.Combine(targetPath, Path.GetDirectoryName(theEntry.Name));
                        var fileName = tempDirectory + theEntry.Name;
                        if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                        }
                        if (fileName != string.Empty)
                        {
                            using (FileStream streamWriter = File.Create(fileName))
                            {
                                int size = 4096;
                                byte[] data = new byte[4 * 1024];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else break;
                                }
                            }
                        }
                    }
                }
                var newDir = tempDirectory.RemoveLastChar("temp");
                if (Directory.Exists(newDir))
                {
                    if (isOverWrite)
                    {
                        Directory.Delete(newDir, true);
                    }
                    else
                    {
                        newDir = Path.Combine(dir, Guid.NewGuid().ToString());
                    }
                }
                Directory.Move(tempDirectory, newDir);
                opLogs = newDir;
                //把temp文件夹改名：去掉"temp"
                return true;
            }
            catch (Exception ex)
            {
                opLogs = ex.Message;
                return false;
            }
        }


        #region 压缩文件
        /// <summary>    
        /// 压缩文件    
        /// </summary>    
        /// <param name="fileNames">要打包的文件列表</param>    
        /// <param name="GzipFileName">目标文件名</param>    
        /// <param name="CompressionLevel">压缩品质级别（0~9）</param>    
        /// <param name="deleteFile">是否删除原文件</param>  
        public static void CompressFile(List<FileInfo> fileNames, string GzipFileName, int CompressionLevel, string password, bool deleteFile)
        {
            ZipOutputStream s = new ZipOutputStream(File.Create(GzipFileName));
            if (!string.IsNullOrEmpty(password))
            {
                s.Password = password;
            }
            try
            {
                s.SetLevel(CompressionLevel);   //0 - store only to 9 - means best compression    
                foreach (FileInfo file in fileNames)
                {
                    FileStream fs = null;
                    try
                    {
                        fs = file.Open(FileMode.Open, FileAccess.ReadWrite);
                    }
                    catch
                    { continue; }
                    //  方法二，将文件分批读入缓冲区    
                    byte[] data = new byte[2048];
                    int size = 2048;
                    ZipEntry entry = new ZipEntry(Path.GetFileName(file.Name));
                    entry.DateTime = (file.CreationTime > file.LastWriteTime ? file.LastWriteTime : file.CreationTime);
                    s.PutNextEntry(entry);
                    while (true)
                    {
                        size = fs.Read(data, 0, size);
                        if (size <= 0) break;
                        s.Write(data, 0, size);
                    }
                    fs.Close();
                    if (deleteFile)
                    {
                        file.Delete();
                    }
                }
            }
            finally
            {
                s.Finish();
                s.Close();
            }
        }
        /// <summary>    
        /// 压缩文件夹    
        /// </summary>    
        /// <param name="dirPath">要打包的文件夹</param>    
        /// <param name="GzipFileName">目标文件名</param>    
        /// <param name="CompressionLevel">压缩品质级别（0~9）</param>
        /// <param name="password">压缩密码</param>    
        /// <param name="deleteDir">是否删除原文件夹</param>  
        public static void CompressDirectory(string dirPath, string GzipFileName, int CompressionLevel, string password, bool deleteDir)
        {
            //压缩文件为空时默认与压缩文件夹同一级目录    
            if (GzipFileName == string.Empty)
            {
                GzipFileName = dirPath.Substring(dirPath.LastIndexOf("//") + 1);
                GzipFileName = dirPath.Substring(0, dirPath.LastIndexOf("//")) + "//" + GzipFileName + ".zip";
            }
            //if (Path.GetExtension(GzipFileName) != ".zip")  
            //{  
            //    GzipFileName = GzipFileName + ".zip";  
            //}  
            using (ZipOutputStream zipoutputstream = new ZipOutputStream(File.Create(GzipFileName)))
            {
                if (!string.IsNullOrEmpty(password))
                {
                    zipoutputstream.Password = password;
                }
                zipoutputstream.SetLevel(CompressionLevel);
                Crc32 crc = new Crc32();
                Dictionary<string, DateTime> fileList = GetAllFies(dirPath);
                foreach (KeyValuePair<string, DateTime> item in fileList)
                {
                    FileStream fs = File.OpenRead(item.Key.ToString());
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    ZipEntry entry = new ZipEntry(item.Key.Substring(dirPath.Length));
                    entry.DateTime = item.Value;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipoutputstream.PutNextEntry(entry);
                    zipoutputstream.Write(buffer, 0, buffer.Length);
                }
            }
            if (deleteDir)
            {
                Directory.Delete(dirPath, true);
            }
        }
        /// <summary>    
        /// 获取所有文件    
        /// </summary>    
        /// <returns></returns>    
        private static Dictionary<string, DateTime> GetAllFies(string dir)
        {
            Dictionary<string, DateTime> FilesList = new Dictionary<string, DateTime>();
            DirectoryInfo fileDire = new DirectoryInfo(dir);
            if (!fileDire.Exists)
            {
                throw new System.IO.FileNotFoundException("目录:" + fileDire.FullName + "没有找到!");
            }
            GetAllDirFiles(fileDire, FilesList);
            GetAllDirsFiles(fileDire.GetDirectories(), FilesList);
            return FilesList;
        }
        /// <summary>    
        /// 获取一个文件夹下的所有文件夹里的文件    
        /// </summary>    
        /// <param name="dirs"></param>    
        /// <param name="filesList"></param>    
        private static void GetAllDirsFiles(DirectoryInfo[] dirs, Dictionary<string, DateTime> filesList)
        {
            foreach (DirectoryInfo dir in dirs)
            {
                foreach (FileInfo file in dir.GetFiles("*.*"))
                {
                    filesList.Add(file.FullName, file.LastWriteTime);
                }
                GetAllDirsFiles(dir.GetDirectories(), filesList);
            }
        }
        /// <summary>    
        /// 获取一个文件夹下的文件    
        /// </summary>    
        /// <param name="dir">目录名称</param>    
        /// <param name="filesList">文件列表HastTable</param>    
        private static void GetAllDirFiles(DirectoryInfo dir, Dictionary<string, DateTime> filesList)
        {
            foreach (FileInfo file in dir.GetFiles("*.*"))
            {
                filesList.Add(file.FullName, file.LastWriteTime);
            }
        }
        #endregion
        #region 解压缩文件
        /// <summary>    
        /// 解压缩文件    
        /// </summary>    
        /// <param name="GzipFile">压缩包文件名</param>    
        /// <param name="targetPath">解压缩目标路径</param>           
        public static void Decompress(string GzipFile, string targetPath, string password)
        {
            //string directoryName = Path.GetDirectoryName(targetPath + "//") + "//";    
            string directoryName = targetPath;
            if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);//生成解压目录    
            string CurrentDirectory = directoryName;
            byte[] data = new byte[2048];
            int size = 2048;
            ZipEntry theEntry = null;
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(GzipFile)))
            {
                if (!string.IsNullOrEmpty(password))
                {
                    s.Password = password;
                }
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    if (theEntry.IsDirectory)
                    {// 该结点是目录    
                        if (!Directory.Exists(CurrentDirectory + theEntry.Name)) Directory.CreateDirectory(CurrentDirectory + theEntry.Name);
                    }
                    else
                    {
                        if (theEntry.Name != String.Empty)
                        {
                            //  检查多级目录是否存在  
                            if (theEntry.Name.Contains("//"))
                            {
                                string parentDirPath = theEntry.Name.Remove(theEntry.Name.LastIndexOf("//") + 1);
                                if (!Directory.Exists(parentDirPath))
                                {
                                    Directory.CreateDirectory(CurrentDirectory + parentDirPath);
                                }
                            }

                            //解压文件到指定的目录    
                            using (FileStream streamWriter = File.Create(CurrentDirectory + theEntry.Name))
                            {
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size <= 0) break;
                                    streamWriter.Write(data, 0, size);
                                }
                                streamWriter.Close();
                            }
                        }
                    }
                }
                s.Close();
            }
        }
        #endregion
    }
}

