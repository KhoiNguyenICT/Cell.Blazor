using Cell.Blazor.Internal.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cell.Blazor.Internal.Class
{
    internal class Validator
    {
        private IDictionary<string, IValidator> validatorOfRules = (IDictionary<string, IValidator>)new Dictionary<string, IValidator>()
    {
      {
        "required",
        (IValidator) new RequiredValidator()
      },
      {
        "rangeLength",
        (IValidator) new StringLengthValidator()
      },
      {
        "range",
        (IValidator) new RangeValidator()
      },
      {
        "minLength",
        (IValidator) new MinLengthValidator()
      },
      {
        "maxLength",
        (IValidator) new MaxLengthValidator()
      },
      {
        "regex",
        (IValidator) new RegexValidator()
      },
      {
        "email",
        (IValidator) new EmailAddressValidator()
      },
      {
        "number",
        (IValidator) new NumberValidator()
      },
      {
        "max",
        (IValidator) new MaxValidator()
      },
      {
        "min",
        (IValidator) new MinValidator()
      }
    };

        private IDictionary<string, string> defaultMessages = (IDictionary<string, string>)new Dictionary<string, string>()
    {
      {
        "required",
        "This field is required"
      },
      {
        "rangeLength",
        "Please enter a value between {0} and {1} characters long."
      },
      {
        "range",
        "Please enter a value between {0} and {1}."
      },
      {
        "minLength",
        "Please enter at least {0} characters."
      },
      {
        "maxLength",
        "Please enter no more than {0} characters."
      },
      {
        "regex",
        "Please enter a correct value."
      },
      {
        "email",
        "Please enter a valid email address"
      },
      {
        "number",
        "Please enter a valid number."
      },
      {
        "max",
        "Please enter a value less than or equal to {0}."
      },
      {
        "min",
        "Please enter a value greater than or equal to {0}."
      }
    };

        internal string[] availableRules => this.validatorOfRules.Keys.ToArray<string>();

        public void TryValidate(
          object value,
          Type propertyType,
          in ValidationContext context,
          in ValidationResult result)
        {
            if (context.ValidationRules == null)
            {
                result.IsValid = true;
            }
            else
            {
                (string, object)[] rules = this.GetRules(context.ValidationRules, propertyType);
                if (!((IEnumerable<(string, object)>)rules).Any<(string, object)>())
                    result.IsValid = true;
                foreach ((string, object) tuple in rules)
                {
                    bool flag = this.GetValidator(tuple.Item1).IsValid(value, tuple.Item2);
                    result.IsValid = flag;
                    if (!result.IsValid)
                    {
                        result.Rule = tuple.Item1;
                        result.Message = this.GetMessage(tuple.Item1, context.ValidationRules, result.FieldName);
                        break;
                    }
                }
            }
        }

        private (string, object)[] GetRules(object validationRule, Type propertyType)
        {
            List<(string, object)> valueTupleList = new List<(string, object)>();
            Type type = validationRule.GetType();
            bool flag = validationRule is IDictionary<string, object>;
            IDictionary<string, object> dictionary = (IDictionary<string, object>)null;
            if (flag)
                dictionary = (IDictionary<string, object>)validationRule;
            foreach (string availableRule in this.availableRules)
            {
                if (flag)
                {
                    if (dictionary.ContainsKey(availableRule))
                    {
                        if (availableRule == "required")
                            valueTupleList.Add((availableRule, (object)propertyType));
                        else
                            valueTupleList.Add((availableRule, dictionary[availableRule]));
                    }
                }
                else if (type.GetProperty(availableRule) != (PropertyInfo)null)
                {
                    if (availableRule == "required")
                        valueTupleList.Add((availableRule, (object)propertyType));
                    else
                        valueTupleList.Add((availableRule, type.GetProperty(availableRule).GetValue(validationRule)));
                }
            }
            return valueTupleList.ToArray();
        }

        private IValidator GetValidator(string rule)
        {
            IValidator validator;
            if (this.validatorOfRules.TryGetValue(rule, out validator))
                return validator;
            throw new ArgumentException("Valitor for rule " + rule + " is not found");
        }

        private object[] ToObjectArray(object arg)
        {
            List<object> objectList = new List<object>();
            foreach (object obj in arg as IEnumerable)
                objectList.Add(obj);
            return objectList.ToArray();
        }

        private string GetMessage(string rule, object validationRule, string fieldName)
        {
            string str = (string)null;
            PropertyInfo property = validationRule.GetType().GetProperty(rule);
            bool flag1 = validationRule is IDictionary<string, object>;
            bool flag2 = false;
            IDictionary<string, object> dictionary1 = (IDictionary<string, object>)null;
            IDictionary<string, object> dictionary2 = (IDictionary<string, object>)null;
            if (flag1)
            {
                dictionary1 = (IDictionary<string, object>)validationRule;
                object obj = (object)null;
                if (dictionary1.TryGetValue("messages", out obj))
                {
                    flag2 = true;
                    dictionary2 = (IDictionary<string, object>)obj;
                }
            }
            if (property != (PropertyInfo)null || flag1 && dictionary1.ContainsKey(rule))
            {
                object obj = flag1 ? dictionary1[rule] : property.GetValue(validationRule);
                if (obj.GetType().IsArray)
                {
                    object[] objectArray = this.ToObjectArray(obj);
                    if (objectArray != null)
                    {
                        if (!(objectArray[objectArray.Length - 1] is string format))
                            format = !flag2 || !dictionary2.ContainsKey(rule) ? this.defaultMessages[rule] : dictionary2[rule]?.ToString();
                        str = string.Format(format, objectArray);
                    }
                    else
                        str = string.Format(!flag2 || !dictionary2.ContainsKey(rule) ? this.defaultMessages[rule] : dictionary2[rule]?.ToString(), objectArray);
                }
                else
                {
                    string format = !flag2 || !dictionary2.ContainsKey(rule) ? this.defaultMessages[rule] : dictionary2[rule]?.ToString();
                    str = rule.Equals("email") ? string.Format(format, (object)fieldName) : string.Format(format, obj);
                }
            }
            return str;
        }
    }
}