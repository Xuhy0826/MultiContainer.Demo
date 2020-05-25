using System;

namespace Mark.Common
{
    public class PinYinTool
    {
        /// <summary>
        /// 取汉字首拼字码，如是字母，则返回此字母
        /// 生僻字不支持，如喆
        /// </summary>
        /// <param name="SourceStr"></param>
        /// <returns></returns>
        public static String GetPym(String SourceStr)
        {
            return _CHS1PinYin.GetPym(SourceStr);
        }

        /// <summary>
        /// 将指定中文字符串转换为拼音形式。
        /// </summary>
        /// <param name="chs">要转换的中文字符串。</param>
        /// <param name="separator">连接拼音之间的分隔符。</param>
        /// <param name="initialCap">指定是否将首字母大写。</param>
        /// <returns>包含中文字符串的拼音的字符串。</returns>
        public static string Convert(string chs, string separator, bool initialCap)
        {
            return _CHS2PinYin.Convert(chs, separator, initialCap);
        }
        /**/
        /// <summary>
        /// 将指定中文字符串转换为拼音形式。
        /// </summary>
        /// <param name="chs">要转换的中文字符串。</param>
        /// <param name="separator">连接拼音之间的分隔符。</param>
        /// <returns>包含中文字符串的拼音的字符串。</returns>
        public static string Convert(string chs, string separator)
        {
            return _CHS2PinYin.Convert(chs, separator, false);
        }
        /**/
        /// <summary>
        /// 将指定中文字符串转换为拼音形式。
        /// </summary>
        /// <param name="chs">要转换的中文字符串。</param>
        /// <param name="initialCap">指定是否将首字母大写。</param>
        /// <returns>包含中文字符串的拼音的字符串。</returns>
        public static string Convert(string chs, bool initialCap)
        {
            return _CHS2PinYin.Convert(chs, "", initialCap);
        }
        /**/
        /// <summary>
        /// 将指定中文字符串转换为拼音形式。
        /// </summary>
        /// <param name="chs">要转换的中文字符串。</param>
        /// <returns>包含中文字符串的拼音的字符串。</returns>
        public static string Convert(string chs)
        {
            return _CHS2PinYin.Convert(chs, "");
        }

        /// <summary>
        /// 比较两字符是否符合模糊查询结果
        /// </summary>
        /// <param name="inputSource">输入内容，如"ces",如输入的是中文，则不进行汉转拼音后的匹配操作</param>
        /// <param name="target">等待检测的值，如“测试案件一”</param>
        /// <param name="isCap">是否区分大小写，仅对英文有效.</param>
        /// <returns>如果中文相同或者英文首字符能匹配，或者汉字转的英文全拼能匹配，都返回true</returns>
        public static bool IsSame(string inputSource, string target, bool isCap)
        {
            string chsFirst = PinYinTool.GetPym(target);
            string chs = PinYinTool.Convert(target, false);
            if (isCap)
            {
                return target.Contains(inputSource)
                    || chsFirst.Contains(inputSource)
                    || chs.Contains(inputSource);
            }
            else
            {
                return target.ToUpper().Contains(inputSource.ToUpper())
                    || chsFirst.ToUpper().Contains(inputSource.ToUpper())
                    || chs.ToUpper().Contains(inputSource.ToUpper());
            }
        }
    }

}
