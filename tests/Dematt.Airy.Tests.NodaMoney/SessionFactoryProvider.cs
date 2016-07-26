using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Dematt.Airy.Nhibernate.NodaMoney;
using Dematt.Airy.Tests.NodaMoney.Entities;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;

namespace Dematt.Airy.Tests.NodaMoney
{
    /// <summary>
    /// NHibernate session factory provider we only ever want one of these :-)
    /// </summary>
    public class SessionFactoryProvider
    {
        private readonly Configuration _configuration;

        /// <summary>
        /// Default constructer, builds a session factory looking in the app.config, web.config or hibernate.cfg.xml for config details.
        /// </summary>
        public SessionFactoryProvider()
        {
            _configuration = new Configuration();
            _configuration.Configure();
            CreateNHibernateSessionFactory();
        }

        /// <summary>
        /// Builds a session factory using the supplied configuration file.
        /// </summary>        
        public SessionFactoryProvider(string nHhibernateConfigFile)
        {
            Console.WriteLine("Using config file: " + nHhibernateConfigFile);
            var xml = XDocument.Load(nHhibernateConfigFile);
            if (xml.Root != null)
            {
                var connectionQuery =
                    from c in xml.Root.Descendants()
                    where (string)c.Attribute("name") == "connection.connection_string"
                    select c.Value;
                Console.WriteLine("Using connection string: " + connectionQuery.FirstOrDefault());
            }

            _configuration = new Configuration();
            _configuration.Configure(nHhibernateConfigFile);
            CreateNHibernateSessionFactory();
        }

        /// <summary>
        /// The NHibernate session factory use to obtain sessions.
        /// </summary>
        public ISessionFactory DefaultSessionFactory;

        /// <summary>
        /// Drops and rebuilds the database schema.
        /// </summary>        
        public void BuildSchema()
        {
            // Build the schema.
            var createSchemaSql = new StringWriter();
            var schemaExport = new SchemaExport(_configuration);

            // Drop the existing schema.
            schemaExport.Drop(true, true);

            // Print the Sql that will be used to build the schema.
            schemaExport.Create(createSchemaSql, false);
            Debug.Print(createSchemaSql.ToString());

            // Create the schema.
            schemaExport.Create(false, true);
        }

        /// <summary>
        /// Creates the NHibernare session factory.
        /// </summary>
        private void CreateNHibernateSessionFactory()
        {
            if (DefaultSessionFactory == null)
            {

                // Get the domain entities to map and a mapper to map them.
                var domainType = typeof(MoneyTestEntity);
                var domainTypes = domainType.Assembly.GetTypes().Where(t => t.IsClass && t.Namespace == domainType.Namespace);
                var domainMapper = new ConventionModelMapper();

                // Customise the mapping before adding the mappings to the configuration.
                domainMapper.Class<MoneyTestEntity>(c =>
                {
                    c.Id(p => p.Id, m =>
                    {
                        m.Generator(Generators.GuidComb);
                    });
                    c.Property(p => p.Description, m =>
                    {
                        m.Length(100);
                    });
                    c.Property(p => p.Cost, m =>
                    {
                        m.Type<MoneyType>();
                        m.Columns(f => f.Name("CostAmount"), f => f.Name("CostCurrency"));
                    });
                    c.Property(p => p.Retail, m =>
                    {
                        m.Type<MoneyType>();
                        m.Columns(f => f.Name("RetailAmount"), f => f.Name("RetailCurrency"));
                    });
                });

                // Build and the mappings for the test domain entities.
                _configuration.AddMapping(domainMapper.CompileMappingFor(domainTypes));

                _configuration.DataBaseIntegration(x =>
                {
                    x.LogSqlInConsole = true;
                });

                DefaultSessionFactory = _configuration.BuildSessionFactory();
            }
        }

        /// <summary>
        /// As unit test runners often shadow copy the files to be executed and do not always reliably copy the content files.
        /// This allows us to get the path to config and other files that may be needed for the tests to execute.
        /// </summary>
        /// <returns></returns>
        public static string GetExecutingDirectoryForTests()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) ?? "";
            return new Uri(path).LocalPath;
        }

        /// <summary>
        /// As unit test runners often shadow copy the files to be executed and do not always reliably copy the content files.
        /// This allows us to get the full path of a content file that may be needed for the tests to execute.
        /// </summary>
        /// <returns></returns>
        public static string GetFullPathForContentFile(string localFilename)
        {
            return Path.Combine(GetExecutingDirectoryForTests(), localFilename);
        }
    }
}
