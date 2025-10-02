using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace InputLog.Core.Util.Server
{
    public class SimpleWebappInteraction : WebClient
    {
        private const string LOGIN_PAGE_ADDRESS = "Account/LogOn";
        private const string TASK_ADDRESS = "Task/Index";
        public readonly string BaseURL;
        private AccountSettings Account;
        private CookieContainer CookieContainer { get; set; }

        public SimpleWebappInteraction(string baseURL)
        {
            BaseURL = baseURL;
            CookieContainer = new CookieContainer();
        }

        /// <summary>
        /// Starts the login procedure in the hidden WebBrowser Control.
        /// </summary>
        /// <returns>True if the user logged in successfully</returns>
        public bool Login(bool silent, AccountSettings accSett)
        {
            try
            {
                if (!IsLoggedIn())
                {
                    Account = accSett ?? AccountSettings.LoadSettings();
                    if (Account.AutoLogin || silent)
                    {
                        TryLogin();
                        if (IsLoggedIn()) return true;
                    }
                    while (!IsLoggedIn())
                    {
                        if (!silent)
                        {
                            DialogResult dialog = CredentialsDialog();
                            if (dialog != DialogResult.OK) return false;
                        }
                        TryLogin();
                        if (!silent && !IsLoggedIn())
                        {
                            MessageBox.Show("Login failed. The entered credentials might be incorrect. " +
                                            "If the problem persists, please contact the Inputlog server administrators",
                                "Login failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                return true;
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(null, exc, Severity.ERROR, "Unable to connect to InputLog server.");
                return false;
            }
        }

        public DialogResult CredentialsDialog()
        {
            var form = new Form();
            var idLabel = new Label();
            var idText = new TextBox();
            var passLabel = new Label();
            var passText = new TextBox();
            var buttonOk = new Button();
            var buttonCancel = new Button();

            form.Text = "Please enter your credentials";
            idLabel.Text = "Username";
            idText.Text = Account.ID;
            passLabel.Text = "Password";
            passText.Text = Account.Pass;
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
            Account.ID = idText.Text;
            Account.Pass = passText.Text;
            return dialogResult;
        }

        public bool IsLoggedIn()
        {
            var response = Navigate("Task");
            bool res = !response.ResponseUri.ToString().StartsWith(BaseURL + LOGIN_PAGE_ADDRESS);
            response.Close();
            return res;
        }

        public WebResponse Navigate(string url)
        {
            var request = GetWebRequest(new Uri(BaseURL + url));
            request.Method = "GET";
            if (request is HttpWebRequest)
            {
                ((HttpWebRequest)request).CookieContainer = CookieContainer;
                ((HttpWebRequest)request).SendChunked = false;
                ((HttpWebRequest)request).ContentLength = 0;
            }
            return request.GetResponse();
        }

        public XmlDocument NavigateXml(string url)
        {
            string s = DownloadString(BaseURL + url);
            return ToXmlDoc(s);
        }

        public bool TryLogin()
        {
            var data = new NameValueCollection();
            data["Password"] = Account.Pass;
            data["UserName"] = Account.ID;
            data["RememberMe"] = Account.AutoLogin.ToString();

            var b = UploadValues(BaseURL + LOGIN_PAGE_ADDRESS, "POST", data);
            var c = Encoding.UTF8.GetString(b);
            XmlDocument d = ToXmlDoc(c);
            var title = d.GetElementsByTagName("title").Item(0);
            return title != null && !title.InnerText.Contains("Log On");
        }

        public bool PostWithFile(string url, string filePath, Dictionary<string, string> info)
        {
            WebResponse response = PostWithFileResponse(url, filePath, info);
            return !response.ResponseUri.ToString().Equals(BaseURL + url);
        }

        public WebResponse PostWithFileResponse(string url, string filePath, Dictionary<string, string> info)
        {
            var postData = new NameValueCollection();
            foreach (var kvp in info)
            {
                postData.Add(kvp.Key, kvp.Value);
            }
            WebResponse response = Upload.PostFile(new Uri(BaseURL + url),
                        postData, filePath, null, null, CookieContainer, null);
            return response;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null && request is HttpWebRequest)
            {
                ((HttpWebRequest)request).CookieContainer = CookieContainer;
            }
            return request;
        }

        private XmlDocument ToXmlDoc(string response)
        {
            if (response.StartsWith("<!--"))
            {
                response = response.Substring(response.IndexOf("-->") + 3);
            }
            string x = response.Substring(response.IndexOf('\r') + 2);
            int scPos = x.IndexOf("<!-- Start of StatCounter");
            if (scPos > 0)
            {
                x = x.Substring(0, scPos) + "</body>" + "</html>";
            }
            string s = x.Replace("&", " ");
            var doc = new XmlDocument();
            doc.Load(new XmlSanitizingStream(GenerateStreamFromString(s)));
            return doc;
        }

        private Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        
        public void DownloadThisFile(string url, string path)
        {
            DownloadFile(BaseURL + url, path);
        }
    }
}
