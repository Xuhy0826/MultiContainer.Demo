using System.Xml;

namespace Mark.Common.Helper
{
    public class HisHelper
    {
        /// <summary>
        /// 用於生成XML節點的功用方法。
        /// </summary>
        /// <param name="doc">XmlDocument Object</param>
        /// <param name="name">節點名</param>
        /// <param name="text">節點內容</param>
        /// <returns></returns>
        public static XmlNode CreateElement(XmlDocument doc, string name, string text)
        {
            XmlNode element = doc.CreateElement(name);
            if (text != null)
                element.InnerText = text;
            return element;
        }

        /// <summary>
        /// 修正病歷號，補齊0
        /// </summary>
        /// <param name="ptId"></param>
        /// <returns></returns>
        public static string RepairPatientID(string ptId)
        {
            int shortCount = ptId.Length;
            string resultString = ptId;
            for (int i = 0; i < 10 - shortCount; i++)
            {
                resultString = "0" + resultString;
            }
            return resultString;
        }
    }
}
