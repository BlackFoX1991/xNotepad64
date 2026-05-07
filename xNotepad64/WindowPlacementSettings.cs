using System.Drawing;

namespace xNotepad64
{
    public sealed class WindowPlacementSettings
    {
        private const int MinimumDimension = 120;

        public int Left { get; set; }

        public int Top { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public FormWindowState WindowState { get; set; } = FormWindowState.Normal;

        public bool HasUsableBounds => Width >= MinimumDimension && Height >= MinimumDimension;

        public Rectangle ToBounds()
        {
            return new Rectangle(Left, Top, Width, Height);
        }

        public WindowPlacementSettings Clone()
        {
            return new WindowPlacementSettings
            {
                Left = Left,
                Top = Top,
                Width = Width,
                Height = Height,
                WindowState = WindowState
            };
        }

        public void Normalize()
        {
            Width = Math.Max(Width, MinimumDimension);
            Height = Math.Max(Height, MinimumDimension);

            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
        }
    }
}
