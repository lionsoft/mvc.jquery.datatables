using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Mvc.JQuery.Datatables.Serialization;
using Newtonsoft.Json;

namespace Mvc.JQuery.Datatables
{
    public class DataTableConfigVm
    {
        public bool HideHeaders { get; set; }
        IDictionary<string, object> m_JsOptions = new Dictionary<string, object>();

        static DataTableConfigVm()
        {
            DefaultTableClass = "table table-bordered table-striped";
        }

        public static string DefaultTableClass { get; set; }
        public string TableClass { get; set; }

        public DataTableConfigVm(string id, string ajaxUrl, IEnumerable<ColDef> columns)
        {
            AjaxUrl = ajaxUrl;
            Id = id;
            Columns = columns;
            ShowSearch = true;
            ShowPageSizes = true;
            TableTools = true;
            UseLegacyFormat = true;
            UseDataArray = true;
            ColumnFilterVm = new ColumnFilterSettingsVm(this);
        }

        public bool ShowSearch { get; set; }

        public string Id { get; private set; }

        public string AjaxUrl { get; private set; }

        public IEnumerable<ColDef> Columns { get; private set; }

        public IDictionary<string, object> JsOptions { get { return m_JsOptions; } }

        public string JsOptionsString
        {
            get
            {
                return convertDictionaryToJsonBody(JsOptions);
            }
        }

        public string ColumnDefsString
        {
            get
            {
                return convertColumnDefsToJson(Columns);
            }
        }
        public bool ColumnFilter { get; set; }

        public ColumnFilterSettingsVm ColumnFilterVm { get; set; }

        public bool TableTools { get; set; }

        public bool AutoWidth { get; set; }

        

        public string Dom
        {
            get { 
                var sdom = "";
                if (TableTools) sdom += "T<\"clear\">";
                if (ShowPageSizes) sdom += "l";
                if (ShowSearch) sdom += "f";
                sdom += "tipr";
                return sdom;
            }
        }

        public string ColumnSortingString
        {
            get
            {
                return convertColumnSortingToJson(Columns);
            }
        }

        public bool ShowPageSizes { get; set; }

        public bool StateSave { get; set; }

        public string Language { get; set; }

        public string DrawCallback { get; set; }

        /// <summary>
        /// Если <c>true</c> - используется формат передачи параметров версии 1.9.x.
        /// Если <c>false</c> - используется формат передачи параметров версии 1.10.x.
        /// Так как <see cref="ColumnFilter"/> сейчас работает только со старым форматом, в случае, если <see cref="ColumnFilter"/> равен <c>true</c> 
        /// значение <see cref="UseLegacyFormat"/> будет всегда <c>true</c>.
        /// </summary>
        public bool UseLegacyFormat
        {
            get { return _useLegacyFormat || ColumnFilter; }
            set { _useLegacyFormat = value; }
        }

        /// <summary>
        /// Если <c>true</c> - используется формат передачи данных по умолчанию в виде массива строк в порядке следования полей.
        /// Если <c>false</c> - данные передаются в виде списка объектов.
        /// </summary>
        public bool UseDataArray { get; set; }

        private bool _columnFilter;
        private bool _useLegacyFormat;


        public class _FilterOn<TTarget>
        {
            private readonly TTarget _target;
            private readonly ColDef _colDef;

            public _FilterOn(TTarget target, ColDef colDef)
            {
                _target = target;
                _colDef = colDef;

            }

            public TTarget Select(params string[] options)
            {
                _colDef.Filter.type = "select";
                _colDef.Filter.values = options.Cast<object>().ToArray();
                return _target;
            }
            public TTarget NumberRange()
            {
                _colDef.Filter.type = "number-range";
                return _target;
            }

            public TTarget DateRange()
            {
                _colDef.Filter.type = "date-range";
                return _target;
            }

            public TTarget Number()
            {
                _colDef.Filter.type = "number";
                return _target;
            }

            public TTarget CheckBoxes(params string[] options)
            {
                _colDef.Filter.type = "checkbox";
                _colDef.Filter.values = options.Cast<object>().ToArray();
                return _target;
            }

            public TTarget Text()
            {
                _colDef.Filter.type = "text";
                return _target;
            }

