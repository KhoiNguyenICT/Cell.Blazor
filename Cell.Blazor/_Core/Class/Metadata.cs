using System.Collections.Generic;
using System.Reflection;

namespace Cell.Blazor._Core.Class
{
    public class Metadata
    {
        internal bool IsPrimaryKey { get; set; }

        internal bool IsAllowedToDisplay { get; set; }

        internal bool IsIdentity { get; set; }

        internal string ForeignKey { get; set; }

        internal bool Visible { get; set; }

        internal bool ApplyFormatInEditMode { get; set; }

        internal bool NeedsHtmlEncode { get; set; }

        internal bool ReadOnly { get; set; }

        internal string HeaderText { get; set; }

        internal string Watermark { get; set; }

        internal bool? ShouldAdd { get; set; }

        internal string GroupDisplayName { get; set; }

        internal string FormatString { get; set; }

        internal string Template { get; set; }

        internal Dictionary<string, object> Validations { get; set; }

        internal string CustomDataType { get; set; }

        internal PropertyInfo Property { get; set; }

        internal Metadata() => this.IsAllowedToDisplay = true;
    }
}