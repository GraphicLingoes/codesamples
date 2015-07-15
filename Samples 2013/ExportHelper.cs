using CommissionFeeViewer.app_code;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using xls = Telerik.Web.UI.ExportInfrastructure;

namespace CommissionFeeViewer
{
    public class ExportHelper
    {
        /// <summary>
        /// Stores a list of RadGrids on current page.
        /// </summary>
        protected List<RadGrid> RadGrids = new List<RadGrid>();
        /// <summary>
        /// Stores a list Grid Names used for xls tab names.
        /// </summary>
        protected List<string> GridNames = new List<string>();
        /// <summary>
        /// String DataSet key used to grab data from singleton resource.
        /// </summary>
        protected string DataSetName = null;
        /// <summary>
        /// Selected grid from drop down menu.
        /// </summary>
        protected int SelectedGrid = 0;
        /// <summary>
        /// Byte array used to store xls output.
        /// </summary>
        protected byte[] OutputData;
        /// <summary>
        /// String used for main xls file name on export.
        /// </summary>
        protected string FileName = "";
        /// <summary>
        /// List of string names used to compare which columns actually
        /// exist in rad grid so we know which ones to include in export. 
        /// </summary>
        protected List<string> ColumnNames = new List<string>();

        
        /// <summary>
        /// Constructor currently does nothing.
        /// </summary>
        public ExportHelper(){}

        /// <summary>
        /// SetDependencies method used to set up necessary variables used in export.
        /// </summary>
        /// <param name="RadGrids"></param>
        /// <param name="DataSetName"></param>
        /// <param name="GridNames"></param>
        public void SetDependencies(int SelectedGrid, List<RadGrid> RadGrids, string DataSetName, List<string> GridNames)
        {
            this.SelectedGrid = SelectedGrid;
            this.RadGrids = RadGrids;
            this.DataSetName = DataSetName;
            this.GridNames = GridNames;
        }

