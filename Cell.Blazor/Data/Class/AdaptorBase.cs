using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using Cell.Blazor._Core.Class;
using Cell.Blazor.Data.Interface;
using Microsoft.VisualBasic.CompilerServices;

namespace Cell.Blazor.Data.Class
{
    public class AdaptorBase : IAdaptor
    {
        public AdaptorBase(DataManager dataManager) => DataManager = dataManager;

        public DataManager DataManager { get; set; }

        public bool RunSyncOnce { get; set; }

        public virtual void SetRunSyncOnce(bool runSync) => RunSyncOnce = false;

        public virtual string GetName() => nameof(AdaptorBase);

        public virtual bool IsRemote() => false;

        public virtual void SetModelType(Type type)
        {
        }

        public virtual object ProcessQuery(DataManagerRequest queries) => null;

        public virtual async Task<object> PerformDataOperation<T>(object queries) => await Task.FromResult((object)null);

        public virtual async Task<object> ProcessResponse<T>(object Data, DataManagerRequest queries)
        {
            return await Task.FromResult(Data);
        }

        public virtual async Task<object> ProcessCrudResponse<T>(
          object Data,
          DataManagerRequest queries)
        {
            return await Task.FromResult(Data);
        }

        public virtual object Insert(
          DataManager dataManager,
          object data,
          string tableName = null,
          Query query = null,
          int position = 0)
        {
            return data;
        }

        public virtual object Update(
          DataManager dataManager,
          string keyField,
          object data,
          string tableName = null,
          Query query = null,
          object original = null,
          IDictionary<string, object> updateProperties = null)
        {
            return data;
        }

        public virtual object Remove(
          DataManager dataManager,
          string keyField,
          object value,
          string tableName = null,
          Query query = null)
        {
            return value;
        }

        public virtual object BatchUpdate(
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
            return new
            {
                changed,
                added,
                deleted
            };
        }

        public virtual object Insert(
          DataManager dataManager,
          IDynamicMetaObjectProvider data,
          string tableName = null,
          Query query = null,
          int position = 0)
        {
            return data;
        }

        public virtual object Update(
          DataManager dataManager,
          string keyField,
          IDynamicMetaObjectProvider data,
          string tableName = null,
          Query query = null,
          object original = null)
        {
            return data;
        }

        public virtual object BatchUpdate(
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
            return new
            {
                changed,
                added,
                deleted
            };
        }

        public virtual void AddParams(RequestOptions options, DataManagerRequest queries)
        {
        }

        public virtual void BeforeSend(HttpRequestMessage request)
        {
        }

        public virtual async Task<object> ProcessBatchResponse<T>(
          object Data,
          DataManagerRequest queries)
        {
            return await Task.FromResult(Data);
        }
    }
}