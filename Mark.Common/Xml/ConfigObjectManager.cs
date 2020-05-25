using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

namespace Mark.Common
{
    /// <summary>
    /// 该特性用于指定对象可使用配置文件进行加载与保存。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ConfigurableAttribute : Attribute
    {
        private string _configName;

        /// <summary>
        /// 初始化 <see cref="ConfigurableAttribute"/> 类的新实例。
        /// </summary>
        public ConfigurableAttribute()
        {
        }

        /// <summary>
        /// 使用指定的配置文件名称初始化 <see cref="ConfigurableAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="configName"></param>
        public ConfigurableAttribute(string configName)
        {
            this._configName = configName;
        }

        /// <summary>
        /// 获取配置文件名称。
        /// </summary>
        public string ConfigName
        {
            get { return this._configName; }
        }
    }

    /// <summary>
    /// 用于管理可配置对象的加载与保存。
    /// </summary>
    public class ConfigObjectManager
    {
        private static Hashtable _ConfigObjects = new Hashtable();

        /// <summary>
        /// 通过加载配置文件并进行反序列化得到一个可配置对象。
        /// </summary>
        /// <typeparam name="T">可配置对象的类型。</typeparam>
        /// <returns></returns>
        public static T GetConfig<T>() where T : class, new()
        {
            return GetConfig<T>(null, "", false);
        }

        /// <summary>
        /// 通过加载配置文件并进行反序列化得到一个可配置对象。
        /// </summary>
        /// <typeparam name="T">可配置对象的类型。</typeparam>
        /// <param name="directory">配置文件所在的文件夹。</param>
        /// <returns></returns>
        public static T GetConfig<T>(string directory) where T : class, new()
        {
            return GetConfig<T>(null, directory, true);
        }

        /// <summary>
        /// 通过加载配置文件并进行反序列化得到一个可配置对象。
        /// </summary>
        /// <typeparam name="T">可配置对象的类型。</typeparam>
        /// <param name="directory">配置文件所在的文件夹。</param>
        /// <param name="absolutePath">是否绝对路径，若为false，则表示相对于可执行文件所在目录去查找 directory 所表示的文件夹。</param>
        /// <returns></returns>
        public static T GetConfig<T>(string directory, bool absolutePath) where T : class, new()
        {
            return GetConfig<T>(null, directory, absolutePath);
        }

        /// <summary>
        /// 通过加载配置文件并进行反序列化得到一个可配置对象。
        /// </summary>
        /// <typeparam name="T">可配置对象的类型。<</typeparam>
        /// <param name="defaultT">用于指定反序列化失败时返回的默认对象。</param>
        /// <returns></returns>
        public static T GetConfig<T>(T defaultT) where T : class, new()
        {
            return GetConfig<T>(defaultT, "", false);
        }

        /// <summary>
        /// 通过加载配置文件并进行反序列化得到一个可配置对象。
        /// </summary>
        /// <typeparam name="T">可配置对象的类型。</typeparam>
        /// <param name="defaultT">用于指定反序列化失败时返回的默认对象。</param>
        /// <param name="directory">配置文件所在的文件夹。</param>
        /// <returns></returns>
        public static T GetConfig<T>(T defaultT, string directory) where T : class, new()
        {
            return GetConfig<T>(defaultT, directory, true);
        }

        /// <summary>
        /// 通过加载配置文件并进行反序列化得到一个可配置对象。
        /// </summary>
        /// <typeparam name="T">可配置对象的类型。</typeparam>
        /// <param name="defaultT">用于指定反序列化失败时返回的默认对象。</param>
        /// <param name="directory">配置文件所在的文件夹。</param>
        /// <param name="absolutePath">是否绝对路径，若为false，则表示相对于可执行文件所在目录去查找 directory 所表示的文件夹。</param>
        /// <returns></returns>
        public static T GetConfig<T>(T defaultT, string directory, bool absolutePath) where T : class, new()
        {
            if (!absolutePath)
                directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"config", directory);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            Type type = typeof(T);
            ConfigObject<T> configObject = _ConfigObjects[type] as ConfigObject<T>;
            if (absolutePath || configObject == null)
            {
                string configName = GetConfigName(type);
                string configPath = Path.Combine(directory, configName);
                T target = Serializer.UnpackFullPathFile(configPath, type) as T;

                if (defaultT == null) defaultT = new T();
                configObject = new ConfigObject<T>(configName, target ?? defaultT);
                if (!absolutePath)
                    _ConfigObjects[type] = configObject;
                // 若配置文件不存在，或者配置对象结构发生变化，则Save一次。
                if (target == null || !EnsureConfigVersion(configPath, defaultT))
                {
                    SaveConfig<T>(configObject.Target, directory);
                }
            }
            return configObject.Target;
        }

