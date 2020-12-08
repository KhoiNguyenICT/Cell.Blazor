using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cell.Blazor._Core.Class;
using Newtonsoft.Json;

namespace Cell.Blazor.Data.Class
{
    public class QueryConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Query);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DataManagerRequest queries = ((Query)value)?.Queries;
            StringBuilder sb = new StringBuilder();
            sb.Append("new cell.data.Query()");
            if (queries.Table != null)
                sb.Append(".").Append("from(\"" + queries.Table + "\")");
            if (queries.Aggregates != null)
                queries.Aggregates.ForEach((Action<Aggregate>)(agg => sb.Append(".").Append("aggregate(\"" + agg.Field + "\",\"" + agg.Type + "\")")));
            if (queries.Expand != null)
            {
                string[] array = queries.Expand.ToArray<string>();
                sb.Append(".").Append("expand([\"" + string.Join("\",\"", array) + "\"])");
            }
            if (queries.Group != null)
                queries.Group.ForEach((Action<string>)(grp => sb.Append(".").Append("group(\"" + grp + "\")")));
            if (queries.Params != null)
            {
                foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>)queries.Params)
                    sb.Append(".").Append(string.Format("addParams(\"{0}\", \"{1}\")", (object)keyValuePair.Key, keyValuePair.Value));
            }
            if (queries.RequiresCounts)
                sb.Append(".").Append("requiresCount()");
            if (queries.Search != null)
                queries.Search.ForEach((Action<SearchFilter>)(search => sb.Append(".").Append("search(\"" + search.Key + "\", [\"" + string.Join("\",\"", (IEnumerable<string>)search.Fields) + "\"], \"" + search.Operator + "\")")));
            if (queries.Select != null)
                sb.Append(".").Append("select([\"" + string.Join("\",\"", queries.Select.ToArray()) + "\"])");
            if (queries.Skip != 0)
                sb.Append(".").Append(string.Format("skip({0})", (object)queries.Skip));
            if (queries.Sorted != null)
                queries.Sorted.ForEach((Action<Sort>)(sort => sb.Append(".").Append("sortBy(\"" + sort.Name + "\", \"" + sort.Direction + "\")")));
            if (queries.Take != 0)
                sb.Append(".").Append(string.Format("take({0})", (object)queries.Take));
            if (queries.Where != null)
                queries.Where.ForEach((Action<WhereFilter>)(filter => sb.Append(".").Append("where(cell.data.Predicate.fromJson(" + JsonConvert.SerializeObject((object)filter) + "))")));
            string json = JsonConvert.SerializeObject((object)sb.ToString(), Formatting.Indented);
            writer.WriteRawValue(json);
        }

        public override object ReadJson(
          JsonReader reader,
          Type objectType,
          object existingValue,
          JsonSerializer serializer)
        {
            return (object)new Query();
        }
    }
}
