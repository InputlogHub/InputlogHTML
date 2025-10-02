using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace InputLog.Core.Reporting.ReportTemplate.Import
{
    /// <summary>
    ///     Import a ReportTemplate from an XML.
    /// </summary>
    public class XmlTemplateImporter : TemplateImporter
    {
        #region Data properties (override from base class)
        private string _currentReportID;
        public override string CurrentReportID => _currentReportID;
        private string _currentReportLocalization;
        public override string CurrentReportLocalization => _currentReportLocalization;
        private string _currentReportTitle;
        public override string CurrentReportTitle => _currentReportTitle;
        private List<ReportTemplate.StructuredText> _currentBlockAppend;
        public override List<ReportTemplate.StructuredText> CurrentBlockAppend => _currentBlockAppend;
        private List<ReportTemplate.StructuredText> _currentBlockPrepend;
        public override List<ReportTemplate.StructuredText> CurrentBlockPrepend => _currentBlockPrepend;
        private string _currentBlockTitle;
        public override string CurrentBlockTitle => _currentBlockTitle;
        private string _currentElementIntroduction;
        public override string CurrentElementIntroduction => _currentElementIntroduction;
        private bool _hasMoreBlocks;
        public override bool HasMoreBlocks => _hasMoreBlocks;
        private bool _hasMoreElements;
        public override bool HasMoreElements => _hasMoreElements;
        private bool _hasMoreValues;
        public override bool HasMoreValues => _hasMoreValues;
        private string _currentValueAppend;
        public override string CurrentValueAppend => _currentValueAppend;
        private string _currentValueLabel;
        public override string CurrentValueLabel => _currentValueLabel;
        private bool _currentValueLabelIsBold;
        public override bool CurrentValueLabelIsBold => _currentValueLabelIsBold;
        private string _currentValuePrepend;
        public override string CurrentValuePrepend => _currentValuePrepend;
        private string _currentValueValueID;
        public override string CurrentValueValueID => _currentValueValueID;
        #endregion

        /// <summary>
        ///     Path to the file to be imported.
        /// </summary>
        private string _path;

        /// <summary>
        ///     The template document xml loaded in memory.
        /// </summary>
        private XmlDocument Xml;

        /// <summary>
        ///     Reference to the current report node we are processing
        ///     in the template xml
        /// </summary>
        private XmlNode CurrentReport;

        /// <summary>
        ///     Reference to the current block node we are processing
        ///     in the template xml
        /// </summary>
        private XmlNode CurrentBlock;

        /// <summary>
        ///     Kept for error reporting purposes.
        /// </summary>
        private int _currentBlockIndex;

        /// <summary>
        ///     Reference to the current element node we are processing
        ///     in the template xml
        /// </summary>
        private XmlNode CurrentElement;

        /// <summary>
        ///     Kept for error reporting purposes.
        /// </summary>
        private int _currentElementIndex;

        /// <summary>
        ///     Reference to the current value node we are processing
        ///     in the template xml
        /// </summary>
        private XmlNode CurrentValue;

        /// <summary>
        ///     Kept for error reporting purposes.
        /// </summary>
        private int _currentValueIndex;

        /// <summary>
        ///     Boolean that keeps track whether the reporting
        ///     resources have already been loaded or not.
        /// </summary>
        private bool _resourcesLoaded;

        /// <summary>
        ///     An empty array constant.
        /// </summary>
        private readonly string[] EMPTY_ARRAY = { };

        /// <summary>
        ///     Array used when a text element is expected
        /// </summary>
        private readonly string[] TEXT_ARRAY = { "#text" };

        /// <summary>
        ///     File that we should import.
        /// </summary>
        /// <param name="file">Path to the file this class should import.</param>
        public XmlTemplateImporter(string file)
        {
            FileInfo fileInfo = new FileInfo(file);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("File (" + file + ") not found.");
            }
            _path = file;
        }

        /// <summary>
        ///     Start the template importing.
        /// </summary>
        public override void StartTemplate()
        {
            try
            {
                base.StartTemplate();
                Xml = new XmlDocument();
                Xml.Load(_path);
            }
            catch (Exception e)
            {
                AddError(new Error(
                    "template-file",
                    "Could not parse template file: " + e.Message,
                    Severity.FATAL
                ));
                throw e;
            }
        }

        /// <summary>
        ///     Unset reference to xml template document.
        /// </summary>
        public override void EndTemplate()
        {
            base.EndTemplate();
            Xml = null;
        }

        /// <summary>
        ///     Start report importing.
        ///     Loads characteristic such as id, language and title.
        /// </summary>
        public override void StartReport()
        {
            base.StartReport();
            CurrentReport = Xml.SelectSingleNode("/report");
            _checkForDanglingSingles(CurrentReport, "/", new[] { "id", "language", "title" });
            _checkForUnknownElements(CurrentReport, "/", new[] { "id", "language", "title", "block" });
            _checkForUnknownAttributes(CurrentReport, "/", EMPTY_ARRAY);

            // Retrieve id
            const string UNKNOWN_ID = "unknown-id";
            XmlNode idNode = CurrentReport.SelectSingleNode("id");
            if (idNode != null)
            {
                _checkForUnknownElements(idNode, "id", TEXT_ARRAY);
                _checkForUnknownAttributes(idNode, "id", EMPTY_ARRAY);

                if (!_nodeContainsSimpleText(idNode))
                {
                    AddError(new Error(
                        "id",
                        "Id content should be a simple, non-empty string. Complex element encountered.",
                        Severity.ERROR
                    ));
                    _currentReportID = UNKNOWN_ID;
                }
                else
                {
                    _currentReportID = _preprocessString(idNode.InnerText);
                }
            }
            else
            {
                AddError(new Error(
                    "id",
                    "Report id not set.",
                    Severity.ERROR
                ));
                _currentReportID = UNKNOWN_ID;
            }

            // Retrieve language
            const string DEFAULT_LANGUAGE = "en-us";
            XmlNode languageNode = CurrentReport.SelectSingleNode("language");
            if (languageNode != null)
            {
                _checkForUnknownAttributes(languageNode, "language", EMPTY_ARRAY);
                _checkForUnknownElements(languageNode, "language", TEXT_ARRAY);

                if (!_nodeContainsSimpleText(languageNode))
                {
                    AddError(new Error(
                        "language",
                        "Language of report not recognized. This should be a simple, non-empty string such as \"en-us\"",
                        Severity.ERROR
                    ));
                    _currentReportLocalization = DEFAULT_LANGUAGE;
                }
                else
                {
                    string language = _preprocessString(languageNode.InnerText);
                    if (_isValidLanguageCode(language))
                    {
                        _currentReportLocalization = language;
                    }
                    else
                    {
                        AddError(new Error(
                            "language",
                            "Language of report not recognized. Language value \"" + language + "\" " +
                                "is invalid. Defaulting to \"" + DEFAULT_LANGUAGE + "\".",
                            Severity.WARNING
                        ));
                        _currentReportLocalization = DEFAULT_LANGUAGE;
                    }
                }
            }

            // Retrieve title
            const string DEFAULT_TITLE = "Report";
            XmlNode titleNode = CurrentReport.SelectSingleNode("title");
            if (titleNode != null)
            {
                _checkForUnknownElements(titleNode, "title", TEXT_ARRAY);
                _checkForUnknownAttributes(titleNode, "title", EMPTY_ARRAY);

                if (!_nodeContainsSimpleText(titleNode))
                {
                    AddError(new Error(
                        "title",
                        "Title invalid. This should be a simple, non-empty string, not a complex element. " +
                            "Using default title \"" + DEFAULT_TITLE + "\"",
                        Severity.WARNING
                    ));
                    _currentReportTitle = DEFAULT_TITLE;
                }
                else
                {
                    _currentReportTitle = _preprocessString(titleNode.InnerText);
                }
            }
            else
            {
                _currentReportTitle = DEFAULT_TITLE;
                AddError(new Error(
                    "title",
                    "Title not set. Using default title \"" + DEFAULT_TITLE + "\"",
                    Severity.WARNING
                ));
            }
        }

        /// <summary>
        ///     Reset report values
        /// </summary>
        public override void EndReport()
        {
            base.EndReport();

            // Reset values.
            CurrentReport = null;
            _currentReportID = null;
            _currentReportLocalization = null;
            _currentReportTitle = null;
        }

        /// <summary>
        ///     Checks whether there are any blocks in the first place and sets index
        ///     and CurrentBlock.
        /// </summary>
        public override void StartBlocks()
        {
            base.StartBlocks();
            _hasMoreBlocks = CurrentReport.SelectNodes("/report/block").Count > 0;

            if (_hasMoreBlocks)
            {
                // Selects first block.
                CurrentBlock = CurrentReport.SelectSingleNode("block");
                _currentBlockIndex = 0;
            }
        }

        /// <summary>
        ///     Reads basic block information such as:
        ///     - title
        ///     - prepend
        ///     - append 
        ///     And sets the current block as CurrentBlock.
        /// </summary>
        public override void StartBlock()
        {
            base.StartBlock();

            string nodeId = "block-" + _currentBlockIndex;

            // Check block
            _checkForUnknownAttributes(CurrentBlock, nodeId, EMPTY_ARRAY);
            _checkForDanglingSingles(CurrentBlock, nodeId, new[] { "title", "prepend", "append", "elements" });
            _checkForUnknownElements(CurrentBlock, nodeId, new[] { "title", "prepend", "append", "elements" });


            // Get title.
            XmlNode titleNode = CurrentBlock.SelectSingleNode("title");
            const string DEFAULT_TITLE = "Block-";
            if (titleNode != null)
            {
                _checkForUnknownElements(titleNode, nodeId + "-title", TEXT_ARRAY);
                _checkForUnknownAttributes(titleNode, nodeId + "-title", EMPTY_ARRAY);

                if (_nodeContainsSimpleText(titleNode))
                {
                    _currentBlockTitle = _preprocessString(titleNode.InnerText);
                }
                else
                {
                    AddError(new Error(
                        nodeId + "-title",
                        "Title invalid. This should be a simple, non-empty string, not a complex element.",
                        Severity.ERROR
                    ));
                    _currentBlockTitle = DEFAULT_TITLE + _currentBlockIndex;
                }
            }
            else
            {
                AddError(new Error(
                    nodeId + "-title",
                    "Title not set.",
                    Severity.ERROR
                ));
                _currentBlockTitle = DEFAULT_TITLE + _currentBlockIndex;
            }

            // Get prepend
            XmlNode prependNode = CurrentBlock.SelectSingleNode("prepend");
            if (prependNode != null)
            {
                _checkForUnknownAttributes(prependNode, nodeId + "-prepend", new[] { "format" });
                _checkForUnknownElements(prependNode, nodeId + "-prepend", new[] { "#text", "paragraph", "list" });

                XmlAttribute formatNode = prependNode.Attributes["format"];
                bool isSimpleFormat = (formatNode == null) || (formatNode.InnerText == "simple");
                bool isComplexFormat = (formatNode == null) || (formatNode.InnerText == "complex");

                // Either is simple, or is complex. Nothing else.
                if (formatNode == null)
                {
                    AddError(new Error(
                        nodeId + "-prepend",
                        "Must specify prepend format. Format should be either 'simple' or 'complex'.",
                        Severity.ERROR
                    ));
                }
                else if (formatNode != null && !(isSimpleFormat ^ isComplexFormat))
                {
                    AddError(new Error(
                        nodeId + "-prepend",
                        "Invalid format. Should be 'simple' or 'complex'.",
                        Severity.ERROR
                    ));
                }
                else
                {
                    if (!_nodeContainsSimpleText(prependNode) && isSimpleFormat)
                    {
                        AddError(new Error(
                            nodeId + "-prepend",
                            "Inconsistent format. Format is 'simple' but content is complex.",
                            Severity.WARNING
                        ));
                        _currentBlockPrepend = _parseStructuredText(
                            prependNode,
                            nodeId + "-prepend"
                        );
                    }
                    else if (_nodeContainsSimpleText(prependNode))
                    {
                        if (isComplexFormat)
                        {
                            AddError(new Error(
                                nodeId + "-prepend",
                                "Inconsistent format. Format is 'complex' but content is simple.",
                                Severity.WARNING
                            ));
                        }
                        _currentBlockPrepend = new List<ReportTemplate.StructuredText>(1);
                        _currentBlockPrepend.Add(
                            new ReportTemplate.StructuredText(
                                _preprocessString(prependNode.InnerText)));
                    }
                    else
                    {
                        // Read the paragraphs and lists.
                        _currentBlockPrepend = _parseStructuredText(
                            prependNode,
                            nodeId + "-prepend"
                        );
                    }
                }
            }

            // Get append
            XmlNode appendNode = CurrentBlock.SelectSingleNode("append");
            if (appendNode != null)
            {
                _checkForUnknownAttributes(appendNode, nodeId + "-append", new[] { "format" });
                _checkForUnknownElements(appendNode, nodeId + "-append", new[] { "#text", "paragraph", "list" });

                XmlAttribute formatNode = appendNode.Attributes["format"];
                bool isSimpleFormat = (formatNode == null) || (formatNode.InnerText == "simple");
                bool isComplexFormat = (formatNode == null) || (formatNode.InnerText == "complex");

                // Either is simple, or is complex. Nothing else.
                if (formatNode == null)
                {
                    AddError(new Error(
                        nodeId + "-append",
                        "Must specify prepend format. Format should be either 'simple' or 'complex'.",
                        Severity.ERROR
                    ));
                }
                else if (formatNode != null && !(isSimpleFormat ^ isComplexFormat))
                {
                    AddError(new Error(
                        nodeId + "-append",
                        "Invalid format. Should be 'simple' or 'complex'.",
                        Severity.ERROR
                    ));
                }
                else
                {
                    if (!_nodeContainsSimpleText(appendNode) && isSimpleFormat)
                    {
                        AddError(new Error(
                            nodeId + "-append",
                            "Inconsistent format. Format is 'simple' but content is complex.",
                            Severity.WARNING
                        ));
                        _currentBlockAppend = _parseStructuredText(
                            appendNode,
                            nodeId + "-append"
                        );
                    }
                    else if (_nodeContainsSimpleText(appendNode))
                    {
                        if (isComplexFormat)
                        {
                            AddError(new Error(
                                nodeId + "-append",
                                "Inconsistent format. Format is 'simple' but content is complex.",
                                Severity.WARNING
                            ));
                        }
                        _currentBlockAppend = new List<ReportTemplate.StructuredText>(1);
                        _currentBlockAppend.Add(
                            new ReportTemplate.StructuredText(
                                _preprocessString(appendNode.InnerText)));
                    }
                    else
                    {
                        // Read the paragraphs and lists.
                        _currentBlockAppend = _parseStructuredText(
                            appendNode,
                            nodeId + "-append"
                        );
                    }
                }
            }
        }

        /// <summary>
        ///     - Sets hasMoreBlocks
        ///     - Resets current block values.
        /// </summary>
        public override void EndBlock()
        {
            base.EndBlock();
            _hasMoreBlocks =
                (CurrentBlock.NextSibling != null) &&
                (CurrentBlock.NextSibling.Name == "block");

            // Set next block node to current.
            if (_hasMoreBlocks)
            {
                CurrentBlock = CurrentBlock.NextSibling;
                _currentBlockIndex += 1;
            }

            // Reset CurrentBlock Values
            _currentBlockTitle = null;
            _currentBlockPrepend = null;
            _currentBlockAppend = null;
        }

        /// <summary>
        ///     Resets CurrentBlock.
        /// </summary>
        public override void EndBlocks()
        {
            base.EndBlocks();
            CurrentBlock = null;
            _currentBlockIndex = -1;
        }

        /// <summary>
        ///     Checks whether there are any elements at all, and sets CurrentElement
        ///     and index.
        /// </summary>
        public override void StartElements()
        {
            base.StartElements();

            XmlNode elementsNode = CurrentBlock.SelectSingleNode("elements");
            if (elementsNode != null)
            {
                string nodeId = "block-" + _currentBlockIndex + "-elements";
                _checkForUnknownAttributes(elementsNode, nodeId, EMPTY_ARRAY);
                _checkForUnknownElements(elementsNode, nodeId, new[] { "element" });
            }

            _hasMoreElements = CurrentBlock.SelectSingleNode("elements/element") != null;

            if (_hasMoreElements)
            {
                CurrentElement = CurrentBlock.SelectSingleNode("elements/element");
                _currentElementIndex = 0;
            }
        }

        /// <summary>
        ///     Gets the data from an element
        ///     - Gets description
        ///     - Gets label
        ///     - Gets value id
        /// </summary>
        public override void StartElement()
        {
            base.StartElement();
            string currentNodeId =
                "block-" + _currentBlockIndex +
                "-element-" + _currentElementIndex;

            // Check element
            _checkForDanglingSingles(CurrentElement, currentNodeId, new[] { "introduction" });
            _checkForUnknownElements(CurrentElement, currentNodeId,
                new[] { "introduction", "value", "table" });
            _checkForUnknownAttributes(CurrentElement, currentNodeId, EMPTY_ARRAY);

            // Get introduction
            XmlNode intrNode = CurrentElement.SelectSingleNode("introduction");
            if (intrNode != null)
            {
                _checkForUnknownElements(intrNode, currentNodeId + "-introduction", TEXT_ARRAY);
                _checkForUnknownAttributes(intrNode, currentNodeId + "-introduction", EMPTY_ARRAY);

                if (_nodeIsEmpty(intrNode))
                {
                    _currentElementIntroduction = string.Empty;
                }
                else if (_nodeContainsSimpleText(intrNode))
                {
                    _currentElementIntroduction = _preprocessString(intrNode.InnerText);
                }
                else
                {
                    AddError(new Error(
                        currentNodeId + "-introduction",
                        "Introduction invalid. This should be a simple," +
                        " non-empty string, not a complex element. Introduction ignored.",
                        Severity.WARNING
                    ));
                }
            }
        }

        /// <summary>
        ///     - Sets hasMoreElements
        ///     - Resets current element values.
        /// </summary>
        public override void EndElement()
        {
            base.EndElement();
            _hasMoreElements = CurrentElement.NextSibling is { Name: "element" };

            if (_hasMoreElements)
            {
                CurrentElement = CurrentElement.NextSibling;
                _currentElementIndex += 1;
            }

            // Reset values
            _currentElementIntroduction = null;
        }

        /// <summary>
        ///     Resets CurrentElement & Index
        /// </summary>
        public override void EndElements()
        {
            base.EndElements();

            _currentElementIndex = -1;
            CurrentElement = null;
        }

        /// <summary>
        ///     Initializes hasMoreValues
        ///     Set CurrentValue node
        ///     Set value index to 0
        /// </summary>
        public override void StartValues()
        {
            base.StartValues();
            _hasMoreValues = CurrentElement.SelectSingleNode("value") != null;

            if (_hasMoreValues)
            {
                CurrentValue = CurrentElement.SelectSingleNode("value");
            }
            else
            {
                string nodeId =
                    "block-" + _currentBlockIndex +
                    "-element-" + _currentElementIndex;
                AddError(new Error(
                    nodeId,
                    "Element does not contain any values. Element is ignored.",
                    Severity.ERROR
                ));

                // Unset current element information
                _currentElementIntroduction = null;
            }

            _currentValueIndex = 0;
            _currentValueLabelIsBold = true;
        }

        /// <summary>
        ///     Read value's information
        ///     - Value_id
        ///     - Label (bold/not)
        ///     - Prepend (optional)
        ///     - Append (optional)
        /// </summary>
        public override void StartValue()
        {
            base.StartValue();
            string currentNodeId =
                "block-" + _currentBlockIndex +
                "-element-" + _currentElementIndex +
                "-value-" + _currentValueIndex;

            // Check value
            _checkForDanglingSingles(CurrentValue, currentNodeId, new[] { "value_id", "label", "prepend", "append" });
            _checkForUnknownElements(CurrentValue, currentNodeId, new[] { "value_id", "label", "prepend", "append" });
            _checkForUnknownAttributes(CurrentValue, currentNodeId, EMPTY_ARRAY);

            // Get value-id. This is mandatory as it links to which value of which analysis
            // has to be inserted into the report
            //
            XmlNode valueIdNode = CurrentValue.SelectSingleNode("value_id");
            if (valueIdNode != null)
            {
                _checkForUnknownElements(valueIdNode, currentNodeId + "-value_id", TEXT_ARRAY);
                _checkForUnknownAttributes(valueIdNode, currentNodeId + "-value_id", EMPTY_ARRAY);

                if (_nodeContainsSimpleText(valueIdNode) && _isValidValueID(valueIdNode.InnerText))
                {
                    _currentValueValueID = _preprocessString(valueIdNode.InnerText);
                }
                else
                {
                    AddError(new Error(
                        currentNodeId + "-value_id",
                        "Value-id (" + valueIdNode.InnerText + ") not recognized. Value skipped!",
                        Severity.ERROR
                    ));
                    // Don't process this value any further.
                    return;
                }
            }
            else
            {
                AddError(new Error(
                    currentNodeId + "-value_id",
                    "No value-id specified. This value is mandatory!",
                    Severity.ERROR
                ));
                // Don't process this value any further.
                return;
            }

            // Get label
            XmlNode labelNode = CurrentValue.SelectSingleNode("label");
            if (labelNode != null)
            {
                _checkForUnknownElements(labelNode, currentNodeId + "-label", TEXT_ARRAY);
                _checkForUnknownAttributes(labelNode, currentNodeId + "-label", new[] { "bold" });

                if (_nodeContainsSimpleText(labelNode))
                {
                    _currentValueLabel = _preprocessString(labelNode.InnerText);
                }
                else if (!_nodeIsEmpty(labelNode))
                {
                    AddError(new Error(
                        currentNodeId + "-label",
                        "Label invalid. This should be a simple, string (optionally empty), not a complex element. Alternative label ignored.",
                        Severity.WARNING
                    ));
                }

                // Is bold set or not?
                XmlAttribute boldAttribute = labelNode.Attributes["bold"];

                bool isBold = (boldAttribute == null) || boldAttribute.InnerText == "true";
                bool isNotBold = (boldAttribute == null) || boldAttribute.InnerText == "false";

                if (boldAttribute != null)
                {
                    // Should be either 'true' or 'false', no other values allowed.
                    if (!(isBold ^ isNotBold))
                    {
                        AddError(new Error(
                            currentNodeId + "-label-bold",
                            "Bold value invalid. Should be either 'true' or 'false', or ommitted entirely (default: true). Using default.",
                            Severity.WARNING
                        ));
                    }
                    else
                    {
                        _currentValueLabelIsBold = isBold;
                    }
                }

                // 
                // Special handling of the label node being empty or not: 
                // - If the label node is empty, but bold attribute is present
                //      -> use default label: so leave label null
                // - If the label node is empty, and bold attribute is not present
                //      -> use empty label, bold is irrelevant
                // - If the label is not empty, use the label and the bold value, or the
                //      default bold value.
                bool useDefaultLabel = (_nodeIsEmpty(labelNode) && boldAttribute != null);
                bool useEmptyLabel = (_nodeIsEmpty(labelNode) && boldAttribute == null);
                // label not empty has been handled in the code above.

                if (useDefaultLabel)
                {
                    _currentValueLabel = null;
                }
                else if (useEmptyLabel)
                {
                    _currentValueLabel = "";
                }
            }

            // Get 'prepend'. A Value prepend is limited to a single string of "simple" format. Specifying format
            // for value-prepend is optional.
            XmlNode prependNode = CurrentValue.SelectSingleNode("prepend");
            if (prependNode != null)
            {
                _checkForUnknownElements(prependNode, currentNodeId + "-prepend", TEXT_ARRAY);
                _checkForUnknownAttributes(prependNode, currentNodeId + "-prepend", new[] { "format" });

                XmlAttribute formatAttr = prependNode.Attributes["format"];
                bool isSimple = (formatAttr == null) || formatAttr.InnerText == "simple";

                if (!isSimple)
                {
                    AddError(new Error(
                        currentNodeId + "-prepend-format",
                        "Format value invalid. Only 'simple' is allowed for a value-prepend, " +
                        "or format should be ommitted entirely.",
                        Severity.ERROR
                    ));
                }
                else
                {
                    if (_nodeContainsSimpleText(prependNode))
                    {
                        _currentValuePrepend = _preprocessString(prependNode.InnerText);
                    }
                    else
                    {
                        AddError(new Error(
                            currentNodeId + "-prepend",
                            "Prepend invalid. This should be a simple, non-empty string, " +
                            "not a complex element. Prepend ignored.",
                            Severity.WARNING
                        ));
                    }
                }
            }

            // Get 'append'. A Value append is limited to a single string of "simple" format. Specifying format
            // for value-append is optional.
            XmlNode appendNode = CurrentValue.SelectSingleNode("append");
            if (appendNode != null)
            {
                _checkForUnknownElements(appendNode, currentNodeId + "-append", TEXT_ARRAY);
                _checkForUnknownAttributes(appendNode, currentNodeId + "-append", new[] { "format" });

                XmlAttribute formatAttr = appendNode.Attributes["format"];
                bool isSimple = (formatAttr == null) || formatAttr.InnerText == "simple";

                if (!isSimple)
                {
                    AddError(new Error(
                        currentNodeId + "-append-format",
                        "Format value invalid. Only 'simple' is allowed for a value-append, " +
                        "or format should be ommitted entirely.",
                        Severity.ERROR
                    ));
                }
                else
                {
                    if (_nodeContainsSimpleText(appendNode))
                    {
                        _currentValueAppend = _preprocessString(appendNode.InnerText);
                    }
                    else
                    {
                        AddError(new Error(
                            currentNodeId + "-append",
                            "Append invalid. This should be a simple, non-empty string," +
                            " not a complex element. Append ignored.",
                            Severity.WARNING
                        ));
                    }
                }
            }
        }

        /// <summary>
        ///     - Check whether there's more values
        ///     - Set to next Value if there is a next Value
        ///     - Reset currentValue's data.
        /// </summary>
        public override void EndValue()
        {
            base.EndValue();
            _hasMoreValues =
                CurrentValue.NextSibling is { Name: "value" };

            if (_hasMoreValues)
            {
                CurrentValue = CurrentValue.NextSibling;
                _currentValueIndex += 1;
            }

            // Reset Values
            _currentValueAppend = null;
            _currentValuePrepend = null;
            _currentValueValueID = null;
            _currentValueLabel = null;
            _currentValueLabelIsBold = true;
        }
        /// <summary>
        ///     Resets value index and value node 
        /// </summary>
        public override void EndValues()
        {
            base.EndValue();

            _currentValueIndex = -1;
            CurrentValue = null;
        }

        /// <summary>
        ///     Checks whether a node contains simple text or 
        ///     a complex xml element.
        /// </summary>
        /// <param name="node">The node we are checking.</param>
        /// <returns>True if the node contains only simple text, false
        /// if it contains a complex xml element.</returns>
        private bool _nodeContainsSimpleText(XmlNode node)
        {
            Debug.Assert(node != null);
            return node.ChildNodes.Count == 1 && node.FirstChild.Name == "#text";
        }

        /// <summary>
        ///     Returns true if the node does not contain any children,
        ///     this may be used to check whether a label or introduction node
        ///     is empty or not.
        ///     May also be used to see whether a paragraph is empty or not.
        /// </summary>
        /// <param name="node">Node to inspect</param>
        /// <returns>True if it is empty, false if not.</returns>
        private bool _nodeIsEmpty(XmlNode node)
        {
            Debug.Assert(node != null);
            return node.ChildNodes.Count == 0;
        }

        /// <summary>
        ///     Checks whether a resource with given id actually exists in 
        ///     the list of resources.
        /// </summary>
        /// <param name="valueId">Id of the requested resource</param>
        /// <returns>True if the resource exists, false if not.</returns>
        private bool _isValidValueID(string valueId)
        {
            if (!_resourcesLoaded)
            {
                Report.Report.LoadResources(_currentReportLocalization);
                _resourcesLoaded = true;
            }
            return Report.Report.ResourceExists(valueId);
        }

        /// <summary>
        ///     Checks a node to see whether it has any attributes that it 
        ///     shouldn't have.
        /// </summary>
        /// <param name="node">node to be inspected</param>
        /// <param name="nodeId">Id of the node - used for error reporting</param>
        /// <param name="validAttributes">Attributes that are expected</param>
        private void _checkForUnknownAttributes(XmlNode node, string nodeId, string[] validAttributes)
        {
            foreach (XmlAttribute attr in node.Attributes)
            {
                if (!validAttributes.Contains(attr.Name))
                {
                    string valids = validAttributes.Length == 0 ? "None" : String.Join(", ", validAttributes);
                    AddError(new Error(
                        nodeId,
                        "Node uses unknown attribute \"" + attr.Name + "\"." +
                            "Valid attributes are: " + valids,
                        Severity.WARNING
                    ));
                }
            }
        }

        /// <summary>
        ///     Checks a node to see whether it has any elements that 
        ///     are unknown.
        /// </summary>
        /// <param name="node">Node to be inspected</param>
        /// <param name="nodeId">Id of the node - used for error reporting</param>
        /// <param name="validElements">Elements that are expected</param>
        private void _checkForUnknownElements(XmlNode node, string nodeId, string[] validElements)
        {
            foreach (XmlNode element in node.ChildNodes)
            {
                if (!validElements.Contains(element.Name))
                {
                    string valids = validElements.Length == 0 ? "None" : String.Join(", ", validElements);
                    AddError(new Error(
                        nodeId,
                        "Node has unknown element\"" + element.Name + "\"." +
                            "Valid elements are: " + valids,
                        Severity.WARNING
                    ));
                }
            }
        }

        /// <summary>
        ///     Check to see whether there are are multiple nodes of the same
        ///     tag where we only expect to see one node of that tag.
        /// </summary>
        /// <param name="node">Node to be inspected</param>
        /// <param name="nodeId">Id of the node - used for error reporting</param>
        /// <param name="validElements">Elements that are expected</param>
        private void _checkForDanglingSingles(XmlNode node, string nodeId, string[] singles)
        {
            foreach (string tagName in singles)
            {
                if (node.SelectNodes(tagName).Count > 1)
                {
                    AddError(new Error(
                        nodeId + "-" + tagName,
                        "Encountered multiple nodes of type (\"" + tagName + "\") when only a single " +
                            "node of given type is supported. Undefined behavior",
                        Severity.ERROR
                    ));
                }
            }

        }

        /// <summary>
        ///     Preprocess a string. This filters out all newlines and tabs in the string, 
        ///     and multiple spaces in the string. Replaces invalid characters with spaces,
        ///     but does not create sequences of spaces (more than one space).
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>string after filtering newlines, tabs, and all multiple spaces.</returns>
        private string _preprocessString(string input)
        {
            StringBuilder newString = new StringBuilder(input.Length);
            HashSet<char> invalidCharacters = new HashSet<char>(new[] { '\n', '\r', '\t', ' ' });
            bool isSpaceSequence = false;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (!invalidCharacters.Contains(c))
                {
                    newString.Append(c);
                    isSpaceSequence = false;
                }
                else
                {
                    if (!isSpaceSequence)
                    {
                        newString.Append(' ');
                        isSpaceSequence = true;
                    }
                }
            }
            return newString.ToString();
        }

        /// <summary>
        ///     Get a parent node containing children forming a structured text, and
        ///     parse all the children's XML elements to return a correct sequence
        ///     of StructuredText information for the XmlNode.
        /// </summary>
        /// <param name="parent">The parent node of the structuredText node-children.</param>
        /// <param name="parentNodeId">String id of the parent node that can be used report errors.</param>
        /// <returns>A list of parsed StructuredText items.</returns>
        private List<ReportTemplate.StructuredText> _parseStructuredText(XmlNode parent, string parentNodeId)
        {
            // Upper bound: parent.ChildNodes.Count
            List<ReportTemplate.StructuredText> texts = new List<ReportTemplate.StructuredText>(parent.ChildNodes.Count);

            int index = 0;
            foreach (XmlNode textNode in parent.ChildNodes)
            {
                // Paragraph, should contain simple text.
                if (textNode.Name == "paragraph")
                {
                    string nodeId = parentNodeId + "-node-" + index + "(paragraph)";
                    _checkForUnknownElements(textNode, nodeId, TEXT_ARRAY);
                    _checkForUnknownAttributes(textNode, nodeId, EMPTY_ARRAY);

                    if (_nodeContainsSimpleText(textNode) || _nodeIsEmpty(textNode))
                    {
                        texts.Add(new ReportTemplate.StructuredText(_preprocessString(textNode.InnerText)));
                    }
                    else
                    {
                        AddError(new Error(
                            nodeId,
                            "Paragraph invalid. This should be a simple string, not a complex element.",
                            Severity.ERROR
                        ));
                    }
                }
                // List should contain complex elements <item>
                else if (textNode.Name == "list")
                {
                    string nodeId = parentNodeId + "-node-" + index + "(list)";
                    _checkForUnknownElements(textNode, nodeId, new[] { "item" });
                    _checkForUnknownAttributes(textNode, nodeId, EMPTY_ARRAY);

                    if (_nodeContainsSimpleText(textNode))
                    {
                        AddError(new Error(
                            nodeId,
                            "List invalid. List should be a complex element containing 'item's, " +
                            "not a simple string.",
                            Severity.ERROR
                        ));
                    }
                    else
                    {
                        ReportTemplate.StructuredText list = new ReportTemplate.StructuredText(ReportTemplate.TextType.LIST);
                        texts.Add(list);
                        // Fill list with items.
                        foreach (XmlNode itemNode in textNode.ChildNodes)
                        {
                            if (itemNode.Name != "item")
                            {
                                AddError(new Error(
                                    nodeId,
                                    "Invalid node in 'list'. Valid nodes are 'item'. Item skipped.",
                                    Severity.WARNING
                                ));
                                // Skip item.
                                index += 1;
                                continue;
                            }

                            if (_nodeContainsSimpleText(itemNode))
                            {
                                list.AddItem(_preprocessString(itemNode.InnerText));
                            }
                            else
                            {
                                AddError(new Error(
                                    nodeId,
                                    "Item invalid. This should be a simple, non-empty, string," +
                                    " not a complex element.",
                                    Severity.ERROR
                                ));
                            }

                            // Go on to next item.
                            index += 1;
                        }
                    }
                }

                index += 1;
            }

            return texts;
        }

        /// <summary>
        ///     Checks whether the specified language is recognized as a culture or not.
        /// </summary>
        /// <param name="language">Language code of the specified language</param>
        /// <returns>True if the language code corresponds to a known culture, false if it does not.</returns>
        private bool _isValidLanguageCode(string language)
        {
            try
            {
                CultureInfo info = CultureInfo.CreateSpecificCulture(language);
                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }
    }
}
