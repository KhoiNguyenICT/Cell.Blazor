using Cell.Blazor._Core.Abstract;
using Cell.Blazor._Core.Class;
using System.Threading.Tasks;

namespace Cell.Blazor.Data.Interface
{
    public interface IDataAdaptor
    {
        void SetParent(BaseComponent parent);

        object Read(DataManagerRequest dataManagerRequest, string key = null);

        Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null);

        object Insert(DataManager dataManager, object data, string key);

        Task<object> InsertAsync(DataManager dataManager, object data, string key);

        object Remove(DataManager dataManager, object data, string keyField, string key);

        Task<object> RemoveAsync(
            DataManager dataManager,
            object data,
            string keyField,
            string key);

        object Update(DataManager dataManager, object data, string keyField, string key);

        Task<object> UpdateAsync(
            DataManager dataManager,
            object data,
            string keyField,
            string key);

        object BatchUpdate(
            DataManager dataManager,
            object changedRecords,
            object addedRecords,
            object deletedRecords,
            string keyField,
            string key,
            int? dropIndex);

        Task<object> BatchUpdateAsync(
            DataManager dataManager,
            object changedRecords,
            object addedRecords,
            object deletedRecords,
            string keyField,
            string key,
            int? dropIndex);
    }
}