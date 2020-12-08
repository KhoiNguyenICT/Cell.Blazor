namespace Cell.Blazor.Internal.Class
{
    public class AnimationSettings
    {
        public int Duration { get; set; } = 400;

        public string Name { get; set; } = "FadeIn";

        public string TimingFunction { get; set; } = "ease";

        public int Delay { get; set; }
    }
}