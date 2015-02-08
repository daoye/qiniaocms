using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Plugin
{
    /// <summary>
    /// 动作管理器
    /// </summary>
    public static class ActionManager
    {
        private static Dictionary<string, Func<object[], object>> actions = new Dictionary<string, Func<object[], object>>();

        /// <summary>
        /// 注册动作名称
        /// </summary>
        /// <param name="name"></param>
        /// <param name="func"></param>
        public static void RegisterAction(string name, Func<object[], object> func)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            lock (actions)
            {
                if (actions.ContainsKey(name))
                {
                    throw new ActionExistException("Action was exists.") { ActionName = name };
                }

                actions.Add(name, func);
            }
        }

        public static actionresult ApplyAction(string name, params object[] paras)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }
            
            actionresult result = new actionresult();

            Func<object[], object> act = actions[name];

            if (null != act)
            {
                result.result = act(paras);
                result.success = true;

                return result;
            }
            else
            {
                throw new ActionNotFoundException("Action not found.") { ActionName = name };
            }
        }
    }
}