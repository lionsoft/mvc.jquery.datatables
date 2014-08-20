using System;
using System.Linq;

// ReSharper disable InconsistentNaming
namespace Mvc.JQuery.Datatables
{
    public class DataTablesLegacyData
    {
        internal bool IsLegacyFormat { get; set; }

        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public int sEcho { get; set; }
        public object[] aaData { get; set; }

        public DataTablesLegacyData Transform<TData, TTransform>(Func<TData, TTransform> transformRow)
        {
            var data = new DataTablesLegacyData 
            {
                aaData = aaData.Cast<TData>().Select(transformRow).Cast<object>().ToArray(),
                iTotalDisplayRecords = iTotalDisplayRecords,
                iTotalRecords = iTotalRecords,
                sEcho = sEcho
            };
            return data;
        }
    }
}