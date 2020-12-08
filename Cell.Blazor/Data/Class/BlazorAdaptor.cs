// Decompiled with JetBrains decompiler
// Type: Syncfusion.Blazor.Data.BlazorAdaptor
// Assembly: Syncfusion.Blazor, Version=18.3.0.52, Culture=neutral, PublicKeyToken=null
// MVID: C2354C79-3B80-43BF-8FE6-0DBACD7553E4
// Assembly location: C:\Users\KhoiNguyenICT\source\repos\BlazorApp4\BlazorApp4\bin\Debug\net5.0\Syncfusion.Blazor.dll

using Cell.Blazor._Core.Class;
using Cell.Blazor._Core.Static;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cell.Blazor.Internal.Class;

#nullable enable

namespace Cell.Blazor.Data.Class
{
    public class BlazorAdaptor : AdaptorBase
    {
        public BlazorAdaptor(DataManager dataManager)
          : base(dataManager)
        {
        }

        public override string GetName() => nameof(BlazorAdaptor);

        public override void SetRunSyncOnce(bool runSync) => this.RunSyncOnce = runSync;

        public override object ProcessQuery(DataManagerRequest queries) => CellBaseUtils.ChangeType((object)queries, typeof(DataManagerRequest));

        public override async Task<object> PerformDataOperation<T>(object query)
        {
            BlazorAdaptor blazorAdaptor = this;
            IEnumerable DataSource = (IEnumerable)blazorAdaptor.DataManager.Json;
            DataManagerRequest queries = (DataManagerRequest)CellBaseUtils.ChangeType(query, typeof(DataManagerRequest));
            DataResult DataObject;
            if (!blazorAdaptor.RunSyncOnce)
                return await Task.Run<object>((Func<object>)(() =>
                {
                    DataObject = this.DataOperationInvoke<T>(DataSource, queries);
                    return !queries.RequiresCounts ? (object)DataObject.Result : (object)DataObject;
                }));
            blazorAdaptor.RunSyncOnce = false;
            DataObject = blazorAdaptor.DataOperationInvoke<T>(DataSource, queries);
            return await Task.FromResult<object>(queries.RequiresCounts ? (object)DataObject : (object)DataObject.Result);
        }

        public DataResult DataOperationInvoke<T>(
          IEnumerable DataSource,
          DataManagerRequest queries)
        {
            DataResult dataResult = new DataResult();
            if (DataSource == null || DataSource.Cast<object>().Count<object>() == 0)
            {
                dataResult.Result = DataSource;
                return dataResult;
            }
            if (!(DataSource.GetType() == typeof(List<ExpandoObject>)) && !(DataSource.GetElementType() == typeof(ExpandoObject)))
            {
                Type type = typeof(IDynamicMetaObjectProvider);
                Type elementType = DataSource.GetElementType();
                Type c = (object)elementType != null ? elementType.BaseType : (Type)null;
                if (!type.IsAssignableFrom(c))
                {
                    List<WhereFilter> where = queries.Where;
                    List<SearchFilter> search = queries.Search;
                    List<Sort> sorted = queries.Sorted;
                }
            }
            DataSource = DynamicObjectOperation.PerformDataOperations(DataSource, queries);
            dataResult.Count = DataSource.Cast<object>().Count<object>();
            IEnumerable jsonData = DataSource;
            if (queries.Skip != 0)
                DataSource = DataOperations.PerformSkip(DataSource, queries.Skip);
            if (queries.Take != 0)
                DataSource = DataOperations.PerformTake(DataSource, queries.Take);
            if (queries.IdMapping != null && queries.Where != null)
                DataSource = this.CollectChildRecords(DataSource, queries);
            List<Aggregate> aggregates = queries.Aggregates;
            List<string> group = queries.Group;
            if (group != null && queries.ServerSideGroup)
            {
                foreach (string field in queries.Group)
                    DataSource = DataUtil.Group<T>(DataSource, field, queries.Aggregates, 0, queries.GroupByFormatter);
                dataResult.Result = DataSource;
            }
            else
                dataResult.Result = (IEnumerable)DataSource.Cast<object>().ToList<object>();
            return dataResult;
        }

