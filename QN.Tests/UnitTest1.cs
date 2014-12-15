using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QN.Repository;
using System.Threading;
using NHibernate;
using QN.Repository.Maps;
using FluentNHibernate.Mapping;
using System.Diagnostics;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Reflection;
using QN.Plugin;
using System.Collections.Generic;

namespace QN.Tests
{
    [TestClass]
    public class UnitTest1
    {

        [ImportMany(typeof(IPlugin))]
        public IEnumerable<IPlugin> plugins;

        [TestMethod]
        public void PluginTest()
        {
            Thread.CurrentThread.Name = "test";

            var catalog = new AggregateCatalog();

            foreach (string path in QConfiger.Instance.PluginPaths())
            {
                catalog.Catalogs.Add(new DirectoryCatalog(path, "*.dll"));
            }

            CompositionContainer _container = new CompositionContainer(catalog);
            _container.ComposeParts(this);

            foreach(IPlugin p in plugins)
            {
                p.Active(null, 0);
            }
        }


        [TestMethod]
        public void TestMethod1()
        {
            //Assert.IsTrue(typeof(OptionMap).IsSubclassOfRawGeneric(typeof(ClassMap<>)));
            //Assert.IsTrue(typeof(OptionMap).IsSubclassOf(typeof(ClassMap<option>)));

            Thread.CurrentThread.Name = "test";
            using (ISession s = SessionFactory.Instance.GetSession())
            {
                option o = new option();
                o.value = "1";
                o.name = "1";
                o.siteid = 0;
                s.Save(o);

                post p = new post();
                p.name = "1";

                s.Save(p);
            }
        }
    }
}
