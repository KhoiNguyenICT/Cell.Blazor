using Cell.Blazor._Core.Class;
using Cell.Blazor.Data.Class;
using Cell.Blazor.Internal.Class;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Cell.Blazor._Core.Abstract
{
    public abstract class CellDataBoundComponent : CellBaseComponent
    {
        [JsonIgnore]
        public DataManager DataManager { get; set; }

        protected virtual CellBaseComponent MainParent { get; set; }

        private List<string> directParamKeys { get; set; } = new List<string>();

        public Dictionary<string, object> DirectParameters { get; set; } = new Dictionary<string, object>();

        public bool IsRerendering { get; set; }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);
            if (directParamKeys.Count == 0)
            {
                foreach (ParameterValue parameter in parameters)
                {
                    if (!parameter.Cascading)
                        directParamKeys.Add(parameter.Name);
                }
            }
            return base.SetParametersAsync(parameters);
        }

        protected object SetDataManager<T>(object dataSource)
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
                            Json = Enumerable.Empty<T>().Cast<object>().ToList()
                        };
                    }
                    break;

                default:
                    Type type = dataSource.GetType();
                    if (typeof(IEnumerable).IsAssignableFrom(type) ^ typeof(IEnumerable<object>).IsAssignableFrom(type))
                    {
                        DataManager = new DataManager
                        {
                            Json = ((IEnumerable)dataSource).Cast<object>()
                        };
                        break;
                    }
                    DataManager = new DataManager
                    {
                        Json = (IEnumerable<object>)dataSource
                    };
                    break;
            }
            return DataManager;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            DirectParameters = new Dictionary<string, object>();
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            IsRerendering = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            CellDataBoundComponent dataBoundComponent = this;
            if (!firstRender)
                return;
            foreach (string directParamKey in dataBoundComponent.directParamKeys)
            {
                dataBoundComponent.DirectParameters = dataBoundComponent.DirectParameters == null ? new Dictionary<string, object>() : dataBoundComponent.DirectParameters;
                PropertyInfo property = dataBoundComponent.GetType().GetProperty(directParamKey);
                object data = (object)property != null ? property.GetValue(dataBoundComponent) : null;
                CellBaseUtils.UpdateDictionary(directParamKey, data, dataBoundComponent.DirectParameters);
            }
        }

        public async Task OnPropertyChanged() => await OnParametersSetAsync();

        public virtual async Task<T> UpdateProperty<T>(
          string propertyName,
          T publicValue,
          T privateValue,
          object eventCallback = null,
          Expression<Func<T>> expression = null)
        {
            CellDataBoundComponent dataBoundComponent = this;
            T finalResult = publicValue;
            if (!EqualityComparer<T>.Default.Equals(publicValue, privateValue))
            {
                T newValue = dataBoundComponent.DirectParameters.ContainsKey(propertyName) ? (T)dataBoundComponent.DirectParameters[propertyName] : publicValue;
                bool flag = eventCallback != null && ((EventCallback<T>)eventCallback).HasDelegate;
                bool isPropertyBinding = !CellBaseUtils.Equals(publicValue, newValue) && dataBoundComponent.IsRerendering;
                CellBaseComponent sfBaseComponent = dataBoundComponent.MainParent != null ? dataBoundComponent.MainParent : dataBoundComponent;
                finalResult = dataBoundComponent.IsRerendering & flag | isPropertyBinding || !sfBaseComponent.IsRendered ? publicValue : privateValue;
                if (flag)
                    await ((EventCallback<T>)eventCallback).InvokeAsync(finalResult);
                if (isPropertyBinding)
                {
                    dataBoundComponent.DirectParameters[propertyName] = finalResult;
                    CellBaseUtils.UpdateDictionary(propertyName, finalResult, dataBoundComponent.PropertyChanges);
                }
            }
            T obj = finalResult;
            finalResult = default;
            return obj;
        }

        public override void ComponentDispose()
        {
            DirectParameters?.Clear();
            if (DataManager?.Json != null)
                UpdateObservableEvents("DataSource", DataManager.Json, true);
            DataManager?.Dispose();
        }
    }
}