        /// <summary>
        /// 将可配置对象序列化进行保存。
        /// </summary>
        /// <param name="target">将保存的可配置对象。</param>
        public static void SaveConfig<T>(T target)
        {
            SaveConfig<T>(target, "", false);
        }

        /// <summary>
        /// 将可配置对象序列化进行保存。
        /// </summary>
        /// <typeparam name="T">可配置对象的类型。</typeparam>
        /// <param name="target">将保存的可配置对象。</param>
        /// <param name="directory">配置文件所在的文件夹。</param>
        public static void SaveConfig<T>(T target, string directory)
        {
            SaveConfig<T>(target, directory, true);
        }

        /// <summary>
        /// 将可配置对象序列化进行保存。
        /// </summary>
        /// <typeparam name="T">可配置对象的类型。</typeparam>
        /// <param name="target">将保存的可配置对象。</param>
        /// <param name="directory">配置文件所在的文件夹。</param>
        /// <param name="absolutePath">文件夹是否绝对路径，若为false，则表示相对于可执行文件所在目录去查找 directory 所表示的文件夹。</param>
        public static void SaveConfig<T>(T target, string directory, bool absolutePath)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (!absolutePath)
                directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"config", directory);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string configName = null;
            Type type = typeof(T);
            if (type == typeof(Object))
            {
                type = target.GetType();
            }
            if (_ConfigObjects.ContainsKey(type))
            {
                configName = ((ConfigObject<T>)_ConfigObjects[type]).ConfigName;
            }
            else
            {
                configName = GetConfigName(type);
                _ConfigObjects[type] = new ConfigObject<T>(configName, target);
            }
            string configPath = Path.Combine(directory, configName);
            Serializer.PackFullPathFile(configPath, target);
        }

        private static string GetConfigName(Type type)
        {
            string configName = null;
            Attribute attr = Attribute.GetCustomAttribute(type, typeof(ConfigurableAttribute));
            if (attr != null)
            {
                configName = ((ConfigurableAttribute)attr).ConfigName;
            }
            if (string.IsNullOrEmpty(configName))
            {
                configName = type.Name + ".xml";
            }
            else
            {
                configName = Path.GetFileName(configName);
            }
            return configName;
        }

        /// <summary>
        /// 比较配置对象的结构是否发生改变，若未改变，则返回true，否则返回 false。
        /// </summary>
        private static bool EnsureConfigVersion<T>(string configPath, T current)
        {
            bool flag = true;
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, current);
                stream.Seek(0, SeekOrigin.Begin); //非常重要的一句话！没有的话，引发异常，缺少根元素

                bool flag1, flag2;
                System.Xml.XmlReader xmlReader1 = new System.Xml.XmlTextReader(configPath);
                System.Xml.XmlReader xmlReader2 = new System.Xml.XmlTextReader(stream);
                try
                {
                    while ((flag1 = xmlReader1.Read()) & (flag2 = xmlReader2.Read()))
                    {
                        if (xmlReader1.LocalName != xmlReader2.LocalName)
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                finally
                {
                    xmlReader1.Close();
                    xmlReader2.Close();
                }
                if (flag) flag = flag1 == flag2;
            }
            return flag;
        }

        private class ConfigObject<T>
        {
            private string _configName;
            private T _target;

            public ConfigObject(string configName, T target)
            {
                this._configName = configName;
                this._target = target;
            }

            public string ConfigName
            {
                get { return this._configName; }
            }

            public T Target
            {
                get { return this._target; }
            }
        }
    }
}


#region 使用方法
///=============================================
///调用方法如下
///Persons ps = new Persons();
//Person p1 = new Person();
//p1.Name = "yl";
//p1.Age = 20;
//ps.PersonList.Add(p1);

//Person p2 = new Person();
//p2.Name = "yl1";
//p2.Age = 200;
//ps.PersonList.Add(p2);
//ConfigObjectManager.SaveConfig(ps);//保存配置

//Persons ps= ConfigObjectManager.GetConfig<Persons>() as Persons;//取对象
//ps.PersonList[1].Age = 23;
//ConfigObjectManager.SaveConfig(ps);

//对应的类的上面加上[Configurable("Persons.xml")]属性
///=============================================
#endregion