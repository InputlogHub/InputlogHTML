using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace CopyTaskCreator
{
    public partial class TaskControl : UserControl
    {
        LinkLabel.Link imageLink;
        bool isInformational;
        bool isExample;
        private bool m_containsImage = false;
        private bool m_containsAudio = false;
        bool isChanged = false;

        public bool containsImage
        {
            get
            {
                return m_containsImage;
            }
            set
            {
                m_containsImage = value;
                linkImage.Visible = value;
                removeImage.Visible = value;
            }
        }

        public bool containsAudio
        {
            get
            {
                return m_containsAudio;
            }
            set
            {
                m_containsAudio = value;
                myPlayer.Visible = value;
                removeAudio.Visible = value;
            }
        }


        public TaskControl()
        {
            InitializeComponent();
        }

        public String title()
        {
            return TitleTextField.Text;
        }

        private void pictClose_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
            this.Dispose();
        }

        public TaskControl(XmlNode node):this()
        {
            var info = XMLUtil.GetAttributeValue(node, "informational");
            if (info == "true")
            {
                setInformational();
            }

            var image = XMLUtil.GetAttributeValue(node, "image");
            if(image != "")
            {
                containsImage = true;
                
                imageLink = new LinkLabel.Link();
                imageLink.LinkData = CopyTaskCreator.fileDirectory + "\\" + image; 
                linkImage.Text = image;
                linkImage.Links.Add(imageLink);
            }

            var audio = XMLUtil.GetAttributeValue(node, "audio");
            if(audio != "")
            {
                containsAudio = true;
                myPlayer.URL = CopyTaskCreator.fileDirectory + "\\" + audio;
                myPlayer.Ctlcontrols.stop();
            }


            TitleTextField.Text = XMLUtil.GetAttributeValue(node, "title");
            InstructionsTextField.Text = XMLUtil.GetChildNodeValue(node, "instructions");

            TargetTextField.Text = XMLUtil.GetChildNodeValue(node, "target");

            var attr = node.Attributes["timelimit"];
            if (attr != null)
            {
                cbTimeLimit.Checked = true;
                nudTimeLimit.Value = Decimal.Parse(attr.Value);
            }

            var unlimited = node.Attributes["unlimited"];
            if (unlimited != null)
            {
                cbUnlimited.Checked = true;
                cbRepetition.Visible = false;
                lblTimes.Visible = false;
                nudRepetitionFrequency.Visible = false;
            }
            else try
            {
                var repetition = Decimal.Parse(node.Attributes["repetition"].Value);
                if (repetition > 1)
                {
                    cbRepetition.Checked = true;
                }
                
                nudRepetitionFrequency.Value = Decimal.Parse(node.Attributes["repetition"].Value);
            }
            catch (Exception)
            {
                // No action required. This simply means there is no attribute set.
            }
            
            try
            {
                this.isExample = Boolean.Parse(node.Attributes["example"].Value);
            }
            catch (Exception) {
                // No action required. This simply means there is no attribute set.
            }   
        }

        public void setInformational()
        {
            this.isInformational = true;
            TargetTextField.Visible = false;
            targetLbl.Visible = false;
            nudRepetitionFrequency.Visible = false;
            linkImage.Visible = false;
            btnChooseImage.Visible = false;
            cbRepetition.Visible = false;
            cbTimeLimit.Visible = false;
            InstructionsTextField.Multiline = true;
            InstructionsTextField.Size = new Size(540, 80);
            lblInstructions.Text = "Text";
            lblType.Text = "Text Block";
            cbUnlimited.Visible = false;
            //this.BackColor = Color.RosyBrown;
        }

        public void setExample()
        {
            this.isExample = true;
            lblType.Text = "Example";
            //this.BackColor = Color.SandyBrown;
        }

        /// <summary>
        /// Returns the subtask as an XElement. If an image or sound has been linked to the task, these files are copied to a temporary folder.
        /// </summary>
        /// <returns></returns>
        public XElement toXml()
        {
            XElement task = new XElement("task",new object[]{
                new XAttribute("title",TitleTextField.Text),
                new XElement("instructions",InstructionsTextField.Text),
                new XElement("target", TargetTextField.Text)
            });

            if (isInformational)
            {
                task.Add(new XAttribute("informational", "true"));
            }

            if(containsImage)
            {
                var filePath = CopyTaskCreator.fileDirectory + "\\" + linkImage.Text;
                if(!File.Exists(filePath))
                    File.Copy(imageLink.LinkData.ToString(), filePath);
                task.Add(new XAttribute("image", linkImage.Text));
            }

            if (containsAudio)
            {
                var audioFileName = Path.GetFileName(myPlayer.URL);
                var filePath = CopyTaskCreator.fileDirectory + "\\" + audioFileName;
                if(!filePath.Equals(myPlayer.URL) && !File.Exists(filePath))
                    File.Copy(myPlayer.URL, filePath);
                task.Add(new XAttribute("audio", audioFileName));
            }

            if (cbUnlimited.Checked)
            {
                task.Add(new XAttribute("unlimited", "true"));
            }
            else if (cbRepetition.Checked)
            {
                task.Add(new XAttribute("repetition", nudRepetitionFrequency.Value));
            }

            if (cbTimeLimit.Checked)
            {
                task.Add(new XAttribute("timelimit", nudTimeLimit.Value));
            }

            if(this.isExample)
            {
                task.Add(new XAttribute("example", "true"));
            }
            
            return task;
        }

        private void Repetition_CheckedChanged(object sender, EventArgs e)
        {
            nudRepetitionFrequency.Visible = cbRepetition.Checked;
            lblTimes.Visible = cbRepetition.Checked;
        }

        private void moveUp_Click(object sender, EventArgs e)
        {
            var parent = this.Parent;

            parent.Controls.SetChildIndex(this, parent.Controls.IndexOf(this) - 1);
        }

        private void moveDown_Click(object sender, EventArgs e)
        {
            var parent = this.Parent;

            parent.Controls.SetChildIndex(this, parent.Controls.IndexOf(this) + 1);
        }

        private void linkImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel lnk = (LinkLabel)sender;
            lnk.Links[lnk.Links.IndexOf(e.Link)].Visited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void btnChooseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.Multiselect = false;
            var dialogResult = ofd.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                if (imageLink != null)
                {
                    linkImage.Links.Remove(imageLink);
                }
                imageLink = new LinkLabel.Link();
                imageLink.LinkData = ofd.FileName;
                linkImage.Text = System.IO.Path.GetFileName(ofd.FileName);
                linkImage.Links.Add(imageLink);
                
                containsImage = true;
            }
        }

        private void btnChooseSound_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Audio Files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.Multiselect = false;
            var dialogResult = ofd.ShowDialog();
            
            if (dialogResult == DialogResult.OK)
            {
                myPlayer.URL = ofd.FileName;
                containsAudio = true;
            }
        }

        private void removeAudio_Click(object sender, EventArgs e)
        {
            myPlayer.URL = "";
            containsAudio = false;
        }

        private void removeImage_Click(object sender, EventArgs e)
        {
            containsImage = false;
        }

        private void cbTimeLimit_CheckedChanged(object sender, EventArgs e)
        {
            nudTimeLimit.Visible = cbTimeLimit.Checked;
            lblSeconds.Visible = cbTimeLimit.Checked;
        }

        private void cbUnlimited_CheckedChanged(object sender, EventArgs e)
        {
            cbRepetition.Visible = !cbUnlimited.Checked;
            if (cbUnlimited.Checked)
            {
                lblTimes.Visible = false;
                nudRepetitionFrequency.Visible = false;
            }
            else
            {
                if (cbRepetition.Checked)
                {
                    lblTimes.Visible = true;
                    nudRepetitionFrequency.Visible = true;
                }
            } 
        }
    }
}