        public IEnumerable CollectChildRecords(IEnumerable datasource, DataManagerRequest dm)
        {
            IEnumerable enumerable1 = (IEnumerable)SfBaseUtils.ChangeType((object)datasource, datasource.GetType());
            string idMapping = dm.IdMapping;
            object[] objArray = new object[0];
            if (enumerable1.GetType() == typeof(List<ExpandoObject>) || enumerable1.GetElementType() == typeof(ExpandoObject))
            {
                foreach (IDictionary<string, object> dictionary in datasource.Cast<ExpandoObject>().ToList<ExpandoObject>())
                {
                    object obj = dictionary[idMapping];
                    objArray = ((IEnumerable<object>)objArray).Concat<object>((IEnumerable<object>)new object[1]
                    {
            obj
                    }).ToArray<object>();
                }
            }
            else
            {
                foreach (object obj1 in datasource)
                {
                    object obj2 = obj1.GetType().GetProperty(idMapping).GetValue(obj1);
                    objArray = ((IEnumerable<object>)objArray).Concat<object>((IEnumerable<object>)new object[1]
                    {
            obj2
                    }).ToArray<object>();
                }
            }
            IEnumerable enumerable2 = (IEnumerable)null;
            foreach (object obj in objArray)
            {
                dm.Where[0].value = obj;
                IEnumerable enumerable3 = enumerable1.GetType() == typeof(List<ExpandoObject>) || enumerable1.GetElementType() == typeof(ExpandoObject) ? (IEnumerable)DynamicObjectOperation.PerformFiltering(enumerable1, dm.Where, dm.Where[0].Operator) : DataOperations.PerformFiltering(enumerable1, dm.Where, dm.Where[0].Operator);
                enumerable2 = enumerable2 == null || enumerable2.AsQueryable().Count() == 0 ? enumerable3 : (IEnumerable)((IEnumerable<object>)enumerable2).Concat<object>((IEnumerable<object>)enumerable3);
            }
            if (enumerable2 != null)
            {
                IEnumerable enumerable3 = this.CollectChildRecords(enumerable2, dm);
                if (dm.Sorted != null && dm.Sorted.Count > 0)
                    enumerable3 = enumerable1.GetType() == typeof(List<ExpandoObject>) || enumerable1.GetElementType() == typeof(ExpandoObject) ? (IEnumerable)DynamicObjectOperation.PerformSorting(enumerable3.AsQueryable(), dm.Sorted) : DataOperations.PerformSorting(enumerable3, dm.Sorted);
                datasource = (IEnumerable)((IEnumerable<object>)datasource).Concat<object>((IEnumerable<object>)enumerable3);
            }
            return datasource;
        }

        public override async Task<object> ProcessResponse<T>(
          object Data,
          DataManagerRequest queries)
        {
            if (queries.RequiresCounts)
            {
                List<string> group = queries.Group;
                // ISSUE: explicit non-virtual call
                return (group != null ? (__nonvirtual(group.Count) > 0 ? 1 : 0) : 0) != 0 ? (object)await Task.FromResult<DataResult<object>>(Data as DataResult<object>) : (object)await Task.FromResult<DataResult<object>>(Data as DataResult<object>);
            }
            List<string> group1 = queries.Group;
            // ISSUE: explicit non-virtual call
            return (group1 != null ? (__nonvirtual(group1.Count) > 0 ? 1 : 0) : 0) != 0 ? (object)await Task.FromResult<List<Group<T>>>(((IEnumerable)Data).Cast<Group<T>>().ToList<Group<T>>()) : (object)await Task.FromResult<List<T>>(((IEnumerable)Data).Cast<T>().ToList<T>());
        }

        public override object Insert(
          DataManager dataManager,
          object data,
          string tableName = null,
          Query query = null,
          int position = 0)
        {
            ((IList)(this.DataManager.Json ?? (IEnumerable<object>)Enumerable.Empty<object>().ToList<object>())).Insert(position, data);
            return data;
        }

