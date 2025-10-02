using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using WebApp.Controllers;

namespace CopyTaskCreator
{
    class ServerConnection
    {
        public static async Task<bool> checkCredentials(string username, string password)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string> { { "username", username }, { "password", password } };
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("http://localhost:18477/Account/LogOnAjax", content);


                var responseString = await response.Content.ReadAsStringAsync();
                var status = JsonConvert.DeserializeObject<AccountController.LoginStatus>(responseString);

                return status.Success;
            }
        }

        public static async Task<String> uploadCopyTask(string username, string password, string zipPath, string filename)
        {
            using (var client = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    var values = new Dictionary<string, string> { { "username", username }, { "password", password }, { "filename", filename } };
                    var otherContent = new FormUrlEncodedContent(values);
                    content.Add(new StringContent(username), "\"username\"");
                    content.Add(new StringContent(password), "\"password\"");
                    content.Add(new StringContent(filename), "\"filename\"");

                    string dataString = HttpUtility.UrlEncode(File.ReadAllBytes(zipPath)); //new ByteArrayContent()
                    content.Add(new StringContent(dataString), "\"file\"");

                    var response = await client.PostAsync("http://localhost:18477/Copy/UploadCopyTask", content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    return responseString;
                }
            }
        }
    }
}
