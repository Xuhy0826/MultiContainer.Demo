using System;
using System.Collections.Generic;
using System.IO;
using Mark.Common.Helper;

namespace Mark.Common
{
    public class PathHelper
    {
        /// <summary>
        /// 获取指定目录所有文件夹名称
        /// </summary>
        /// <param name="path"></param>
        public static IEnumerable<string> GetDirectoriesName(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                yield break;
            }
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                yield return d.Name;
            }
        }

        /// <summary>
        /// 获取指定目录所有文件名称
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFilesName(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                yield break;
            }
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (FileInfo f in root.GetFiles())
            {
                yield return f.Name;
            }
        }
        /// <summary>
        /// 获取指定目录下所有指定后缀的所有文件名称
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllFilesNameByExt(string path, string ext)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                yield break;
            }
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (FileInfo f in root.GetFiles())
            {
                if (ext.Equals(Path.GetExtension(f.Name)))
                {
                    yield return f.Name;
                }
            }
        }
        /// <summary>
        /// 获取指定目录所有文件名称(带路径)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFilesFullName(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                yield break;
            }
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (FileInfo f in root.GetFiles())
            {
                yield return f.FullName;
            }
        }
        /// <summary>
        /// 获取指定目录下所有指定后缀的所有文件名称(带路径)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllFilesFullNameByExt(string path, string ext)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                yield break;
            }
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (FileInfo f in root.GetFiles())
            {
                if (ext.Equals(Path.GetExtension(f.Name)))
                {
                    yield return f.FullName;
                }
            }
        }
        /// <summary>
        /// 获取指定文件的版本号
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileVersionInfo(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return null;
            }
            try
            {
                System.Diagnostics.FileVersionInfo fileInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(path);
                return $"{fileInfo.ProductMajorPart}.{fileInfo.ProductMinorPart}.{fileInfo.ProductBuildPart}.{fileInfo.ProductPrivatePart}";
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return null;
            }
        }
        /// <summary>
        /// 获取指定文件的大小（kb）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static double GetFileSize(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return 0;
            }
            try
            {
                var fileInfo = new System.IO.FileInfo(path);
                return System.Math.Ceiling(fileInfo.Length / 1024.0);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return 0;
            }
        }

        /// <summary>
        /// 创建隐藏文件夹
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isCreateNew"></param>
        public static void CreateHiddenFolder(string filePath, bool isCreateNew = false)
        {
            if (!isCreateNew)
            {
                if (Directory.Exists(filePath))
                {
                    File.SetAttributes(filePath, FileAttributes.Hidden);
                    return;
                }
            }
            else
            {
                if (Directory.Exists(filePath))
                {
                    Directory.Delete(filePath);
                }

            }
            Directory.CreateDirectory(filePath);
            File.SetAttributes(filePath, FileAttributes.Hidden);
        }
    }
}