        public override object Update(
          DataManager dataManager,
          string keyField,
          object data,
          string tableName = null,
          Query query = null,
          object original = null,
          IDictionary<string, object> updateProperties = null)
        {
            IEnumerable<object> json = this.DataManager.Json;
            string str = data.GetType().GetProperty(keyField).GetValue(data).ToString();
            foreach (object obj in (IEnumerable)json)
            {
                if (obj.GetType().GetProperty(keyField).GetValue(obj).ToString() == str)
                {
                    foreach (PropertyInfo property in data.GetType().GetProperties())
                    {
                        if (property.SetMethod != (MethodInfo)null && (updateProperties != null ? (updateProperties.ContainsKey(property.Name) ? 1 : 0) : 1) != 0)
                            obj.GetType().GetProperty(property.Name).SetValue(obj, property.GetValue(data));
                    }
                    data = obj;
                }
            }
            return data;
        }

        public override object Remove(
          DataManager dataManager,
          string keyField,
          object value,
          string tableName = null,
          Query query = null)
        {
            IEnumerable json = (IEnumerable)this.DataManager.Json;
            foreach (object obj in json.Cast<object>().ToList<object>())
            {
                if ((!(obj.GetType() == typeof(ExpandoObject)) ? (!(obj.GetType().BaseType == typeof(DynamicObject)) ? obj.GetType().GetProperty(keyField).GetValue(obj).ToString() : DataUtil.GetDynamicValue(obj as DynamicObject, keyField).ToString()) : ((IDictionary<string, object>)obj)[keyField].ToString()) == value.ToString())
                    (json as IList).Remove(obj);
            }
            return value;
        }

        public override object BatchUpdate(
          DataManager dataManager,
          object changed,
          object added,
          object deleted,
          Utils e,
          string keyField,
          int? dropIndex,
          Query query = null,
          object original = null)
        {
            IEnumerable<object> source = this.DataManager.Json ?? (IEnumerable<object>)Enumerable.Empty<object>().ToList<object>();
            object obj1 = (object)null;
            object obj2 = (object)null;
            object obj3 = (object)null;
            List<object> objectList = new List<object>();
            if (changed != null)
            {
                foreach (object obj4 in (IEnumerable)changed)
                {
                    foreach (object obj5 in (IEnumerable)source)
                    {
                        if (obj5.GetType().GetProperty(keyField).GetValue(obj5).ToString() == obj4.GetType().GetProperty(keyField).GetValue(obj4).ToString())
                        {
                            foreach (PropertyInfo property in obj4.GetType().GetProperties())
                            {
                                if (property.SetMethod != (MethodInfo)null)
                                    obj5.GetType().GetProperty(property.Name).SetValue(obj5, property.GetValue(obj4));
                                objectList.Add(obj5);
                            }
                        }
                    }
                }
                obj1 = (object)objectList;
            }
            if (added != null)
            {
                obj2 = added;
                foreach (object obj4 in (IEnumerable)obj2)
                {
                    if (!dropIndex.HasValue)
                    {
                        ((IList)source).Add(obj4);
                    }
                    else
                    {
                        ((IList)source).Insert(dropIndex.Value, obj4);
                        int? nullable = dropIndex;
                        dropIndex = nullable.HasValue ? new int?(nullable.GetValueOrDefault() + 1) : new int?();
                    }
                }
            }
            if (deleted != null)
            {
                obj3 = deleted;
                List<object> list = source.Cast<object>().ToList<object>();
                foreach (object obj4 in (IEnumerable)obj3)
                {
                    foreach (object obj5 in list)
                    {
                        if (obj5.GetType().GetProperty(keyField).GetValue(obj5).ToString() == obj4.GetType().GetProperty(keyField).GetValue(obj4).ToString())
                            ((IList)source).Remove(obj5);
                    }
                }
            }
            return (object)new
            {
                changedRecords = obj1,
                addedRecords = obj2,
                deletedRecords = obj3
            };
        }

