using System.Drawing;
using System.Drawing.Imaging;

namespace TNT.Drawing.Extensions;

public static class GraphicsExt
{
  public static void DrawImage(this Graphics graphics, Image image, System.Drawing.Point centerPoint, Color tint)
  {
    int size = image.Width;
    int x = centerPoint.X - size / 2;
    int y = centerPoint.Y - size / 2;

    float r = tint.R / 255f;
    float g = tint.G / 255f;
    float b = tint.B / 255f;

    // The first three rows (R, G, B) are set to 0 to remove the original image color.
    // This allows the tint color (set in the last row) to fully replace the image's color channels.
    ColorMatrix colorMatrix = new ColorMatrix(
      new float[][]
      {
        new float[] { 0, 0, 0, 0, 0 },
        new float[] { 0, 0, 0, 0, 0 },
        new float[] { 0, 0, 0, 0, 0 },
        new float[] { 0, 0, 0, 1, 0 },
        new float[] { r, g, b, 0, 1 },
      }
    );

    using ImageAttributes attributes = new ImageAttributes();
    attributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
    graphics.DrawImage(image, new Rectangle(x, y, size, size), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
  }
}
