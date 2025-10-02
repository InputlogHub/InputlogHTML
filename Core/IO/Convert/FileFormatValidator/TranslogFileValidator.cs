using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Schema;
using InputLog.Core.Util;
using InputLog.Core.Util.Validation;

namespace InputLog.Core.IO.Convert.FileFormatValidator
{
    /// <summary>
    ///  Validates a Translog file against its xsd and the xml extension. 
    /// </summary>
    public class TranslogFileValidator : IValidator<string>
    {
        #region private_fields

        /// <summary>
        /// The expected file extension.
        /// </summary>
        private const string TL_EXT = ".xml";

        /// <summary>
        /// Remarks on the validition so far.
        /// </summary>
        private string CurrentRemark = "";

        /// <summary>
        /// Translog file is valid or not.
        /// </summary>
        private bool IsValid;

        #endregion

        /// <summary>
        ///  Validates a Translog file against its xsd and the xml extension.
        /// </summary>
        /// <param name="input">File to be validated</param>
        /// <returns>True if the file is valid, false if it is not.</returns>
        public bool Validate(string input)
        {
            string extension = Path.GetExtension(input);
            CurrentRemark = "";
            if (extension != TL_EXT)
            {
                CurrentRemark = "Translog files have a 'xml' extension.";
                return false;
            }

            var schemas = new XmlSchemaSet();
            string fullPath = Path.GetDirectoryName(Application.ExecutablePath) +
                              @"\IO\Convert\FileFormatValidator\Translog.xsd";
            schemas.Add(null, fullPath);
            XDocument doc;
            try
            {
                doc = XDocument.Load(input);
            }
            catch (Exception e)
            {
                CurrentRemark = "Could not load the document.\n" + e.Message;
                return false;
            }

            // Checks if doc conforms to Translog.xsd.
            string msg = "";
            doc.Validate(schemas, (o, e) => { msg = e.Message; });

            if (msg == "")
            {
                CurrentRemark = string.Format("File \"{0}\" is a valid Translog document.\n",
                    StringUtils.ShortenPathname(input, 50));
                IsValid = true;
            }
            else
            {
                CurrentRemark = string.Format("File \"{0}\" does not match the current Translog XML Schema." +
                                              "\nIf this is a true Translog record, consider to update its XSD file\n" +
                                              "located at: \"\\IO\\Convert\\FileFormatValidator\\Translog.xsd\".\nRemark: ",
                                              StringUtils.ShortenPathname(input, 50)) + msg;
                
                IsValid = false;
            }
            return IsValid;
        }

        /// <summary>
        ///     Gets the remark for the last validated file. Validating a new file resets the remark of
        ///     the previous file.
        /// </summary>
        /// <returns>A string remark for the file that has been last validated.</returns>
        public string Remark()
        {
            return CurrentRemark;
        }
    }
}