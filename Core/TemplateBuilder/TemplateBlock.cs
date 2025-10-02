using System;
using System.Linq;
using System.Xml;


namespace InputLog.Core.TemplateBuilder
{
    public class TemplateBlock
    {
        #region Fields

        public string Title;

        public string Introduction;

        public string Data;

        public string IntroductionXML 
        {
            get 
            {
                if (_introductionXML != "")
                    return _introductionXML;


                var newlines = Introduction.Count(c => c == '\n');
                var complex = false;
                var retvalue = "";

                if (newlines != 0)
                {
                    complex = true;
                    var split = Introduction.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in split)
                    {
                        if (line.Length > 2 && line.Substring(0, 2) == "*)")
                            retvalue += "<list>" + line.Substring(2) + "</list>\n";
                        else
                            retvalue += "<paragraph>" + line + "</paragraph>\n";
                    }
                }
                else 
                {
                    retvalue = Introduction;
                }
                retvalue = "<prepend format=" + (complex ? "complex" : "simple") + ">" + retvalue.TrimEnd(new char[] { ' ', '\n' }) + "<\\prepend>\n";
                _introductionXML = retvalue;
                return _introductionXML;
            }
        }

        private string _introductionXML = "";

        #endregion

        public TemplateBlock(string title, string intro, string data)
        {
            Title = title;
            Introduction = intro;
            Data = data;
        }

        public void WriteIntroductionXML(XmlWriter writer)
        {
            if (Introduction == "")  // Stop early if there is no Introduction
                return;
            var newlines = Introduction.Count(c => c == '\n');
            var complex = newlines != 0;

            writer.WriteStartElement("prepend");
            writer.WriteAttributeString("format", complex ? "complex" : "simple");
            if (complex)
            {
                var split = Introduction.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.None);
                var list = false;
                foreach (var line in split)
                {
                    if (line.Length > 2 && line.Substring(0, 2) == "*)")
                    {
                        if (!list) //write list start element
                            writer.WriteStartElement("list");

                        list = true;
                        writer.WriteElementString("item", line.Substring(2));
                    }
                    else
                    {
                        if (list) // write list end element
                            writer.WriteEndElement();

                        list = false;
                        writer.WriteElementString("paragraph", line);
                    }
                }
                if (list) // write list end element
                    writer.WriteEndElement();
            }
            else
            {
                writer.WriteString(Introduction);
            }
            writer.WriteEndElement();
        }

        public void WriteElementsXML(XmlWriter writer)
        {
            var split = Data.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            writer.WriteStartElement("elements");

            foreach (var line in split)
            {
                var split_element = line.Split(new string[] { "[[", "]]" }, StringSplitOptions.RemoveEmptyEntries);
                
                if (split_element.Length == 1 && split_element[0].Trim() == "")  // This is an empty line (with spaces), this should not be disallowed as an 
                    continue;
                
                if (split_element.Length < 2)
                    throw new InvalidOperationException("line \"" + line + "\" does not represent a valid element");

                writer.WriteStartElement("element");
                writer.WriteElementString("introduction", "");
                writer.WriteStartElement("value");

                writer.WriteStartElement("label");
                writer.WriteAttributeString("bold", "false");
                writer.WriteString(split_element[0].Trim() + " ");
                writer.WriteEndElement();

                writer.WriteElementString("value_id", split_element[1]);

                if (split_element.Length > 2)
                    writer.WriteElementString("append", " " + split_element[2].Trim());

                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