        public static object BatchUpdateArray<T>(
          DataManager dataManager,
          object changed,
          object added,
          object deleted,
          Utils e,
          string keyField,
          int? dropIndex,
          Query query = null,
          object original = null)
        {
            IEnumerable list1 = (IEnumerable)((IEnumerable)(dataManager.Json ?? (IEnumerable<object>)Enumerable.Empty<object>().ToList<object>()) as IEnumerable<T>).ToList<T>();
            object obj1 = (object)null;
            object obj2 = (object)null;
            object obj3 = (object)null;
            List<object> objectList = new List<object>();
            if (changed != null)
            {
                foreach (object obj4 in (IEnumerable)changed)
                {
                    foreach (object obj5 in list1)
                    {
                        if (obj5.GetType().GetProperty(keyField).GetValue(obj5).ToString() == obj4.GetType().GetProperty(keyField).GetValue(obj4).ToString())
                        {
                            foreach (PropertyInfo property in obj4.GetType().GetProperties())
                            {
                                if (property.SetMethod != (MethodInfo)null)
                                    obj5.GetType().GetProperty(property.Name).SetValue(obj5, property.GetValue(obj4));
                                objectList.Add(obj5);
                            }
                        }
                    }
                }
                obj1 = (object)objectList;
            }
            if (added != null)
            {
                obj2 = added;
                foreach (object obj4 in (IEnumerable)obj2)
                {
                    if (!dropIndex.HasValue)
                    {
                        ((IList)list1).Add(obj4);
                    }
                    else
                    {
                        ((IList)list1).Insert(dropIndex.Value, obj4);
                        int? nullable = dropIndex;
                        dropIndex = nullable.HasValue ? new int?(nullable.GetValueOrDefault() + 1) : new int?();
                    }
                }
            }
            if (deleted != null)
            {
                obj3 = deleted;
                List<object> list2 = list1.Cast<object>().ToList<object>();
                foreach (object obj4 in (IEnumerable)obj3)
                {
                    foreach (object obj5 in list2)
                    {
                        if (obj5.GetType().GetProperty(keyField).GetValue(obj5).ToString() == obj4.GetType().GetProperty(keyField).GetValue(obj4).ToString())
                            ((IList)list1).Remove(obj5);
                    }
                }
            }
            IEnumerable array = (IEnumerable)(list1 as IEnumerable<T>).ToArray<T>();
            dataManager.Json = array as IEnumerable<object>;
            return (object)new
            {
                changedRecords = obj1,
                addedRecords = obj2,
                deletedRecords = obj3
            };
        }

        public static object InsertArray<T>(DataManager dataManager, object data, int position)
        {
            IEnumerable list = (IEnumerable)((IEnumerable)(dataManager.Json ?? (IEnumerable<object>)Enumerable.Empty<object>().ToList<object>()) as IEnumerable<T>).ToList<T>();
            ((IList)list).Insert(position, data);
            IEnumerable array = (IEnumerable)(list as IEnumerable<T>).ToArray<T>();
            dataManager.Json = array as IEnumerable<object>;
            return data;
        }

        public static object RemoveArray<T>(DataManager dataManager, string keyField, object value)
        {
            IEnumerable source = (IEnumerable)((IEnumerable)dataManager.Json as IEnumerable<T>).ToList<T>();
            foreach (object obj in source.Cast<object>().ToList<object>())
            {
                if (obj.GetType().GetProperty(keyField).GetValue(obj).ToString() == value.ToString())
                {
                    (source as IList).Remove(obj);
                    source = (IEnumerable)(source as IEnumerable<T>).ToArray<T>();
                    dataManager.Json = source as IEnumerable<object>;
                }
            }
            return value;
        }

