using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Resources;
using System.Globalization;


using InputLog.Core.TemplateBuilder;
using InputLog.Core.Reporting.Resources;

namespace GUI.TemplateBuilder
{
    public partial class TemplateBuilder : Form
    {
        public TemplateBuilder()
        {
            InitializeComponent();
            PopulateResources();
        }

        private void PopulateResources() 
        {
            ResourceManager rm = new ResourceManager(typeof(ReportContentResources));
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var resources = rm.GetResourceSet(CultureInfo.InvariantCulture, true, true);
            var buttons = new List<PropertyButton>();
            foreach (DictionaryEntry resource in resources)
            {
                if ((resource.Key as string).Contains("_label")) // These are useless.
                    continue;

                var key = (resource.Key as string).Split(new String[] { "_introduction" }, StringSplitOptions.RemoveEmptyEntries)[0]; // each key ends with _introduction, so we want to split it

                var button = new PropertyButton(key, resource.Value as string);
                buttons.Add(button);
            }
            buttons = buttons.OrderBy(c => c.Property).ToList();
            for (var i = 0; i < buttons.Count; ++i)
            {
                buttons[i].Location = new Point(buttons[i].Location.X, i * buttons[i].Height);
            }
            panel3.Controls.AddRange(buttons.ToArray());
        }

        private void btn_add_block_Click(object sender, EventArgs e)
        {
            var previous_y = 0;
            if (panel2.Controls.Count > 0)
            {
                var previous = panel2.Controls[panel2.Controls.Count - 1];
                previous_y = previous.Location.Y + previous.Height;

            }
            var added = new Block();
            added.Location = new Point(added.Location.X, previous_y);
            panel2.Controls.Add(added);
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            var path = text_filename.Text;

            if (path == "")
                return;
            if (File.Exists(path))
                if (MessageBox.Show("A file with this filename already exists, are you sure you want to override this file?", "WARNING!", MessageBoxButtons.YesNo) 
                    != DialogResult.Yes) // Aks for confirmation to overwrite existing file
                    return;

            (sender as Control).Enabled = false;
            var blocks = new List<TemplateBlock>();
            foreach (var control in this.panel2.Controls)
            {
                var block = control as Block;
                if (block != null)
                {
                    blocks.Add(new TemplateBlock(block.Title, block.Introduction, block.Elements));
                }
            }

            var writer = new TemplateXMLWriter(path);
            writer.WriteStart(text_title.Text);
            writer.WriteBlocks(blocks);
            writer.WriteEnd();
            writer.Dispose();

            (sender as Control).Enabled = true;

        }

        public Block LastBlock()
        {
            if (panel2.Controls.Count != 0)
                return panel2.Controls[panel2.Controls.Count - 1] as Block;
            
            return null;
        }

        private void btn_loadExisting_Click(object sender, EventArgs e)
        {
            var path = text_filename.Text;

            if (path == "" || !File.Exists(path))
                return;

            var reader = new TemplateXMLReader(path);
            reader.Read();
            var blocks = reader.GetBlocks();
            text_title.Text = reader.GetTitle();

            panel2.Controls.Clear();
            foreach (var block in blocks)
            {
                var block_control = new Block(block.Title, block.Introduction, block.Data);
                block_control.Location = new Point(block_control.Location.X, panel2.Controls.Count * block_control.Height);
                panel2.Controls.Add(block_control);
            }
        }

        private void btn_browse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            var filename = openFileDialog1.FileNames.ToList()[0];
            this.text_filename.Text = filename;
        }
    }
}
