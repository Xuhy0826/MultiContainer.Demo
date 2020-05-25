using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;

namespace Mark.Common.Xml
{
    /// <summary>
    /// 字符串压缩
    /// </summary>
    public class SharpZipLibUtils
    {
        #region Compressing.

        // ------------------------------------------------------------------

        /// <summary>
        /// Compress a folder with multiple files and subfolders with 
        /// the ZIP algorithm. Use the DecompressFolder() routine to
        /// decompress the compressed bytes.
        /// </summary>
        /// <param name="folderPath">The path to the folder
        /// that will be compressed.</param>
        /// <returns>Returns the compressed folder contents.</returns>
        public static byte[] CompressFolder(string folderPath)
        {
            if (folderPath == null || folderPath.Length <= 0)
            {
                return null;
            }
            else
            {
                using (var buf = new MemoryStream())
                using (var zip = new ZipOutputStream(buf))
                {
                    var crc = new Crc32();
                    zip.SetLevel(9); // 0..9.

                    DoCompressFolder(
                        buf,
                        zip,
                        crc,
                        folderPath,
                        folderPath);

                    zip.Finish();

                    // --

                    var c = new byte[buf.Length];
                    buf.Seek(0, SeekOrigin.Begin);
                    buf.Read(c, 0, c.Length);

                    // --

                    zip.Close();

                    return c;
                }
            }
        }

        /// <summary>
        /// Compress multiple files with the ZIP algorithm. 
        /// Use the DecompressFiles() routine to decompress the compressed bytes.
        /// </summary>
        /// <param name="filePaths">The list of all files to 
        /// compress.</param>
        /// <returns>Returns the compressed files contents.</returns>
        public static byte[] CompressFiles(
            string[] filePaths)
        {
            if (filePaths == null || filePaths.Length <= 0)
            {
                return null;
            }
            else
            {
                using (var buf = new MemoryStream())
                using (var zip = new ZipOutputStream(buf))
                {
                    var crc = new Crc32();
                    zip.SetLevel(9); // 0..9.

                    foreach (string filePath in filePaths)
                    {
                        using (var fs = new FileStream(
                            filePath,
                            FileMode.Open,
                            FileAccess.Read))
                        using (var r = new BinaryReader(fs))
                        {
                            var buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, buffer.Length);

                            var entry = new ZipEntry(Path.GetFileName(filePath));
                            entry.DateTime = DateTime.Now;
                            entry.Size = buffer.Length;

                            crc.Reset();
                            crc.Update(buffer);

                            entry.Crc = crc.Value;

                            zip.PutNextEntry(entry);
                            zip.Write(buffer, 0, buffer.Length);
                        }
                    }

                    zip.Finish();

                    // --

                    var c = new byte[buf.Length];
                    buf.Seek(0, SeekOrigin.Begin);
                    buf.Read(c, 0, c.Length);

                    // --

                    zip.Close();

                    return c;
                }
            }
        }

        /// <summary>
        /// Compress a file with the ZIP algorithm.
        /// Use the DecompressFile() routine to decompress the compressed bytes.
        /// </summary>
        /// <param name="filePath">The file to compress.</param>
        /// <returns>Returns the compressed file content.</returns>
        public static byte[] CompressFile(
            string filePath)
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
        /// Compress a XML document with the ZIP algorithm.
        /// Use the DecompressXmlDocument() routine to decompress 
        /// the compressed bytes.
        /// </summary>
        /// <param name="input">The XML document to compress.</param>
        /// <returns>Returns the compressed XML document.</returns>
        public static byte[] CompressXmlDocument(
            XmlDocument input)
        {
            return CompressString(input.InnerXml);
        }

        /// <summary>
        /// Compress a string with the ZIP algorithm.
        /// Use the DecompressString() routine to decompress the compressed bytes.
        /// </summary>
        /// <param name="input">The string to compress.</param>
        /// <returns>Returns the compressed string.</returns>
        public static byte[] CompressString(
            string input)
        {
            return CompressBytes(
                Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        /// Compress a DataSet with the ZIP algorithm.
        /// Use the DecompressDataSet() routine to decompress the compressed bytes.
        /// </summary>
        /// <param name="input">The DataSet to compress.</param>
        /// <returns>Returns the compressed DataSet.</returns>
        public static byte[] CompressDataSet(
            DataSet input)
        {
            var bf = new BinaryFormatter();

            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, input);
                return CompressBytes(ms.GetBuffer());
            }
        }

