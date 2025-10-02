using System;
using System.Windows.Forms.DataVisualization.Charting;

namespace GUI.Util
{
    class ChartImageUtils
    {
        public static ChartImageFormat StrToImageFormat(String s)
        {
            if (s.Equals("PNG"))
            {
                return ChartImageFormat.Png;
            }
            if (s.Equals("BMP"))
            {
                return ChartImageFormat.Bmp;
            }
            if (s.Equals("EMF"))
            {
                return ChartImageFormat.Emf;
            }
            if (s.Equals("TIFF"))
            {
                return ChartImageFormat.Tiff;
            }
            if (s.Equals("JPEG"))
            {
                return ChartImageFormat.Jpeg;
            }
            return ChartImageFormat.Png;
        }
    }
}
