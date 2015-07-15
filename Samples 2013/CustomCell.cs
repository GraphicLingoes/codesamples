using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Web.UI;

namespace CommissionFeeViewer
{
    class CustomCell
    {
        private List<RadGrid> RadGrids;
        private Dictionary<string, string> ColumnFormats;
        private Dictionary<string, string> DataFormatExpressions = new Dictionary<string, string>();
        private Dictionary<string, string> DataFormatTypes = new Dictionary<string, string>();
        
        public CustomCell(List<RadGrid> RadGrids, Dictionary<string, string> ColumnFormats)
        {
            this.RadGrids = RadGrids;
            this.ColumnFormats = ColumnFormats;
            //http://davidpenton.com/testsite/scratch/formats.aspx
            this.DataFormatExpressions.Add("date", "{0:MM/dd/yyyy HH:mm:ss}");
            this.DataFormatTypes.Add("date", "System.DateTime");
            this.DataFormatExpressions.Add("number", "{0:N}");
            this.DataFormatTypes.Add("number", "System.Number");
            this.DataFormatExpressions.Add("currency", "{0:C4}");
            this.DataFormatTypes.Add("currency", "System.Decimal");
            this.DataFormatExpressions.Add("currencyTwoDecimalPlaces", "{0:C2}");
            this.DataFormatTypes.Add("currencyTwoDecimalPlaces", "System.Decimal");
            this.DataFormatExpressions.Add("percentage", "{0:P4}");
            this.DataFormatTypes.Add("percentage", "System.Decimal");
        }



        public void format()
        {
            var VarTable = new Dictionary<string, GridBoundColumn>();
            for (var i = 0; i < RadGrids.Count; i++)
            {
                foreach (KeyValuePair<string, string> pair in this.ColumnFormats)
                {
                    try
                    {
                        VarTable["col" + pair.Key + i] = RadGrids[i].MasterTableView.GetColumn(pair.Key) as GridBoundColumn;
                        VarTable["col" + pair.Key + i].DataType = Type.GetType(this.DataFormatTypes[pair.Value]);
                        VarTable["col" + pair.Key + i].DataFormatString = this.DataFormatExpressions[pair.Value];
                    }
                    catch (Exception ex)
                    {
                       // Currenlty we do not need to do anything here since we actually do not care if it can not find the column
                    }
                }
                // Rebind everything
                RadGrids[i].Rebind();
            }
        }
        /// <summary>
        /// Public method used to reset local storage members
        /// </summary>
        public void Reset()
        {
            RadGrids = null;
            ColumnFormats = null;
        }
    }
}