        /// <summary>
        /// CreateExport method used to create xls export for one grid or all grids
        /// on page. The output of this method is stored in the Output property of
        /// this class and is accessed by the ExportToExcel method.
        /// </summary>
        public void CreateExport()
        {
            // If selected grid number is less than total radgrids then
            // only exporting one grid, otherwise you are exporting all grids.
            if (SelectedGrid < RadGrids.Count)
            {
                // Grab RadGrid from page
                RadGrid radGrid = RadGrids[SelectedGrid];
                ConfigureExport(radGrid);
                // Set up friendly names for worksheet and file name
                string gridFriendlyName = GridNames[SelectedGrid];
                FileName = gridFriendlyName.Replace(" ", "-");
                // Set custom column export flag
                bool notAllColumns = false;
                // Loop through rad grid and determine which columns are actually showing.
                if (!radGrid.MasterTableView.AutoGenerateColumns)
                {
                    notAllColumns = true;
                    foreach (GridColumn col in radGrid.MasterTableView.Columns)
                    {
                        string DataFieldName = col.UniqueName.ToString();
                        ColumnNames.Add(DataFieldName);
                    }
                }

                // Set up new Export structure to create excel export manually

                xls.ExportStructure structure = new xls.ExportStructure();
                xls.Table table = new xls.Table(gridFriendlyName);
                // Iterate through data to populate excel spreadsheet
                int currentCell = 1;
                int currentRow = 1;
                var data = GridDataStore.Instance.getData(DataSetName);
                var gridData = data.Tables[SelectedGrid];

                foreach (DataRow row in gridData.Rows)
                {
                    table.Rows[currentCell].Height = 20;
                    int i = 0;
                    int j = 0;
                    foreach (var item in row.ItemArray)
                    {
                        DataColumn column = row.Table.Columns[i];
                        string dataType = column.DataType.FullName.ToString();
                        if (!notAllColumns || column.ColumnName.ToString() == ColumnNames[j].ToString())
                        {
                            switch (dataType)
                            {
                                case "System.String":
                                    table.Cells[currentCell, currentRow].Value = item.ToString();
                                    break;
                                case "System.Int32":
                                    int number32;
                                    Int32.TryParse(item.ToString(), out number32);
                                    table.Cells[currentCell, currentRow].Value = number32;
                                    break;
                                case "System.Int64":
                                    long number64;
                                    Int64.TryParse(item.ToString(), out number64);
                                    table.Cells[currentCell, currentRow].Value = number64;
                                    break;
                                case "System.Decimal":
                                    decimal decimalNumber;
                                    decimal.TryParse(item.ToString(), out decimalNumber);
                                    table.Cells[currentCell, currentRow].Value = decimalNumber;
                                    break;
                                case "System.Boolean":
                                    table.Cells[currentCell, currentRow].Value = item;
                                    break;
                                case "System.DateTime":
                                    table.Cells[currentCell, currentRow].Value = item.ToString();
                                    break;
                            }
                            // We only need to increment currentCell and ColumnName if we actually use that column.
                            currentCell++;
                            j++;
                        }
                        // We always need to increment i to make sure we compare the correct value to ColumnNames[j]
                        i++;
                    }
                    currentRow++;
                    currentCell = 1;
                }

                // Add new row for headers
                table.ShiftRowsDown(1, 1);
                // Loop through headers and poulate in table
                int headerCell = 1;
                if (radGrid.MasterTableView.AutoGenerateColumns)
                {
                    foreach (GridColumn col in radGrid.MasterTableView.AutoGeneratedColumns)
                    {
                        int headerTextLength = col.HeaderText.Length;
                        table.Columns[headerCell].Width = headerTextLength * 1.25;
                        table.Cells[headerCell, 1].Style.Font.Bold = true;
                        table.Cells[headerCell, 1].Value = col.HeaderText;
                        headerCell++;
                    }
                }

                if (!radGrid.MasterTableView.AutoGenerateColumns)
                {
                    foreach (GridColumn col in radGrid.MasterTableView.Columns)
                    {
                        int headerTextLength = col.HeaderText.Length;
                        table.Columns[headerCell].Width = headerTextLength * 1.25;
                        table.Cells[headerCell, 1].Style.Font.Bold = true;
                        table.Cells[headerCell, 1].Value = col.HeaderText;
                        headerCell++;
                    }
                }

                structure.Tables.Add(table);
                xls.XlsBiffRenderer renderer = new xls.XlsBiffRenderer(structure);
                OutputData = renderer.Render();
            }
            else
            {
                // Set up new Export structure to create excel export manually
                xls.ExportStructure structure = new xls.ExportStructure();
                RadGrid radGrid;
                FileName = "commissionTransactionDetail";
                var data = GridDataStore.Instance.getData(DataSetName);
                for (var i = 0; i < RadGrids.Count; i++)
                {
                    radGrid = RadGrids[i];
                    ConfigureExport(radGrid);
                    // Set up friendly names for worksheet and file name
                    string gridFriendlyName = GridNames[i];
                    xls.Table table = new xls.Table(gridFriendlyName);
                    // Iterate through data to populate excel spreadsheet
                    int currentCell = 1;
                    int currentRow = 1;
                    // Grab data from singleton
                    var gridData = data.Tables[i];

                    bool notAllColumns = false;
                    // Loop through rad grid and determine which columns are actually showing.
                    if (!radGrid.MasterTableView.AutoGenerateColumns)
                    {
                        notAllColumns = true;
                        ColumnNames = new List<string>();

                        foreach (GridColumn col in radGrid.MasterTableView.Columns)
                        {
                            string DataFieldName = col.UniqueName.ToString();
                            ColumnNames.Add(DataFieldName);
                        }
                    }

                    foreach (DataRow row in gridData.Rows)
                    {
                        table.Rows[currentCell].Height = 20;
                        int j = 0;
                        int k = 0;
                        foreach (var item in row.ItemArray)
                        {
                            DataColumn column = row.Table.Columns[j];
                            string dataType = column.DataType.FullName.ToString();
                            if (!notAllColumns || column.ColumnName.ToString() == ColumnNames[k].ToString())
                            {
                                switch (dataType)
                                {
                                    case "System.String":
                                        table.Cells[currentCell, currentRow].Value = item;
                                        break;
                                    case "System.Int32":
                                        int number32;
                                        Int32.TryParse(item.ToString(), out number32);
                                        table.Cells[currentCell, currentRow].Value = number32;
                                        break;
                                    case "System.Int64":
                                        long number64;
                                        Int64.TryParse(item.ToString(), out number64);
                                        table.Cells[currentCell, currentRow].Value = number64;
                                        break;
                                    case "System.Decimal":
                                        decimal decimalNumber;
                                        decimal.TryParse(item.ToString(), out decimalNumber);
                                        table.Cells[currentCell, currentRow].Value = decimalNumber;
                                        break;
                                    case "System.Boolean":
                                        table.Cells[currentCell, currentRow].Value = item;
                                        break;
                                    case "System.DateTime":
                                        table.Cells[currentCell, currentRow].Value = item.ToString();
                                        break;
                                }
                                // We only need to increment currentCell and ColumnName if we actually use that column.
                                currentCell++;
                                k++;
                            }
                            // We always need to increment i to make sure we compare the correct value to ColumnNames[k]
                            j++;
                        }
                        currentRow++;
                        currentCell = 1;
                    }
                    // Add new row for headers
                    table.ShiftRowsDown(1, 1);
                    // Loop through headers and poulate in table
                    int headerCell = 1;
                    if (radGrid.MasterTableView.AutoGenerateColumns)
                    {
                        foreach (GridColumn col in radGrid.MasterTableView.AutoGeneratedColumns)
                        {
                            int headerTextLength = col.HeaderText.Length;
                            table.Columns[headerCell].Width = headerTextLength * 1.25;
                            table.Cells[headerCell, 1].Style.Font.Bold = true;
                            table.Cells[headerCell, 1].Value = col.HeaderText;
                            headerCell++;
                        }
                    }

                    if (!radGrid.MasterTableView.AutoGenerateColumns)
                    {
                        foreach (GridColumn col in radGrid.MasterTableView.Columns)
                        {
                            int headerTextLength = col.HeaderText.Length;
                            table.Columns[headerCell].Width = headerTextLength * 1.25;
                            table.Cells[headerCell, 1].Style.Font.Bold = true;
                            table.Cells[headerCell, 1].Value = col.HeaderText;
                            headerCell++;
                        }
                    }

                    structure.Tables.Add(table);
                }

                xls.XlsBiffRenderer renderer = new xls.XlsBiffRenderer(structure);
                OutputData = renderer.Render();
            }
        }

        /// <summary>
        /// ConfigureExport method used to set up special configurations for xls export.
        /// </summary>
        /// <param name="radGrid"></param>
        public void ConfigureExport(RadGrid radGrid)
        {
            radGrid.ExportSettings.ExportOnlyData = true;
            radGrid.ExportSettings.IgnorePaging = true;
            radGrid.ExportSettings.OpenInNewWindow = true;
            radGrid.ExportSettings.UseItemStyles = true;
            radGrid.ExportSettings.HideStructureColumns = true;
        }

        /// <summary>
        /// ExportToExcel method used to write output to Response stream with correct headers to prompt
        /// xls save or open dialog.
        /// </summary>
        /// <param name="Response"></param>
        public void ExportToExcel(HttpResponse Response)
        {
            if (FileName != null)
            {
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName + ".xls");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Buffer = false;
                Response.BinaryWrite(OutputData);
            }
        }
    }
}