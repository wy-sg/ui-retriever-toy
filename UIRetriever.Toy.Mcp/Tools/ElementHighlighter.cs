using System.Drawing;
using System.Windows.Forms;
using UIRetriever.Bridge;

namespace UIRetriever.Toy.Mcp.Tools;

internal static class ElementHighlighter
{
    internal static void Show(BoundsData bounds, int durationMilliseconds = 2000)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        var thread = new Thread(() =>
        {
            using var form = new HighlightForm(bounds, durationMilliseconds);
            Application.Run(form);
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.IsBackground = true;
        thread.Start();
    }

    private sealed class HighlightForm : Form
    {
        private readonly System.Windows.Forms.Timer _timer;

        internal HighlightForm(BoundsData bounds, int durationMilliseconds)
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;
            Bounds = new Rectangle(bounds.X - 3, bounds.Y - 3, bounds.Width + 6, bounds.Height + 6);

            _timer = new System.Windows.Forms.Timer
            {
                Interval = durationMilliseconds <= 0 ? 2000 : durationMilliseconds
            };
            _timer.Tick += (_, _) => Close();
        }

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                const int wsExNoActivate = 0x08000000;
                const int wsExTransparent = 0x00000020;
                const int wsExToolWindow = 0x00000080;

                var cp = base.CreateParams;
                cp.ExStyle |= wsExNoActivate | wsExTransparent | wsExToolWindow;
                return cp;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using var pen = new Pen(Color.Red, 3);
            e.Graphics.DrawRectangle(pen, 1, 1, Width - 3, Height - 3);
        }
    }
}
