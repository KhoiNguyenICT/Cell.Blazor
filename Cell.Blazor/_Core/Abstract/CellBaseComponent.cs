using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Cell.Blazor._Core.Service;
using Cell.Blazor.Internal.Class;
using Cell.Blazor.Internal.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace Cell.Blazor._Core.Abstract
{
    public abstract class CellBaseComponent : OwningComponentBase
    {
        public List<ScriptModules> DependentScripts = new List<ScriptModules>();

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public CellBlazorService SyncfusionService { get; set; }

        public bool IsRendered { get; set; }

        public CellScriptModules ScriptModules { get; set; }

        public Dictionary<string, object> PropertyChanges { get; set; }

        public DotNetObjectReference<object> DotnetObjectReference { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            this.PropertyChanges = new Dictionary<string, object>();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            CellBaseComponent cellBaseComponent = this;
            if (firstRender)
            {
                cellBaseComponent.DotnetObjectReference = DotNetObjectReference.Create<object>((object)cellBaseComponent);
                cellBaseComponent.IsRendered = firstRender;
                await CellBaseUtils.ImportModule(cellBaseComponent.JSRuntime, CellScriptModules.CellBase);
                await CellBaseUtils.InvokeDeviceMode(cellBaseComponent.SyncfusionService, cellBaseComponent.JSRuntime);
                await CellBaseUtils.ImportModules(cellBaseComponent.JSRuntime, cellBaseComponent.DependentScripts, cellBaseComponent.SyncfusionService.ScriptHashKey);
                if (cellBaseComponent.ScriptModules != CellScriptModules.None)
                    await CellBaseUtils.ImportModule(cellBaseComponent.JSRuntime, cellBaseComponent.ScriptModules, cellBaseComponent.SyncfusionService.ScriptHashKey);
                await cellBaseComponent.OnAfterScriptRendered();
            }
            cellBaseComponent.PropertyChanges?.Clear();
        }

        public virtual void Dispose() => this.Dispose(true);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(true);
            if (!disposing)
                return;
            this.DotnetObjectReference?.Dispose();
            this.PropertyChanges?.Clear();
            this.DependentScripts?.Clear();
            this.ComponentDispose();
        }

        public virtual void ComponentDispose()
        {
        }

        public virtual async Task OnAfterScriptRendered() => await Task.CompletedTask;

        public async Task InvokeMethod(string methodName, params object[] methodParams) => await CellBaseUtils.InvokeMethod(this.JSRuntime, methodName, methodParams);

        public async Task<T> InvokeMethod<T>(
          string methodName,
          bool isObjectReturnType,
          params object[] methodParams)
        {
            if (!isObjectReturnType)
                return await CellBaseUtils.InvokeMethod<T>(this.JSRuntime, methodName, methodParams);
            string str = await CellBaseUtils.InvokeMethod<string>(this.JSRuntime, methodName, methodParams);
            T obj = default(T);
            if (str != null)
                obj = JsonConvert.DeserializeObject<T>(str, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
            return obj;
        }

        public T NotifyPropertyChanges<T>(string propertyName, T publicValue, T privateValue)
        {
            if (!CellBaseUtils.Equals<T>(publicValue, privateValue))
                CellBaseUtils.UpdateDictionary(propertyName, (object)publicValue, this.PropertyChanges);
            return publicValue;
        }

        public PropertyChangedEventHandler PropertyChanged { get; set; }

        public NotifyCollectionChangedEventHandler CollectionChanged { get; set; }

        public void UpdateObservableEvents<T>(string propertyName, T dataValue, bool unwire = false)
        {
            if ((object)dataValue == null)
                return;
            if ((object)dataValue is INotifyCollectionChanged)
            {
                if (this.CollectionChanged == null)
                    this.CollectionChanged = (NotifyCollectionChangedEventHandler)((sender, e) => this.ObservableCollectionChanged(propertyName, sender, e));
                if (!unwire)
                {
                    ((INotifyCollectionChanged)(object)dataValue).CollectionChanged += this.CollectionChanged;
                }
                else
                {
                    ((INotifyCollectionChanged)(object)dataValue).CollectionChanged -= this.CollectionChanged;
                    this.CollectionChanged = (NotifyCollectionChangedEventHandler)null;
                }
            }
            if (!((object)dataValue is INotifyPropertyChanged))
                return;
            List<object> source = new List<object>((IEnumerable<object>)(object)dataValue);
            if (!(source.FirstOrDefault<object>() is INotifyPropertyChanged))
                return;
            if (this.PropertyChanged == null)
                this.PropertyChanged = (PropertyChangedEventHandler)((sender, e) => this.ObservablePropertyChanged(propertyName, sender, e));
            foreach (object obj in source)
            {
                if (!unwire)
                {
                    ((INotifyPropertyChanged)obj).PropertyChanged += this.PropertyChanged;
                }
                else
                {
                    ((INotifyPropertyChanged)obj).PropertyChanged -= this.PropertyChanged;
                    this.PropertyChanged = (PropertyChangedEventHandler)null;
                }
            }
        }

        private void ObservableCollectionChanged(
          string propertyName,
          object sender1,
          NotifyCollectionChangedEventArgs e1)
        {
            if (this.PropertyChanged == null)
                this.PropertyChanged = (PropertyChangedEventHandler)((sender2, e2) => this.ObservablePropertyChanged(propertyName, sender2, e2));
            if (e1.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object oldItem in (IEnumerable)e1.OldItems)
                {
                    if (oldItem is INotifyPropertyChanged)
                        ((INotifyPropertyChanged)oldItem).PropertyChanged -= this.PropertyChanged;
                }
            }
            else if (e1.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object newItem in (IEnumerable)e1.NewItems)
                {
                    if (newItem is INotifyPropertyChanged)
                        ((INotifyPropertyChanged)newItem).PropertyChanged += this.PropertyChanged;
                }
            }
            CellBaseUtils.UpdateDictionary(propertyName, sender1, this.PropertyChanges);
            this.OnObservableChange(propertyName, sender1, true);
        }

        private void ObservablePropertyChanged(
          string propertyName,
          object sender,
          PropertyChangedEventArgs e)
        {
            CellBaseUtils.UpdateDictionary(propertyName, sender, this.PropertyChanges);
            this.OnObservableChange(propertyName, sender);
        }

        protected virtual void OnObservableChange(
          string propertyName,
          object sender,
          bool isCollectionChanged = false)
        {
        }
    }
}