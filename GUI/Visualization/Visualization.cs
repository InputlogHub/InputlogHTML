using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using System.IO;

namespace GUI.Visualization
{
    public class Visualization : Form
    {
        protected virtual Chart GetChart() { return null; }
        protected virtual SaveFileDialog GetSaveDialog() { return null; }

        public void Save(string fileName, ChartImageFormat form)
        {
            GetChart().SaveImage(fileName, form);
        }

        public void SaveToStream(Stream stream, ChartImageFormat form)
        {
            GetChart().SaveImage(stream, form);
        }

        protected void SelectFormat()
        {

            DialogResult d = GetSaveDialog().ShowDialog();
            if (d == DialogResult.OK)
            {
                switch (GetSaveDialog().FilterIndex)
                {
                    case 1:
                        GetChart().SaveImage(GetSaveDialog().FileName, ChartImageFormat.Png);
                        break;
                    case 2:
                        GetChart().SaveImage(GetSaveDialog().FileName, ChartImageFormat.Bmp);
                        break;
                    case 3:
                        GetChart().SaveImage(GetSaveDialog().FileName, ChartImageFormat.Jpeg);
                        break;
                    case 4:
                        GetChart().SaveImage(GetSaveDialog().FileName, ChartImageFormat.Tiff);
                        break;
                    case 5:
                        GetChart().SaveImage(GetSaveDialog().FileName, ChartImageFormat.Emf);
                        break;
                }
            }
        }

        /// <summary>
        /// Preventing the graph from disposing its components when the user closes the window, because
        /// this resulted in an exception when the user tried to open the graph window again.
        /// Closing the analysis form now takes care of disposing the visualization window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void VisualisationClosing(object sender, FormClosingEventArgs e)
        {
            var findForm = FindForm();
            if (findForm != null) findForm.Hide();
            e.Cancel = true;
        }
    }
}
