using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Cell.Blazor.Internal.Enums
{
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CellScriptModules
    {
        [EnumMember(Value = "none")] None,
        [EnumMember(Value = "cell-blazor")] CellBase,
        [EnumMember(Value = "cell-grid")] CellGrid,
        [EnumMember(Value = "cell-treegrid")] CellTreeGrid,
        [EnumMember(Value = "cell-textbox")] CellTextBox,
        [EnumMember(Value = "cell-numerictextbox")] CellNumericTextBox,
        [EnumMember(Value = "cell-dropdownlist")] CellDropDownList,
        [EnumMember(Value = "cell-calendarbase")] CellCalendarBase,
        [EnumMember(Value = "cell-datepicker")] CellDatePicker,
        [EnumMember(Value = "cell-timepicker")] CellTimePicker,
        [EnumMember(Value = "cell-toolbar")] CellToolbar,
        [EnumMember(Value = "cell-splitter")] CellSplitter,
        [EnumMember(Value = "cell-dialog")] CellDialog,
        [EnumMember(Value = "cell-tab")] CellTab,
        [EnumMember(Value = "cell-drop-down-button")] CellDropDownButton,
        [EnumMember(Value = "cell-tooltip")] CellTooltip,
        [EnumMember(Value = "cell-barcode")] CellBarcode,
        [EnumMember(Value = "cell-accordion")] CellAccordion,
        [EnumMember(Value = "cell-contextmenu")] CellContextMenu,
        [EnumMember(Value = "cell-menu")] CellMenu,
        [EnumMember(Value = "cell-listview")] CellListView,
        [EnumMember(Value = "cell-treeview")] CellTreeView,
        [EnumMember(Value = "cell-sidebar")] CellSidebar,
        [EnumMember(Value = "cell-toast")] CellToast,
        [EnumMember(Value = "cell-spinner")] CellSpinner,
        [EnumMember(Value = "cell-circulargauge")] CellCircularGauge,
        [EnumMember(Value = "cell-richtexteditor")] CellRichTextEditor,
        [EnumMember(Value = "cell-kanban")] CellKanban,
        [EnumMember(Value = "cell-schedule")] CellSchedule,
        [EnumMember(Value = "cell-treemap")] CellTreeMap,
        [EnumMember(Value = "cell-lineargauge")] CellLinearGauge,
        [EnumMember(Value = "cell-listbox")] CellListBox,
    }
}