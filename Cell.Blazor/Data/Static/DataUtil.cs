using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Cell.Blazor.Data.Static
{
    public class DataUtil
    {
        public static IDictionary<string, string> odUniOperator =
            (IDictionary<string, string>)new Dictionary<string, string>()
            {
                {"$=", "endswith"},
                {"^=", "startswith"},
                {"*=", "substringof"},
                {"endswith", "endswith"},
                {"startswith", "startswith"},
                {"contains", "substringof"}
            };

        public static IDictionary<string, string> odBiOperator =
            (IDictionary<string, string>) new Dictionary<string, string>()
            {
                {"<", " lt "},
                {">", " gt "},
                {"<=", " le "},
                {">=", " ge "},
                {"==", " eq "},
                {"!=", " ne "},
                {"lessthan", " lt "},
                {"lessthanorequal", " le "},
                {"greaterthan", " gt "},
                {"greaterthanorequal", " ge "}, 
                {"equal", " eq "},
                {"notequal", " ne "}
            };

        public static IDictionary<string, string> Odv4UniOperator =
            (IDictionary<string, string>) new Dictionary<string, string>()
            {
                {"$=", "endswith"},
                {"^=", "startswith"},
                {"*=", "contains"},
                {"endswith", "endswith"},
                {"startswith", "startswith"},
                {"contains", "contains"}
            };

        public static IDictionary<string, string> consts = (IDictionary<string, string>)new Dictionary<string, string>()
    {
      {
        "GroupGuid",
        "{271bbba0-1ee7}"
      }
    };

        public static string GetUrl(string baseUrl, string relativeUrl, string queryParams = null)
        {
            bool flag1 = baseUrl[baseUrl.Length - 1] == '/';
            string str1 = baseUrl;
            string str2 = "";
            if (relativeUrl != "" && relativeUrl != null)
            {
                bool flag2 = relativeUrl != string.Empty && relativeUrl[0] == '/';
                str1 = !(flag1 ^ flag2) ? (flag1 || flag2 ? (!(flag1 & flag2) ? baseUrl + relativeUrl : baseUrl + relativeUrl.Substring(1, relativeUrl.Length - 1)) : baseUrl + "/" + relativeUrl) : baseUrl + relativeUrl;
            }
            if (string.IsNullOrEmpty(queryParams))
                return str1;
            if (str1[str1.Length - 1] != '?' && str1.IndexOf("?") > -1)
                str2 = "&" + queryParams;
            else if (str1.IndexOf("?") < 0 && !string.IsNullOrEmpty(queryParams))
                str2 = "?" + queryParams;
            return str1 + str2;
        }

        public static string GetKeyValue(string key, object value)
        {
            PropertyInfo property = value?.GetType().GetProperty(key);
            Type type = property.PropertyType;
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                type = NullableHelperpublic.GetUnderlyingType(type);
            object obj1 = property.GetValue(value);
            object obj2 = obj1;
            if (type.Name == "DateTime")
                obj2 = (object)((DateTime)obj1).ToString("s", (IFormatProvider)CultureInfo.InvariantCulture);
            return obj2?.ToString();
        }

        public static string ToQueryParams(IDictionary<string, object> Params)
        {
            string[] strArray = new string[Params != null ? Params.Count : 0];
            int num = 0;
            foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>)Params)
            {
                if (keyValuePair.Value != null)
                    strArray[num++] = keyValuePair.Key + "=" + keyValuePair.Value.ToString();
            }
            return string.Join("&", strArray);
        }

        public static IEnumerable<T> GetDistinct<T>(
          IEnumerable<T> dataSource,
          string propertyName)
        {
            List<T> source = new List<T>();
            IDictionary<string, object> dictionary1 = (IDictionary<string, object>)new Dictionary<string, object>();
            foreach (T obj1 in dataSource)
            {
                if (!((object)obj1 is ExpandoObject) || !(obj1 is IDictionary<string, object> dictionary2) || dictionary2.ContainsKey(propertyName))
                {
                    object obj2 = DataUtil.GetObject(propertyName, (object)obj1);
                    string key = obj2 == null ? "null" : obj2.ToString();
                    if (!dictionary1.ContainsKey(key))
                    {
                        dictionary1.Add(key, (object)obj1);
                        source.Add(obj1);
                    }
                }
            }
            return source.AsEnumerable<T>();
        }

        public static int GetValue(int value, object inst) => value;

        public static IEnumerable Group<T>(
          IEnumerable jsonArray,
          string field,
          List<Aggregate> aggregates,
          int level,
          IDictionary<string, string> format)
        {
            if (level == 0)
                level = 1;
            string str = "GroupGuid";
            if (jsonArray.GetType().GetProperty(str) != (PropertyInfo)null && (jsonArray as Syncfusion.Blazor.Data.Group<T>).GroupGuid == DataUtil.consts[str])
            {
                Syncfusion.Blazor.Data.Group<T> group = (Syncfusion.Blazor.Data.Group<T>)jsonArray;
                for (int index = 0; index < group.Count; ++index)
                {
                    group[index].Items = DataUtil.Group<T>(group[index].Items, field, aggregates, level + 1, format);
                    group[index].CountItems = group[index].Items.Cast<object>().ToList<object>().Count;
                }
                ++group.ChildLevels;
                return (IEnumerable)group;
            }
            object[] array = jsonArray.Cast<object>().ToArray<object>();
            IDictionary<object, Syncfusion.Blazor.Data.Group<T>> dictionary1 = (IDictionary<object, Syncfusion.Blazor.Data.Group<T>>)new Dictionary<object, Syncfusion.Blazor.Data.Group<T>>();
            Syncfusion.Blazor.Data.Group<T> group1 = new Syncfusion.Blazor.Data.Group<T>()
            {
                GroupGuid = DataUtil.consts["GroupGuid"],
                Level = level,
                ChildLevels = 0,
                Records = array
            };
            for (int index = 0; index < array.Length; ++index)
            {
                object key = DataUtil.GetVal((IEnumerable)array, index, field);
                if (format != null && format.ContainsKey(field) && (format[field] != null && key != null))
                    key = (object)DataUtil.GetFormattedValue(key, format[field]);
                if (key == null)
                    key = (object)"null";
                if (!dictionary1.ContainsKey(key))
                {
                    dictionary1.Add(key, new Syncfusion.Blazor.Data.Group<T>()
                    {
                        Key = key,
                        CountItems = 0,
                        Level = level,
                        Items = (IEnumerable)new List<T>(),
                        Aggregates = new object(),
                        Field = field
                    });
                    group1.Add(dictionary1[key]);
                }
                dictionary1[key].CountItems = ++dictionary1[key].CountItems;
                (dictionary1[key].Items as List<T>).Add((T)array[index]);
            }
            if (aggregates != null && aggregates.Count > 0)
            {
                for (int index1 = 0; index1 < group1.Count; ++index1)
                {
                    IDictionary<string, object> dictionary2 = (IDictionary<string, object>)new Dictionary<string, object>();
                    group1[index1].Items.Cast<object>().ToArray<object>()[0].GetType();
                    group1[index1].Items = (IEnumerable)(group1[index1].Items as List<T>);
                    for (int index2 = 0; index2 < aggregates.Count; ++index2)
                    {
                        Func<IEnumerable, string, string, object> aggregateFunc = DataUtil.CalculateAggregateFunc();
                        if (aggregateFunc != null)
                            dictionary2[aggregates[index2].Field + " - " + aggregates[index2].Type] = aggregateFunc(group1[index1].Items, aggregates[index2].Field, aggregates[index2].Type);
                    }
                    group1[index1].Aggregates = (object)dictionary2;
                }
            }
            return array.Length != 0 ? (IEnumerable)group1 : (IEnumerable)array;
        }

        public static IDictionary<string, object> PerformAggregation(
          IEnumerable jsonData,
          List<Aggregate> aggregates)
        {
            IDictionary<string, object> dictionary = (IDictionary<string, object>)new Dictionary<string, object>();
            if (jsonData == null || jsonData.Cast<object>().Count<object>() == 0)
                return dictionary;
            IEnumerable enumerable = DataUtil.CastList(jsonData.Cast<object>().ToArray<object>()[0].GetType(), jsonData.Cast<object>().ToList<object>());
            for (int index = 0; index < aggregates.Count; ++index)
            {
                Func<IEnumerable, string, string, object> aggregateFunc = DataUtil.CalculateAggregateFunc();
                if (aggregateFunc != null)
                    dictionary[aggregates[index].Field + " - " + aggregates[index].Type.ToLower()] = aggregateFunc(enumerable, aggregates[index].Field, aggregates[index].Type);
            }
            return dictionary;
        }

        public static IEnumerable CastList(Type type, List<object> items)
        {
            Type type1 = type;
            Type type2 = typeof(Enumerable);
            MethodInfo methodInfo1 = type2.GetMethod("Cast").MakeGenericMethod(type1);
            MethodInfo methodInfo2 = type2.GetMethod("ToList").MakeGenericMethod(type1);
            IEnumerable<object> objects = (IEnumerable<object>)items;
            object obj = methodInfo1.Invoke((object)null, (object[])new IEnumerable<object>[1]
            {
        objects
            });
            return methodInfo2.Invoke((object)null, new object[1]
            {
        obj
            }) as IEnumerable;
        }

        public static object GetVal(IEnumerable jsonData, int index, string field)
        {
            if (jsonData.Cast<object>().Count<object>() <= 0)
                return (object)null;
            return field != null ? DataUtil.GetObject(field, jsonData.Cast<object>().ToArray<object>()[index]) : jsonData.Cast<object>().ToArray<object>()[index];
        }

        public static object GetObject(string nameSpace, object from)
        {
            if (string.IsNullOrEmpty(nameSpace))
                return from;
            if (from == null)
                return (object)null;
            if (from is ExpandoObject)
                return DataUtil.GetExpandoValue((IDictionary<string, object>)(from as ExpandoObject), nameSpace);
            if (from is DynamicObject)
                return DataUtil.GetDynamicValue(from as DynamicObject, nameSpace);
            if (nameSpace.IndexOf(".") == -1)
            {
                if (from.GetType().GetField(nameSpace) != (FieldInfo)null)
                {
                    FieldInfo field = from.GetType().GetField(nameSpace);
                    return (object)field == null ? (object)null : field.GetValue(from);
                }
                PropertyInfo property = from.GetType().GetProperty(nameSpace);
                return (object)property == null ? (object)null : property.GetValue(from);
            }
            object obj = from;
            string[] strArray = nameSpace.Split('.');
            for (int index = 0; index < strArray.Length && obj != null; ++index)
            {
                if (from.GetType().GetField(nameSpace) != (FieldInfo)null)
                {
                    FieldInfo field = obj.GetType().GetField(strArray[index]);
                    return (object)field == null ? (object)null : field.GetValue(obj);
                }
                switch (obj)
                {
                    case ExpandoObject _:
                        obj = DataUtil.GetExpandoValue((IDictionary<string, object>)(obj as ExpandoObject), strArray[index]);
                        break;

                    case DynamicObject _:
                        obj = DataUtil.GetDynamicValue(obj as DynamicObject, strArray[index]);
                        break;

                    default:
                        PropertyInfo property = obj.GetType().GetProperty(strArray[index]);
                        obj = (object)property != null ? property.GetValue(obj) : (object)null;
                        break;
                }
            }
            return obj;
        }

        public static Func<IEnumerable, string, string, object> CalculateAggregateFunc() => (Func<IEnumerable, string, string, object>)((items, property, pd) =>
        {
            string str = pd;
            IQueryable source1 = items.AsQueryable();
            bool flag1 = items.Cast<object>().ToList<object>()[0].GetType().BaseType == typeof(DynamicObject);
            bool flag2 = items.Cast<object>().ToList<object>()[0].GetType() == typeof(ExpandoObject);
            if (flag1 | flag2)
            {
                IQueryable<IDynamicMetaObjectProvider> source2 = items.Cast<IDynamicMetaObjectProvider>().AsQueryable<IDynamicMetaObjectProvider>();
                switch (str)
                {
                    case "Average":
                        if (flag1)
                            return (object)source2.Select<IDynamicMetaObjectProvider, object>((Expression<Func<IDynamicMetaObjectProvider, object>>)(item => DataUtil.GetDynamicValue(item as DynamicObject, property))).ToList<object>().Average<object>((Func<object, double>)(value => Convert.ToDouble(value)));
                        return (object)source2.Select<IDynamicMetaObjectProvider, object>((Expression<Func<IDynamicMetaObjectProvider, object>>)(item => DataUtil.GetExpandoValue(item as ExpandoObject, property))).ToList<object>().Average<object>((Func<object, double>)(value => Convert.ToDouble(value)));

                    case "Count":
                        return (object)source2.Count();

                    case "FalseCount":
                        List<WhereFilter> whereFilter1 = new List<WhereFilter>()
            {
              new WhereFilter()
              {
                Field = property,
                Operator = "equal",
                value = (object) false
              }
            };
                        return (object)DynamicObjectOperation.PerformFiltering(items, whereFilter1, (string)null).Count();

                    case "Max":
                        if (flag1)
                            return source2.Max<IDynamicMetaObjectProvider, object>((Expression<Func<IDynamicMetaObjectProvider, object>>)(item => DataUtil.GetDynamicValue(item as DynamicObject, property)));
                        return source2.Max<IDynamicMetaObjectProvider, object>((Expression<Func<IDynamicMetaObjectProvider, object>>)(item => DataUtil.GetExpandoValue(item as ExpandoObject, property)));

                    case "Min":
                        if (flag1)
                            return source2.Min<IDynamicMetaObjectProvider, object>((Expression<Func<IDynamicMetaObjectProvider, object>>)(item => DataUtil.GetDynamicValue(item as DynamicObject, property)));
                        return source2.Min<IDynamicMetaObjectProvider, object>((Expression<Func<IDynamicMetaObjectProvider, object>>)(item => DataUtil.GetExpandoValue(item as ExpandoObject, property)));

                    case "Sum":
                        if (flag1)
                            return (object)source2.Select<IDynamicMetaObjectProvider, object>((Expression<Func<IDynamicMetaObjectProvider, object>>)(item => DataUtil.GetDynamicValue(item as DynamicObject, property))).ToList<object>().Sum<object>((Func<object, double>)(value => Convert.ToDouble(value)));
                        return (object)source2.Select<IDynamicMetaObjectProvider, object>((Expression<Func<IDynamicMetaObjectProvider, object>>)(item => DataUtil.GetExpandoValue(item as ExpandoObject, property))).ToList<object>().Sum<object>((Func<object, double>)(value => Convert.ToDouble(value)));

                    case "TrueCount":
                        List<WhereFilter> whereFilter2 = new List<WhereFilter>()
            {
              new WhereFilter()
              {
                Field = property,
                Operator = "equal",
                value = (object) true
              }
            };
                        return (object)DynamicObjectOperation.PerformFiltering(items, whereFilter2, (string)null).Count();

                    default:
                        return (object)null;
                }
            }
            else
            {
                switch (str)
                {
                    case "Average":
                        return source1.Average(property);

                    case "Count":
                        return (object)source1.Count();

                    case "FalseCount":
                        return (object)source1.Where(property, (object)false, FilterType.Equals, false).Count();

                    case "Max":
                        return source1.Max(property);

                    case "Min":
                        return source1.Min(property);

                    case "Sum":
                        return source1.Sum(property);

                    case "TrueCount":
                        return (object)source1.Where(property, (object)true, FilterType.Equals, false).Count();

                    default:
                        return (object)null;
                }
            }
        });

        public static object CompareAndRemove(object data1, object original, string key = "")
        {
            if (original == null)
                return data1;
            Type type = data1.GetType();
            foreach (PropertyInfo propertyInfo in (IEnumerable<PropertyInfo>)new List<PropertyInfo>((IEnumerable<PropertyInfo>)type.GetProperties()))
            {
                PropertyInfo property = original.GetType().GetProperty(propertyInfo.Name);
                object data = propertyInfo.GetValue(data1);
                if (data != null && !(data is string) && !data.GetType().GetTypeInfo().IsPrimitive)
                {
                    switch (data)
                    {
                        case TimeSpan _:
                        case Decimal _:
                        case DateTime _:
                        case IEnumerable _:
                        case DateTimeOffset _:
                        case ICollection _:
                        case Guid _:
                            break;

                        default:
                            if (!data.GetType().GetTypeInfo().IsEnum)
                            {
                                DataUtil.CompareAndRemove(data, property.GetValue(original));
                                propertyInfo.GetType();
                                if (new List<PropertyInfo>((IEnumerable<PropertyInfo>)type.GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>)(data2 => data2.Name != "@odata.etag")).Count<PropertyInfo>() == 0)
                                {
                                    propertyInfo.SetValue(data1, (object)null);
                                    continue;
                                }
                                continue;
                            }
                            break;
                    }
                }
                if (propertyInfo.Name != key && propertyInfo.Name != "@odata.etag" && (data != null && data.Equals(property.GetValue(original))))
                    propertyInfo.SetValue(data1, (object)null);
            }
            return data1;
        }

        public static string GetFormattedValue(object value, string format)
        {
            List<string> source = new List<string>()
      {
        "Double",
        "Int64",
        "Int32",
        "Int16",
        "Decimal",
        "Single"
      };
            if (value?.GetType().Name == "DateTime" || value?.GetType().Name == "DateTimeOffset")
                return Intl.GetDateFormat<object>(value, format);
            return value != null && source.Any<string>((Func<string, bool>)(t => value.GetType().Name.Contains(t))) ? Intl.GetNumericFormat<object>(value, format) : (string)value;
        }

        public static IDictionary<string, Type> GetColumnType(
          IEnumerable dataSource,
          bool nullable = true,
          string columnName = null)
        {
            IDictionary<string, Type> dictionary1 = (IDictionary<string, Type>)new Dictionary<string, Type>();
            List<IDynamicMetaObjectProvider> list = dataSource.Cast<IDynamicMetaObjectProvider>().ToList<IDynamicMetaObjectProvider>();
            Type type1 = (Type)null;
            if (list.Count > 0)
                type1 = list[0].GetType();
            if (type1 == (Type)null || type1.IsSubclassOf(typeof(DynamicObject)))
                return (IDictionary<string, Type>)null;
            foreach (IDictionary<string, object> dictionary2 in dataSource.Cast<ExpandoObject>().ToList<ExpandoObject>())
            {
                foreach (string key in (IEnumerable<string>)dictionary2.Keys)
                {
                    object obj = dictionary2[key];
                    if (obj != null)
                    {
                        Type type2 = obj.GetType();
                        if (type2.IsValueType & nullable)
                            type2 = typeof(Nullable<>).MakeGenericType(type2);
                        dictionary1.Add(key, type2);
                    }
                    else
                        dictionary1.Add(key, typeof(object));
                }
                if (dictionary1.Count == dictionary2.Keys.Count)
                    break;
            }
            return dictionary1;
        }

        public static string GetODataUrlKey(object rowData, string keyField, object value = null)
        {
            object obj = value ?? DataUtil.GetObject(keyField, rowData);
            if (obj?.GetType() == typeof(string))
            {
                if (Guid.TryParse((string)obj, out Guid _) || int.TryParse((string)obj, out int _) || Decimal.TryParse((string)obj, out Decimal _) || (obj?.GetType() == (Type)null || double.TryParse((string)obj, out double _)))
                    return string.Format("({0})", obj);
                if (!DateTime.TryParse((string)obj, out DateTime _))
                    return string.Format("('{0}')", obj);
                return Regex.IsMatch(obj.ToString(), "(Z|[+-]\\d{2}:\\d{2})$") ? "(" + DateTimeOffset.Parse((string)obj).ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", (IFormatProvider)CultureInfo.InvariantCulture) + ")" : "(" + Convert.ToDateTime(obj).ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", (IFormatProvider)CultureInfo.InvariantCulture) + ")";
            }
            if (obj?.GetType() == typeof(DateTime))
                return "(" + ((DateTime)obj).ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", (IFormatProvider)CultureInfo.InvariantCulture) + ")";
            return obj?.GetType() == typeof(DateTimeOffset) ? "(" + ((DateTimeOffset)obj).ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", (IFormatProvider)CultureInfo.InvariantCulture) + ")" : string.Format("({0})", obj);
        }

        public static object GetDynamicValue(DynamicObject obj, string name)
        {
            object result = (object)null;
            obj.TryGetMember((GetMemberBinder)new DataMemberBinder(name, false), out result);
            return result;
        }

        public static object GetExpandoValue(IDictionary<string, object> obj, string name)
        {
            object obj1 = (object)null;
            obj.TryGetValue(name, out obj1);
            return obj1;
        }

        public static object UpdateDictionary(IEnumerable<object> ExpandData, string[] columns)
        {
            List<IDictionary<string, object>> source = new List<IDictionary<string, object>>();
            if (ExpandData.Count<object>() == 0)
                return (object)null;
            PropertyInfo[] props = ExpandData != null ? ExpandData.First<object>()?.GetType().GetProperties() : (PropertyInfo[])null;
            foreach (object o in ExpandData)
            {
                string str = Guid.NewGuid().ToString();
                switch (o)
                {
                    case DynamicObject _:
                        ((DynamicObject)o).TrySetMember((SetMemberBinder)new DataSetMemberBinder("BlazId", false), (object)("BlazTempId_" + str));
                        Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
                        foreach (string column in columns)
                        {
                            object result;
                            (o as DynamicObject).TryGetMember((GetMemberBinder)new DataMemberBinder(column, false), out result);
                            dictionary1.Add(column, result);
                        }
                        source.Add((IDictionary<string, object>)dictionary1);
                        continue;
                    case ExpandoObject _:
                        IDictionary<string, object> dict = (IDictionary<string, object>)o;
                        dict.AddOrUpdateDataItem("BlazId", (object)("BlazTempId_" + str));
                        source.Add(dict);
                        continue;
                    default:
                        IDictionary<string, object> dictionary2 = DataUtil.ObjectToDictionary(o, props);
                        dictionary2.AddOrUpdateDataItem("BlazId", (object)("BlazTempId_" + str));
                        source.Add(dictionary2);
                        continue;
                }
            }
            return source.Count<IDictionary<string, object>>() <= 0 ? (object)ExpandData : (object)(IEnumerable<object>)source;
        }

        public static IDictionary<string, object> ObjectToDictionary(
          object o,
          PropertyInfo[] props)
        {
            IDictionary<string, object> dict = (IDictionary<string, object>)new Dictionary<string, object>();
            for (int index = 0; index < props.Length; ++index)
            {
                if (props[index].CanRead && !Attribute.IsDefined((MemberInfo)props[index], typeof(Newtonsoft.Json.JsonIgnoreAttribute)) && !Attribute.IsDefined((MemberInfo)props[index], typeof(System.Text.Json.Serialization.JsonIgnoreAttribute)))
                    dict.AddOrUpdateDataItem(props[index].Name, props[index].GetValue(o, (object[])null));
            }
            return dict;
        }
    }
}