        public override object Insert(
          DataManager dataManager,
          IDynamicMetaObjectProvider data,
          string tableName = null,
          Query query = null,
          int position = 0)
        {
            IEnumerable enumerable = (IEnumerable)(this.DataManager.Json ?? (IEnumerable<object>)Enumerable.Empty<object>().ToList<object>());
            IDictionary<string, Type> columnType = DataUtil.GetColumnType(enumerable, false);
            bool flag = typeof(DynamicObject).IsAssignableFrom(enumerable.Cast<object>().ToList<object>().FirstOrDefault<object>()?.GetType());
            if ((enumerable as IEnumerable<object>).Count<object>() != 0)
            {
                if (flag)
                {
                    foreach (string name in (data as DynamicObject).GetDynamicMemberNames().ToArray<string>())
                    {
                        object obj = enumerable.Cast<object>().ToList<object>().FirstOrDefault<object>();
                        object dynamicValue1 = DataUtil.GetDynamicValue(data as DynamicObject, name);
                        object dynamicValue2 = DataUtil.GetDynamicValue(obj as DynamicObject, name);
                        if (dynamicValue1 != null && dynamicValue2 != null)
                            (data as DynamicObject).TrySetMember((SetMemberBinder)new DataSetMemberBinder(name, false), SfBaseUtils.ChangeType(dynamicValue1, dynamicValue2.GetType()));
                        else
                            (data as DynamicObject).TrySetMember((SetMemberBinder)new DataSetMemberBinder(name, false), dynamicValue1);
                    }
                }
                else
                {
                    IDictionary<string, object> dictionary = (IDictionary<string, object>)data;
                    foreach (string key in dictionary.Keys.ToList<string>())
                    {
                        Type conversionType = columnType[key];
                        if (dictionary[key] != null)
                            ((IDictionary<string, object>)data)[key] = SfBaseUtils.ChangeType(dictionary[key], conversionType);
                        else
                            ((IDictionary<string, object>)data)[key] = dictionary[key];
                    }
                }
            }
          ((IList)enumerable).Insert(position, (object)data);
            return (object)data.ToString();
        }

        public override object Update(
          DataManager dataManager,
          string keyField,
          IDynamicMetaObjectProvider data,
          string tableName = null,
          Query query = null,
          object original = null)
        {
            IEnumerable<object> json = this.DataManager.Json;
            string str1 = (string)null;
            IDictionary<string, Type> columnType = DataUtil.GetColumnType((IEnumerable)json, false);
            Type type = data.GetType();
            bool flag = ((object)type != null ? type.BaseType : (Type)null) == typeof(DynamicObject);
            if (flag)
            {
                str1 = DataUtil.GetDynamicValue(data as DynamicObject, keyField).ToString();
            }
            else
            {
                IDictionary<string, object> dictionary = (IDictionary<string, object>)data;
                if (dictionary[keyField] != null)
                    str1 = dictionary[keyField].ToString();
            }
            foreach (object obj in (IEnumerable)json)
            {
                string str2 = (string)null;
                if (flag)
                {
                    str2 = DataUtil.GetDynamicValue(obj as DynamicObject, keyField).ToString();
                }
                else
                {
                    IDictionary<string, object> dictionary = (IDictionary<string, object>)obj;
                    if (dictionary[keyField] != null)
                        str2 = dictionary[keyField].ToString();
                }
                if (str2 == str1)
                {
                    if (flag)
                    {
                        foreach (string name in (data as DynamicObject).GetDynamicMemberNames().ToArray<string>())
                        {
                            if (name != keyField)
                            {
                                object dynamicValue1 = DataUtil.GetDynamicValue(data as DynamicObject, name);
                                object dynamicValue2 = DataUtil.GetDynamicValue(obj as DynamicObject, name);
                                if (dynamicValue1 != null && dynamicValue2 != null)
                                    (obj as DynamicObject).TrySetMember((SetMemberBinder)new DataSetMemberBinder(name, false), SfBaseUtils.ChangeType(dynamicValue1, dynamicValue2.GetType()));
                                else
                                    (obj as DynamicObject).TrySetMember((SetMemberBinder)new DataSetMemberBinder(name, false), dynamicValue1);
                            }
                        }
                    }
                    else
                    {
                        IDictionary<string, object> dictionary = (IDictionary<string, object>)data;
                        int count = dictionary.Keys.Count;
                        List<string> list = dictionary.Keys.ToList<string>();
                        for (int index = 0; index < count; ++index)
                        {
                            string key = list[index];
                            if (((IDictionary<string, object>)obj).ContainsKey(key))
                            {
                                if (dictionary[key] != null)
                                {
                                    Type conversionType = ((IDictionary<string, object>)obj)[key] != null ? ((IDictionary<string, object>)obj)[key].GetType() : columnType[key];
                                    if (conversionType == dictionary[key].GetType())
                                        ((IDictionary<string, object>)obj)[key] = dictionary[key];
                                    else
                                        ((IDictionary<string, object>)obj)[key] = SfBaseUtils.ChangeType(dictionary[key], conversionType);
                                }
                                else
                                    ((IDictionary<string, object>)obj)[key] = dictionary[key];
                            }
                            else
                                ((IDictionary<string, object>)obj).Add(key, dictionary[key]);
                        }
                    }
                    data = (IDynamicMetaObjectProvider)obj;
                }
            }
            return (object)data;
        }

