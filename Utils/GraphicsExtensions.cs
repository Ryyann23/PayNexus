using System.Drawing.Drawing2D;

namespace PayNexus.Utils;

public static class GraphicsExtensions
{
    public static GraphicsPath ToRoundedPath(this Rectangle rectangle, int radius)
    {
        return ToRoundedPath(new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height), radius);
    }

    public static GraphicsPath ToRoundedPath(this RectangleF rectangle, float radius)
    {
        var path = new GraphicsPath();
        var diameter = radius * 2;

        if (radius <= 0)
        {
            path.AddRectangle(rectangle);
            path.CloseFigure();
            return path;
        }

        path.AddArc(rectangle.X, rectangle.Y, diameter, diameter, 180, 90);
        path.AddArc(rectangle.Right - diameter, rectangle.Y, diameter, diameter, 270, 90);
        path.AddArc(rectangle.Right - diameter, rectangle.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rectangle.X, rectangle.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}
