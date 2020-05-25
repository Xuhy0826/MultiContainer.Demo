using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mark.Common
{
    public enum Constellation
    {
        [Description("白羊座")]
        Aries,
        [Description("金牛座")]
        Taurus,
        [Description("双子座")]
        Gemini,
        [Description("巨蟹座")]
        Cancer,
        [Description("狮子座")]
        Leo,
        [Description("处女座")]
        Virgo,
        [Description("天秤座")]
        Libra,
        [Description("天蝎座")]
        Scorpio,
        [Description("射手座")]
        Sagittarius,
        [Description("摩羯座")]
        Capricorn,
        [Description("水瓶座")]
        Aquarius,
        [Description("双鱼座")]
        Pisces
    }
}
