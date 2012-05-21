using System;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

using Common;

namespace YQCMS.General
{
    /// <summary>
    /// 网站基本设置类 by discuz.nt
    /// </summary>
    public class GeneralConfigs
    {
        private static object lockHelper = new object();

        private static System.Timers.Timer generalConfigTimer = new System.Timers.Timer(15000);

        private static GeneralConfigInfo m_configinfo;

        /// <summary>
        /// 静态构造函数初始化相应实例和定时器
        /// </summary>
        static GeneralConfigs()
        {
            m_configinfo = GeneralConfigFileManager.LoadConfig();

            generalConfigTimer.AutoReset = true;
            generalConfigTimer.Enabled = true;
            generalConfigTimer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            generalConfigTimer.Start();
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ResetConfig();
        }


        /// <summary>
        /// 重设配置类实例
        /// </summary>
        public static void ResetConfig()
        {
            m_configinfo = GeneralConfigFileManager.LoadConfig();
        }

        public static GeneralConfigInfo GetConfig()
        {
            return m_configinfo;
        }


        /// <summary>
        /// 保存配置类实例
        /// </summary>
        /// <param name="generalconfiginfo"></param>
        /// <returns></returns>
        public static bool SaveConfig(GeneralConfigInfo generalconfiginfo)
        {
            GeneralConfigFileManager gcf = new GeneralConfigFileManager();
            GeneralConfigFileManager.ConfigInfo = generalconfiginfo;
            return gcf.SaveConfig();
        }



        #region Helper

        /// <summary>
        /// 序列化配置信息为XML
        /// </summary>
        /// <param name="configinfo">配置信息</param>
        /// <param name="configFilePath">配置文件完整路径</param>
        public static GeneralConfigInfo Serialiaze(GeneralConfigInfo configinfo, string configFilePath)
        {
            lock (lockHelper)
            {
                SerializationHelper.Save(configinfo, configFilePath);
            }
            return configinfo;
        }


        public static GeneralConfigInfo Deserialize(string configFilePath)
        {
            return (GeneralConfigInfo)SerializationHelper.Load(typeof(GeneralConfigInfo), configFilePath);
        }

        #endregion



    }
}