        public override object BatchUpdate(
          DataManager dataManager,
          List<IDynamicMetaObjectProvider> changed,
          List<IDynamicMetaObjectProvider> added,
          List<IDynamicMetaObjectProvider> deleted,
          Utils e,
          string keyField,
          int? dropIndex,
          Query query = null,
          object original = null)
        {
            IEnumerable<object> source = this.DataManager.Json ?? (IEnumerable<object>)Enumerable.Empty<object>().ToList<object>();
            object obj1 = (object)null;
            object obj2 = (object)null;
            object obj3 = (object)null;
            List<object> objectList = new List<object>();
            string str1 = (string)null;
            IDictionary<string, Type> columnType = DataUtil.GetColumnType((IEnumerable)source, false);
            bool flag = typeof(DynamicObject).IsAssignableFrom(source.Cast<object>().ToList<object>().FirstOrDefault<object>()?.GetType());
            if (changed != null)
            {
                foreach (object obj4 in (IEnumerable)changed)
                {
                    foreach (object obj5 in (IEnumerable)source)
                    {
                        string str2 = (string)null;
                        if (flag)
                        {
                            string str3 = DataUtil.GetDynamicValue(obj4 as DynamicObject, keyField)?.ToString();
                            str1 = DataUtil.GetDynamicValue(obj5 as DynamicObject, keyField)?.ToString();
                            string[] array = (obj5 as DynamicObject).GetDynamicMemberNames().ToArray<string>();
                            if (str1 == str3)
                            {
                                foreach (string name in array)
                                {
                                    if (name != keyField && name != "BlazId")
                                    {
                                        object dynamicValue1 = DataUtil.GetDynamicValue(obj4 as DynamicObject, name);
                                        object dynamicValue2 = DataUtil.GetDynamicValue(obj5 as DynamicObject, name);
                                        if (dynamicValue1 != null && dynamicValue2 != null)
                                            (obj5 as DynamicObject).TrySetMember((SetMemberBinder)new DataSetMemberBinder(name, false), SfBaseUtils.ChangeType(dynamicValue1, dynamicValue2.GetType()));
                                        else
                                            (obj5 as DynamicObject).TrySetMember((SetMemberBinder)new DataSetMemberBinder(name, false), dynamicValue1);
                                    }
                                }
                                objectList.Add(obj5);
                            }
                        }
                        else
                        {
                            IDictionary<string, object> dictionary1 = (IDictionary<string, object>)obj4;
                            if (dictionary1[keyField] != null)
                                str2 = dictionary1[keyField].ToString();
                            IDictionary<string, object> dictionary2 = (IDictionary<string, object>)obj5;
                            if (dictionary2[keyField] != null)
                                str1 = dictionary2[keyField].ToString();
                            if (str1 == str2)
                            {
                                foreach (string key in (IEnumerable<string>)dictionary1.Keys)
                                {
                                    if (((IDictionary<string, object>)obj5).ContainsKey(key))
                                    {
                                        if (dictionary1[key] != null)
                                        {
                                            Type conversionType = ((IDictionary<string, object>)obj5)[key] != null ? ((IDictionary<string, object>)obj5)[key].GetType() : columnType[key];
                                            if (conversionType == dictionary1[key].GetType())
                                                ((IDictionary<string, object>)obj5)[key] = dictionary1[key];
                                            else
                                                ((IDictionary<string, object>)obj5)[key] = SfBaseUtils.ChangeType(dictionary1[key], conversionType);
                                        }
                                        else
                                            ((IDictionary<string, object>)obj5)[key] = dictionary1[key];
                                    }
                                }
                                objectList.Add(obj5);
                            }
                        }
                    }
                }
                obj1 = (object)objectList;
            }
            if (added != null)
            {
                obj2 = (object)added;
                foreach (object obj4 in (IEnumerable)obj2)
                {
                    if (flag)
                    {
                        foreach (string name in (obj4 as DynamicObject).GetDynamicMemberNames().ToArray<string>())
                        {
                            object obj5 = source.Cast<object>().ToList<object>().FirstOrDefault<object>();
                            object dynamicValue1 = DataUtil.GetDynamicValue(obj4 as DynamicObject, name);
                            object dynamicValue2 = DataUtil.GetDynamicValue(obj5 as DynamicObject, name);
                            if (dynamicValue1 != null && dynamicValue2 != null)
                                (obj4 as DynamicObject).TrySetMember((SetMemberBinder)new DataSetMemberBinder(name, false), SfBaseUtils.ChangeType(dynamicValue1, dynamicValue2.GetType()));
                            else
                                (obj4 as DynamicObject).TrySetMember((SetMemberBinder)new DataSetMemberBinder(name, false), dynamicValue1);
                        }
                    }
                    else
                    {
                        IDictionary<string, object> dictionary = (IDictionary<string, object>)obj4;
                        foreach (string key in dictionary.Keys.ToList<string>())
                        {
                            if (columnType != null && columnType.ContainsKey(key))
                            {
                                Type conversionType = columnType[key];
                                if (dictionary[key] != null)
                                    ((IDictionary<string, object>)obj4)[key] = SfBaseUtils.ChangeType(dictionary[key], conversionType);
                                else
                                    ((IDictionary<string, object>)obj4)[key] = dictionary[key];
                            }
                        }
                    }
                    if (!dropIndex.HasValue)
                    {
                        ((IList)source).Add(obj4);
                    }
                    else
                    {
                        ((IList)source).Insert(dropIndex.Value, obj4);
                        int? nullable = dropIndex;
                        dropIndex = nullable.HasValue ? new int?(nullable.GetValueOrDefault() + 1) : new int?();
                    }
                }
            }
            if (deleted != null)
            {
                obj3 = (object)deleted;
                List<IDynamicMetaObjectProvider> list = source.Cast<IDynamicMetaObjectProvider>().ToList<IDynamicMetaObjectProvider>();
                foreach (object obj4 in (IEnumerable)obj3)
                {
                    foreach (IDynamicMetaObjectProvider metaObjectProvider in list)
                    {
                        string str2;
                        string str3;
                        if (flag)
                        {
                            str2 = DataUtil.GetDynamicValue(metaObjectProvider as DynamicObject, keyField)?.ToString();
                            str3 = DataUtil.GetDynamicValue(obj4 as DynamicObject, keyField)?.ToString();
                        }
                        else
                        {
                            str2 = ((IDictionary<string, object>)metaObjectProvider)[keyField].ToString();
                            str3 = ((IDictionary<string, object>)obj4)[keyField].ToString();
                        }
                        if (str2 == str3)
                            (source as IList).Remove((object)metaObjectProvider);
                    }
                }
            }
            return (object)new
            {
                changedRecords = obj1,
                addedRecords = obj2,
                deletedRecords = obj3
            };
        }
    }
}