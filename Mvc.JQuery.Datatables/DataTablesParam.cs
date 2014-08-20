using System.Collections.Generic;

namespace Mvc.JQuery.Datatables
{
    /// <summary>
    /// Model binder for datatables.js parameters a la http://geeksprogramando.blogspot.com/2011/02/jquery-datatables-plug-in-with-asp-mvc.html
    /// </summary>

    public class DataTablesParam
    {
        public int iDisplayStart { get; set; }
        public int iDisplayLength { get; set; }
        public int iColumns { get; set; }
        public string sSearch { get; set; }
        public bool bEscapeRegex { get; set; }
        public int iSortingCols { get; set; }
        public int sEcho { get; set; }
        public List<bool> bSortable { get; private set; }
        public List<bool> bSearchable { get; set; }
        public List<string> sSearchColumns { get; set; }
        public List<int> iSortCol { get; private set; }
        public List<string> sSortDir { get; private set; }
        public List<bool> bEscapeRegexColumns { get; private set; }

        /// <summary>
        /// Перечень идентификаторов колонок и их имён.
        /// </summary>
        public Dictionary<string, string> Columns { get; private set; }

        /// <summary>
        /// Если <c>true</c> - используется формат передачи параметров версии 1.9.x.
        /// Если <c>false</c> - используется формат передачи параметров версии 1.10.x.
        /// </summary>
        public bool IsLegacyFormat { get; set; }

        /// <summary>
        /// Если <c>true</c> - используется формат передачи данных по умолчанию в виде массива строк в порядке следования полей.
        /// Если <c>false</c> - данные передаются в виде списка объектов.
        /// Признаком того, что используется DataArray есть отсутствие имён колонок в передавамых параметрах.
        /// </summary>
        public bool UseDataArray { get; set; }

        public DataTablesParam()
        {
            bSortable = new List<bool>();
            bSearchable = new List<bool>();
            sSearchColumns = new List<string>();
            iSortCol = new List<int>();
            sSortDir = new List<string>();
            bEscapeRegexColumns = new List<bool>();

            Columns = new Dictionary<string, string>();
        }

        public DataTablesParam(int iColumns)
        {
            this.iColumns = iColumns;
            bSortable = new List<bool>(iColumns);
            bSearchable = new List<bool>(iColumns);
            sSearchColumns = new List<string>(iColumns);
            iSortCol = new List<int>(iColumns);
            sSortDir = new List<string>(iColumns);
            bEscapeRegexColumns = new List<bool>(iColumns);
            Columns = new Dictionary<string, string>(iColumns);
        }
    }
    //public enum DataType
    //{
    //    tInt,
    //    tString,
    //    tnone
    //}
}