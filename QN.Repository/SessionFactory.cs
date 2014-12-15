
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using NHibernate;
using NHibernate.Cfg;
using FluentNHibernate.Cfg;
using FluentNHibernate.Automapping;
using NHibernate.Tool.hbm2ddl;
using QN.Repository.Maps;
using FluentNHibernate.Mapping;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using QN.Repository.Entities;
using System.Web.Compilation;
using System.Web;

namespace QN.Repository
{
    public class SessionFactory
    {
        [ImportMany(typeof(IAggregateRoot))]
        public IEnumerable<IAggregateRoot> roots;

        private ISessionFactory sessionFactory;
        private Configuration config;

        private List<Assembly> mapAssemblys = new List<Assembly>();

        public static SessionFactory Instance { get; private set; }

        static SessionFactory()
        {
            Instance = new SessionFactory();
        }

        private SessionFactory()
        {
            LoadCompose();

            config = new Configuration();
            config.Configure();
        }

        /// <summary>
        /// 加载组件
        /// </summary>
        private void LoadCompose()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Assembly currentAsm = this.GetType().Assembly;

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(currentAsm));

            foreach (string path in QConfiger.Instance.PluginPaths())
            {
                catalog.Catalogs.Add(new DirectoryCatalog(path, "*.dll"));
            }

            CompositionContainer _container = new CompositionContainer(catalog);
            _container.ComposeParts(this);

            mapAssemblys.Add(currentAsm);

            if (null != roots)
            {
                foreach (IAggregateRoot root in roots)
                {
                    Assembly asm = root.GetType().Assembly;
                    if (!mapAssemblys.Contains(asm))
                    {
                        mapAssemblys.Add(asm);
                    }
                }
            }
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.Name))
            {
                return mapAssemblys.FirstOrDefault(m => m.FullName == args.Name);
            }

            return null;
        }

        /// <summary>
        /// 获取NH数据库会话
        /// </summary>
        /// <returns></returns>
        public ISession GetSession()
        {
            ISession session = null;

            session = SessionStorageFactory.GetSessionStorageContainer().GetCurrentSession();

            if (null == session)
            {
                session = BuildSessionFactory().OpenSession();
                SessionStorageFactory.GetSessionStorageContainer().Store(session);
            }

            return session;
        }

        private ISessionFactory BuildSessionFactory()
        {
            if (null == sessionFactory)
            {
                sessionFactory = Fluently
                    .Configure(config)
                    .Mappings(m => m.AutoMappings
                                    .Add(AutoMap.Assemblies(mapAssemblys.ToArray())
                                                .Where(type => type.IsSubclassOfRawGeneric(typeof(entity<>)))
                                    )
                    )
                    .BuildSessionFactory();
            }

            return sessionFactory;
        }
    }
}