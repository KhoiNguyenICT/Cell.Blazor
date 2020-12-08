using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Cell.Blazor._Core.Class
{
    public class Annotation
    {
        private PropertyInfo property;
        private object[] attributes;
        private Metadata meta;

        internal Annotation(PropertyInfo property, Metadata meta)
        {
            this.meta = meta;
            meta.Property = property;
            meta.Visible = true;
            attributes = property.GetCustomAttributes(true).ToArray();
            this.property = property;
            EnsureDisplay();
            EnsureDisplayFormat();
            EnsureEdit();
            EnsureVisibility();
            EnsureValidations();
            EnsureDisplay();
        }

        private void EnsureValidations()
        {
            Dictionary<string, object> valid = new Dictionary<string, object>();
            Dictionary<string, object> messages = new Dictionary<string, object>();
            GetCustomAttribute((Action<RequiredAttribute>)(e =>
            {
                if (e.ErrorMessage != null)
                    messages.Add("required", e.ErrorMessage);
                valid.Add("required", true);
            }));
            GetCustomAttribute((Action<StringLengthAttribute>)(str =>
            {
                if (str.ErrorMessage != null)
                    messages.Add("rangeLength", str.ErrorMessage);
                valid["rangeLength"] = new int[2]
                {
                    str.MinimumLength,
                    str.MaximumLength
                };
            }));
            GetCustomAttribute((Action<RangeAttribute>)(range =>
            {
                if (range.ErrorMessage != null)
                    messages.Add(nameof(range), range.ErrorMessage);
                valid[nameof(range)] = new object[2]
                {
                    range.Minimum,
                    range.Maximum
                };
            }));
            GetCustomAttribute((Action<RegularExpressionAttribute>)(regex =>
            {
                if (regex.ErrorMessage != null)
                    messages.Add(nameof(regex), regex.ErrorMessage);
                valid[nameof(regex)] = regex.Pattern;
            }));
            GetCustomAttribute((Action<MinLengthAttribute>)(minLength =>
            {
                if (minLength.ErrorMessage != null)
                    messages.Add(nameof(minLength), minLength.ErrorMessage);
                valid[nameof(minLength)] = minLength.Length;
            }));
            GetCustomAttribute((Action<MaxLengthAttribute>)(maxLength =>
            {
                if (maxLength.ErrorMessage != null)
                    messages.Add(nameof(maxLength), maxLength.ErrorMessage);
                valid[nameof(maxLength)] = maxLength.Length;
            }));
            GetCustomAttribute((Action<EmailAddressAttribute>)(mail =>
            {
                if (mail.ErrorMessage != null)
                    messages.Add("email", mail.ErrorMessage);
                valid.Add("email", true);
            }));
            GetCustomAttribute((Action<CompareAttribute>)(compare =>
            {
                if (compare.ErrorMessage != null)
                    messages.Add("equalTo", compare.ErrorMessage);
                valid["equalTo"] = compare.OtherProperty;
            }));
            GetCustomAttribute((Action<DataTypeAttribute>)(dt =>
            {
                string key;
                switch (dt.DataType)
                {
                    case DataType.Custom:
                        key = dt.CustomDataType;
                        break;
                    case DataType.DateTime:
                    case DataType.Date:
                        key = "date";
                        break;
                    case DataType.EmailAddress:
                        key = "email";
                        break;
                    case DataType.Url:
                        key = "url";
                        break;
                    case DataType.ImageUrl:
                        key = "url";
                        meta.Validations["accept"] = "image/*";
                        break;
                    default:
                        meta.CustomDataType = dt.DataType.ToString();
                        return;
                }
                meta.CustomDataType = key;
                valid[key] = true;
            }));
            if (messages.Count != 0)
                valid.Add("messages", messages);
            meta.Validations = valid;
        }

        private void EnsureDisplayFormat() => GetCustomAttribute((Action<DisplayFormatAttribute>)(format =>
        {
            meta.FormatString = format.DataFormatString;
            meta.ApplyFormatInEditMode = format.ApplyFormatInEditMode;
            meta.NeedsHtmlEncode = format.HtmlEncode;
        }));

        private void EnsureDisplay()
        {
            DisplayAttribute customAttribute = GetCustomAttribute<DisplayAttribute>();
            if (customAttribute == null)
                return;
            meta.ShouldAdd = customAttribute.GetAutoGenerateField();
            meta.GroupDisplayName = customAttribute.GetGroupName();
            meta.HeaderText = customAttribute.GetName();
            meta.Watermark = customAttribute.GetPrompt();
        }

        private void EnsureVisibility() => GetCustomAttribute((Action<ScaffoldColumnAttribute>)(s => meta.Visible = s.Scaffold));

        private void EnsureEdit()
        {
            GetCustomAttribute((Action<KeyAttribute>)(key => meta.IsPrimaryKey = true));
            GetCustomAttribute((Action<EditableAttribute>)(edit => meta.ReadOnly = !edit.AllowEdit));
            GetCustomAttribute((Action<DatabaseGeneratedAttribute>)(edit => meta.IsIdentity = edit.DatabaseGeneratedOption.Equals(DatabaseGeneratedOption.Identity)));
        }

        private T GetCustomAttribute<T>() where T : class => attributes.FirstOrDefault(a => a.GetType() == typeof(T)) as T;

        private void GetCustomAttribute<T>(Action<T> onComplete) where T : class
        {
            T customAttribute = GetCustomAttribute<T>();
            if (customAttribute == null || onComplete == null)
                return;
            onComplete(customAttribute);
        }
    }
}