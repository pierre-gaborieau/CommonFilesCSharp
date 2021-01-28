using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Common
{

    public class PropertyMap
    {
        public PropertyInfo SourceProperty { get; set; }
        public PropertyInfo TargetProperty { get; set; }
    }

    public static class MappingModel
    {

        private static Dictionary<string, PropertyMap[]> _maps =
            new Dictionary<string, PropertyMap[]>();


        public static IList<PropertyMap> GetMatchingProperties(Type sourceType, Type targetType)
        {
            var sourceProperties = sourceType.GetProperties();
            var targetProperties = targetType.GetProperties();

            var properties = (from s in sourceProperties
                                from t in targetProperties
                                where s.Name == t.Name &&
                                    s.CanRead &&
                                    t.CanWrite &&
                                    s.PropertyType.IsPublic &&
                                    t.PropertyType.IsPublic &&
                                (s.PropertyType == t.PropertyType || 
                                    s.PropertyType == Nullable.GetUnderlyingType(t.PropertyType)
                                ) &&
                            (

                                (s.PropertyType.IsValueType &&

                                t.PropertyType.IsValueType

                                ) ||

                                (s.PropertyType == typeof(string) &&

                                t.PropertyType == typeof(string)

                                )

                            )
                        select new PropertyMap

                        {

                            SourceProperty = s,

                            TargetProperty = t

                        }).ToList();

            return properties;

        }


        public static void AddPropertyMap<T, TU>()
        {
            var props = GetMatchingProperties(typeof(T), typeof(TU));
            var className = GetClassName(typeof(T), typeof(TU));
            try
            {
                var propMap = _maps[className];
            }
            catch
            {
                _maps.Add(className, props.ToArray());
            }
        }


        public static void AddPropertyMap( Type T, Type TU)
        {
            var props = GetMatchingProperties(T, TU);
            var className = GetClassName(T, TU);
            try
            {
                var propMap = _maps[className];
            }
            catch
            {
                _maps.Add(className, props.ToArray());
            }
        }

        public static void CopyProperties(object source, object target)
        {
            MappingModel.AddPropertyMap(source.GetType(), target.GetType());
            MappingModel.CopyMatchingCachedProperties(source, target);
        }

        public static void CopyMatchingCachedProperties(object source, object target)
        {
            

            var className = GetClassName(source.GetType(),
                                            target.GetType());
            var propMap = _maps[className];


            for (var i = 0; i < propMap.Length; i++)
            {
                var prop = propMap[i];
                var sourceValue = prop.SourceProperty.GetValue(source,
                                    null);
                prop.TargetProperty.SetValue(target, sourceValue, null);
            }

        }

        public static object  Mapping(this object source,object target)
        {
            MappingModel.AddPropertyMap(source.GetType(), target.GetType());

            var className = GetClassName(source.GetType(),
                                            target.GetType());
            var propMap = _maps[className];


            for (var i = 0; i < propMap.Length; i++)
            {
                var prop = propMap[i];
                var sourceValue = prop.SourceProperty.GetValue(source,
                                    null);
                prop.TargetProperty.SetValue(target, sourceValue, null);
            }
            return target;

        }

        public static TCible Mapping<TSource, TCible>(this TSource source) 
        {
            MappingModel.AddPropertyMap(source.GetType(), typeof(TCible));

            var className = GetClassName(source.GetType(),
                                            typeof(TCible));
            var propMap = _maps[className];


            TCible target = (TCible)Activator.CreateInstance(typeof(TCible).GenericTypeArguments.First());


            for (var i = 0; i < propMap.Length; i++)
            {
                var prop = propMap[i];
                var sourceValue = prop.SourceProperty.GetValue(source,
                                    null);
                prop.TargetProperty.SetValue(target, sourceValue, null);
            }
            return (TCible)target;

        }

        public static string GetClassName(Type sourceType, Type targetType)
        {
            var className = "Copy_";

            className += sourceType.FullName.Replace(".", "_");

            className += "_";

            className += targetType.FullName.Replace(".", "_");

            return className;

        }

        public static List<TCible> MappingList<TSource, TCible>(this IEnumerable<TSource> source)
        {
            List<TCible> ListeRetour = source
                                        .AsParallel()
                                        .Select(element => JsonConvert.DeserializeObject<TCible>(JsonConvert.SerializeObject(element)))
                                        .ToList();

            return ListeRetour;
        }


    }
}