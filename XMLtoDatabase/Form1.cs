using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Xml;

namespace XMLtoDatabase
{
    public partial class Form1 : Form
    {
        // Getting connection string from App.config file
        string StrCon = ConfigurationManager.ConnectionStrings["strcon"].ToString();

        public Form1()
        {
            InitializeComponent();
        }

        // File Browser Button Click
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtFilePath.Text = OFD.FileName;

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            string XMlFile = txtFilePath.Text;
            if (File.Exists(XMlFile))
            {
                // Conversion Xml file to DataTable
                DataTable dt = CreateDataTableXML(XMlFile);
                if (dt.Columns.Count == 0)
                    dt.ReadXml(XMlFile);

                // Creating Query for Table Creation
                string Query = CreateTableQuery(dt);
                SqlConnection con = new SqlConnection(StrCon);
                con.Open();

                // Deletion of Table if already Exist
                SqlCommand cmd = new SqlCommand("IF OBJECT_ID('dbo." + dt.TableName + "', 'U') IS NOT NULL DROP TABLE dbo." + dt.TableName + ";", con);
                cmd.ExecuteNonQuery();

                // Table Creation
                cmd = new SqlCommand(Query, con);
                int check = cmd.ExecuteNonQuery();
                if (check != 0)
                {
                // Copy Data from DataTable to Sql Table
                using (var bulkCopy = new SqlBulkCopy(con.ConnectionString, SqlBulkCopyOptions.KeepIdentity))
                {
                    // my DataTable column names match my SQL Column names, so I simply made this loop. However if your column names don't match, just pass in which datatable name matches the SQL column name in Column Mappings
                    foreach (DataColumn col in dt.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = dt.TableName;
                    bulkCopy.WriteToServer(dt);
                }

                    MessageBox.Show("Table Created Successfully");
                }
                con.Close();
            }

        }

        // Getting Table Name as Per the Xml File Name
        public string GetTableName(string file)
        {
            FileInfo fi = new FileInfo(file);
            string TableName = fi.Name.Replace(fi.Extension, "");

            return TableName;
        }

        // Getting Query for Table Creation
        public string CreateTableQuery(DataTable table)
        {
            string sqlsc = "CREATE TABLE " + table.TableName + "(";
            progressBar1.Maximum = table.Columns.Count;
            progressBar1.Value = 0;
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sqlsc += "[" + table.Columns[i].ColumnName + "]";
                string columnType = table.Columns[i].DataType.ToString();
                switch (columnType)
                {
                    case "System.Int32":
                        sqlsc += " int ";
                        break;
                    case "System.Int64":
                        sqlsc += " bigint ";
                        break;
                    case "System.Int16":
                        sqlsc += " smallint";
                        break;
                    case "System.Byte":
                        sqlsc += " tinyint";
                        break;
                    case "System.Decimal":
                        sqlsc += " decimal ";
                        break;
                    case "System.DateTime":
                        sqlsc += " datetime ";
                        break;
                    case "System.String":
                    default:
                        sqlsc += string.Format(" nvarchar({0}) ", table.Columns[i].MaxLength == -1 ? "max" : table.Columns[i].MaxLength.ToString());
                        break;
                }
                if (table.Columns[i].AutoIncrement)
                    sqlsc += " IDENTITY(" + table.Columns[i].AutoIncrementSeed.ToString() + "," + table.Columns[i].AutoIncrementStep.ToString() + ") ";
                if (!table.Columns[i].AllowDBNull)
                    sqlsc += " NOT NULL ";
                sqlsc += ",";

                Progress();
            }
            return sqlsc.Substring(0, sqlsc.Length - 1) + "\n)";
        }

        // Conversion Xml file to DataTable
        public DataTable CreateDataTableXML(string XmlFile)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(XmlFile);

            DataTable Dt = new DataTable();

            try
            {
                Dt.TableName = GetTableName(XmlFile);
                XmlNode NodoEstructura = doc.DocumentElement.ChildNodes.Cast<XmlNode>().ToList()[0];
                progressBar1.Maximum = NodoEstructura.ChildNodes.Count;
                progressBar1.Value = 0;
                foreach (XmlNode columna in NodoEstructura.ChildNodes)
                {
                    Dt.Columns.Add(columna.Name, typeof(String));
                    Progress();
                }

                XmlNode Filas = doc.DocumentElement;
                progressBar1.Maximum = Filas.ChildNodes.Count;
                progressBar1.Value = 0;
                foreach (XmlNode Fila in Filas.ChildNodes)
                {
                    List<string> Valores = Fila.ChildNodes.Cast<XmlNode>().ToList().Select(x => x.InnerText).ToList();
                    Dt.Rows.Add(Valores.ToArray());
                    Progress();
                }
            }
            catch (Exception ex)
            {

            }
            return Dt;
        }

        // Show Progress Bar
        public void Progress()
        {
            if (progressBar1.Value < progressBar1.Maximum)
            {
                progressBar1.Value++;
                int percent = (int)(((double)progressBar1.Value / (double)progressBar1.Maximum) * 100);
                progressBar1.CreateGraphics().DrawString(percent.ToString() + "%", new Font("Arial", (float)8.25, FontStyle.Regular), Brushes.Black, new PointF(progressBar1.Width / 2 - 10, progressBar1.Height / 2 - 7));

                Application.DoEvents();
            }
        }


    }
}
