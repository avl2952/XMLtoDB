using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Xml;

namespace XMLtoDatabase
{
    public partial class Form1 : Form
    {

		#region Declarations

		// Getting connection string from App.config file
		string StrCon = ConfigurationManager.ConnectionStrings["strcon"].ToString();
		private List<string> _xmlFiles = new List<string>();

		#endregion Declarations

		#region Constructor
		public Form1()
        {
            InitializeComponent();
        }
		#endregion Constructor

		#region Event Methods

		//TODO: Optimize code to be able to add more files anytime unless import button is clicked;
		//FileExplorer browser
		private void BtnBrowse_Click(object sender, EventArgs e)
        {
            string message = "Selected files:\n";
            string txtBox = "";
			//if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			//    txtFilePath.Text = OFD.FileName;
			if (OFD.ShowDialog() == DialogResult.OK)
			{
				foreach (string file in OFD.FileNames)
				{
					message += $"{Path.GetFileName(file)} {Environment.NewLine}";
					txtBox += $"{Path.GetFileName(file)}, ";
                    _xmlFiles.Add(file);
				}
				MessageBox.Show(message);
			}
            txtFilePath.Text = txtBox;
		}

		private void BtnImport_Click(object sender, EventArgs e)
        {
			//TODO: insert code to loop through xmlFiles List (or probably no need na? and just use OFD.FileNames) 
			//Used the populated list instead coz para sure duh -- done
			foreach (string file in _xmlFiles)
			{
                string XMlFile = file;

				if (File.Exists(XMlFile))
                {
                    // Conversion Xml file to DataTable
                    DataTable dt = CreateDataTableXML(XMlFile);
                    if (dt.Columns.Count == 0)
                        dt.ReadXml(XMlFile);

                    string Query;

                    //Checks existing table
                    if (!IsTableExisting())
                    {
                        // Creating Query for Table Creation
                        Query = CreateTableQuery(dt);
                    }
                    else
                    {
						Query = CreateInsertIntoQuery(dt);
					}

					SqlConnection con = new SqlConnection(StrCon);
					con.Open();

					//// Deletion of Table if already Exist
					//SqlCommand cmd = new SqlCommand("IF OBJECT_ID('dbo." + dt.TableName + "', 'U') IS NOT NULL DROP TABLE dbo." + dt.TableName + ";", con);
					//cmd.ExecuteNonQuery();

					// Proceed to perform query
					SqlCommand cmd = new SqlCommand(Query, con);
					//int check = cmd.ExecuteNonQuery();
					if (cmd.ExecuteNonQuery() != 0)
					{
						// Copy Data from DataTable to Sql Table
						using (var bulkCopy = new SqlBulkCopy(con.ConnectionString, SqlBulkCopyOptions.KeepIdentity))
						{
							// assuming DataTable column names match SQL Column names. However if column names don't match, just pass in which datatable name matches the SQL column name in Column Mappings
							foreach (DataColumn col in dt.Columns)
							{
                                //TODO: refactor mapping
								bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
							}

							bulkCopy.BulkCopyTimeout = 600;
							bulkCopy.DestinationTableName = dt.TableName;
							bulkCopy.WriteToServer(dt);
						}

						MessageBox.Show("Table Created Successfully"); //TODO: maka one for insert into
					}
					con.Close();
                    
                }
            }

        }

		#endregion Event Methods

		#region  SQL Methods

        /// <summary>
        /// Checks if table is existing
        /// </summary>
		private bool IsTableExisting() 
		{
            //TODO: Insert code that checks database if table is existing. If yes, then use Insert Into query. If not, use create table query
            //throw new NotImplementedException();
            return true;
		}

		// Getting Table Name as Per the Xml File Name
		//TODO: Might not be necessary? Let's Just add another input to ask for a table name
		//and probably database conn details so that we can configure it dynamically? or no?
		public string GetTableName(string file)
        {
            //TODO:  change into a standard naming where we will use string.Contains() to look for key to know which table name to use
            //Option 1: Loop list and perform a string.Contains() ---> downside, we have to manually update list/dictionary if there is new company
            //or if naming is constant, extract comp name using substring then list.Any()
            Dictionary<string, string> CompanyDirectory = new Dictionary<string, string>();
            FileInfo fi = new FileInfo(file);

            string TableName = fi.Name.Replace(fi.Extension, "");


            return TableName;
        }

        //TODO: Create Query for INSERT INTO table
        public string CreateInsertIntoQuery(DataTable table)
        {
            string query = $"INSERT INTO {table.TableName}(";
			progressBar1.Maximum = table.Columns.Count; //TODO: Modify for multiple files coz currently just for one file only
			progressBar1.Value = 0;
			for (int i = 0; i < table.Columns.Count; i++)
			{
				query += "[" + table.Columns[i].ColumnName + "]";
				string columnType = table.Columns[i].DataType.ToString();
				switch (columnType)
				{
					case "System.Int32":
						query += " int ";
						break;
					case "System.Int64":
						query += " bigint ";
						break;
					case "System.Int16":
						query += " smallint";
						break;
					case "System.Byte":
						query += " tinyint";
						break;
					case "System.Decimal":
						query += " decimal ";
						break;
					case "System.DateTime":
						query += " datetime ";
						break;
					case "System.String":
					default:
						query += string.Format(" nvarchar({0}) ", table.Columns[i].MaxLength == -1 ? "max" : table.Columns[i].MaxLength.ToString());
						break;
				}
				if (table.Columns[i].AutoIncrement)
					query += " IDENTITY(" + table.Columns[i].AutoIncrementSeed.ToString() + "," + table.Columns[i].AutoIncrementStep.ToString() + ") ";
				if (!table.Columns[i].AllowDBNull)
					query += " NOT NULL ";
				query += ",";

				Progress();
			}
			return query.Substring(0, query.Length - 1) + "\n)";

		}

        // Getting Query for Table Creation
        public string CreateTableQuery(DataTable table)
        {
            string sqlsc = $"CREATE TABLE " + table.TableName + "(";
            progressBar1.Maximum = table.Columns.Count; //TODO: Modify for multiple files coz currently just for one file only
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

		#endregion  SQL Methods

		#region XML Methods
		// Converst Xml file to DataTable
		public DataTable CreateDataTableXML(string XmlFile)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(XmlFile);

            DataTable Dt = new DataTable();

            try
            {
                Dt.TableName = GetTableName(XmlFile);  //--------> do we need do we need?
                XmlNode Nodes = doc.DocumentElement.ChildNodes.Cast<XmlNode>().ToList()[0]; //---for
                progressBar1.Maximum = Nodes.ChildNodes.Count;
                progressBar1.Value = 0;
                foreach (XmlNode column in Nodes.ChildNodes)
                {
                    Dt.Columns.Add(column.Name, typeof(String));
                    Progress();
                }

                XmlNode File = doc.DocumentElement;
                progressBar1.Maximum = File.ChildNodes.Count;
                progressBar1.Value = 0;
                foreach (XmlNode file in File.ChildNodes)
                {
                    List<string> Valores = file.ChildNodes.Cast<XmlNode>().ToList().Select(x => x.InnerText).ToList();
                    Dt.Rows.Add(Valores.ToArray());
                    Progress();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return Dt;
        }

		#endregion XML Methods

		#region UI
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
		#endregion UI
	}
}
