using System;
using System.Linq;

// ReSharper disable InconsistentNaming
namespace Mvc.JQuery.Datatables
{
    public class DataTablesData
    {
        public DataTablesData()
        {
        }

        public DataTablesData(DataTablesLegacyData legacyData)
        {
            draw = legacyData.sEcho;
            recordsTotal = legacyData.iTotalRecords;
            recordsFiltered = legacyData.iTotalDisplayRecords;
            data = legacyData.aaData;
        }

        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public object[] data { get; set; }

        public string error { get; set; }

        public DataTablesData Transform<TData, TTransform>(Func<TData, TTransform> transformRow)
        {
            var res = new DataTablesData
            {
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data.Cast<TData>().Select(transformRow).Cast<object>().ToArray(),
                error = error,
            };
            return res;
        }
    }

}