            public TTarget None()
            {
                _colDef.Filter = null;
                return _target;
            }
        }
        public _FilterOn<DataTableConfigVm> FilterOn<T>()
        {
            return FilterOn<T>(null); 
        }
        public _FilterOn<DataTableConfigVm> FilterOn<T>(object jsOptions)
        {
            IDictionary<string, object> optionsDict = DataTableConfigVm.convertObjectToDictionary(jsOptions);
            return FilterOn<T>(optionsDict); 
        }
        ////public _FilterOn<DataTableConfigVm> FilterOn<T>(IDictionary<string, object> jsOptions)
        ////{
        ////    return new _FilterOn<DataTableConfigVm>(this, this.FilterTypeRules, (c, t) => t == typeof(T), jsOptions);
        ////}
        public _FilterOn<DataTableConfigVm> FilterOn(string columnName)
        {
            return FilterOn(columnName, null);
        }
        public _FilterOn<DataTableConfigVm> FilterOn(string columnName, object jsOptions)
        {
            IDictionary<string, object> optionsDict = convertObjectToDictionary(jsOptions);
            return FilterOn(columnName, optionsDict); 
        }
        public _FilterOn<DataTableConfigVm> FilterOn(string columnName, IDictionary<string, object> jsOptions)
        {
            var colDef = this.Columns.Single(c => c.Name == columnName);
            if (jsOptions != null)
            {
                foreach (var jsOption in jsOptions)
                {
                    colDef.Filter[jsOption.Key] = jsOption.Value;
                }
            }
            return new _FilterOn<DataTableConfigVm>(this, colDef);
        }

        private static string convertDictionaryToJsonBody(IDictionary<string, object> dict)
        {
            // Converting to System.Collections.Generic.Dictionary<> to ensure Dictionary will be converted to Json in correct format
            var dictSystem = new Dictionary<string, object>(dict);
            var json = JsonConvert.SerializeObject((object)dictSystem, Formatting.None, new RawConverter());
            return json.Substring(1, json.Length - 2);
        }

        private static string convertColumnDefsToJson(IEnumerable<ColDef> columns)
        {
            var nonSortableColumns = columns.Select((x, idx) => x.Sortable ? -1 : idx).Where( x => x > -1).ToArray();
            var nonVisibleColumns = columns.Select((x, idx) => x.Visible ? -1 : idx).Where(x => x > -1).ToArray();
            var nonSearchableColumns = columns.Select((x, idx) => x.Searchable ? -1 : idx).Where(x => x > -1).ToArray();
            var mRenderColumns = columns.Select((x, idx) => string.IsNullOrEmpty(x.MRenderFunction) ? new { x.MRenderFunction, Index = -1 } : new { x.MRenderFunction, Index = idx }).Where(x => x.Index > -1).ToArray();

            var defs = new List<dynamic>();

            if (nonSortableColumns.Any())
                defs.Add(new { bSortable = false, aTargets = nonSortableColumns });
            if (nonVisibleColumns.Any())
                defs.Add(new { bVisible = false, aTargets = nonVisibleColumns });
            if (nonSearchableColumns.Any())
                defs.Add(new { bSearchable = false, aTargets = nonSearchableColumns }); 
            if (mRenderColumns.Any())
                foreach (var mRenderColumn in mRenderColumns)
                {
                    defs.Add(new { mRender = "%" + mRenderColumn.MRenderFunction + "%", aTargets = new[] {mRenderColumn.Index} });
                }

            if (defs.Count > 0)
                return new JavaScriptSerializer().Serialize(defs).Replace("\"%", "").Replace("%\"", "");

            return "[]";
        }

        private static string convertColumnSortingToJson(IEnumerable<ColDef> columns)
        {
            var sortList = columns.Select((c, idx) => c.SortDirection == SortDirection.None ? new dynamic[] { -1, "" } : (c.SortDirection == SortDirection.Ascending ? new dynamic[] { idx, "asc" } : new dynamic[] { idx, "desc" })).Where(x => x[0] > -1).ToArray();

            if (sortList.Length > 0) 
                return new JavaScriptSerializer().Serialize(sortList);

            return "[]";
        }

        private static IDictionary<string, object> convertObjectToDictionary(object obj)
        {
            // Doing this way because RouteValueDictionary converts to Json in wrong format
            return new Dictionary<string, object>(new RouteValueDictionary(obj));
        }
    }
}