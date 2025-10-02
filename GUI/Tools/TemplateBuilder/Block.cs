using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI.TemplateBuilder
{
    public partial class Block : UserControl
    {
        public Block()
        {
            InitializeComponent();
        }

        public Block(string title, string introduction, string data)
        {
            InitializeComponent();
            this.text_title.Text = title;
            this.text_intro.Text = introduction;
            this.text_elements.Text = data;
            this.Text_elements_TextChanged(this, null); // update the text formatting.
        }

        private void swapBoxes(int index1, int index2) 
        {
            var old = Parent.Controls[index1];
            var new_control = Parent.Controls[index2];
            var new_location = new_control.Location;

            this.Parent.Controls.SetChildIndex(old, index2);
            this.Parent.Controls.SetChildIndex(new_control, index1);

            new_control.Location = old.Location;
            old.Location = new_location;
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            var index = Parent.Controls.IndexOf(this);
            for (var i = index + 1; i < Parent.Controls.Count; ++i) 
            {
                var control = Parent.Controls[i];
                control.Location = new Point(control.Location.X, control.Location.Y - this.Height);
            }
            Parent.Controls.Remove(this);
        }

        private void btn_up_Click(object sender, EventArgs e)
        {
            var index = Parent.Controls.IndexOf(this);
            if (index != 0)
            {
                var new_index = index - 1;
                swapBoxes(index, new_index);
            }
        }

        private void btn_down_Click(object sender, EventArgs e)
        {
            var index = Parent.Controls.IndexOf(this);
            if (index != Parent.Controls.Count-1)
            {
                var new_index = index + 1;
                swapBoxes(index, new_index);
            }
        }

        private void Text_elements_TextChanged(object sender, System.EventArgs e)
        {
            var cursor_position = text_elements.SelectionStart; // Assume nothing is selected.
            text_elements.Select(0, text_elements.TextLength);
            text_elements.SelectionFont = text_elements.Font;

            if (text_elements.Text.Contains("[["))
            {
                var current_index = 0;
                while (text_elements.Text.IndexOf("[[", current_index) != -1)
                {
                    var start_index = text_elements.Text.IndexOf("[[", current_index);
                    if (text_elements.Text.Contains("]]"))
                    {
                        var end_index = text_elements.Text.IndexOf("]]", start_index);
                        if (end_index > start_index)
                        {
                            var bold_font = new Font(text_elements.Font, FontStyle.Bold);
                            text_elements.Select(start_index, end_index - start_index + 2); // + 2, as ]] has length 2
                            text_elements.SelectionFont = bold_font;
                        }
                    }
                    current_index = Math.Min(start_index + 1, text_elements.TextLength-1);
                }
            }

            text_elements.Select(cursor_position, 0); // reset to original position
        }

        public void AddPropertyToElements(string property)
        {
            this.text_elements.Text += property;
        }
    }
}
