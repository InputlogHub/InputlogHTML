using System.Windows.Forms.DataVisualization.Charting;

namespace GUI.Visualization
{
    public static class VisualizationExtensions
    {
        public static int RoundTo(int split, int val)
        {
            int mod = split % val;
            return split + (val - mod);
        }

        public static void AddAll(this DataPointCollection lhs, DataPointCollection rhs)
        {
            foreach (DataPoint p in rhs)
            {
                lhs.Add(p);
            }
        }
    }
}
