using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Mark.Common.Helper;

namespace Mark.Common
{
    public class CommonMethod
    {
        /// <summary>
        /// 获取枚举类型的“Description”特性的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetEnumDescription<T>(T obj)
        {
            var type = obj.GetType();
            string enumName = Enum.GetName(type, obj);
            if (enumName == null)
            {
                return null;
            }
            FieldInfo field = type.GetField(enumName);
            DescriptionAttribute descAttr =
                Attribute.GetCustomAttribute(field, typeof (DescriptionAttribute)) as DescriptionAttribute;
            if (descAttr == null)
            {
                return string.Empty;
            }
            return descAttr.Description;
        }

        /// <summary>
        /// 获取本地IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIp()
        {
            var tempIp = "127.0.0.1";
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                foreach (var t in IpEntry.AddressList.Where(t => t.AddressFamily == AddressFamily.InterNetwork))
                {
                    return t.ToString();
                }
                return tempIp;
            }
            catch (Exception ex)
            {
                return tempIp;
            }
        }

        /// <summary>
        /// 判断是否是一个合法的IP
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool IsValidIP(string address)
        {
            if (string.IsNullOrEmpty(address) || address.Length < 7 || address.Length > 15) return false;
            const string pattern = @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$";
            Regex re = new Regex(pattern);

            return re.IsMatch(address);
        }
        /// <summary>
        /// 判断是否是一个合法的Port
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool IsValidPort(string port)
        {
            Regex regex = new Regex(@"^[1-9]$|(^[1-9][0-9]$)|(^[1-9][0-9][0-9]$)|(^[1-9][0-9][0-9][0-9]$)|(^[1-6][0-5][0-5][0-3][0-5]$)");
            Match match = regex.Match(port);
            return match.Success;
        }
        /// <summary>
        /// 判断字符串是否是数字
        /// </summary>
        /// <param name="_string"></param>
        /// <returns></returns>
        public static bool isNumberic(string _string)
        {
            if (string.IsNullOrEmpty(_string))
                return false;
            foreach (char c in _string)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 判断字符串是否是数字
        /// </summary>
        /// <param name="_string"></param>
        /// <returns></returns>
        public static bool isNumbericEx(string _string)
        {
            if (string.IsNullOrEmpty(_string))
                return false;
            foreach (char c in _string)
            {
                if (!char.IsDigit(c) && c != '.')
                    return false;
            }
            return true;
        }
        /// <summary>  
        /// 计算文本长度，区分中英文字符，中文算两个长度，英文算一个长度
        /// </summary>
        /// <param name="Text">需计算长度的字符串</param>
        /// <returns>int</returns>
        public static int GetStringLength(string Text)
        {
            int len = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                byte[] byte_len = Encoding.Default.GetBytes(Text.Substring(i, 1));
                if (byte_len.Length > 1)
                    len += 2;  //如果长度大于1，是中文，占两个字节，+2
                else
                    len += 1;  //如果长度等于1，是英文，占一个字节，+1
            }
            return len;

        }
        /// <summary> 
        /// 截取文本，区分中英文字符，中文算两个长度，英文算一个长度
        /// </summary>
        /// <param name="str">待截取的字符串</param>
        /// <param name="length">需计算长度的字符串</param>
        /// <returns>string</returns>
        public static string GetSubString(string str, int length)
        {
            string temp = str;
            int j = 0;
            int k = 0;
            for (int i = 0; i < temp.Length; i++)
            {
                if (Regex.IsMatch(temp.Substring(i, 1), @"[\u4e00-\u9fa5]+"))
                {
                    j += 2;
                }
                else
                {
                    j += 1;
                }
                if (j <= length)
                {
                    k += 1;
                }
                if (j > length)
                {
                    return temp.Substring(0, k) + "..";
                }
            }
            return temp;
        }
        

        /// <summary>
        /// 取SaveFileDialoge Filter里的后缀
        /// </summary>
        /// <param name="str">FILTER原字符串 264 files (*.264)|*.264" + "|601 files (*.601)|*.601"</param>
        /// <param name="index">选中的序号 从1开始的</param>
        /// <returns></returns>
        public static string GetSaveDialogFilterExtension(string str, int index)
        {
            //数组是从0开始的
            index -= 1;
            if (index < 0)
            {
                return "";
            }
            string[] extens = str.Split('|');
            string[] tempExtens = new string[extens.Length / 2];
            int idex = 0;
            for (int i = 0; i < extens.Length; i++)
            {
                if (extens[i].Length < 1)
                {
                    continue;
                }
                if (i % 2 == 1)
                {
                    tempExtens[idex] = extens[i];
                    idex++;
                }
            }
            string stemp = "";
            if (tempExtens.Length > index)
            {
                stemp = tempExtens[index];
                stemp = Path.GetExtension(stemp);
            }
            return stemp;
        }
        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }
        
        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIpString()
        {
            string localipString = null;
            //获取本机名 
            string hostName = Dns.GetHostName();
            //获取所有地址，包括IPv4和IPv6 
            System.Net.IPAddress[] addressList = Dns.GetHostAddresses(hostName);
            if (addressList == null || addressList.Length < 1)
            {
                return localipString;
            }
            foreach (var ip in addressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    string ipAddr = ip.ToString();
                    localipString = ipAddr;
                }
            }
            return localipString;
        }
        /// <summary>
        /// 获取CPU的编号
        /// </summary>
        /// <returns></returns>
        public static string GetCpuID()
        {
            try
            {
                string cpuInfo = "";
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
                moc = null;
                mc = null;
                return cpuInfo;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            return null;
        }
        /// <summary>
        /// 监测特定名称的Windows服务是否存在
        /// </summary>
        /// <param name="serviceName">要检测的服务名称</param>
        /// <returns>存在true，不存在false</returns>
        public static bool CheckService(string serviceName)
        {
            bool bCheck = false;
            //获取windows服务列表
            ServiceController[] serviceList = ServiceController.GetServices();
            //循环查找该名称的服务
            for (int i = 0; i < serviceList.Length; i++)
            {
                if (serviceList[i].DisplayName.ToString() == serviceName)
                {
                    bCheck = true;
                    break;
                }
            }
            return bCheck;
        }
    }
}

