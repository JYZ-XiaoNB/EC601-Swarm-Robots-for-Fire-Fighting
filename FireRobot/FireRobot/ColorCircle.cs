using System.Drawing;
using System.Windows.Forms;

public class ColorCircle : Control
{
    private Color _circleColor = Color.Green;

    public Color CircleColor
    {
        get => _circleColor;
        set
        {
            _circleColor = value;
            Invalidate();
        }
    }

    public ColorCircle()
    {
        DoubleBuffered = true;
        Width = 80;
        Height = 80;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        using (Brush b = new SolidBrush(_circleColor))
        {
            e.Graphics.FillEllipse(b, 0, 0, Width - 1, Height - 1);
        }
        using (Pen p = new Pen(Color.Black, 2))
        {
            e.Graphics.DrawEllipse(p, 0, 0, Width - 1, Height - 1);
        }
    }
}
