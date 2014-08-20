using System;
using System.Collections.Generic;

namespace Mvc.JQuery.Datatables
{
    public class TransformTypeInfo<TTransform>
    {
        public static Dictionary<string, object> MergeToDictionary<TInput>(Func<TInput, TTransform> transformInput, TInput tInput)
        {
            var transform = transformInput(tInput);
            var dict = DataTablesTypeInfo<TInput>.ToDictionary(tInput);
            foreach (var propertyInfo in typeof(TTransform).GetProperties())
            {
                dict[propertyInfo.Name] = propertyInfo.GetValue(transform, null);
            }
            return dict;
        }
    }
}