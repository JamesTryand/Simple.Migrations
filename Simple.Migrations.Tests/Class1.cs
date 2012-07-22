using System;
using System.Data.SqlClient;
using Simple.Testing.ClientFramework;
using grensesnitt.Framework;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;
using System.Collections.Generic;


namespace Simple.Migrations.Tests
{
    [SetUpFixture]
    public class TestConfig
    {
        
        [SetUp]
        public void Setup()
        {
            GrensesnittObjectLocator.Register<DatabaseChecker>(() => new DatabaseChecker(""));
        }
    }

    [InterfaceSpecification]
    public class MyClass
    {

    }

    public class DatabaseCheckerTests
    {
        public static string SampleConnectionString = "";

        public Specification CanGetAConnectionToTheDatabase()
        {
            return new QuerySpecification<DatabaseChecker, SqlConnection>()
            {
                On = () => new DatabaseChecker(SampleConnectionString),
                When = sut => sut.GetConnection(),
                Expect =
                {
                    e => e is SqlConnection,
                    e => e != null
                }
            } as Specification;
        }

        public Specification CanGetDatabaseStructure()
        {
            return new QuerySpecification<DatabaseChecker, IEnumerable<TableColumn>>()
            {
                On = () => new DatabaseChecker(SampleConnectionString),
                When = sut => sut.GetDatabaseStructure(),
                Expect = {
                    cols => cols is IEnumerable<TableColumn>,
                    cols => cols != null,
                    cols => cols.Count() > 0,
                },
            };
        }

        public Specification GetsTheStructureAsTables()
        {
            return new QuerySpecification<DatabaseChecker, IEnumerable<TableColumn>>()
            {
                On = () => new DatabaseChecker(SampleConnectionString),
                When = sut => sut.GetDatabaseStructure(),
                Expect = {
                    cols => cols is IEnumerable<TableColumn>,
                    cols => cols != null,
                    cols => cols.Count() > 0,
                },
            };
        }

    }

    public class AssemblyReaderTests
    {
        public Specification AGivenAssemblyCanBeRead() {
            return new QuerySpecification<DatabaseChecker, Assembly>()
            {
                On = () => new DatabaseChecker(""),
                When = sut => sut.GetObjectAssembly(@"C:\Dev\Codebase\Simple.Migrations\Simple.Migrations.Tests.SampleAssembly\bin\Debug\Simple.Migrations.Tests.SampleAssembly.dll"),
                Expect = {
                    asmbly => asmbly != null,
                    asmbly => asmbly is Assembly,
                }
            };
        }

        public Specification CanReturnAllValidClassesForANamespaceWithAPattern()
        {
            return new QuerySpecification<DatabaseChecker, IEnumerable<Type>>()
            {
                On = () => new DatabaseChecker(""),
                When = sut => sut.GetModels("Simple.Migrations.Tests.SampleAssembly", "(.*)View$"),
                Expect = {
                    results => results is IEnumerable<Type>,
                    results => results != null,
                    results => results.Count().Equals(3),
                }
            };
        }
    }

    public class DatabaseChecker
    {
        private string connectionString;
        public DatabaseChecker(string connectionString)
        {
            this.connectionString = connectionString;
        }

        internal SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        internal Assembly GetObjectAssembly(string p)
        {
            return System.Reflection.Assembly.LoadFrom(p);
        }

        internal IEnumerable<TableColumn> GetDatabaseStructure()
        {
            throw new NotImplementedException();
        }

        internal IEnumerable<Type> GetModels(string p, string p_2)
        {
            throw new NotImplementedException();
        }
    }

    public class TableColumn
    {
        public string TABLE_CATALOG { get; set; }
        public string TABLE_SCHEMA { get; set; }
        public string TABLE_NAME { get; set; }
        public string COLUMN_NAME { get; set; }
        public int ORDINAL_POSITION { get; set; }
        public string IS_NULLABLE { get; set; }
        public string DATA_TYPE { get; set; }
        public int CHARACTER_MAXIMUM_LENGTH { get; set; }
    }
}
