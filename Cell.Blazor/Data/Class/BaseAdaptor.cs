using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Cell.Blazor._Core.Abstract;
using Cell.Blazor._Core.Class;
using Cell.Blazor.Data.Interface;
using Cell.Blazor.Internal.Class;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cell.Blazor.Data.Class
{
    public class BaseAdaptor
    {
        public Type GenericType;

        public object ParentComponent;
        public DataManager DataManagerInstance;

        public static JsonSerializerSettings DeserializeSettings = new JsonSerializerSettings
        {
            DateParseHandling = DateParseHandling.DateTimeOffset,
            DateTimeZoneHandling = DateTimeZoneHandling.Local,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static JsonSerializerSettings SerializeSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new DefaultContractResolver()
        };

        public IDataAdaptor Instance { get; set; }

        public BaseAdaptor(Type type, object parentComponent, DataManager DataManagerInstance)
        {
            ParentComponent = parentComponent;
            this.DataManagerInstance = DataManagerInstance;
            if (type != null)
            {
                Instance = (IDataAdaptor)DataManagerInstance.ServiceProvider.GetService(type);
                Instance = Instance == null ? (IDataAdaptor)Activator.CreateInstance(type) : Instance;
                Instance.SetParent(parentComponent as BaseComponent);
            }
            GenericType = ParentComponent.GetType();
            if (GenericType.IsGenericType && GenericType.GetGenericArguments().Length != 0)
                GenericType = GenericType.GetGenericArguments()[0];
            else
                GenericType = null;
        }

        [JSInvokable]
        public async Task<string> BaseRead(string request, string key)
        {
            object obj = await Read(JsonConvert.DeserializeObject<DataManagerRequest>(request), key);
            return obj.GetType().Name == "String" ? obj as string : JsonConvert.SerializeObject(obj, Formatting.Indented, SerializeSettings);
        }

        public async Task<object> Read(DataManagerRequest dataRequest, string key = null)
        {
            Task<object> task = Instance.ReadAsync(dataRequest);
            if (task.Status == TaskStatus.RanToCompletion)
                return await task ?? Instance.Read(dataRequest, key);
            object obj;
            if (task.Status == TaskStatus.Canceled)
                obj = Instance.Read(dataRequest, key);
            else
                obj = await task;
            return obj;
        }

        [JSInvokable]
        public async Task<string> BaseInsert(string baseData, string key) => JsonConvert.SerializeObject(await Insert(!(GenericType != null) ? JsonConvert.DeserializeObject<object>(baseData, DeserializeSettings) : JsonConvert.DeserializeObject(baseData, GenericType, DeserializeSettings), key), Formatting.Indented, SerializeSettings);

        public async Task<object> Insert(object data, string key = null)
        {
            object obj = Instance.Insert(DataManagerInstance, data, key);
            Task<object> task = Instance.InsertAsync(DataManagerInstance, data, key);
            if (task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Canceled)
                obj = await task;
            return obj;
        }

        [JSInvokable]
        public async Task<string> BaseRemove(string baseData, string keyField, string key)
        {
            object data;
            if (GenericType != null)
            {
                PropertyInfo propertyInfo = GenericType.GetProperty(keyField);
                if (propertyInfo == null)
                {
                    foreach (PropertyInfo property in GenericType.GetProperties())
                    {
                        foreach (object customAttribute in property.GetCustomAttributes())
                        {
                            if (customAttribute is JsonPropertyAttribute && (customAttribute as JsonPropertyAttribute).PropertyName == keyField)
                                propertyInfo = property;
                        }
                    }
                }
                Type propertyType = propertyInfo.PropertyType;
                data = propertyType.Name == "Guid" ? JsonConvert.DeserializeObject<Guid>(baseData) : CellBaseUtils.ChangeType(JsonConvert.DeserializeObject(baseData, DeserializeSettings), propertyType);
            }
            else
                data = JsonConvert.DeserializeObject<object>(baseData, DeserializeSettings);
            return JsonConvert.SerializeObject(await Remove(data, keyField, key), Formatting.Indented, SerializeSettings);
        }

        public async Task<object> Remove(object data, string keyField, string key)
        {
            object obj = Instance.Remove(DataManagerInstance, data, keyField, key);
            Task<object> task = Instance.RemoveAsync(DataManagerInstance, data, keyField, key);
            if (task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Canceled)
                obj = await task;
            return obj;
        }

        [JSInvokable]
        public async Task<object> BaseUpdate(string baseData, string keyField, string key) => JsonConvert.SerializeObject(await Update(!(GenericType != null) ? JsonConvert.DeserializeObject<object>(baseData, DeserializeSettings) : JsonConvert.DeserializeObject(baseData, GenericType, DeserializeSettings), keyField, key), Formatting.Indented, SerializeSettings);

        public async Task<object> Update(object data, string keyField, string key)
        {
            object obj = Instance.Update(DataManagerInstance, data, keyField, key);
            Task<object> task = Instance.UpdateAsync(DataManagerInstance, data, keyField, key);
            if (task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Canceled)
                obj = await task;
            return obj;
        }

        [JSInvokable]
        public async Task<object> BaseBatchUpdate(
          string changed,
          string added,
          string deleted,
          string keyField,
          string key,
          int? dropIndex)
        {
            object changedRecords = null;
            object addedRecords = null;
            object deletedRecords = null;
            if (changed != null)
            {
                if (GenericType != null)
                {
                    Type type = typeof(List<>);
                    Type[] typeArray = new Type[1] {GenericType};
                    changedRecords = JsonConvert.DeserializeObject(changed, type.MakeGenericType(typeArray), DeserializeSettings);
                }
                else
                    changedRecords = JsonConvert.DeserializeObject<object>(changed, DeserializeSettings);
            }
            if (added != null)
            {
                if (GenericType != null)
                {
                    Type type = typeof(IEnumerable<>);
                    Type[] typeArray = new Type[1] {GenericType};
                    addedRecords = JsonConvert.DeserializeObject(added, type.MakeGenericType(typeArray), DeserializeSettings);
                }
                else
                    addedRecords = JsonConvert.DeserializeObject<object>(added, DeserializeSettings);
            }
            if (deleted != null)
            {
                if (GenericType != null)
                {
                    Type type = typeof(IEnumerable<>);
                    Type[] typeArray = new Type[1] {GenericType};
                    deletedRecords = JsonConvert.DeserializeObject(deleted, type.MakeGenericType(typeArray), DeserializeSettings);
                }
                else
                    deletedRecords = JsonConvert.DeserializeObject<object>(deleted, DeserializeSettings);
            }
            return JsonConvert.SerializeObject(await BatchUpdate(changedRecords, addedRecords, deletedRecords, keyField, key, dropIndex), Formatting.Indented, SerializeSettings);
        }

        public async Task<object> BatchUpdate(
          object changedRecords,
          object addedRecords,
          object deletedRecords,
          string keyField,
          string key,
          int? dropIndex)
        {
            object obj = Instance.BatchUpdate(DataManagerInstance, changedRecords, addedRecords, deletedRecords, keyField, key, dropIndex);
            Task<object> task = Instance.BatchUpdateAsync(DataManagerInstance, changedRecords, addedRecords, deletedRecords, keyField, key, dropIndex);
            if (task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Canceled)
                obj = await task;
            return obj;
        }
    }
}