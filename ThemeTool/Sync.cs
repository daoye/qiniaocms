using QN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThemeTool
{
    public static class Sync
    {
        public static void Do(IList<Data> data)
        {
            foreach(Data d in data)
            {
                QFile.DeepCopy(d.Target, d.Src, false);
            }
        }
    }
}
