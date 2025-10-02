
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;
using InputLog.Core.Reporting.Output;
using InputLog.Core.Reporting.ReportTemplate;
using InputLog.Core.Reporting.Resources;

namespace InputLog.Core.Reporting.Report
{
    /// <summary>
    /// This class is used to construct the reports. It is filled up with all the values
    /// that should be included in the report, including how they should be structured.
    /// </summary>
    public class Report
    {
        /// <summary>
        /// The output template for the report.
        /// </summary>
        private readonly ReportTemplate.ReportTemplate _template;

        /// <summary>
        ///     Maps analysis names to the targets contained in them.
        /// </summary>
        public Dictionary<string, ICollection<string>> TargetsPerAnalysis
        {
            get
            {
                if (this._targetsPerAnalysis == null)
                {
                    if (_template == null)
                    {
                        return null;
                    }
                    IEnumerable<Report.ReportResource> resourceTargets = this._template.GetReportTargets();
                    this._targetsPerAnalysis = new Dictionary<string, ICollection<string>>();
                    foreach (Report.ReportResource target in resourceTargets)
                    {
                        if (!this._targetsPerAnalysis.ContainsKey(target.Analysis))
                        {
                            this._targetsPerAnalysis.Add(target.Analysis, new List<string>());
                        }
                        this._targetsPerAnalysis[target.Analysis].Add(target.BaseId);
                    }
                }
                return this._targetsPerAnalysis;
            }
        }
        private Dictionary<string, ICollection<string>> _targetsPerAnalysis;


        /// <summary>
        ///     Dictionary mapping the value_id's present in this report
        ///     to their value_id's for easy retrieval from the template.
        /// </summary>
        private Dictionary<string, ReportValue> Values;

        /// <summary>
        /// Create a new report, this is based on a template.
        /// </summary>
        /// <param name="template"></param>
        public Report(ReportTemplate.ReportTemplate template)
        {
            Debug.Assert(template != null, "Can not create a report from a null template");
            this._template = template;
            this.Values = new Dictionary<string, ReportValue>();
        }

        /// <summary>
        ///     Add a value with given value id to the report.
        /// </summary>
        /// <param name="value_id">The id of the value - equal to the id in the template</param>
        /// <param name="value">The actual value of the item (post analysis)</param>
        public void AddValue(string value_id, ReportValue value)
        {
            this.Values[value_id] = value;
        }


        /// <summary>
        /// Format the report using the specified formatter.
        /// </summary>
        /// <param name="formatter"></param>
        public void Format(Formatter formatter)
        {
            formatter.StartReport(this._template);
            formatter.StartBlocks(this._template.Blocks.Count());

            foreach (BlockTemplate blockTemplate in this._template.Blocks)
            {
                this.FormatBlock(formatter, blockTemplate);
            }

            formatter.EndBlocks();
            formatter.EndReport();
        }

        /// <summary>
        ///     Format a block using the specified formatter and
        ///     block template
        /// </summary>
        private void FormatBlock(Formatter formatter, BlockTemplate blockTemplate)
        {
            formatter.StartBlock(blockTemplate);
            formatter.StartElements(blockTemplate.Elements.Count);
            for (int i = 0; i < blockTemplate.Elements.Count; i++)
            {
                ElementTemplate eTemplate = blockTemplate.Elements[i];
                this.FormatElement(formatter, eTemplate);
            }
            formatter.EndElements();
            formatter.EndBlock();
        }

        /// <summary>
        ///     Format an element using the specified formatter and
        ///     the specified element template
        /// </summary>
        /// <param name="formatter"></param>
        /// <param name="elementTemplate"></param>
        private void FormatElement(Formatter formatter, ElementTemplate elementTemplate)
        {
            formatter.StartElement(elementTemplate);
            formatter.StartValues(elementTemplate.Values.Count);
            foreach (ValueTemplate template in elementTemplate.Values)
            {
                ReportValue value;
                if (!this.Values.ContainsKey(template.TargetID))
                {
                    value = new EmptyValue(template.TargetID);
                }
                else 
                {
                    value = this.Values[template.TargetID];
                }
                value.Format(formatter, template);
            }
            formatter.EndValues();
            formatter.EndElement();
        }

        //
        // Functionality and fields associated with the loading and changing 
        // of the resource values for the reportable fields
        // 
        #region Static report resources strings & functions

        private static Dictionary<string, ReportResource> _loadedResources;
        private static List<string> _supportedLanguages;
        private static bool _resourcesAreLoaded = false;

        /// <summary>
        /// Loads the ReportingResources.resx file into memory and makes allows
        /// </summary>
        /// <param name="language">The language for the resource file. Default is 'en-us'.</param>
        public static void LoadResources(string language="en-us")
        {
            _loadedResources = new Dictionary<string, ReportResource>();

            // Language specific resources that get shown in the report
            CultureInfo culture = CultureInfo.CreateSpecificCulture(language);
            ResourceSet content_loadedResources = ReportContentResources.ResourceManager.GetResourceSet(culture, true, true);
            foreach (DictionaryEntry entry in content_loadedResources)
            {
                ProcessResourceEntry(entry);
            }

            // Language invariable resources - show the mapping between the ids for the report 
            // and the methods & summaries bound to it.
            ResourceSet mapping_loadedResources = ReportMappingResources.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);
            foreach (DictionaryEntry entry in mapping_loadedResources)
            {
                ProcessResourceEntry(entry);
            }

