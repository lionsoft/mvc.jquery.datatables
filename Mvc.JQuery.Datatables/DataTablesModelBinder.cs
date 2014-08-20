using System.Web.Mvc;

namespace Mvc.JQuery.Datatables
{
    /// <summary>
    /// Model binder for datatables.js parameters a la http://geeksprogramando.blogspot.com/2011/02/jquery-datatables-plug-in-with-asp-mvc.html
    /// </summary>
    public class DataTablesModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProvider = bindingContext.ValueProvider;

            var obj = new DataTablesParam(GetValue<int>(valueProvider, "iColumns"));

            if (obj.iColumns > 0)
            {
                // The legacy params format
                obj.IsLegacyFormat = true;
                var columns = (GetValue<string>(valueProvider, "sColumns") ?? "").Trim(',');
                obj.UseDataArray = string.IsNullOrWhiteSpace(columns);
                obj.iDisplayStart = GetValue<int>(valueProvider, "iDisplayStart");
                obj.iDisplayLength = GetValue<int>(valueProvider, "iDisplayLength");
                obj.sSearch = GetValue<string>(valueProvider, "sSearch");
                obj.bEscapeRegex = GetValue<bool>(valueProvider, "bEscapeRegex");
                obj.iSortingCols = GetValue<int>(valueProvider, "iSortingCols");
                obj.sEcho = GetValue<int>(valueProvider, "sEcho");

                for (int i = 0; i < obj.iColumns; i++)
                {
                    obj.bSortable.Add(GetValue<bool>(valueProvider, "bSortable_" + i));
                    obj.bSearchable.Add(GetValue<bool>(valueProvider, "bSearchable_" + i));
                    obj.sSearchColumns.Add(GetValue<string>(valueProvider, "sSearch_" + i));
                    obj.bEscapeRegexColumns.Add(GetValue<bool>(valueProvider, "bEscapeRegex_" + i));
                    obj.iSortCol.Add(GetValue<int>(valueProvider, "iSortCol_" + i));
                    obj.sSortDir.Add(GetValue<string>(valueProvider, "sSortDir_" + i));
                }
            }
            else
            {
                // The new params format
                obj.iDisplayStart = GetValue<int>(valueProvider, "start");
                obj.iDisplayLength = GetValue<int>(valueProvider, "length");
                obj.sSearch = GetValue<string>(valueProvider, "search[value]");
                obj.bEscapeRegex = GetValue<bool>(valueProvider, "search[regex]");
                obj.sEcho = GetValue<int>(valueProvider, "draw");

                for (var i = 0; i < int.MaxValue; i++)
                {
                    var data = GetValue<string>(valueProvider, string.Format("columns[{0}][data]", i));
                    if (data == null) break;
                    obj.iColumns++;
                    var columnName = GetValue<string>(valueProvider, string.Format("columns[{0}][name]", i));
                    obj.Columns[data] = columnName;
                    obj.UseDataArray = string.IsNullOrWhiteSpace(columnName);

                    obj.bSortable.Add(GetValue<bool>(valueProvider, string.Format("columns[{0}][orderable]", i)));
                    obj.bSearchable.Add(GetValue<bool>(valueProvider, string.Format("columns[{0}][searchable]", i)));
                    obj.sSearchColumns.Add(GetValue<string>(valueProvider, string.Format("columns[{0}][search][value]", i)));
                    obj.bEscapeRegexColumns.Add(GetValue<bool>(valueProvider, string.Format("columns[{0}][search][regex]", i)));

                    var sortingCol = GetValue<int?>(valueProvider, string.Format("order[{0}][column]", i));
                    if (sortingCol.HasValue)
                        obj.iSortingCols++;

                    obj.iSortCol.Add(sortingCol ?? 0);
                    obj.sSortDir.Add(GetValue<string>(valueProvider, string.Format("order[{0}][dir]", i)));
                }
            }
            return obj;            
        }

        private static T GetValue<T>(IValueProvider valueProvider, string key)
        {
            ValueProviderResult valueResult = valueProvider.GetValue(key);
            return (valueResult==null)
                ? default(T)
                : (T)valueResult.ConvertTo(typeof(T));
        }
    }
}