using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CopyTaskCreator
{
    class Credentials
    {
        public static DialogResult CredentialsDialog()
        {
            var form = new Form();
            var idLabel = new Label();
            var idText = new TextBox();
            var passLabel = new Label();
            var passText = new TextBox();
            var buttonOk = new Button();
            var buttonCancel = new Button();

            form.Icon = Properties.Resources.inputlog;
            form.Text = "Please enter your credentials";
            idLabel.Text = "Username";
            idText.Text = Properties.Settings.Default.username;
            passLabel.Text = "Password";
            passText.Text = Properties.Settings.Default.password;
            passText.UseSystemPasswordChar = true;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            idLabel.SetBounds(10, 20, 60, 20);
            idText.SetBounds(85, 15, 250, 20);
            passLabel.SetBounds(10, 50, 70, 20);
            passText.SetBounds(85, 45, 250, 20);
            buttonOk.SetBounds(150, 85, 75, 23);
            buttonCancel.SetBounds(250, 85, 75, 23);

            idLabel.AutoSize = true;
            idText.Anchor = idText.Anchor | AnchorStyles.Right;
            passText.Anchor = passText.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new System.Drawing.Size(396, 128);
            form.Controls.AddRange(new Control[] { idLabel, idText, passLabel, passText, buttonOk, buttonCancel });
            form.ClientSize = new System.Drawing.Size(Math.Max(300, idLabel.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                Properties.Settings.Default.username = idText.Text;
                Properties.Settings.Default.password = passText.Text;
                Properties.Settings.Default.Save();
            }

            return dialogResult;
        }
    }
}