            _resourcesAreLoaded = true;
        }

        /// <summary>
        ///     Save the currently loaded resources to their respective
        ///     language file as determined by the language parameter.
        ///     Note, this assumes that the changes to the resources have been
        ///     made directly to the ReportResource elements returned from the
        ///     GetResources() or GetResource(string) methods.
        /// </summary>
        /// <param name="language">Language id of the updated resource file.</param>
        public static void SaveResources(string language = "en-us")
        {
            // Resource files (.resx) can not be changed unless
            // the executable is recompiled...

            // We need to switch the from resource files to some form
            // of localized settings. This will require some more 
            // work from our part.
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Get a list of all the languages that are supported for the copyTask.
        ///     This returns the 2-land-code/2-language-code strings to identify
        ///     the supported languages.
        /// </summary>
        /// <returns>The list of all available reporting languages.</returns>
        public static List<string> GetSupportedLanguages()
        {
            if (_supportedLanguages != null)
            {
                return _supportedLanguages;
            }

            _supportedLanguages = new List<string>();
            ResourceManager rm = new ResourceManager(typeof(ReportContentResources));
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (CultureInfo culture in cultures)
            {
                try
                {
                    ResourceSet rs = rm.GetResourceSet(culture, true, false);
                    if (rs != null)
                    {
                        _supportedLanguages.Add(culture.Name);
                    }
                }
                catch (CultureNotFoundException exc)
                {
                    // Ignore.
                }
            }
            return _supportedLanguages;
        }

        /// <summary>
        /// Handles a single entry from a reporting resource file, and aggregates
        /// the data by baseId.
        /// </summary>
        /// <param name="entry">Entry from a resource file</param>
        private static void ProcessResourceEntry(DictionaryEntry entry)
        {
            string resourceKey = (string)entry.Key;
            string resource = (string)entry.Value;

            // E.g.: separate 'Pause_PauseTime_label' into: 
            // baseId: 'Pause_PauseTime', and 
            // field: 'label' 
            int indexOfIdSplitter = resourceKey.LastIndexOf('_');
            string baseId = resourceKey.Substring(0, indexOfIdSplitter);
            string field = resourceKey.Substring(indexOfIdSplitter + 1);

            ReportResource currentResource = null;
            if (_loadedResources.ContainsKey(baseId))
            {
                currentResource = _loadedResources[baseId];
            }
            else
            {
                currentResource = new ReportResource(baseId);
                _loadedResources.Add(baseId, currentResource);
            }
            currentResource.SetText(field, resource);
        }

        /// <summary>
        /// Load a reporting element's resource information such as labels, descriptions,
        /// introductions etc.
        /// </summary>
        /// <param name="resourceId">The id of the reporting element.</param>
        /// <returns></returns>
        public static ReportResource GetResource(string resourceId)
        {
            Debug.Assert(_resourcesAreLoaded);
            ReportResource returnValue = null;
            _loadedResources.TryGetValue(resourceId, out returnValue);
            return returnValue;
        }

        public static IEnumerable<ReportResource> GetResources()
        {
            return _loadedResources.Values;
        }

        public static IEnumerable<string> GetResourceIDs()
        {
            return _loadedResources.Keys;
        }

        public static bool ResourceExists(string resourceId)
        {
            return _loadedResources.Keys.Contains(resourceId);
        }

        /// <summary>
        ///     Compares to report resources based on their id.
        /// </summary>
        public class ReportResourceIDComparer : IComparer<ReportResource>
        {
            public int Compare(ReportResource a, ReportResource b)
            {
                return String.Compare(a.BaseId, b.BaseId);
            }
        }

        /// <summary>
        /// Class is a container for all the information about a ReportResource. Every
        /// resource with a baseId has different information available it, such as
        /// the label used for printing. A description of the origin of the resource and 
        /// a possible introduction text to be printed in a report.
        /// </summary>
        public class ReportResource
        {
            public string BaseId
            {
                get;
                set;
            }

            public string Label
            {
                get;
                set;

            }

            public string Description
            {
                get;
                set;

            }

            public string Introduction
            {
                get;
                set;
            }

            public string Analysis
            {
                get;
                set;
            }

            public string Method
            {
                get;
                set;
            }

            public ReportResource(string baseId)
            {
                Debug.Assert(baseId != null);
                this.BaseId = baseId;
            }

            public void SetText(string field, string value) 
            {
                switch (field)
                {
                    case "label": 
                        this.Label = value;
                        break;
                    case "description": 
                        this.Description = value;
                        break;
                    case "introduction":
                        this.Introduction = value;
                        break;
                    case "analysis":
                        this.Analysis = value;
                        break;
                    case "method":
                        this.Method = value;
                        break;
                    default:
                        Debug.Assert(false, "Unknown field specified for report resource text.");
                        break;
                }
            }
        }
        #endregion
    }
}
