using System.ComponentModel;

namespace Cell.Blazor._Core.Class
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class DiagramLocale
    {
        public string Copy { get; set; } = nameof(Copy);

        public string Cut { get; set; } = nameof(Cut);

        public string Paste { get; set; } = nameof(Paste);

        public string Undo { get; set; } = nameof(Undo);

        public string Redo { get; set; } = nameof(Redo);

        public string SelectAll { get; set; } = "Select All";

        public string Grouping { get; set; } = nameof(Grouping);

        public string Group { get; set; } = nameof(Group);

        public string UnGroup { get; set; } = "Ungroup";

        public string Order { get; set; } = nameof(Order);

        public string BringToFront { get; set; } = "Bring To Front";

        public string MoveForward { get; set; } = "Move Forward";

        public string SendToBack { get; set; } = "Send To Back";

        public string SendBackward { get; set; } = "Send Backward";
    }
}