using System.Web;
using System.Web.Script.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Mvc.JQuery.Datatables
{
    public class DataTablesResult 
    {


        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTransform"></typeparam>
        /// <param name="q">A queryable for the data. The properties of this can be marked up with [DataTablesAttribute] to control sorting/searchability/visibility</param>
        /// <param name="dataTableParam"></param>
        /// <param name="transform">//a transform for custom column rendering e.g. to do a custom date row => new { CreatedDate = row.CreatedDate.ToString("dd MM yy") } </param>
        /// <returns></returns>
        public static DataTablesResult<TSource> Create<TSource, TTransform>(IQueryable<TSource> q, DataTablesParam dataTableParam, Func<TSource, TTransform> transform)
        {
            var result = new DataTablesResult<TSource>(q, dataTableParam);
            if (dataTableParam.UseDataArray)
            {
                result.LegacyData = result.LegacyData
                    .Transform<TSource, Dictionary<string, object>>(row => TransformTypeInfo<TTransform>.MergeToDictionary(transform, row))
                    .Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.TransformDictionary)
                    .Transform<Dictionary<string, object>, object[]>(d => d.Values.ToArray());
            }
            else
            {
                var transformData = result.LegacyData.Transform(transform).aaData;
                for (var i = 0; i < result.LegacyData.aaData.Length; i++)
                {
                    var tb = AnonimousTypeBuilder.CreateTypeBuilder();
                    var data = result.LegacyData.aaData[i];
                    foreach (var pi in data.GetType().GetProperties().Union(transformData[i].GetType().GetProperties()))
                    {
                        tb.AddProperty(pi.Name, typeof(string));
                    }
                    var transData = Activator.CreateInstance(tb.CreateType());
                    foreach (var pi1 in transData.GetType().GetProperties())
                    {
                        var pi = data.GetType().GetProperty(pi1.Name);
                        var piT = transformData[i].GetType().GetProperty(pi1.Name);
                        pi1.SetValue(transData, ((piT == null ? pi.GetValue(data) : piT.GetValue(transformData[i])) ?? "").ToString());
                    }
                    result.LegacyData.aaData[i] = transData;
                }
            }
            return result;
        }


        public static DataTablesResult<TSource> Create<TSource>(IQueryable<TSource> q, DataTablesParam dataTableParam)
        {
            var result = new DataTablesResult<TSource>(q, dataTableParam);
            if (dataTableParam.UseDataArray)
            {
                result.LegacyData = result.LegacyData
                    .Transform<TSource, Dictionary<string, object>>(DataTablesTypeInfo<TSource>.ToDictionary)
                    .Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.TransformDictionary)
                    .Transform<Dictionary<string, object>, object[]>(d => d.Values.ToArray()); ;
            }            
            return result;
        }

        //public static DataTablesResult Create(object queryable, DataTablesParam dataTableParam)
        //{
        //    queryable = ((IEnumerable)queryable).AsQueryable();
        //    var s = "Create";

        //    var openCreateMethod =
        //        typeof(DataTablesResult).GetMethods().Single(x => x.Name == s && x.GetGenericArguments().Count() == 1);
        //    var queryableType = queryable.GetType().GetGenericArguments()[0];
        //    var closedCreateMethod = openCreateMethod.MakeGenericMethod(queryableType);
        //    return (DataTablesResult)closedCreateMethod.Invoke(null, new[] { queryable, dataTableParam });
        //}

        public static DataTablesResult<T> CreateResultUsingEnumerable<T>(IEnumerable<T> q, DataTablesParam dataTableParam)
        {
            return Create(q.AsQueryable(), dataTableParam);
        }

    }


    public class DataTablesResult<TSource> : ActionResult
    {

        public DataTablesLegacyData LegacyData { get; set; }
        public DataTablesData Data { get { return new DataTablesData(LegacyData); } }

        internal DataTablesResult(IQueryable<TSource> q, DataTablesParam dataTableParam)
        {
            LegacyData = GetResults(q, dataTableParam);
        }
/*
        internal DataTablesResult(DataTablesLegacyData data)
        {
            LegacyData = data;
        }
*/

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            HttpResponseBase response = context.HttpContext.Response;

            var scriptSerializer = new JavaScriptSerializer();
            if (LegacyData.IsLegacyFormat)
                response.Write(scriptSerializer.Serialize(LegacyData));
            else
                response.Write(scriptSerializer.Serialize(Data));
        }

        DataTablesLegacyData GetResults(IQueryable<TSource> data, DataTablesParam param)
        {
            var totalRecords = data.Count(); //annoying this, as it causes an extra evaluation..

            var filters = new DataTablesFilter();

            var outputProperties = DataTablesTypeInfo<TSource>.Properties;
            var searchColumns = outputProperties.Select(p => new ColInfo(p.Item1.Name, p.Item1.PropertyType)).ToArray();
            
            var filteredData = filters.FilterPagingSortingSearch(param, data, searchColumns).Skip(param.iDisplayStart);

            var page = (param.iDisplayLength <= 0 ? filteredData : filteredData.Take(param.iDisplayLength)).ToArray();

            var result = new DataTablesLegacyData
            {
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = filteredData.Count() + param.iDisplayStart,
                sEcho = param.sEcho,
                aaData = page.Cast<object>().ToArray(),
                IsLegacyFormat = param.IsLegacyFormat,
            };

            return result;
        }

        
    }
}