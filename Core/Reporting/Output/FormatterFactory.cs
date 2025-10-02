using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace InputLog.Core.Reporting.Output
{
    /// <summary>
    ///     Can be used to request which formats exist for outputting of reports 
    ///     and to get a formatter for a certain output type.
    /// </summary>
    public static class FormatterFactory
    {
        /// <summary>
        ///     Known formats for output
        /// </summary>
        public enum Format { PDF, XML };

        /// <summary>
        ///     Available format;
        /// </summary>
        public static IEnumerable<Format> FORMATS = new List<Format>(
            new[] {
                Format.PDF, 
                Format.XML
            }
        );

        /// <summary>
        ///     Get a formatter for specified type.
        /// </summary>
        /// <param name="format">Format type</param>
        /// <returns>Formatter to output a report in given format type.</returns>
        public static Formatter CreateFormatter(Format format)
        {
            switch (format)
            {
                case Format.XML:
                    return new XmlFormatter();
                case Format.PDF:
                    return new PdfFormatter();
                default:
                    Debug.Assert(false, "Unknown output format for Reporting - FormatterFactory");
                    return null;
            }
        }

        public static Formatter CreateFormatter(string format)
        {
            switch (format)
            {
                case "PDF":
                    return CreateFormatter(Format.PDF);
                case "XML":
                    return CreateFormatter(Format.XML);
                default:
                    throw new ArgumentException("Unknown output format for reporting - FormatterFactory");
            }
        }

    }
}
