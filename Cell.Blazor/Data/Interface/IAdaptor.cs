using Cell.Blazor._Core.Class;
using Cell.Blazor.Data.Class;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cell.Blazor.Data.Interface
{
    public interface IAdaptor
    {
        string GetName();

        void SetRunSyncOnce(bool runSync);

        object ProcessQuery(DataManagerRequest queries);

        Task<object> ProcessResponse<T>(object Data, DataManagerRequest queries);

        Task<object> ProcessCrudResponse<T>(object Data, DataManagerRequest queries);

        Task<object> PerformDataOperation<T>(object queries);

        bool IsRemote();

        void SetModelType(Type type);

        object Insert(
          DataManager dataManager,
          object data,
          string tableName = null,
          Query query = null,
          int position = 0);

        object Update(
          DataManager dataManager,
          string keyField,
          object data,
          string tableName = null,
          Query query = null,
          object original = null,
          IDictionary<string, object> updateProperties = null);

        object Remove(
          DataManager dataManager,
          string keyField,
          object value,
          string tableName = null,
          Query query = null);

        object BatchUpdate(
          DataManager dataManager,
          object changed,
          object added,
          object deleted,
          Utils e,
          string keyField,
          int? dropIndex,
          Query query = null,
          object original = null);

        object Insert(
          DataManager dataManager,
          IDynamicMetaObjectProvider data,
          string tableName = null,
          Query query = null,
          int position = 0);

        object Update(
          DataManager dataManager,
          string keyField,
          IDynamicMetaObjectProvider data,
          string tableName = null,
          Query query = null,
          object original = null);

        object BatchUpdate(
          DataManager dataManager,
          List<IDynamicMetaObjectProvider> changed,
          List<IDynamicMetaObjectProvider> added,
          List<IDynamicMetaObjectProvider> deleted,
          Utils e,
          string keyField,
          int? dropIndex,
          Query query = null,
          object original = null);

        void AddParams(RequestOptions options, DataManagerRequest queries);

        void BeforeSend(HttpRequestMessage request);

        Task<object> ProcessBatchResponse<T>(object Data, DataManagerRequest queries);
    }
}