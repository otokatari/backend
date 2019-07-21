using System;
using System.Collections.Generic;
using System.Linq;

namespace OtokatariBackend.Utils.TypeMerger
{
    public class TypeMerger
    {
        public static TSource MergeProperties<TSource,TAppend>(TSource source,TAppend append,params string[] ignore)
        {
            var sourceProps = typeof(TSource).GetProperties().SkipWhile(x => ignore.Contains(x.Name));
            var appendProps = typeof(TAppend).GetProperties().SkipWhile(x => ignore.Contains(x.Name));
            var tupleProps = sourceProps.Join(appendProps,x => x.Name, y => y.Name, (_source,_append) => (_source,_append));
            foreach (var prop in tupleProps)
            {
                prop._source.SetValue(source,prop._append.GetValue(append));
            }
            return source;
        }
    }
}
