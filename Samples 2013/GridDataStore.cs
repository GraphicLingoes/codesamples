using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Telerik.Web.UI;

namespace CommissionFeeViewer.app_code
{
    public sealed class GridDataStore
    {
        /// <summary>
        /// Private volatile singleton instance member.
        /// </summary>
        private static volatile GridDataStore instance;
        private static object syncRoot = new object();
        public static Dictionary<string, DataSet> persistentData = new Dictionary<string,DataSet>();
        public static Dictionary<string, DataSet> persistentAppData = new Dictionary<string, DataSet>();
        public static Dictionary<string, DataSet> persistentDashboardData = new Dictionary<string, DataSet>();
        public static List<RadGrid> RadGrids = new List<RadGrid>();
        public static List<string> GridNames = new List<string>();

        // Singleton
        private GridDataStore() { }

        /// <summary>
        /// Singleton instance used to retrieve GridDataStore instance.
        /// </summary>
        public static GridDataStore Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new GridDataStore();
                        }
                    }
                }
                return instance;
            }
        }
        /// <summary>
        /// Public static method used to set persistent data to be used by telerik RadGrid control.
        /// </summary>
        /// <param name="data"></param>
        internal void setData(DataSet DataSets, string key, string type = "report")
        {
            switch (type)
            {
                case "report":
                    persistentData[key] = DataSets;
                    break;
                case "appData":
                    persistentAppData[key] = DataSets;
                    break;
                case "dashboardData":
                    persistentDashboardData[key] = DataSets;
                    break;
                default:
                    persistentData[key] = DataSets;
                    break;
            }
        }

        /// <summary>
        /// Internal method used to set Export dependency variables.
        /// </summary>
        /// <param name="Grids"></param>
        /// <param name="Names"></param>
        internal void setExportDependencies(List<RadGrid> Grids, List<string> Names)
        {
            RadGrids.Clear();
            GridNames.Clear();
            RadGrids = Grids;
            GridNames = Names;
        }

        /// <summary>
        /// Internal method used to see if cached data exists for current report page
        /// </summary>
        /// <param name="reportPageKey"></param>
        /// <returns>bool</returns>
        internal bool _contains(string reportPageKey)
        {
            if (persistentData.ContainsKey(reportPageKey))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Internal method used to retrieve persistent data store.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal DataSet getData(string key, string type="report")
        {
            DataSet retVal;
            switch(type)
            {
                case "report":
                    retVal = persistentData[key];
                    break;
                case "appData":
                    retVal = persistentAppData[key];
                    break;
                case "dashboardData":
                    retVal = persistentDashboardData[key];
                    break;
                default:
                    retVal = persistentData[key];
                    break;
            }
            return retVal;
        }
        /// <summary>
        /// Accessor method used to get RadGrids for excel export.
        /// </summary>
        /// <returns>List\<RadGrids\></returns>
        internal List<RadGrid> getRadGrids()
        {
            return RadGrids;
        }

        /// <summary>
        /// Accessor method used to get GridNames for excel export.
        /// </summary>
        /// <returns>List\<string\></returns>
        internal List<string> getGridNames()
        {
            return GridNames;
        }

        /// <summary>
        /// Internal method used to clear out persistent data to free up memory.
        /// </summary>
        /// <param name="type"></param>
        internal void clearData(string type = "report", string key = null, bool removeAllDashboard = false)
        {
            switch(type)
            {
                case "report":
                    persistentData = new Dictionary<string,DataSet>();
                    RadGrids = new List<RadGrid>();
                    GridNames = new List<string>();
                    break;
                case "appData":
                    persistentAppData = new Dictionary<string, DataSet>();
                    break;
                case "dashboardData":
                    if (removeAllDashboard)
                    {
                        persistentDashboardData = new Dictionary<string, DataSet>();
                    }
                    else
                    {
                        persistentDashboardData.Remove(key);
                    }
                    break;
                default:
                    persistentData = new Dictionary<string,DataSet>();
                    RadGrids = new List<RadGrid>();
                    GridNames = new List<string>();
                    break;
            }
        }

    }
}