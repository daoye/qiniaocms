using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace QN.Plugin
{
    [InheritedExport(typeof(IPlugin))]
    public interface IPlugin
    {
        /// <summary>
        /// 安装插件
        /// </summary>
        /// <param name="version">主程序版本号</param>
        void Install(string version);

        /// <summary>
        /// 启动插件
        /// </summary>
        /// <param name="version">主程序版本号</param>
        /// <param name="pluginId">当前插件的ID</param>
        void Active(string version, int pluginId);

        /// <summary>
        /// 禁用插件
        /// </summary>
        /// <param name="version">主程序版本号</param>
        /// <param name="pluginId">当前插件的ID</param>
        void Disable(string version, int pluginId);

        /// <summary>
        /// 删除插件
        /// </summary>
        /// <param name="version">主程序版本号</param>
        /// <param name="pluginId">当前插件的ID</param>
        void Delete(string version, int pluginId);

        /// <summary>
        /// 插件名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 插件版本
        /// </summary>

        string Version { get; }
    }
}
