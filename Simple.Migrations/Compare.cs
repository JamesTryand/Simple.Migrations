using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.SqlClient;
using Dapper;

namespace Simple.Migrations
{
    public class Compare
    {
        public void Load(string connectionstring)
        {
            var connectionString = "";
            var namespacename = "";
            var assemblypath = "";
            var assembly = Assembly.LoadFrom(assemblypath);
            var querystring = @"
            SELECT 
	TABLE_CATALOG, -- string
	TABLE_SCHEMA, -- string
	TABLE_NAME, -- string
	COLUMN_NAME,  -- string
	ORDINAL_POSITION, --  int
	IS_NULLABLE,  -- string
	DATA_TYPE, -- string
	CHARACTER_MAXIMUM_LENGTH,  -- int
	from INFORMATION_SCHEMA.COLUMNS";

            List<InformationSchemaColumns> dbstructure = null; 
            using (var conn = new SqlConnection(connectionString))
            {
                dbstructure = conn.Query<InformationSchemaColumns>(querystring).ToList();
            }

            var clrTables = assembly.GetTypes().Where(t => t.IsClass && t.Namespace.Equals(namespacename));
            // var clrTableNames = clrTables
            var dbTables = dbstructure.GroupBy(i => i.TABLE_NAME);
            var tbTableNames = dbTables.Select(t => t.TABLE_NAME).Distinct();

            var newTables = clrTables.Where( i => i.Name.Except( dbTables.SelectMany( i => i.Select( j => j.TABLE_NAME ).Distinct() ) //NOT CONVINCED WITH THIS ONE.
            var deadTables = clrTables;

            var furtherInspectionTables = dbTables.Except(newTables).Except(deadTables);

            var dbTablestructure = dbstructure.GroupBy( dbr => dbr.Name);

            var comparison = from clrtable in furtherInspectionTables
            from dbtable in dbTablestructure
                where dbtable.Name.Equals(clrtable.Name)
                    select new { DbTable = dbtable, ClrTable = clrtable};

            //var alteredtables = from comp in comparsion
            //                    from dbcol in comp.DbTable
            //                    from clrcol in comp.ClrTable.GetProperties()
            //                    where clrcol.
            var alteredtables_removed = comparison.Where(c =>
                c.DbTable.Select(d => d.Name).Except(c.ClrTable.GetProperties().Select(p => p.Name)).Count() > 0)
                .Select(t => t);

            var alteredtables_added = comparison.Where(c =>
                c.ClrTable.GetProperties().Select(p => p.Name).Except(c.DbTable.Select(d => d.Name)).Count() > 0)
                .Select(t => t);

            var AdductingTableScripts = new List<string>();
            var AbductingTableScripts = new List<string>();
            var AdductingFieldScripts = new List<string>();
            var AbductingFieldScripts = new List<string>();

            AdductingTableScripts = AddNewTableScripts(newTables);
            AdductingTableScripts = RemoveTableScripts(deadTables);
            AdductingFieldScripts = AddNewFieldScripts(alteredtables_added);
            AbductingFieldScripts = RemoveFieldScripts(alteredtables_removed);


        }
    }
    public class InformationSchemaColumns
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
