using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using InputLog.Core.Analyses.Fluency;
using System.IO;

namespace GUI.Visualization
{
    /// <summary>
    /// Displays a chart containing logged process data from a General Analysis.
    /// Series can be toggled on or off
    /// </summary>
    public partial class FluencyStdDevGraph : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FluencyStdDevGraph()
        {
            InitializeComponent();
        }

        public virtual void Process(string idfx, FluencyAnalysisSummary summary)
        {
            string name = Path.GetFileNameWithoutExtension(idfx);
            var point = new DataPoint
                            {
                                XValue = (100*summary.AverageSPM/summary.PersonalMaximum),
                                YValues = new[] {summary.StdDev},
                                ToolTip = name
                            };
            chart1.Series["Plot"].Points.Add(point);
        }

        public void Save(String fileName, ChartImageFormat form)
        {
            chart1.SaveImage(fileName, form);
        }

        private void VisualSaveButtonClick(object sender, EventArgs e)
        {
            DialogResult d = VisualSaveFileDialog.ShowDialog();
            if (d == DialogResult.OK)
            {
                switch (VisualSaveFileDialog.FilterIndex)
                {
                    case 1:
                        chart1.SaveImage(VisualSaveFileDialog.FileName, ChartImageFormat.Png);
                        break;
                    case 2:
                        chart1.SaveImage(VisualSaveFileDialog.FileName, ChartImageFormat.Bmp);
                        break;
                    case 3:
                        chart1.SaveImage(VisualSaveFileDialog.FileName, ChartImageFormat.Jpeg);
                        break;
                    case 4:
                        chart1.SaveImage(VisualSaveFileDialog.FileName, ChartImageFormat.Tiff);
                        break;
                    case 5:
                        chart1.SaveImage(VisualSaveFileDialog.FileName, ChartImageFormat.Emf);
                        break;
                }
            }
        }

        private void VisualizationFormFormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true; // this cancels the close event.
        }

    }
}