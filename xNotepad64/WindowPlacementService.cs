using System.Drawing;

namespace xNotepad64
{
    public static class WindowPlacementService
    {
        public static WindowPlacementSettings Capture(Form form)
        {
            ArgumentNullException.ThrowIfNull(form);

            Rectangle bounds = form.WindowState == FormWindowState.Normal ? form.Bounds : form.RestoreBounds;
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                bounds = form.Bounds;
            }

            return new WindowPlacementSettings
            {
                Left = bounds.Left,
                Top = bounds.Top,
                Width = bounds.Width,
                Height = bounds.Height,
                WindowState = form.WindowState == FormWindowState.Minimized ? FormWindowState.Normal : form.WindowState
            };
        }

        public static void Apply(Form form, WindowPlacementSettings? placement)
        {
            ArgumentNullException.ThrowIfNull(form);

            if (placement is null)
            {
                return;
            }

            placement.Normalize();
            if (!placement.HasUsableBounds)
            {
                return;
            }

            Rectangle bounds = placement.ToBounds();
            if (!IntersectsAnyScreen(bounds))
            {
                return;
            }

            Rectangle workingArea = Screen.FromRectangle(bounds).WorkingArea;
            int width = Math.Min(bounds.Width, workingArea.Width);
            int height = Math.Min(bounds.Height, workingArea.Height);
            int left = Math.Max(workingArea.Left, Math.Min(bounds.Left, workingArea.Right - width));
            int top = Math.Max(workingArea.Top, Math.Min(bounds.Top, workingArea.Bottom - height));

            form.StartPosition = FormStartPosition.Manual;
            form.SetBounds(left, top, width, height);

            if (placement.WindowState == FormWindowState.Maximized)
            {
                form.WindowState = FormWindowState.Maximized;
            }
        }

        private static bool IntersectsAnyScreen(Rectangle bounds)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(bounds))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
