using System.Collections.Generic;
using System.Linq;
using Cell.Blazor._Core.Class;
using Newtonsoft.Json;

namespace Cell.Blazor.Data.Class
{
    [JsonConverter(typeof(QueryConverter))]
    public class Query
    {
        public DataManagerRequest Queries { get; set; } = new DataManagerRequest();

        public string Key { get; set; }

        public string FKey { get; set; }

        public string FromTable { get; set; }

        public string[] Lookups { get; set; }

        public List<object> Expands { get; set; }

        public object[] SortedColumns { get; set; }

        public object[] GroupedColumns { get; set; }

        public string SubQuerySelector { get; set; }

        public Query SubQuery { get; set; }

        public bool IsChild { get; set; }

        public IDictionary<string, object> Params { get; set; }

        public bool IsCountRequired { get; set; }

        public DataManager DataManager { get; set; }

        public List<string> Distincts { get; set; }

        public string IdMapping { get; set; }

        public Query From(string tableName)
        {
            FromTable = tableName;
            Queries.Table = tableName;
            return this;
        }

        public Query Page(int pageIndex, int pageSize)
        {
            int num = pageIndex;
            Queries.Take = pageSize;
            Queries.Skip = (num - 1) * Queries.Take;
            return this;
        }

        public Query Take(int pageSize)
        {
            Queries.Take = pageSize;
            return this;
        }

        public Query Skip(int pageIndex, int pageSize)
        {
            Queries.Skip = pageIndex - Queries.Take;
            return this;
        }

        public Query Skip(int skip)
        {
            Queries.Skip = skip;
            return this;
        }

        public Query Range(int start, int end)
        {
            Queries.Skip = start;
            Queries.Take = end;
            return this;
        }

        public Query Select(List<string> fieldNames)
        {
            Queries.Select = Queries.Select ?? new List<string>();
            fieldNames.ForEach(select => Queries.Select.Add(select));
            return this;
        }

        public Query Where(
          string fieldName,
          string @operator = null,
          object value = null,
          bool ignoreCase = false,
          bool ignoreAccent = false)
        {
            Queries.Where = Queries.Where ?? new List<WhereFilter>();
            Queries.Where.Add(new WhereFilter
            {
                Field = fieldName,
                Operator = @operator,
                value = value,
                IgnoreCase = ignoreCase
            });
            return this;
        }

        public Query Where(WhereFilter predicate)
        {
            Queries.Where = Queries.Where ?? new List<WhereFilter>();
            Queries.Where.Add(predicate);
            return this;
        }

        public Query Where(List<WhereFilter> predicates)
        {
            Queries.Where = Queries.Where ?? new List<WhereFilter>();
            predicates.ForEach(predicate => Queries.Where.Add(predicate));
            return this;
        }

        public Query Search(
          string searchKey,
          List<string> fieldNames,
          string @operator = null,
          bool ignoreCase = false,
          bool ignoreAccent = false)
        {
            @operator = @operator == null || !(@operator != "none") ? "contains" : @operator;
            Queries.Search = Queries.Search ?? new List<SearchFilter>();
            Queries.Search.Add(new SearchFilter
            {
                Fields = fieldNames,
                Operator = @operator,
                Key = searchKey
            });
            return this;
        }

        public Query RequiresCount()
        {
            Queries.RequiresCounts = true;
            IsCountRequired = true;
            return this;
        }

        public Query Sort(string name, string direction = null)
        {
            string str = direction ?? "Ascending";
            Queries.Sorted = Queries.Sorted ?? new List<Sort>();
            Queries.Sorted.Add(new Sort
            {
                Name = name,
                Direction = str
            });
            return this;
        }

        public Query Group(List<string> fieldNames, IDictionary<string, string> groupFormat = null)
        {
            Queries.Group = Queries.Group ?? new List<string>();
            foreach (string fieldName in fieldNames)
                Queries.Group.Add(fieldName);
            if (groupFormat != null)
            {
                Queries.GroupByFormatter = Queries.GroupByFormatter ?? new Dictionary<string, string>();
                Queries.GroupByFormatter = Queries.GroupByFormatter.Concat(groupFormat).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            return this;
        }

        public Query Group(string fieldName)
        {
            Queries.Group = Queries.Group ?? new List<string>();
            Queries.Group.Add(fieldName);
            return this;
        }

        public Query Group(string fieldName, string columnFormat)
        {
            Queries.Group = Queries.Group ?? new List<string>();
            Queries.Group.Add(fieldName);
            Queries.GroupByFormatter = Queries.GroupByFormatter ?? new Dictionary<string, string>();
            if (Queries.GroupByFormatter.ContainsKey(fieldName))
                Queries.GroupByFormatter[fieldName] = columnFormat;
            else
                Queries.GroupByFormatter.Add(fieldName, columnFormat);
            return this;
        }

        public Query Aggregates(string field, string type)
        {
            Queries.Aggregates = Queries.Aggregates ?? new List<Aggregate>();
            Queries.Aggregates.Add(new Aggregate
            {
                Field = field,
                Type = type
            });
            return this;
        }

        public Query AddParams(string key, object value)
        {
            Queries.Params = Queries.Params ?? new Dictionary<string, object>();
            if (Queries.Params.ContainsKey(key))
                Queries.Params[key] = value;
            else
                Queries.Params.Add(key, value);
            return this;
        }

        public Query Distinct(List<string> fieldNames)
        {
            Queries.Distinct = Queries.Distinct ?? new List<string>();
            fieldNames.ForEach(distinct => Queries.Distinct.Add(distinct));
            return this;
        }

        public Query Expand(List<string> fieldNames)
        {
            Queries.Expand = Queries.Expand ?? new List<string>();
            fieldNames.ForEach(expand => Queries.Expand.Add(expand));
            return this;
        }

        public Query Clone() => new Query
        {
            Queries = JsonConvert.DeserializeObject<DataManagerRequest>(JsonConvert.SerializeObject(Queries))
        };

        public static bool IsEqual(Query source, Query destination) => JsonConvert.SerializeObject(source.Queries).Equals(JsonConvert.SerializeObject(destination.Queries));
    }
}