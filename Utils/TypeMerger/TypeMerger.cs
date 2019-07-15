using System;
using System.Linq;

namespace OtokatariBackend.Utils.TypeMerger
{
    public class TypeMerger
    {
        public static T MergeProperties<T>(T source,T append,params string[] ignore)
        {
            var props = typeof(T).GetProperties().SkipWhile(x => ignore.Contains(x.Name));
            foreach (var prop in props)
            {
                prop.SetValue(source, prop.GetValue(append));
            }
            return source;
        }
    }
}
