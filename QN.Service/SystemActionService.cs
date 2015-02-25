using QN.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Service
{
    public class SystemActionService
    {
        /// <summary>
        /// 初始化系统Action
        /// </summary>
        public static void InitSystemAction()
        {
            //添加评论
            ActionManager.RegisterAction("addcomment", (m) =>
            {
                CommentService commentService = new CommentService();

                if (m.Length == 0)
                {
                    throw new ArgumentNullException();
                }

                comment entity = m[0] as comment;
                entity.siteid = R.siteid;

                commentService.Add(entity);

                return entity;
            });
        }
    }
}