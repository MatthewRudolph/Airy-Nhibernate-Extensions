using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Dematt.Airy.Nhibernate.NodaTime;
using Dematt.Airy.Nhibernate.NodaTime.Helpers;
using Dematt.Airy.Tests.NodaTime.Entities;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;

namespace Dematt.Airy.Tests.NodaTime
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
                var domainType = typeof(DateTimeZoneTestEntity);
                var domainTypes = domainType.Assembly.GetTypes().Where(t => t.IsClass && t.Namespace == domainType.Namespace);
                var domainMapper = new ConventionModelMapper();

                // Customise the mapping before adding the mappings to the configuration.
                domainMapper.Class<DateTimeOffsetTestEntity>(c =>
                {
                    c.Id(p => p.Id, m =>
                    {
                        m.Generator(Generators.Native);
                    });
                    c.Property(p => p.Description, m =>
                    {
                        m.Length(100);
                    });
                });

                domainMapper.Class<DateTimeZoneTestEntity>(c =>
                {
                    c.Id(p => p.Id, m =>
                    {
                        m.Generator(Generators.Native);
                    });
                    c.Property(p => p.Description, m =>
                    {
                        m.Length(100);
                    });
                    c.Property(p => p.StartDateTimeZone, m =>
                    {
                        m.Type<DateTimeZoneTzdbType>();
                    });
                    c.Property(p => p.FinishDateTimeZone, m =>
                    {
                        m.Type<DateTimeZoneTzdbType>();
                    });
                });

                domainMapper.Class<InstantTestEntity>(c =>
                {
                    c.Id(p => p.Id, m =>
                    {
                        m.Generator(Generators.Native);
                    });
                    c.Property(p => p.Description, m =>
                    {
                        m.Length(100);
                    });
                    c.Property(p => p.StartInstant, m =>
                    {
                        m.Type<InstantType>();
                    });
                    c.Property(p => p.FinishInstant, m =>
                    {
                        m.Type<InstantType>();
                    });
                });

                domainMapper.Class<LocalDateTimeTestEntity>(c =>
                {
                    c.Id(p => p.Id, m =>
                    {
                        m.Generator(Generators.Native);
                    });
                    c.Property(p => p.Description, m =>
                    {
                        m.Length(100);
                    });
                    c.Property(p => p.StartLocalDateTime, m =>
                    {
                        m.Type<LocalDateTimeType>();
                    });
                    c.Property(p => p.FinishLocalDateTime, m =>
                    {
                        m.Type<LocalDateTimeType>();
                    });
                });

                domainMapper.Class<LocalDateTestEntity>(c =>
                {
                    c.Id(p => p.Id, m =>
                    {
                        m.Generator(Generators.Native);
                    });
                    c.Property(p => p.Description, m =>
                    {
                        m.Length(100);
                    });
                    c.Property(p => p.StartLocalDate, m =>
                    {
                        m.Type<LocalDateType>();
                    });
                    c.Property(p => p.FinishLocalDate, m =>
                    {
                        m.Type<LocalDateType>();
                    });
                });

                domainMapper.Class<LocalTimeTestEntity>(c =>
                {
                    c.Id(p => p.Id, m =>
                    {
                        m.Generator(Generators.Native);
                    });
                    c.Property(p => p.Description, m =>
                    {
                        m.Length(100);
                    });
                    c.Property(p => p.StartLocalTime, m =>
                    {
                        m.Type<LocalTimeType>();
                    });
                    c.Property(p => p.FinishLocalTime, m =>
                    {
                        m.Type<LocalTimeType>();
                    });
                });

                domainMapper.Class<LocationTestEntity>(c =>
                {
                    c.Id(p => p.Id, m =>
                    {
                        m.Generator(Generators.Native);
                    });
                    c.Property(p => p.Description, m =>
                    {
                        m.Length(100);
                    });
                    c.Property(p => p.LocationDateTimeZone, m =>
                    {
                        m.Type<DateTimeZoneTzdbType>();
                    });
                });

                domainMapper.Class<OffsetDateTimeTestEntity>(c =>
                {
                    c.Id(p => p.Id, m =>
                    {
                        m.Generator(Generators.Native);
                    });
                    c.Property(p => p.Description, m =>
                    {
                        m.Length(100);
                    });
                    c.Property(p => p.StartOffsetDateTime, m =>
                    {
                        m.Type<OffsetDateTimeType>();
                    });
                    c.Property(p => p.FinishOffsetDateTime, m =>
                    {
                        m.Type<OffsetDateTimeType>();
                    });
                });

                domainMapper.Class<ZonedDateTimeTestEntity>(c =>
                {
                    c.Id(p => p.Id, m =>
                    {
                        m.Generator(Generators.Native);
                    });
                    c.Property(p => p.Description, m =>
                    {
                        m.Length(100);
                    });
                    c.Property(p => p.StartZonedDateTime, m =>
                    {
                        m.Type<ZonedDateTimeTzdbType>();
                        m.Columns(f => f.Name("StartZonedDateTime"), f => f.Name("StartZoneDateTimeTimeZoneId"));
                    });
                    c.Property(p => p.FinishZonedDateTime, m =>
                    {
                        m.Type<ZonedDateTimeTzdbType>();
                        m.Columns(f => f.Name("FinishZonedDateTime"), f => f.Name("FinishZoneDateTimeTimeZoneId"));
                    });
                });

                // Build and the mappings for the test domain entities.
                _configuration.AddMapping(domainMapper.CompileMappingFor(domainTypes));

                _configuration.LinqToHqlGeneratorsRegistry<LinqToHqlGeneratorsRegistry>();

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
