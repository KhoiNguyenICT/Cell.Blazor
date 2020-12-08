namespace Cell.Blazor.Internal.Class
{
    public class RippleSettings
    {
        public string Selector { get; set; }

        public string Ignore { get; set; }

        public bool RippleFlag { get; set; } = true;

        public bool ISCenterRipple { get; set; }

        public int Duration { get; set; } = 350;
    }
}