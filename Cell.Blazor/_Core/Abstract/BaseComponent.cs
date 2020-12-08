using Cell.Blazor._Core.Class;
using Cell.Blazor._Core.Interface;
using Cell.Blazor._Core.Service;
using Cell.Blazor._Core.Static;
using Cell.Blazor.Internal.Class;
using Cell.Blazor.Internal.Enums;
using Cell.Blazor.Internal.Static;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cell.Blazor._Core.Abstract
{
    public abstract class BaseComponent : OwningComponentBase, IDisposable
    {
        private List<string> directParamKeys = new List<string>();

        public Dictionary<string, object> ObservableData = new Dictionary<string, object>();
        public Dictionary<string, EventData> DelegateList = new Dictionary<string, EventData>();
        public Dictionary<string, object> ChildDotNetObjectRef = new Dictionary<string, object>();

        protected int uniqueId { get; set; }

        [Inject]
        protected IJSRuntime jsRuntime { get; set; }

        [JsonIgnore]
        [CascadingParameter]
        protected EditContext EditContext { get; set; }

        protected virtual string nameSpace { get; set; }

        protected virtual string jsProperty { get; set; }

        protected virtual BaseComponent mainParent { get; set; }

        protected virtual JSInteropAdaptor CreateJsAdaptor() => null;

        protected DotNetObjectReference<object> DotNetObjectRef { get; set; }

        public bool IsDataBound { get; set; }

        public string LocaleText { get; set; }

        public bool IsRerendering { get; set; }

        public bool IsClientChanges { get; set; }

        public bool IsServerRendered { get; set; }

        public bool IsEventTriggered { get; set; }

        public bool IsPropertyChanged { get; set; }

        public bool IsAutoInitialized { get; set; }

        public bool isObservableCollectionChanged { get; set; }

        public Dictionary<string, object> htmlAttributes { get; set; }

        public List<object> InvokedEvents { get; set; } = new List<object>();

        public List<string> ObservableChangedList { get; set; } = new List<string>();

        public List<ScriptModules> DynamicScripts { get; set; } = new List<ScriptModules>();

        public List<ScriptModules> DependentScripts { get; set; } = new List<ScriptModules>();

        public Dictionary<string, object> ClientChanges { get; set; } = new Dictionary<string, object>();

        public Dictionary<string, object> DirectParameters { get; set; } = new Dictionary<string, object>();

        public Dictionary<string, object> BindableProperties { get; set; } = new Dictionary<string, object>();

        [JsonIgnore]
        public JSInteropAdaptor JsAdaptor { get; set; }

        [Inject]
        [JsonIgnore]
        public CellBlazorService CellService { get; set; }

        [Inject]
        [JsonIgnore]
        public ICellStringLocalizer Localizer { get; set; }

        [JsonIgnore]
        public Dictionary<string, DataManager> DataManagerContainer { get; set; } = new Dictionary<string, DataManager>();

        public virtual string ID { get; set; }

        [JsonIgnore]
        public bool IsRendered { get; set; }

        [JsonIgnore]
        public virtual Type ModelType { get; set; }

        [JsonIgnore]
        public DataManager DataManager { get; set; }

        [JsonIgnore]
        public bool TemplateClientChanges { get; set; }

        [JsonProperty("guid")]
        public int UniqueId
        {
            get
            {
                Random random = new Random();
                if (uniqueId != 0)
                    return uniqueId;
                uniqueId = random.Next(1, 100000);
                return uniqueId;
            }
        }

        [JsonIgnore]
        public virtual Dictionary<string, object> DataContainer { get; set; } = new Dictionary<string, object>();

        [JsonIgnore]
        public virtual Dictionary<string, object> DataHashTable { get; set; } = new Dictionary<string, object>();

        protected override async Task OnInitializedAsync()
        {
            JsAdaptor = CreateJsAdaptor();
            JsAdaptor?.Init();
            await base.OnInitializedAsync();
            IsRerendering = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            BaseComponent baseComponent = this;
            if (firstRender)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                foreach (string directParamKey in baseComponent.directParamKeys)
                {
                    PropertyInfo property = baseComponent.GetType().GetProperty(directParamKey);
                    object obj = (object)property != null ? property.GetValue(baseComponent) : null;
                    baseComponent.updateDictionary(directParamKey, obj, dictionary);
                }
                baseComponent.DirectParameters = dictionary.ToDictionary(prop => prop.Key, prop => prop.Value);
            }
            if (baseComponent.nameSpace != null && !baseComponent.IsServerRendered)
            {
                if (firstRender && !baseComponent.IsClientChanges)
                    await baseComponent.InitComponent();
                else
                    await baseComponent.DataBind();
            }
            baseComponent.IsRerendering = true;
            baseComponent.isObservableCollectionChanged = false;
            baseComponent.ObservableChangedList = new List<string>();
        }

        public async Task InitComponent()
        {
            BaseComponent baseComponent = this;
            await CellBaseUtils.ImportModule(baseComponent.jsRuntime, CellScriptModules.CellBase);
            if (!CellBase.DisableScriptManager && baseComponent.DependentScripts.Count > 0)
            {
                foreach (ScriptModules dependentScript in baseComponent.DependentScripts)
                    await CellBaseUtils.ImportScripts(baseComponent.jsRuntime, dependentScript, baseComponent.CellService.ScriptHashKey);
            }
            await CellBaseUtils.ImportModules(baseComponent.jsRuntime, baseComponent.DynamicScripts, baseComponent.CellService.ScriptHashKey);
            await CellBaseUtils.InvokeDeviceMode(baseComponent.CellService, baseComponent.jsRuntime);
            if (baseComponent.CellService.ICellirstBaseResource)
            {
                CultureInfo currentCulture = Intl.CurrentCulture;
                baseComponent.CellService.ICellirstBaseResource = false;
                await CellBaseUtils.InvokeMethod(baseComponent.jsRuntime, "CellBlazor.loadCldr", (object)GlobalizeJsonGenerator.GetGlobalizeJsonString(currentCulture));
                await CellBaseUtils.InvokeMethod(baseComponent.jsRuntime, "CellBlazor.setCulture", string.IsNullOrEmpty(currentCulture.Name) ? (object)"en-US" : (object)currentCulture.Name, Intl.GetCultureFormats(currentCulture.Name));
                currentCulture = null;
            }
            baseComponent.DotNetObjectRef = DotNetObjectReference.Create((object)baseComponent);
            string bindableProps = baseComponent.serialiazeBindableProp(baseComponent.BindableProperties);
            string key = baseComponent.nameSpace + ".dataSource";
            if (baseComponent.DataContainer.ContainsKey(key) && baseComponent.DataContainer[key] != null)
                baseComponent.SetDataHashTable(key, (IEnumerable)baseComponent.DataContainer[key]);
            string str = await CellInterop.Init<string>(baseComponent.jsRuntime, baseComponent.ID, baseComponent.getUpdateModel(true), baseComponent.GetEventList(), baseComponent.nameSpace, baseComponent.DotNetObjectRef, bindableProps, baseComponent.htmlAttributes, baseComponent.ChildDotNetObjectRef, baseComponent.JsAdaptor?.GetRef(), baseComponent.LocaleText);
            if (baseComponent.DelegateList.ContainsKey("created") && baseComponent.BindableProperties.Count > 0)
                await baseComponent.DataBind();
            baseComponent.BindableProperties.Clear();
            baseComponent.IsRerendering = true;
            baseComponent.isObservableCollectionChanged = false;
            baseComponent.ObservableChangedList = new List<string>();
            baseComponent.DynamicScripts.Clear();
            baseComponent.DependentScripts.Clear();
            await baseComponent.InitialRendered();
        }

        public virtual async Task InitialRendered() => await Task.CompletedTask;

        public override Task SetParametersAsync(ParameterView parameters)
        {
            if (DirectParameters.Count() == 0)
            {
                foreach (ParameterValue parameter in parameters)
                {
                    if (!parameter.Cascading)
                        directParamKeys.Add(parameter.Name);
                }
            }
            return base.SetParametersAsync(parameters);
        }

        public virtual void ComponentDispose()
        {
        }

        public void CommonDispose()
        {
            EditContext = null;
            DataManager?.Dispose();
            Localizer = null;
            mainParent = null;
            BindableProperties.Clear();
            ClientChanges.Clear();
            InvokedEvents.Clear();
            directParamKeys.Clear();
            DirectParameters.Clear();
            ObservableData.Clear();
            DelegateList.Clear();
            DynamicScripts.Clear();
            DataContainer.Clear();
            DataManagerContainer.Clear();
            DataHashTable.Clear();
            ObservableChangedList.Clear();
            htmlAttributes?.Clear();
            ChildDotNetObjectRef.Clear();
            UnWireObservableEvents();
            DotNetObjectRef?.Dispose();
        }

        public virtual void Dispose()
        {
            CommonDispose();
            ComponentDispose();
            Dispose(true);
            if (nameSpace != null && IsRendered)
                CellInterop.InvokeMethod<object>(jsRuntime, ID, "destroy", null, null, nameSpace);
            JsAdaptor?.Dispose();
            JsAdaptor = null;
        }

        public async void Refresh()
        {
            if (nameSpace == null || !IsRendered)
                return;
            object obj = await CellInterop.InvokeMethod<object>(jsRuntime, ID, "refresh", null, null, nameSpace);
        }

        public async Task DataBind(bool hasStateChanged = false)
        {
            IsDataBound = false;
            IsEventTriggered = false;
            if (ClientChanges.Count > 0)
                await OnClientChanged(ClientChanges);
            clearClientChanges();
            if (IsRendered && nameSpace != null && BindableProperties.Count() > 0)
            {
                await CellBaseUtils.ImportModules(jsRuntime, DynamicScripts, CellService.ScriptHashKey);
                string model = serialiazeBindableProp(BindableProperties);
                BindableProperties.Clear();
                object obj = await CellInterop.Update<object>(jsRuntime, ID, model, nameSpace);
            }
            else
                BindableProperties.Clear();
            IsPropertyChanged = false;
            IsDataBound = true;
            if (IsRendered)
            {
                DynamicScripts.Clear();
                DependentScripts.Clear();
            }
            InvokedEvents = new List<object>();
        }

        protected void clearClientChanges(bool clearBindables = false)
        {
            if (((!IsClientChanges ? 0 : (!IsDataBound ? 1 : 0)) | (clearBindables ? 1 : 0)) == 0)
                return;
            foreach (KeyValuePair<string, object> clientChange in ClientChanges)
            {
                if (BindableProperties.ContainsKey(clientChange.Key))
                    BindableProperties.Remove(clientChange.Key);
            }
            if (clearBindables)
                return;
            IsClientChanges = false;
            ClientChanges.Clear();
        }

        public void updateDictionary(string key, object value, Dictionary<string, object> dictionary)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }

        public void SetDataHashTable(string key, IEnumerable DataSource)
        {
            foreach (object obj in DataSource)
                DataHashTable.Add("BlazTempId_" + Guid.NewGuid(), obj);
        }

        public static string ConvertJsonString(object jsonElement)
        {
            string str = jsonElement.ToString();
            if (str.IndexOf(",") == -1)
                str = str.Replace("\\", string.Empty);
            if ((str.IndexOf("\"[") != str.IndexOf("\"[\\") || str.IndexOf("\"{") != str.IndexOf("\"{\\")) && (str.LastIndexOf("\"{") > 0 && str.IndexOf(",") == -1))
                str = str.Replace("\"[", "[").Replace("]\"", "]").Replace("\"{", "{").Replace("}\"", "}");
            return str;
        }

        public async Task InvokeMethod(
          string methodName,
          string moduleName = null,
          params object[] methodParams)
        {
            int num = await IsScriptRendered() ? 1 : 0;
            methodParams = methodParams == null || methodParams.Length == 0 ? null : methodParams;
            object obj = await CellInterop.InvokeMethod<object>(jsRuntime, ID, methodName, moduleName, methodParams, nameSpace);
        }

        public async Task<T> InvokeMethod<T>(
          string methodName,
          bool isObjectReturnType,
          string moduleName = null,
          params object[] methodParams)
        {
            int num = await IsScriptRendered() ? 1 : 0;
            methodParams = methodParams == null || methodParams.Length == 0 ? null : methodParams;
            if (!isObjectReturnType)
                return await CellInterop.InvokeMethod<T>(jsRuntime, ID, methodName, moduleName, methodParams, nameSpace);
            string str = await CellInterop.InvokeMethod<string>(jsRuntime, ID, methodName, moduleName, methodParams, nameSpace);
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            return str != null ? JsonConvert.DeserializeObject<T>(str, settings) : default;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task InvokeSet<T>(
          string moduleName,
          string methodName,
          params object[] methodParams)
        {
            T obj = await CellInterop.InvokeSet<T>(jsRuntime, ID, moduleName, methodName, methodParams, nameSpace);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task<T> InvokeGet<T>(string moduleName, string methodName) => await CellInterop.InvokeGet<T>(jsRuntime, ID, moduleName, methodName, nameSpace);

        private void observableCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            isObservableCollectionChanged = true;
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object oldItem in e.OldItems)
                {
                    if (oldItem is INotifyPropertyChanged)
                        ((INotifyPropertyChanged)oldItem).PropertyChanged -= observablePropertyChanged;
                }
            }
            else
            {
                if (e.Action != NotifyCollectionChangedAction.Add)
                    return;
                foreach (object newItem in e.NewItems)
                {
                    if (newItem is INotifyPropertyChanged)
                        ((INotifyPropertyChanged)newItem).PropertyChanged += observablePropertyChanged;
                }
            }
        }

        private void observablePropertyChanged(object sender, PropertyChangedEventArgs e) => isObservableCollectionChanged = true;

        protected virtual void WireObservableEvents(object collection)
        {
            if (collection == null || !collection.GetType().IsGenericType)
                return;
            if (collection is INotifyCollectionChanged)
                ((INotifyCollectionChanged)collection).CollectionChanged += observableCollectionChanged;
            if (!(collection is INotifyPropertyChanged))
                return;
            List<object> source = new List<object>((IEnumerable<object>)collection);
            if (!(source.FirstOrDefault() is INotifyPropertyChanged))
                return;
            foreach (INotifyPropertyChanged notifyPropertyChanged in source)
                notifyPropertyChanged.PropertyChanged += observablePropertyChanged;
        }

        private void UnWireObservableEvents()
        {
            if (ObservableData.Count <= 0)
                return;
            foreach (KeyValuePair<string, object> keyValuePair in ObservableData)
            {
                if (keyValuePair.Value is INotifyCollectionChanged)
                    ((INotifyCollectionChanged)keyValuePair.Value).CollectionChanged -= observableCollectionChanged;
                if (keyValuePair.Value is INotifyPropertyChanged)
                {
                    List<object> source = new List<object>((IEnumerable<object>)keyValuePair.Value);
                    if (source.FirstOrDefault() is INotifyPropertyChanged)
                    {
                        foreach (INotifyPropertyChanged notifyPropertyChanged in source)
                            notifyPropertyChanged.PropertyChanged -= observablePropertyChanged;
                    }
                }
            }
        }

        public virtual async Task<T> updateProperty<T>(
          string key,
          T publicValue,
          T privateValue,
          object eventCallback = null,
          Expression<Func<T>> expression = null,
          bool isDataSource = false,
          bool isObservable = false)
        {
            BaseComponent baseComponent1 = this;
            string propertyKey = !baseComponent1.jsProperty.StartsWith("Cell.") ? baseComponent1.jsProperty + "." + key : key;
            string key1 = baseComponent1.jsProperty != string.Empty ? baseComponent1.jsProperty + "." + key : key;
            BaseComponent baseComponent = baseComponent1.mainParent != null ? baseComponent1.mainParent : baseComponent1;
            string str1 = key.Substring(1);
            string str2 = char.ToUpper(key[0]) + str1;
            T finalResult = publicValue;
            if (isDataSource | isObservable)
            {
                T directParam = baseComponent1.getDirectParam<T>(str2);
                object obj = isObservable ? publicValue : baseComponent1.GetDataManager(publicValue, key1);
                if (baseComponent.DataContainer.ContainsKey(key1))
                {
                    if (!EqualityComparer<T>.Default.Equals(publicValue, directParam) && !baseComponent1.IsClientChanges)
                    {
                        baseComponent1.updateDictionary(propertyKey, obj, baseComponent.BindableProperties);
                        if (!((object)publicValue is DefaultAdaptor) && !(typeof(int[,]).Name == publicValue.GetType().Name))
                        {
                            baseComponent1.DataHashTable.Clear();
                            baseComponent1.SetDataHashTable(key, (IEnumerable)publicValue);
                            baseComponent.DataContainer[key1] = publicValue;
                            baseComponent1.DirectParameters[str2] = publicValue;
                            baseComponent.IsPropertyChanged = baseComponent1.IsRerendering;
                        }
                    }
                    else if (privateValue != null && baseComponent1.IsClientChanges && baseComponent1.ClientChanges.ContainsKey(key))
                    {
                        finalResult = (T)CellBaseUtils.ChangeType((object)privateValue, publicValue.GetType());
                        baseComponent.DataContainer[key1] = publicValue;
                        baseComponent1.DataHashTable.Clear();
                        baseComponent1.SetDataHashTable(key, (IEnumerable)publicValue);
                        baseComponent.IsPropertyChanged = baseComponent1.IsRerendering;
                    }
                    if (baseComponent1.isObservableCollectionChanged && !baseComponent1.ObservableChangedList.Contains(key))
                    {
                        baseComponent1.ObservableChangedList.Add(key);
                        if (((object)publicValue as IEnumerable<object>).Count() != baseComponent1.DataHashTable.Count())
                        {
                            baseComponent1.DataHashTable.Clear();
                            baseComponent1.SetDataHashTable(key, (IEnumerable)publicValue);
                            baseComponent.IsPropertyChanged = baseComponent1.IsRerendering;
                        }
                        baseComponent1.updateDictionary(propertyKey, obj, baseComponent.BindableProperties);
                    }
                }
                else
                {
                    baseComponent.DataContainer.Add(key1, publicValue);
                    baseComponent1.updateDictionary(propertyKey, obj, baseComponent.BindableProperties);
                    baseComponent1.WireObservableEvents(publicValue);
                    if (!baseComponent1.ObservableData.ContainsKey(key1))
                        baseComponent1.ObservableData.Add(key1, publicValue);
                }
            }
            else if (baseComponent1.CompareValues(publicValue, privateValue))
            {
                bool forceUpdate = false;
                T directParam = baseComponent1.getDirectParam<T>(str2);
                bool flag = baseComponent.IsClientChanges;
                if (flag && baseComponent1.IsEventTriggered)
                    flag = baseComponent1.ClientChanges.ContainsKey(key1) && baseComponent1.CompareValues(publicValue, privateValue) || !baseComponent1.CompareValues(directParam, publicValue);
                if ((baseComponent1.CompareValues(directParam, publicValue) || !baseComponent.IsRendered) && !flag)
                {
                    forceUpdate = true;
                    baseComponent1.DirectParameters[str2] = publicValue;
                    baseComponent.IsPropertyChanged = true;
                }
                else
                    finalResult = publicValue = privateValue;
                if (eventCallback != null)
                {
                    EventCallback<T> eventCallback1 = (EventCallback<T>)eventCallback;
                    if (eventCallback1.HasDelegate && !baseComponent1.InvokedEvents.Contains(eventCallback1))
                    {
                        baseComponent1.DirectParameters[str2] = publicValue;
                        if (publicValue != null && !publicValue.GetType().IsArray)
                            baseComponent1.InvokedEvents.Add(eventCallback1);
                        await eventCallback1.InvokeAsync(publicValue);
                    }
                }
                if (expression != null)
                {
                    EditContext editContext = baseComponent1.EditContext;
                    if (editContext != null)
                    {
                        FieldIdentifier fieldIdentifier = FieldIdentifier.Create(expression);
                        editContext.NotifyFieldChanged(in fieldIdentifier);
                    }
                }
                if (forceUpdate && !isDataSource)
                    baseComponent1.updateDictionary(propertyKey, publicValue, baseComponent.BindableProperties);
            }
            propertyKey = null;
            T obj1 = finalResult;
            propertyKey = null;
            baseComponent = null;
            finalResult = default;
            return obj1;
        }

        public T getDirectParam<T>(string publicKey) => !DirectParameters.ContainsKey(publicKey) ? default : (T)DirectParameters[publicKey];

        public bool CompareValues<T>(T oldValue, T newValue)
        {
            Type type = oldValue?.GetType();
            return (!(type != null) ? 0 : (type.Namespace == null || !type.Namespace.Contains("Collections") ? (type.IsArray ? 1 : 0) : 1)) != 0 ? !string.Equals(JsonConvert.SerializeObject(oldValue), JsonConvert.SerializeObject(newValue)) : !EqualityComparer<T>.Default.Equals(oldValue, newValue);
        }

        public void RenderNewChild()
        {
            Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(getSerializedModel());
            dictionary.Add("isNewComponent", true);
            foreach (KeyValuePair<string, object> keyValuePair in dictionary)
            {
                string key = jsProperty + "." + keyValuePair.Key;
                DirectParameters[keyValuePair.Key] = keyValuePair.Value;
                updateDictionary(key, keyValuePair.Value, mainParent.BindableProperties);
            }
        }

        [JSInvokable]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task<object> Trigger(string eventName, string arg)
        {
            EventData eventData = DelegateList[eventName];
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };
            object obj1 = JsonConvert.DeserializeObject(arg, eventData.ArgumentType, settings);
            if (obj1 != null && eventData.ArgumentType.Namespace != "System")
                eventData.ArgumentType.GetProperty("JsRuntime", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(obj1, jsRuntime);
            IsDataBound = false;
            IsEventTriggered = true;
            clearClientChanges(true);
            object argumentData = obj1;
            object handler = eventData.Handler;
            // ISSUE: reference to a compiler-generated field
            if (BaseComponent.\u003CTrigger\u003Ed__173.\u003C\u003Eo__173.\u003C\u003Ep__0 == null)
      {
                // ISSUE: reference to a compiler-generated field
                BaseComponent.\u003CTrigger\u003Ed__173.\u003C\u003Eo__173.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "GetAwaiter", null, typeof(BaseComponent), new CSharpArgumentInfo[1]
                {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
                }));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, object> target1 = BaseComponent.\u003CTrigger\u003Ed__173.\u003C\u003Eo__173.\u003C\u003Ep__0.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, object>> p0 = BaseComponent.\u003CTrigger\u003Ed__173.\u003C\u003Eo__173.\u003C\u003Ep__0;
            // ISSUE: reference to a compiler-generated field
            if (BaseComponent.\u003C\u003Eo__173.\u003C\u003Ep__0 == null)
      {
                // ISSUE: reference to a compiler-generated field
                BaseComponent.\u003C\u003Eo__173.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "InvokeAsync", null, typeof(BaseComponent), new CSharpArgumentInfo[2]
                {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
                }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj2 = BaseComponent.\u003C\u003Eo__173.\u003C\u003Ep__0.Target((CallSite)BaseComponent.\u003C\u003Eo__173.\u003C\u003Ep__0, handler, argumentData);
            object obj3 = target1(p0, obj2);
            // ISSUE: reference to a compiler-generated field
            if (BaseComponent.\u003CTrigger\u003Ed__173.\u003C\u003Eo__173.\u003C\u003Ep__2 == null)
      {
                // ISSUE: reference to a compiler-generated field
                BaseComponent.\u003CTrigger\u003Ed__173.\u003C\u003Eo__173.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(bool), typeof(BaseComponent)));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, bool> target2 = BaseComponent.\u003CTrigger\u003Ed__173.\u003C\u003Eo__173.\u003C\u003Ep__2.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, bool>> p2 = BaseComponent.\u003CTrigger\u003Ed__173.\u003C\u003Eo__173.\u003C\u003Ep__2;
            // ISSUE: reference to a compiler-generated field
            if (BaseComponent.\u003CTrigger\u003Ed__173.\u003C\u003Eo__173.\u003C\u003Ep__1 == null)
      {
                // ISSUE: reference to a compiler-generated field
                BaseComponent.\u003CTrigger\u003Ed__173.\u003C\u003Eo__173.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "IsCompleted", typeof(BaseComponent), new CSharpArgumentInfo[1]
                {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
                }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj4 = BaseComponent.\u003CTrigger\u003Ed__173.\u003C\u003Eo__173.\u003C\u003Ep__1.Target((CallSite)BaseComponent.\u003CTrigger\u003Ed__173.\u003C\u003Eo__173.\u003C\u003Ep__1, obj3);
            if (!target2(p2, obj4))
            {
                int num;
                // ISSUE: explicit reference operation
                // ISSUE: reference to a compiler-generated field
                (^this).\u003C\u003E1__state = num = 0;
                object obj = obj3;
                if (!(obj3 is ICriticalNotifyCompletion awaiter))
                {
                    INotifyCompletion awaiter = (INotifyCompletion)obj3;
                    // ISSUE: explicit reference operation
                    // ISSUE: reference to a compiler-generated field
                    (^this).\u003C\u003Et__builder.AwaitOnCompleted < INotifyCompletion, BaseComponent.\u003CTrigger\u003Ed__173 > (ref awaiter, this);
                    awaiter = null;
                }
                else
                {
                    // ISSUE: explicit reference operation
                    // ISSUE: reference to a compiler-generated field
                    (^this).\u003C\u003Et__builder.AwaitUnsafeOnCompleted < ICriticalNotifyCompletion, BaseComponent.\u003CTrigger\u003Ed__173 > (ref awaiter, this);
                }
                awaiter = null;
            }
            else
            {
                // ISSUE: reference to a compiler-generated field
                if (BaseComponent.\u003CTrigger\u003Ed__173.\u003C\u003Eo__173.\u003C\u003Ep__3 == null)
        {
                    // ISSUE: reference to a compiler-generated field
                    BaseComponent.\u003CTrigger\u003Ed__173.\u003C\u003Eo__173.\u003C\u003Ep__3 = CallSite<Action<CallSite, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "GetResult", null, typeof(BaseComponent), new CSharpArgumentInfo[1]
                    {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
                    }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                BaseComponent.\u003CTrigger\u003Ed__173.\u003C\u003Eo__173.\u003C\u003Ep__3.Target((CallSite)BaseComponent.\u003CTrigger\u003Ed__173.\u003C\u003Eo__173.\u003C\u003Ep__3, obj3);
                // ISSUE: reference to a compiler-generated field
                if (BaseComponent.\u003C\u003Eo__173.\u003C\u003Ep__1 == null)
        {
                    // ISSUE: reference to a compiler-generated field
                    BaseComponent.\u003C\u003Eo__173.\u003C\u003Ep__1 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "SerializeObject", null, typeof (BaseComponent), new CSharpArgumentInfo[2]
                    {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
                    }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj5 = BaseComponent.\u003C\u003Eo__173.\u003C\u003Ep__1.Target((CallSite) BaseComponent.\u003C\u003Eo__173.\u003C\u003Ep__1, typeof (JsonConvert), argumentData);
        argumentData = null;
        return obj5;
      }
    }

    public object InvokeGenericMethod(
      string name,
      Type type,
      Type parentType,
      params object[] arguments)
    {
      MethodInfo method = parentType.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
      if (name == "SetEvent")
      {
        if (nameSpace == null && !jsProperty.Contains("Cell."))
          arguments = arguments.Concat(new object[1]
          {
              mainParent
          }).ToArray();
        else
          arguments = arguments.Concat(new object[1]).ToArray();
      }
      return method.MakeGenericMethod(type).Invoke(this, arguments);
    }

    public static string ConvertToProperCase(string text) => char.ToUpper(text[0]) + text.Substring(1);

    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task UpdateModel(Dictionary<string, object> properties)
    {
      BaseComponent parentObject = this;
      parentObject.IsClientChanges = true;
      parentObject.UpdateComponentModel(properties, parentObject);
      await parentObject.OnParametersSetAsync();
      parentObject.StateHasChanged();
    }

    public void UpdateComponentModel(
      Dictionary<string, object> properties,
      BaseComponent parentObject,
      bool isAutoInitialized = false)
    {
      foreach (string key1 in properties.Keys)
      {
        string str = key1;
        string key2 = key1;
        int? CellIndex = new int?();
        if (key1.IndexOf("-") != -1)
        {
          string[] strArray = key1.Split('-');
          str = strArray[0];
          CellIndex = Convert.ToInt32(strArray[1]);
        }
        Type type1 = parentObject.GetType();
        PropertyInfo property1 = type1.GetProperty(ConvertToProperCase(str));
        PropertyInfo property2 = type1.GetProperty("_" + str, BindingFlags.Instance | BindingFlags.NonPublic);
        if (property2 == null && type1.BaseType != null && type1.BaseType.Namespace.Contains("Cell"))
        {
          property2 = type1.BaseType.GetProperty("_" + str, BindingFlags.Instance | BindingFlags.NonPublic);
          if (property2 == null && type1.BaseType.BaseType != null && type1.BaseType.BaseType.Namespace.Contains("Cell"))
            property2 = type1.BaseType.BaseType.GetProperty("_" + str, BindingFlags.Instance | BindingFlags.NonPublic);
        }
        Type nullableType = (object) property2 != null ? property2.PropertyType : null;
        if (property2 != null)
        {
          object propertyValue = property1.GetValue(parentObject);
          Type type2 = propertyValue == null || !(Nullable.GetUnderlyingType(nullableType) == null) ? nullableType : propertyValue.GetType();
          if (propertyValue is BaseComponent && properties[str] != null && (!type2.IsPrimitive && !type2.IsValueType) && (!type2.IsEnum && type2 != typeof (string)))
          {
            UpdateComponentModel(JsonConvert.DeserializeObject<Dictionary<string, object>>(properties[str].ToString()), (BaseComponent) propertyValue);
            if ((object) property2 != null)
              property2.SetValue(parentObject, property1.GetValue(parentObject), null);
          }
          else if ((type2.Namespace != null && type2.Namespace.Contains("Collection") || type2.IsArray) && properties[key2] != null)
          {
            object obj = type2.IsArray ? UpdateArrayValue(type2, properties[key2]) : UpdateCollectionValue(propertyValue, type2, CellIndex, properties[key2], parentObject.IsAutoInitialized);
            if ((object) property2 != null)
              property2.SetValue(parentObject, obj, null);
            if (parentObject.IsAutoInitialized && (object) property1 != null)
              property1.SetValue(parentObject, obj, null);
            BaseComponent baseComponent = parentObject.mainParent != null ? parentObject.mainParent : parentObject;
            updateDictionary(parentObject.jsProperty + "." + str, obj, baseComponent.ClientChanges);
          }
          else
          {
            object obj = CellBaseUtils.ChangeType(properties[str], type2, true);
            if ((object) property2 != null)
              property2.SetValue(parentObject, obj, null);
            if (parentObject.IsAutoInitialized && (object) property1 != null)
              property1.SetValue(parentObject, obj, null);
            BaseComponent baseComponent = parentObject.mainParent != null ? parentObject.mainParent : parentObject;
            updateDictionary(parentObject.jsProperty + "." + str, obj, baseComponent.ClientChanges);
          }
        }
      }
    }

    public object UpdateCollectionValue(
      object propertyValue,
      Type propertyType,
      int? CellIndex,
      object model,
      bool isAutoInitialized = false)
    {
      if (!CellIndex.HasValue)
        return CellBaseUtils.ChangeType(model, propertyType, true);
      IList list = (IList) propertyValue ?? (IList) Activator.CreateInstance(propertyType);
      Dictionary<string, object> properties = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.ToString());
      object dataValue;
      if (propertyValue == null || list.Count == 0)
      {
        dataValue = CellBaseUtils.ChangeType((object) ("[" + model + "]"), propertyType, true);
      }
      else
      {
        int? nullable = CellIndex;
        int num = list.Count - 1;
        if (nullable.GetValueOrDefault() <= num & nullable.HasValue)
        {
          if (properties.ContainsKey("CellAction") && (string) properties["CellAction"] == "pop")
            list.RemoveAt(CellIndex.Value);
          else
            UpdateComponentModel(properties, (BaseComponent) list[CellIndex.Value], isAutoInitialized);
          dataValue = list;
        }
        else
        {
          list.Add(CellBaseUtils.ChangeType(model, list[0].GetType(), true));
          dataValue = list;
        }
      }
      return CellBaseUtils.ChangeType(dataValue, propertyType, true);
    }

    public object UpdateArrayValue(Type propertyType, object model) => JsonConvert.DeserializeObject(model.ToString(), propertyType);

    public virtual async Task OnClientChanged(IDictionary<string, object> properties) => await Task.CompletedTask;

    public virtual void SetEvent<T>(
      string name,
      EventCallback<T> eventCallback,
      BaseComponent BaseParent = null)
    {
      BaseComponent baseComponent = BaseParent != null ? BaseParent : this;
      if (baseComponent.DelegateList.ContainsKey(name))
        baseComponent.DelegateList[name] = new EventData().Set(eventCallback, typeof (T));
      else
        baseComponent.DelegateList.Add(name, new EventData().Set(eventCallback, typeof (T)));
    }

    public virtual object GetEvent(string name) => !DelegateList.ContainsKey(name) ? null : DelegateList[name].Handler;

    public string[] GetEventList()
    {
      string[] strArray = new string[DelegateList.Count];
      int index = 0;
      foreach (string key in DelegateList.Keys)
      {
        strArray.SetValue(key, index);
        ++index;
      }
      return strArray;
    }

    private JsonSerializerSettings getJsonSerializerSettings()
    {
      JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
      serializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
      serializerSettings.NullValueHandling = NullValueHandling.Ignore;
      serializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
      serializerSettings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
      serializerSettings.Converters.Add(new NonFlagStringEnumConverter());
      return serializerSettings;
    }

    public string serialiazeBindableProp(Dictionary<string, object> bindableProp)
    {
      JsonSerializerSettings settings = new JsonSerializerSettings();
      settings.Converters.Add(new NonFlagStringEnumConverter());
      return JsonConvert.SerializeObject(bindableProp, Formatting.Indented, settings);
    }

    protected virtual string getSerializedModel() => JsonConvert.SerializeObject(this, Formatting.Indented, getJsonSerializerSettings());

    protected virtual string getUpdateModel(bool isInit = false)
    {
      if (!isInit)
        return serialiazeBindableProp(BindableProperties);
      string serializedModel = getSerializedModel();
      if (BindableProperties.Count <= 0)
        return serializedModel;
      Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedModel);
      foreach (KeyValuePair<string, object> bindableProperty in BindableProperties)
      {
        if (dictionary.ContainsKey(bindableProperty.Key))
          dictionary[bindableProperty.Key] = bindableProperty.Value;
        else
          dictionary.Add(bindableProperty.Key, bindableProperty.Value);
      }
      return JsonConvert.SerializeObject(dictionary, Formatting.Indented, getJsonSerializerSettings());
    }

    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ErrorHandling(string message, string stack) => Console.Error.WriteLine(message + "\n" + stack);

    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task<object> DataProcess(string dataManager, string key)
    {
      DataManagerRequest queries = JsonConvert.DeserializeObject<DataManagerRequest>(dataManager);
      DataRequest dataRequest = new DataRequest();
      DataManager dataManager1 = DataManagerContainer.ContainsKey(key) ? DataManagerContainer[key] : DataManager;
      object obj = null;
      if (dataManager1 != null)
      {
        obj = await dataManager1.ExecuteQuery<object>(queries);
        if (DataContainer.ContainsKey(key) && DataContainer[key] == null)
          DataContainer[key] = obj;
      }
      JsonSerializerSettings settings = new JsonSerializerSettings
      {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        ContractResolver = new DefaultContractResolver()
      };
      settings.Converters.Add(new BlazorIdJsonConverter(DataHashTable));
      return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
    }

    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task<object> Insert(string value, string key, int position, string query = null)
    {
      BaseComponent baseComponent = this;
      if (!baseComponent.DataContainer.ContainsKey(key))
        return null;
      // ISSUE: explicit non-virtual call
      DataManager dataManager = baseComponent.DataManagerContainer.ContainsKey(key) ? baseComponent.DataManagerContainer[key] : __nonvirtual (baseComponent.DataManager);
      if (baseComponent.DataContainer[key] == null)
      {
        baseComponent.DataContainer[key] = Enumerable.Empty<object>().ToList();
        dataManager.Json = (IEnumerable<object>) baseComponent.DataContainer[key];
      }
      Type type1 = baseComponent.GetType();
      object obj1 = baseComponent.DataContainer[key];
      Type type2 = type1;
      if ((object) type2 == null)
        type2 = obj1.GetType();
      Type type3 = type2;
      if (!(baseComponent.ModelType != null) && (!type3.IsGenericType || type3.GetGenericArguments().Count() <= 0))
        return null;
      Type type4 = baseComponent.ModelType != (Type) null ? baseComponent.ModelType : type3.GetGenericArguments()[0];
      object newrec = JsonConvert.DeserializeObject(value, type4, new JsonSerializerSettings
      {
        DateTimeZoneHandling = DateTimeZoneHandling.Local
      });
      Query query1 = new Query();
      query1.Queries = JsonConvert.DeserializeObject<DataManagerRequest>(query);
      object obj2 = await dataManager.Insert<object>(newrec, query1?.Queries?.Table, query1, position);
      if (baseComponent.DataHashTable.Count() != 0)
      {
        Random random = new Random();
        baseComponent.DataHashTable.Add("BlazTempId_" + random.Next(), newrec);
      }
      if (obj2 == null)
        obj2 = newrec;
      return JsonConvert.SerializeObject(obj2, Formatting.Indented, new JsonSerializerSettings
      {
          ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
          ContractResolver = new DefaultContractResolver()
      });
    }

    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task<object> Update(string value, string keyField, string key, string query = null)
    {
      BaseComponent baseComponent = this;
      if (!baseComponent.DataContainer.ContainsKey(key))
        return null;
      // ISSUE: explicit non-virtual call
      DataManager dataManager = baseComponent.DataManagerContainer.ContainsKey(key) ? baseComponent.DataManagerContainer[key] : __nonvirtual (baseComponent.DataManager);
      object obj = baseComponent.DataContainer[key];
      Type type1 = baseComponent.GetType();
      if ((object) type1 == null)
        type1 = obj.GetType();
      Type type2 = type1;
      if (!(baseComponent.ModelType != null) && (!type2.IsGenericType || type2.GetGenericArguments().Count() <= 0))
        return value;
      Type type3 = baseComponent.ModelType != (Type) null ? baseComponent.ModelType : type2.GetGenericArguments()[0];
      IDictionary<string, object> updateProperties = JsonConvert.DeserializeObject<IDictionary<string, object>>(value);
      object updatedrec = JsonConvert.DeserializeObject(value, type3, new JsonSerializerSettings
      {
        DateTimeZoneHandling = DateTimeZoneHandling.Local
      });
      Query query1 = new Query();
      query1.Queries = JsonConvert.DeserializeObject<DataManagerRequest>(query);
      return JsonConvert.SerializeObject(await dataManager.Update<object>(keyField, updatedrec, query1?.Queries?.Table, query1, updateProperties: updateProperties) ?? updatedrec, Formatting.Indented, new JsonSerializerSettings
      {
          ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
          ContractResolver = new DefaultContractResolver()
      });
    }

    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task<object> Remove(string value, string keyField, string key, string query = null)
    {
      BaseComponent baseComponent = this;
      if (!baseComponent.DataContainer.ContainsKey(key))
        return null;
      // ISSUE: explicit non-virtual call
      DataManager dataManager = baseComponent.DataManagerContainer.ContainsKey(key) ? baseComponent.DataManagerContainer[key] : __nonvirtual (baseComponent.DataManager);
      object obj1 = baseComponent.DataContainer[key];
      if ((object) baseComponent.GetType() == null)
        obj1.GetType();
      Query query1 = new Query();
      query1.Queries = JsonConvert.DeserializeObject<DataManagerRequest>(query);
      string keyField1 = keyField;
      string str1 = value;
      string table = query1?.Queries?.Table;
      Query query2 = query1;
      object obj2 = await dataManager.Remove<object>(keyField1, str1, table, query2);
      string str2;
      if (obj2 == null)
        str2 = key;
      else
        str2 = JsonConvert.SerializeObject(obj2, Formatting.Indented, new JsonSerializerSettings
        {
          ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
          ContractResolver = new DefaultContractResolver()
        });
      return str2;
    }

    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task<object> BatchUpdate(
      string changed,
      string added,
      string deleted,
      string keyField,
      string key,
      int? dropIndex,
      string query = null)
    {
      BaseComponent baseComponent = this;
      if (!baseComponent.DataContainer.ContainsKey(key))
        return null;
      object obj1 = baseComponent.DataContainer[key];
      Type type1 = baseComponent.GetType();
      if ((object) type1 == null)
        type1 = obj1.GetType();
      Type type2 = type1;
      object changedRecords = null;
      object addedRecords = null;
      object deletedRecords = null;
      List<object> objectList = new List<object>();
      Query query1 = new Query();
      query1.Queries = JsonConvert.DeserializeObject<DataManagerRequest>(query);
      if (baseComponent.ModelType != null || type2.IsGenericType && type2.GetGenericArguments().Count() > 0)
      {
        Type type3 = typeof (IEnumerable<>).MakeGenericType(baseComponent.ModelType != (Type) null ? baseComponent.ModelType : type2.GetGenericArguments()[0]);
        if (changed != null)
          changedRecords = JsonConvert.DeserializeObject(changed, type3, new JsonSerializerSettings
          {
            DateTimeZoneHandling = DateTimeZoneHandling.Local
          });
        if (added != null)
        {
          addedRecords = JsonConvert.DeserializeObject(added, type3, new JsonSerializerSettings
          {
            DateTimeZoneHandling = DateTimeZoneHandling.Local
          });
          foreach (object obj2 in (IEnumerable) addedRecords)
          {
            if (baseComponent.DataHashTable.Count() != 0)
            {
              Random random = new Random();
              baseComponent.DataHashTable.Add("BlazTempId_" + random.Next(), obj2);
            }
          }
        }
        if (deleted != null)
          deletedRecords = JsonConvert.DeserializeObject(deleted, type3, new JsonSerializerSettings
          {
            DateTimeZoneHandling = DateTimeZoneHandling.Local
          });
        // ISSUE: explicit non-virtual call
        return JsonConvert.SerializeObject(await (baseComponent.DataManagerContainer.ContainsKey(key) ? baseComponent.DataManagerContainer[key] : __nonvirtual (baseComponent.DataManager)).SaveChanges<object>(changedRecords, addedRecords, deletedRecords, keyField, dropIndex, query1?.Queries?.Table, query1) ?? new
        {
            changedRecords,
            addedRecords,
            deletedRecords
        }, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new DefaultContractResolver()
        });
      }
      return JsonConvert.SerializeObject(new
      {
          changedRecords,
          addedRecords,
          deletedRecords
      }, Formatting.Indented, new JsonSerializerSettings
      {
          ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
          ContractResolver = new DefaultContractResolver()
      });
    }

    protected object GetDataManager(object dataSource, string key = null)
    {
      switch (dataSource)
      {
        case CellDataManager _:
        case DataManager _:
          return dataSource;

        case null:
          if (DataManager == null)
          {
            DataManager = new DataManager
            {
              Json = Enumerable.Empty<object>()
            };
          }
          break;

        default:
          Type type = dataSource.GetType();
          if (typeof (IEnumerable).IsAssignableFrom(type) ^ typeof (IEnumerable<object>).IsAssignableFrom(type))
          {
            DataManager = new DataManager
            {
              Json = ((IEnumerable) dataSource).Cast<object>()
            };
            break;
          }
          DataManager = new DataManager
          {
            Json = (IEnumerable<object>) dataSource
          };
          break;
      }
      if (mainParent != null && dataSource != null)
      {
        if (mainParent.DataManagerContainer.ContainsKey(key))
          mainParent.DataManagerContainer[key] = DataManager;
        else
          mainParent.DataManagerContainer.Add(key, DataManager);
      }
      using (DefaultAdaptor defaultAdaptor = new DefaultAdaptor(key))
        return (object) defaultAdaptor;
    }

    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void UpdateTemplate(
      string Name,
      string TemplateData,
      string TemplateID,
      List<string> TemplateItems,
      List<string> BlazTempIds)
    {
      if (TemplateData != null && BlazTempIds.Count() == 0)
        GetType().GetProperty(Name + "Data").SetValue(this, JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(TemplateData, new JsonSerializerSettings
        {
            DateParseHandling = DateParseHandling.DateTimeOffset
        }));
      else if (BlazTempIds.Count() != 0)
      {
        Dictionary<string, object> dataHashTable = DataHashTable;
        List<Dictionary<string, object>> dictionaryList = new List<Dictionary<string, object>>();
        foreach (string blazTempId in BlazTempIds)
        {
          string KeyValue = blazTempId;
          object obj = dataHashTable.FirstOrDefault(x => x.Key == KeyValue).Value;
          Dictionary<string, object> dictionary = new Dictionary<string, object>();
          if (obj != null)
          {
            dictionary.Add(KeyValue, obj);
            dictionaryList.Add(dictionary);
          }
        }
        GetType().GetProperty(Name + "Data").SetValue(this, dictionaryList);
      }
      GetType().GetProperty(Name + "ID").SetValue(this, TemplateID);
      if (GetType().GetProperty("TemplateClientChanges") != null)
        GetType().GetProperty("TemplateClientChanges").SetValue(this, true);
      if (TemplateItems != null)
        GetType().GetProperty(Name + "Items").SetValue(this, TemplateItems);
      StateHasChanged();
    }

    public object GetObject(Dictionary<string, object> Data, Type ModelType)
    {
      object obj1 = Data.GetType().GetConstructors().Any(c => c.GetParameters().Length == 0) ? FormatterServices.GetUninitializedObject(ModelType) : Activator.CreateInstance(ModelType);
      foreach (KeyValuePair<string, object> keyValuePair in Data)
      {
        bool flag = keyValuePair.Key.Split('_').Length != 0 && keyValuePair.Key.Split('_')[0] == "BlazTempId";
        string properCase = ConvertToProperCase(keyValuePair.Key);
        PropertyInfo property = ModelType.GetProperty(keyValuePair.Key);
        PropertyInfo propertyInfo = property != (PropertyInfo) null ? property : ModelType.GetProperty(properCase);
        if (propertyInfo != null && flag.ToString() == "False")
        {
          if (IsCollection(propertyInfo.PropertyType, keyValuePair.Value))
          {
            string str = JsonConvert.SerializeObject(keyValuePair.Value);
            Type type1 = propertyInfo.PropertyType.GetTypeInfo();
            if (!propertyInfo.PropertyType.IsGenericType)
              type1 = typeof (IEnumerable<object>);
            Type type2 = type1;
            object obj2 = JsonConvert.DeserializeObject(str, type2);
            propertyInfo.SetValue(obj1, obj2);
          }
          else if (IsComplexObject(propertyInfo.PropertyType, keyValuePair.Value))
          {
            object obj2 = GetObject(JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(keyValuePair.Value)), propertyInfo.PropertyType);
            propertyInfo.SetValue(obj1, obj2);
          }
          else if (propertyInfo.CanWrite || propertyInfo.SetMethod != null)
            propertyInfo.SetValue(obj1, CellBaseUtils.ChangeType(keyValuePair.Value, propertyInfo.PropertyType));
        }
        else if (flag.ToString() == "True")
          return keyValuePair.Value;
      }
      return obj1;
    }

    public static bool IsCollection(Type type, object propertyValue) => type.IsGenericType && (type.GetGenericTypeDefinition().Equals(typeof (List<>)) || type.GetGenericTypeDefinition().Equals(typeof (ICollection<>)) || (type.GetGenericTypeDefinition().Equals(typeof (IEnumerable<>)) || type.IsAssignableFrom(typeof (IEnumerable)))) && propertyValue is JArray;

    public static bool IsComplexObject(Type type, object propertyValue)
    {
      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
        return IsComplexObject(type.GetGenericArguments()[0].GetTypeInfo(), propertyValue);
      switch (propertyValue)
      {
        case JObject _:
          return true;

        case null:
        case string _:
label_7:
          return false;

        default:
          if (!type.GetTypeInfo().IsPrimitive)
          {
            switch (propertyValue)
            {
              case long _:
              case double _:
              case Decimal _:
              case DateTimeOffset _:
              case DateTime _:
                goto label_7;
              default:
                if (!type.GetTypeInfo().IsEnum && !typeof (IEnumerable).IsAssignableFrom(type))
                  return true;
                goto label_7;
            }
          }
          else
            goto case null;
      }
    }

    public void InitTemplates(string template, object templateParent)
    {
      BaseComponent baseComponent = templateParent as BaseComponent;
      if (!baseComponent.ChildDotNetObjectRef.ContainsKey(template))
        baseComponent.ChildDotNetObjectRef.Add(template, DotNetObjectReference.Create((object) this));
      else
        ChildDotNetObjectRef.Add(template, DotNetObjectReference.Create((object) this));
    }

    public void InitTemplates(
      string template,
      object templateParent,
      DotNetObjectReference<object> dotnetInstance = null)
    {
      BaseComponent baseComponent = templateParent as BaseComponent;
      if (!baseComponent.ChildDotNetObjectRef.ContainsKey(template))
        baseComponent.ChildDotNetObjectRef.Add(template, dotnetInstance);
      else
        ChildDotNetObjectRef.Add(template, dotnetInstance);
    }

    public async Task SetTemplates(
      string TemplateName,
      string TemplateID,
      RenderFragment<object> Template,
      List<Dictionary<string, object>> TemplateData,
      int UniqueId,
      bool TemplateClientChanges = true,
      DotNetObjectReference<object> dotnetInstance = null)
    {
      int num = await IsScriptRendered() ? 1 : 0;
      if (TemplateData == null && Template != null)
      {
        string str1 = await CellInterop.SetTemplateInstance<string>(jsRuntime, TemplateName, dotnetInstance, UniqueId);
      }
      if (!(TemplateData != null & TemplateClientChanges))
        return;
      string str2 = await CellInterop.SetTemplates<string>(jsRuntime, TemplateID);
    }

    public async Task SetTemplates(
      string TemplateName,
      string TemplateID,
      RenderFragment<object> Template,
      List<Dictionary<string, object>> TemplateData,
      int UniqueId,
      bool TemplateClientChanges = true)
    {
      BaseComponent baseComponent = this;
      int num = await baseComponent.IsScriptRendered() ? 1 : 0;
      if (TemplateData == null && Template != null)
      {
        string str1 = await CellInterop.SetTemplateInstance<string>(baseComponent.jsRuntime, TemplateName, DotNetObjectReference.Create((object) baseComponent), UniqueId);
      }
      if (!(TemplateData != null & TemplateClientChanges))
        return;
      string str2 = await CellInterop.SetTemplates<string>(baseComponent.jsRuntime, TemplateID);
    }

    public async Task SetContainerTemplates(
      string TemplateName,
      string TemplateID,
      RenderFragment Template,
      List<Dictionary<string, object>> TemplateData,
      int UniqueId)
    {
      BaseComponent baseComponent = this;
      int num = await baseComponent.IsScriptRendered() ? 1 : 0;
      if (TemplateData == null && Template != null)
      {
        string str1 = await CellInterop.SetTemplateInstance<string>(baseComponent.jsRuntime, TemplateName, DotNetObjectReference.Create((object) baseComponent), UniqueId);
      }
      if (TemplateData == null)
        return;
      string str2 = await CellInterop.SetTemplates<string>(baseComponent.jsRuntime, TemplateID);
    }

    public async Task SetContainerTemplates(
      string TemplateName,
      string TemplateID,
      RenderFragment Template,
      List<Dictionary<string, object>> TemplateData,
      int UniqueId,
      DotNetObjectReference<object> dotnetInstance = null)
    {
      int num = await IsScriptRendered() ? 1 : 0;
      if (TemplateData == null && Template != null)
      {
        string str1 = await CellInterop.SetTemplateInstance<string>(jsRuntime, TemplateName, dotnetInstance, UniqueId);
      }
      if (TemplateData == null)
        return;
      string str2 = await CellInterop.SetTemplates<string>(jsRuntime, TemplateID);
    }

    public async Task<bool> IsScriptRendered()
    {
      if (!CellBase.DisableScriptManager && CellService.IsScriptRendered || CellBase.DisableScriptManager)
        return true;
      await Task.Delay(10);
      return await IsScriptRendered();
    }

    public class DataRequest
    {
      [JsonProperty("result")]
      public object Result { get; set; }

      [JsonProperty("count")]
      public int Count { get; set; }
    }
  }
}