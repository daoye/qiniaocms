using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;

namespace QN.Plugin
{
    public class PluginManager
    {
        public static PluginManager Instance { get; private set; }

        [ImportMany(typeof(IPlugin))]
        public IEnumerable<IPlugin> Plugins { get; private set; }

        private PluginManager()
        {
            var catalog = new AggregateCatalog();

            foreach (string path in QConfiger.Instance.PluginPaths())
            {
                catalog.Catalogs.Add(new DirectoryCatalog(path, "*.dll"));
            }

            CompositionContainer container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }

        static PluginManager()
        {
            Instance = new PluginManager();
        }

        public void Install()
        {
            foreach(IPlugin p in Plugins)
            {
                p.Install(null);
            }
        }
    }
}
