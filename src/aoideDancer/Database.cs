using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using ADOX;

namespace aoideDancer
{
    class Database
    {

        public static List<Song> songs = new List<Song>();


        public static OleDbConnection conn;
        public static void init()
        {
            new System.IO.DirectoryInfo(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "aoide\\aoideDancer")).Create();

            //ADOX.CatalogClass cat = new ADOX.CatalogClass();
            //cat.Create("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\\AccessDB\\NewMDB.mdb; Jet OLEDB:Engine Type=5");

            string mdbFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "aoide\\aoideDancer\\aoide.mdb");
            //string mdbFile = "aoide.mdb";
            string connString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}", mdbFile);

            try
            {
                var catType = Type.GetTypeFromProgID("ADOX.Catalog");
                object o = Activator.CreateInstance(catType);
                catType.InvokeMember("Create", System.Reflection.BindingFlags.InvokeMethod, null, o, new object[] { connString });
            }
            catch (Exception ex) { }

            
            conn = new OleDbConnection(connString);

            try
            {
                new OleDbCommand(string.Format("DROP TABLE {0}", "songs_tmp"), conn).ExecuteNonQuery();
            }
            catch (Exception ex) { }
            //new OleDbCommand("CREATE TABLE songs     (id INTEGER, txt CHAR(20))", conn).ExecuteNonQuery();
            //new OleDbCommand("CREATE TABLE songs_tmp (id INTEGER, txt CHAR(20))", conn).ExecuteNonQuery();
            

            

            /*OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM BookTable;", conn);

            myDataSet = new DataSet();
            adapter.Fill(myDataSet, "BookTable");

            // myListBox is a ListBox control.
            // Set the DataContext of the ListBox to myDataSet
            myListBox.DataContext = myDataSet;*/

        }

        public static void init2()
        {

        }

    }
}
