using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cell.Blazor.Data.Class
{
    public class WhereFilter
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("ignoreCase")]
        public bool IgnoreCase { get; set; }

        [JsonProperty("ignoreAccent")]
        public bool IgnoreAccent { get; set; }

        [JsonProperty("isComplex")]
        public bool IsComplex { get; set; }

        [JsonProperty("operator")]
        public string Operator { get; set; }

        [JsonProperty("condition")]
        public string Condition { get; set; }

        [JsonProperty("value")]
        public object value { get; set; }

        [JsonProperty("predicates")]
        public List<WhereFilter> predicates { get; set; }

        public static WhereFilter And(List<WhereFilter> predicates) => new WhereFilter()
        {
            Condition = "and",
            IsComplex = true,
            predicates = predicates
        };

        public static WhereFilter Or(List<WhereFilter> predicates) => new WhereFilter()
        {
            Condition = "or",
            IsComplex = true,
            predicates = predicates
        };

        public WhereFilter And(
          string fieldName,
          string @operator = null,
          object value = null,
          bool ignoreCase = false,
          bool ignoreAccent = false)
        {
            WhereFilter whereFilter = new WhereFilter()
            {
                Field = fieldName,
                Operator = @operator,
                value = value,
                IgnoreCase = ignoreCase,
                IgnoreAccent = ignoreAccent
            };
            return new WhereFilter()
            {
                Condition = "and",
                IsComplex = true,
                predicates = new List<WhereFilter>()
        {
          this,
          whereFilter
        }
            };
        }

        public WhereFilter And(WhereFilter predicate) => new WhereFilter()
        {
            Condition = "and",
            IsComplex = true,
            predicates = new List<WhereFilter>()
      {
        this,
        predicate
      }
        };

        public WhereFilter Or(
          string fieldName,
          string @operator = null,
          object value = null,
          bool ignoreCase = false,
          bool ignoreAccent = false)
        {
            WhereFilter whereFilter = new WhereFilter()
            {
                Field = fieldName,
                Operator = @operator,
                value = value,
                IgnoreCase = ignoreCase,
                IgnoreAccent = ignoreAccent
            };
            return new WhereFilter()
            {
                Condition = "or",
                IsComplex = true,
                predicates = new List<WhereFilter>()
        {
          this,
          whereFilter
        }
            };
        }

        public WhereFilter Or(WhereFilter predicate) => new WhereFilter()
        {
            Condition = "or",
            IsComplex = true,
            predicates = new List<WhereFilter>()
      {
        this,
        predicate
      }
        };
    }
}