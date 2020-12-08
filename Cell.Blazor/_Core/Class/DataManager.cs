using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Cell.Blazor._Core.Abstract;
using Cell.Blazor._Core.Enums;
using Cell.Blazor.Data.Class;
using Cell.Blazor.Data.Interface;
using Cell.Blazor.Internal.Class;
using Microsoft.VisualBasic.CompilerServices;

namespace Cell.Blazor._Core.Class
{
    public class DataManager : ComponentBase, IDisposable
    {
        [JsonIgnore]
        public HttpHandler HttpHandler;

        [Inject]
        protected IJSRuntime jsRuntime { get; set; }

        [Inject]
        public HttpClient httpClient { get; set; }

        [JsonIgnore]
        [Inject]
        public IServiceProvider ServiceProvider { get; set; }

        [JsonIgnore]
        public BaseAdaptor BaseAdaptor { get; set; }

        [Parameter]
        [DefaultValue("")]
        [JsonProperty("url")]
        public string Url { get; set; } = "";

        [Parameter]
        [JsonIgnore]
        [JsonProperty("adaptorInstance")]
        public Type AdaptorInstance { get; set; }

        [Parameter]
        [DefaultValue(Adaptors.BlazorAdaptor)]
        [JsonProperty("adaptor")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Adaptors Adaptor { get; set; } = Adaptors.BlazorAdaptor;

        [Parameter]
        [JsonIgnore]
        [JsonProperty("dataAdaptor")]
        public IAdaptor DataAdaptor { get; set; }

        [JsonIgnore]
        public DotNetObjectReference<object> DotNetObjectRef { get; set; }

        [Parameter]
        [JsonProperty("insertUrl")]
        public string InsertUrl { get; set; }

        [Parameter]
        [JsonProperty("removeUrl")]
        public string RemoveUrl { get; set; }

        [Parameter]
        [JsonProperty("updateUrl")]
        public string UpdateUrl { get; set; }

        [Parameter]
        [JsonProperty("crudUrl")]
        public string CrudUrl { get; set; }

        [Parameter]
        [JsonProperty("batchUrl")]
        public string BatchUrl { get; set; }

        [Parameter]
        [JsonProperty("json")]
        [JsonConverter(typeof(DataSourceTypeConverter))]
        public IEnumerable<object> Json { get; set; }

        [Parameter]
        [JsonProperty("headers")]
        public IDictionary<string, string> Headers { get; set; }

        [Parameter]
        [JsonProperty("accept")]
        public bool Accept { get; set; }

        [Parameter]
        [JsonProperty("data")]
        public object Data { get; set; }

        [Parameter]
        [JsonProperty("timeTillExpiration")]
        public int TimeTillExpiration { get; set; }

        [Parameter]
        [JsonProperty("cachingPageSize")]
        public int CachingPageSize { get; set; }

        [Parameter]
        [JsonProperty("enableCaching")]
        public bool EnableCaching { get; set; }

        [Parameter]
        [JsonProperty("requestType")]
        public string RequestType { get; set; }

        [Parameter]
        [JsonProperty("key")]
        public string Key { get; set; }

        [Parameter]
        [JsonProperty("crossDomain")]
        public bool CrossDomain { get; set; }

        [Parameter]
        [JsonProperty("jsonp")]
        public string Jsonp { get; set; }

        [Parameter]
        [JsonProperty("dataType")]
        public string DataType { get; set; }

        [Parameter]
        [JsonProperty("offline")]
        public bool Offline { get; set; }

        [Parameter]
        [JsonProperty("requiresFormat")]
        public bool RequiresFormat { get; set; }

        [DefaultValue(false)]
        [JsonProperty("isDataManager")]
        public bool IsDataManager { get; set; }

        protected int _Guid { get; set; }

        [JsonProperty("guid")]
        public int Guid
        {
            get
            {
                Random random = new Random();
                if (this._Guid != 0)
                    return this._Guid;
                this._Guid = random.Next(1, 100000);
                return this._Guid;
            }
        }

        [CascadingParameter]
        protected object Parent { get; set; }

        [CascadingParameter]
        protected BaseComponent BaseParent { get; set; }

        [Parameter]
        [JsonIgnore]
        public RenderFragment ChildContent { get; set; }

        [Inject]
        [JsonIgnore]
        private NavigationManager UriHelper { get; set; }

        public string BaseUri { get; set; }

        public DataManager()
        {
            this.InitDataManagerAdaptor();
            this.HttpHandler = new HttpHandler(this.httpClient);
        }

        protected override async Task OnInitializedAsync()
        {
            DataManager dataManager = this;
            dataManager.BaseUri = dataManager.UriHelper.BaseUri;
            dataManager.InitDataManagerAdaptor();
            dataManager.HttpHandler = new HttpHandler(dataManager.httpClient);
            dataManager.IsDataManager = true;
            if (dataManager.Parent == null)
                return;
            PropertyInfo property1 = dataManager.Parent.GetType().GetProperty(nameof(DataManager));
            if ((object)property1 != null)
                property1.SetValue(dataManager.Parent, (object)dataManager);
            PropertyInfo property2 = dataManager.Parent.GetType().GetProperty("jsProperty", BindingFlags.Instance | BindingFlags.NonPublic);
            string str = ((object)property2 != null ? property2.GetValue(dataManager.Parent).ToString() : (string)null) ?? string.Empty;
            BaseComponent baseComponent = (BaseComponent)null;
            if (string.IsNullOrEmpty(str))
            {
                if (dataManager.Adaptor == Adaptors.CustomAdaptor)
                    dataManager.BaseAdaptor = new BaseAdaptor(dataManager.AdaptorInstance, dataManager.Parent, dataManager);
                if (!(dataManager.Parent is CellDataBoundComponent parent) || !parent.IsRendered || parent.PropertyChanges.ContainsKey("DataSource"))
                    return;
                parent.PropertyChanges.Add("DataSource", (object)dataManager);
                await parent.OnPropertyChanged();
            }
            else
            {
                if (!str.Contains("sf."))
                {
                    baseComponent = (BaseComponent)dataManager.Parent.GetType().GetProperty("BaseParent", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic).GetValue(dataManager.Parent);
                    baseComponent.DataManagerContainer[str + ".dataSource"] = dataManager;
                }
                if (dataManager.Adaptor == Adaptors.CustomAdaptor)
                {
                    dataManager.BaseAdaptor = new BaseAdaptor(dataManager.AdaptorInstance, dataManager.Parent, dataManager);
                    dataManager.DotNetObjectRef = DotNetObjectReference.Create<object>((object)dataManager.BaseAdaptor);
                    if (!str.Contains("sf."))
                        baseComponent.ChildDotNetObjectRef.Add(dataManager.Guid.ToString(), (object)dataManager.DotNetObjectRef);
                    else
                        dataManager.BaseParent.ChildDotNetObjectRef.Add(dataManager.Guid.ToString(), (object)dataManager.DotNetObjectRef);
                    dataManager.BaseParent.updateDictionary("dataSource_custom", (object)dataManager, dataManager.BaseParent.BindableProperties);
                }
                object obj = dataManager.Adaptor == Adaptors.JsonAdaptor ? (object)dataManager : (object)new DefaultAdaptor(str + ".dataSource", dataManager, dataManager.Adaptor);
                if (!str.Contains("sf."))
                    baseComponent.updateDictionary(str + ".dataSource", obj, baseComponent.BindableProperties);
                else
                    dataManager.BaseParent.updateDictionary("dataSource", obj, dataManager.BaseParent.BindableProperties);
            }
        }

        public bool ShouldSerializeJson() => this.Adaptor == Adaptors.JsonAdaptor;

        public async Task<object> ExecuteQuery<T>(Query query) => await this.ExecuteQuery<T>(query.Queries);

        public async Task<object> ExecuteQuery<T>(DataManagerRequest queries)
        {
            if (this.Adaptor == Adaptors.CustomAdaptor)
                return await this.BaseAdaptor.Read(queries);
            if (this.DataAdaptor != null && this.DataAdaptor.IsRemote())
            {
                if (this.Offline)
                    return await this.ProcessOffline<T>(queries);
                this.DataAdaptor.SetModelType(typeof(T));
                HttpRequestMessage request1 = this.HttpHandler.PrepareRequest(this.DataAdaptor?.ProcessQuery(queries) as RequestOptions);
                this.BeforeSend(request1);
                return await this.DataAdaptor?.ProcessResponse<T>(await this.DataAdaptor?.PerformDataOperation<T>((object)request1), queries);
            }
            DataManagerRequest request = (DataManagerRequest)this.DataAdaptor?.ProcessQuery(queries);
            return await this.DataAdaptor?.ProcessResponse<T>(await this.DataAdaptor?.PerformDataOperation<T>((object)request), request);
        }

        public async Task<object> ProcessOffline<T>(DataManagerRequest queries)
        {
            DataManager dataManager = this;
            DataManagerRequest query = new DataManagerRequest();
            object obj1 = dataManager.DataAdaptor.ProcessQuery(query);
            HttpRequestMessage request = dataManager.HttpHandler.PrepareRequest(obj1 as RequestOptions);
            dataManager.BeforeSend(request);
            object Data1 = await dataManager.DataAdaptor.PerformDataOperation<T>((object)request);
            object obj2 = await dataManager.DataAdaptor.ProcessResponse<T>(Data1, query);
            dataManager.Json = (IEnumerable<object>)obj2;
            dataManager.DataAdaptor = (IAdaptor)new BlazorAdaptor(dataManager);
            DataManagerRequest request1 = (DataManagerRequest)dataManager.DataAdaptor.ProcessQuery(queries);
            object Data2 = await dataManager.DataAdaptor.PerformDataOperation<T>((object)request1);
            object obj3 = await dataManager.DataAdaptor.ProcessResponse<T>(Data2, request1);
            query = (DataManagerRequest)null;
            request1 = (DataManagerRequest)null;
            return obj3;
        }

        public void BeforeSend(HttpRequestMessage request)
        {
            this.DataAdaptor.BeforeSend(request);
            IDictionary<string, string> headers = this.Headers;
            if ((headers != null ? (headers.Count > 0 ? 1 : 0) : 0) == 0)
                return;
            foreach (KeyValuePair<string, string> header in (IEnumerable<KeyValuePair<string, string>>)this.Headers)
                request.Headers.Add(header.Key, header.Value);
        }

        public void InitDataManagerAdaptor()
        {
            switch (this.Adaptor)
            {
                case Adaptors.ODataAdaptor:
                    this.DataAdaptor = (IAdaptor)new ODataAdaptor(this);
                    break;

                case Adaptors.ODataV4Adaptor:
                    this.DataAdaptor = (IAdaptor)new ODataV4Adaptor(this);
                    break;

                case Adaptors.UrlAdaptor:
                    this.DataAdaptor = (IAdaptor)new UrlAdaptor(this);
                    break;

                case Adaptors.WebApiAdaptor:
                    this.DataAdaptor = (IAdaptor)new WebApiAdaptor(this);
                    break;

                case Adaptors.RemoteSaveAdaptor:
                    break;

                default:
                    if (!string.IsNullOrEmpty(this.Url))
                    {
                        this.DataAdaptor = (IAdaptor)new ODataAdaptor(this);
                        break;
                    }
                    this.DataAdaptor = (IAdaptor)new BlazorAdaptor(this);
                    break;
            }
        }

        public async Task<object> Insert<T>(
          object data,
          string tableName = null,
          Query query = null,
          int position = 0)
        {
            DataManager dataManager = this;
            if (dataManager.Adaptor == Adaptors.CustomAdaptor)
                return await dataManager.BaseAdaptor.Insert(data);
            if (!dataManager.DataAdaptor.IsRemote())
                return !typeof(IDynamicMetaObjectProvider).IsAssignableFrom(data.GetType()) ? (!(dataManager.Json is Array) ? dataManager.DataAdaptor.Insert(dataManager, data, tableName, query, position) : BlazorAdaptor.InsertArray<T>(dataManager, data, position)) : dataManager.DataAdaptor.Insert(dataManager, data as IDynamicMetaObjectProvider, tableName, query, position);
            object obj = dataManager.DataAdaptor.Insert(dataManager, data, tableName, query, position);
            HttpRequestMessage request = dataManager.HttpHandler.PrepareRequest(obj as RequestOptions);
            dataManager.BeforeSend(request);
            object Data = await dataManager.DataAdaptor.PerformDataOperation<T>((object)request);
            return await dataManager.DataAdaptor.ProcessCrudResponse<T>(Data, query?.Queries);
        }

        public async Task<object> Update<T>(
          string keyField,
          object data,
          string tableName = null,
          Query query = null,
          object original = null,
          IDictionary<string, object> updateProperties = null)
        {
            DataManager dataManager = this;
            if (dataManager.Adaptor == Adaptors.CustomAdaptor)
                return await dataManager.BaseAdaptor.Update(data, keyField, (string)null);
            if (!dataManager.DataAdaptor.IsRemote())
                return !typeof(IDynamicMetaObjectProvider).IsAssignableFrom(data.GetType()) ? dataManager.DataAdaptor.Update(dataManager, keyField, data, tableName, query, original, updateProperties) : dataManager.DataAdaptor.Update(dataManager, keyField, (IDynamicMetaObjectProvider)data, (string)null, (Query)null, (object)null);
            object obj = dataManager.DataAdaptor.Update(dataManager, keyField, data, tableName, query, original, updateProperties);
            HttpRequestMessage request = dataManager.HttpHandler.PrepareRequest(obj as RequestOptions);
            dataManager.BeforeSend(request);
            object Data = await dataManager.DataAdaptor.PerformDataOperation<T>((object)request);
            return await dataManager.DataAdaptor.ProcessCrudResponse<T>(Data, query?.Queries);
        }

        public async Task<object> Remove<T>(
          string keyField,
          object value,
          string tableName = null,
          Query query = null)
        {
            DataManager dataManager = this;
            if (dataManager.Adaptor == Adaptors.CustomAdaptor)
                return await dataManager.BaseAdaptor.Remove(value, keyField, (string)null);
            if (!dataManager.DataAdaptor.IsRemote())
                return !(dataManager.Json is Array) ? dataManager.DataAdaptor.Remove(dataManager, keyField, value) : BlazorAdaptor.RemoveArray<T>(dataManager, keyField, value);
            object obj = dataManager.DataAdaptor.Remove(dataManager, keyField, value, tableName, query);
            HttpRequestMessage request = dataManager.HttpHandler.PrepareRequest(obj as RequestOptions);
            dataManager.BeforeSend(request);
            object Data = await dataManager.DataAdaptor.PerformDataOperation<T>((object)request);
            return await dataManager.DataAdaptor.ProcessCrudResponse<T>(Data, query?.Queries);
        }

        public async Task<object> SaveChanges<T>(
          object changed,
          object added,
          object deleted,
          string keyField,
          int? dropIndex,
          string tableName = null,
          Query query = null,
          object Original = null)
        {
            DataManager dataManager = this;
            Utils e = new Utils()
            {
                Url = tableName,
                Key = keyField
            };
            if (dataManager.Adaptor == Adaptors.CustomAdaptor)
                return await dataManager.BaseAdaptor.BatchUpdate(changed, added, deleted, keyField, (string)null, dropIndex);
            if (dataManager.DataAdaptor.IsRemote())
            {
                object obj = dataManager.DataAdaptor.BatchUpdate(dataManager, changed, added, deleted, e, keyField, dropIndex, query, Original);
                object queries = dataManager.DataAdaptor.GetName() == "ODataAdaptor" || dataManager.DataAdaptor.GetName() == "ODataV4Adaptor" ? (object)dataManager.HttpHandler.PrepareBatchRequest(obj as RequestOptions) : (object)dataManager.HttpHandler.PrepareRequest(obj as RequestOptions);
                dataManager.BeforeSend(queries as HttpRequestMessage);
                object Data = await dataManager.DataAdaptor.PerformDataOperation<T>(queries);
                return dataManager.DataAdaptor.GetName() == "ODataAdaptor" || dataManager.DataAdaptor.GetName() == "ODataV4Adaptor" ? await dataManager.DataAdaptor.ProcessBatchResponse<T>(Data, query?.Queries) : await dataManager.DataAdaptor.ProcessCrudResponse<CRUDModel<T>>(Data, query?.Queries);
            }
            return !typeof(IDynamicMetaObjectProvider).IsAssignableFrom((((changed as IList).Count <= 0 ? ((added as IList).Count <= 0 ? deleted : added) : changed) as IEnumerable).Cast<object>().ToList<object>().FirstOrDefault<object>()?.GetType()) ? (!(dataManager.Json is Array) ? dataManager.DataAdaptor.BatchUpdate(dataManager, changed, added, deleted, e, keyField, dropIndex, query) : BlazorAdaptor.BatchUpdateArray<T>(dataManager, changed, added, deleted, e, keyField, dropIndex, query)) : dataManager.DataAdaptor.BatchUpdate(dataManager, (changed as IEnumerable).Cast<IDynamicMetaObjectProvider>().ToList<IDynamicMetaObjectProvider>(), (added as IEnumerable).Cast<IDynamicMetaObjectProvider>().ToList<IDynamicMetaObjectProvider>(), (deleted as IEnumerable).Cast<IDynamicMetaObjectProvider>().ToList<IDynamicMetaObjectProvider>(), e, keyField, dropIndex, query, (object)null);
        }

        public virtual void Dispose()
        {
            this.DotNetObjectRef?.Dispose();
            this.BaseParent?.ChildDotNetObjectRef.Clear();
        }
    }
}