        /// <summary>
        /// Compress a byte array with the ZIP algorithm.
        /// Use the DecompressBytes() routine to decompress the compressed bytes.
        /// </summary>
        /// <param name="input">The bytes to compress.</param>
        /// <returns>Returns the compressed bytes.</returns>
        public static byte[] CompressBytes(
            byte[] input)
        {
            using (var buf = new MemoryStream())
            using (var zip = new ZipOutputStream(buf))
            {
                var crc = new Crc32();
                zip.SetLevel(9); // 0..9.

                var entry = new ZipEntry(string.Empty);
                entry.DateTime = DateTime.Now;
                entry.Size = input.Length;

                crc.Reset();
                crc.Update(input);

                entry.Crc = crc.Value;

                zip.PutNextEntry(entry);

                zip.Write(input, 0, input.Length);
                zip.Finish();

                // --

                var c = new byte[buf.Length];
                buf.Seek(0, SeekOrigin.Begin);
                buf.Read(c, 0, c.Length);

                // --

                zip.Close();

                return c;
            }
        }

        // ------------------------------------------------------------------

        #endregion

        #region Decompressing.

        // ------------------------------------------------------------------

        /// <summary>
        /// Decompress a byte stream that was formerly compressed
        /// with the CompressFolder() routine with the ZIP algorithm 
        /// and store it to a folder.
        /// </summary>
        /// <param name="input">The buffer that contains the compressed
        /// stream with the files and subfolders.</param>
        /// <param name="folderPath">The base path where the files and folders
        /// will be stored.</param>
        public static void DecompressFolder(
            byte[] input,
            string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (var mem = new MemoryStream(input))
            using (var stm = new ZipInputStream(mem))
            {
                ZipEntry entry;
                while ((entry = stm.GetNextEntry()) != null)
                {
                    // Create this stream new for each zip entry.
                    using (var mem2 = new MemoryStream())
                    {
                        var data = new byte[4096];

                        while (true)
                        {
                            int size = stm.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                mem2.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }

                        // --
                        // Finished reading, now write to file.

                        string filePath = Path.Combine(folderPath, entry.Name);

                        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        }

                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }

                        using (var r = new BinaryReader(mem2))
                        using (var fs = new FileStream(
                            filePath,
                            FileMode.CreateNew,
                            FileAccess.Write))
                        using (var w = new BinaryWriter(fs))
                        {
                            var buf = new byte[mem2.Length];
                            mem2.Seek(0, SeekOrigin.Begin);
                            r.Read(buf, 0, (int)mem2.Length);

                            w.Write(buf);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decompress a byte stream that was formerly compressed
        /// with the CompressFiles() routine with the ZIP algorithm 
        /// and store it to a file.
        /// </summary>
        /// <param name="input">The buffer that contains the compressed
        /// stream with the files.</param>
        /// <param name="folderPath">The base path where the files will be 
        /// stored.</param>
        public static void DecompressFiles(
            byte[] input,
            string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (var mem = new MemoryStream(input))
            using (var stm = new ZipInputStream(mem))
            {
                ZipEntry entry;
                while ((entry = stm.GetNextEntry()) != null)
                {
                    // Make this stream new for each zip entry.
                    using (var mem2 = new MemoryStream())
                    {
                        var data = new byte[4096];

                        while (true)
                        {
                            int size = stm.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                mem2.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }

                        // --
                        // Finished reading, now write to file.

                        string filePath = Path.Combine(folderPath, entry.Name);

                        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        }

                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }

                        using (var r = new BinaryReader(mem2))
                        using (var fs = new FileStream(
                            filePath,
                            FileMode.CreateNew,
                            FileAccess.Write))
                        using (var w = new BinaryWriter(fs))
                        {
                            var buf = new byte[mem2.Length];
                            mem2.Seek(0, SeekOrigin.Begin);
                            r.Read(buf, 0, (int)mem2.Length);

                            w.Write(buf);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decompress a byte stream that was formerly compressed
        /// with the CompressFile() routine with the ZIP algorithm and 
        /// store it to a file.
        /// </summary>
        /// <param name="input">The buffer that contains the compressed
        /// stream with the file.</param>
        /// <param name="filePath">The file path where the file will be 
        /// stored.</param>
        public static void DecompressFile(
            byte[] input,
            string filePath)
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

        /// <summary>
        /// Decompress a byte stream of an XML document that was formerly
        /// compressed with the CompressXmlDocument() routine with 
        /// the ZIP algorithm.
        /// </summary>
        /// <param name="input">The buffer that contains the compressed
        /// stream with the XML document.</param>
        /// <returns>Returns the decompressed XML document.</returns>
        public static XmlDocument DecompressXmlDocument(
            byte[] input)
        {
            var doc = new XmlDocument();
            doc.LoadXml(DecompressString(input));

            return doc;
        }

        /// <summary>
        /// Decompress a byte stream of a string that was formerly 
        /// compressed with the CompressString() routine with the ZIP algorithm.
        /// </summary>
        /// <param name="input">The buffer that contains the compressed
        /// stream with the string.</param>
        /// <returns>Returns the decompressed string.</returns>
        public static string DecompressString(
            byte[] input)
        {
            return Encoding.UTF8.GetString(DecompressBytes(input));
        }

        /// <summary>
        /// Decompress a byte stream of a DataSet that was formerly 
        /// compressed with the CompressDataSet() routine with the ZIP algorithm.
        /// </summary>
        /// <param name="input">The buffer that contains the compressed
        /// stream with the DataSet.</param>
        /// <returns>Returns the decompressed DataSet.</returns>
        public static DataSet DecompressDataSet(
            byte[] input)
        {
            var bf = new BinaryFormatter();

            byte[] buffer = DecompressBytes(input);
            using (var ms = new MemoryStream(buffer))
            {
                return (DataSet)bf.Deserialize(ms);
            }
        }

        /// <summary>
        /// Decompress a byte stream that was formerly compressed
        /// with the CompressBytes() routine with the ZIP algorithm.
        /// </summary>
        /// <param name="input">The buffer that contains the compressed
        /// stream with the bytes.</param>
        /// <returns>Returns the decompressed bytes.</returns>
        public static byte[] DecompressBytes(
            byte[] input)
        {
            using (var mem = new MemoryStream(input))
            using (var stm = new ZipInputStream(mem))
            using (var mem2 = new MemoryStream())
            {
                ZipEntry entry = stm.GetNextEntry();
                if (entry != null)
                {
                    var data = new byte[4096];

                    while (true)
                    {
                        int size = stm.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            mem2.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                using (var r = new BinaryReader(mem2))
                {
                    var c = new byte[mem2.Length];
                    mem2.Seek(0, SeekOrigin.Begin);
                    r.Read(c, 0, (int)mem2.Length);

                    return c;
                }
            }
        }

        // ------------------------------------------------------------------

        #endregion

        #region Helper routines for compressing/decompressing.

        // ------------------------------------------------------------------

        /// <summary>
        /// Helper routine for recursive compressing a folder.
        /// </summary>
        /// <param name="buf">The buffer to write to.</param>
        /// <param name="zip">The associated ZIP stream to write to.</param>
        /// <param name="crc">The associated CRC object.</param>
        /// <param name="baseFolderPath">The path of the initial folder.</param>
        /// <param name="currentFolderPath">The path of the current folder to process.</param>
        private static void DoCompressFolder(
            MemoryStream buf,
            ZipOutputStream zip,
            Crc32 crc,
            string baseFolderPath,
            string currentFolderPath)
        {
            // Add all files of the current folder.
            foreach (string filePath in
                Directory.GetFiles(currentFolderPath))
            {
                // Make relative path for storing the information
                // inside the ZIP file.
                string relativeFilePath = filePath.Substring(
                    baseFolderPath.Length).Trim('\\');

                using (var fs = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read))
                using (var r = new BinaryReader(fs))
                {
                    var buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);

                    var entry = new ZipEntry(relativeFilePath);
                    entry.DateTime = DateTime.Now;
                    entry.Size = buffer.Length;

                    crc.Reset();
                    crc.Update(buffer);

                    entry.Crc = crc.Value;

                    zip.PutNextEntry(entry);
                    zip.Write(buffer, 0, buffer.Length);
                }
            }

            // Recurse all subfolders.
            foreach (string folderPath in
                Directory.GetDirectories(currentFolderPath))
            {
                DoCompressFolder(
                    buf,
                    zip,
                    crc,
                    baseFolderPath,
                    folderPath);
            }
        }

        #endregion
    }
}
