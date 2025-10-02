using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using InputLog.Core.Reporting.ReportTemplate.Import;
using InputLog.Core.Util;

namespace InputLog.Core.Reporting.ReportTemplate
{
    /// <summary>
    ///     Manages the reporting templates.
    /// </summary>
    public class ReportTemplateManager
    {
        #region Singleton
        /// <summary>
        ///     Holds a reference to the single instance of the template
        ///     manager that is available.
        /// </summary>
        private static ReportTemplateManager _templateInstance;

        /// <summary>
        ///     Get the one and only instance of the template manager.
        /// </summary>
        public static ReportTemplateManager TemplateInstance 
        {
            get
            {
                {
                    if (_templateInstance != null) return _templateInstance;
                    lock (Lock)
                    {
                        _templateInstance ??= new ReportTemplateManager();
                    }
                    return _templateInstance;
                }
            }
        }
        #endregion

        #region Constants
        /// <summary>
        ///   Reporting template extension
        /// </summary>
        private const string EXTENSION = ".template";

        /// <summary>
        ///     Path to the reporting templates.
        /// </summary>
        private const string TEMPLATES_PATH = @"Reporting\ReportTemplate";
        #endregion


        /// <summary>
        ///     Lock used for thread safety purposes.
        /// </summary>
        private static readonly object Lock = new();

        /// <summary>
        ///     Dictionary of all the templates that are available, 
        ///     indexed by their template_id
        /// </summary>
        private readonly Dictionary<string, ReportTemplate> _templates;

        /// <summary>
        ///     Returns a list of all the existing templates' ids
        /// </summary>
        public IEnumerable<string> TemplateIDs => _templates.Keys;

        /// <summary>
        ///     Get a template by template id
        /// </summary>
        /// <param name="templateId">Id of the requested template</param>
        /// <returns>The template with given id</returns>
        public ReportTemplate this[string templateId] => _templates[templateId];

        /// <summary>
        ///     Dictionary that maps paths to template files to their errors
        ///     if any were encountered while parsing the template files.
        /// </summary>
        public readonly Dictionary<string, IEnumerable<TemplateImporter.Error>> Errors;

        /// <summary>
        ///     Returns true if there were templates that had faulty formatting.
        /// </summary>
        public bool FoundFaultyTemplates
        {
            get
            {
                return Errors != null && Errors.Any(
                    errorEnumeration => errorEnumeration.Value != null &&
                                        errorEnumeration.Value.Any()
                );
            }
        }

        /// <summary>
        ///     Returns the path to the first template encountered that had an error.
        /// </summary>
        public string FirstTemplateWithError
        {
            get
            {
                if (!FoundFaultyTemplates)
                {
                    return null;
                }
                return Errors.FirstOrDefault(
                    errorListing => errorListing.Value != null && 
                                    errorListing.Value.Any()
                ).Key;
            }
        }

        /// <summary>
        ///     dictionary that maps the path of template file to the templateId.
        ///     Or to an empty string if it has no template id specified.
        ///     The set of keys also stands for all the currently opened
        ///     template files. Even templates that are not in the default template
        ///     folder but have been opened by the user through the interface.
        /// </summary>
        public readonly Dictionary<string, string> OpenedTemplates;

        /// <summary>
        ///     Event that gets triggered whenever the TemplateList is updated.
        /// </summary>
        public event EventHandler<EventArgs> TemplateListUpdated;

        /// <summary>
        ///     Create a new template manager. Loading a new template
        ///     manager also loads all the existing templates.
        /// </summary>
        private ReportTemplateManager()
        {
            _templates = new Dictionary<string, ReportTemplate>();
            OpenedTemplates = new Dictionary<string, string>();
            Errors = new Dictionary<string, IEnumerable<TemplateImporter.Error>>();

            // Find pre-defined templates.
            IEnumerable<FileInfo> templateFiles = _autoDetectTemplateFiles();
            foreach (FileInfo templateFile in templateFiles)
            {
                string templateId = null;
                IEnumerable<TemplateImporter.Error> errors = OpenTemplate(templateFile.FullName, out templateId);
            }
        }

        /// <summary>
        ///     Automatically detect template files in their predesignated
        ///     folders
        /// </summary>
        /// <returns>A list of detected template file fileInfos, or if no template
        /// files were discovered, null.</returns>
        private IEnumerable<FileInfo> _autoDetectTemplateFiles()
        {
            string exeDir = Path.GetDirectoryName(Application.ExecutablePath);
            string fullPath = Path.Combine(exeDir ?? string.Empty, TEMPLATES_PATH);
            DirectoryInfo templateDir = new DirectoryInfo(fullPath);
            return templateDir.Exists ? templateDir.EnumerateFiles("*" + EXTENSION) : null;
        }

        /// <summary>
        ///     Opens a template from given path. If the template can be 
        ///     opened it is added to the list of templates. If it can not
        ///     be parsed correctly it is not added to the list of templates
        ///     and a list of errors is returned.
        /// </summary>
        /// <param name="path">Path to the template</param>
        /// <param name="templateId">Output parameter that - upon successful opening of the template - is set to the 
        /// templateId that may be used to access the template in the ReportTemplateManager</param>
        /// <returns>A list of errors encountered, or null if no errors have
        /// been encountered while opening the template</returns>
        public IEnumerable<TemplateImporter.Error> OpenTemplate(string path, out string templateId)
        {
            try
            {
                XmlTemplateImporter importer = new(path);
                ReportTemplate template = ReportTemplate.ImportFrom(importer);

                // Only add template if there are no errors in it. Warnings etc are allowed.
                if (!importer.HasErrors || importer.HighestSeverity < TemplateImporter.Severity.ERROR)
                {
                    OpenedTemplates[path] = template.ID;
                    Errors[path] = (importer.HasErrors) ? importer.Errors : null;
                    _templates[template.ID] = template;
                    templateId = template.ID;
                    OnTemplateListUpdated(new EventArgs());
                }
                else
                {
                    // Even if the template file contained errors, it has been recognized as a 
                    // template file and thus is added to the list of files. That it contained
                    // errors is of no consequence for this list.

                    OpenedTemplates[path] = (template != null) ? template.ID : "unknown template id";
                    Errors[path] = importer.Errors;
                    templateId = null;
                    return importer.Errors;
                }
            }
            catch (Exception e)
            {
                MessageLogger.CatchException(this, e, Severity.ERROR, "Could not open template file.");
                templateId = null;
            }
            return null;
        }

        /// <summary>
        ///     Reloads all templates - this also parses them again and can check
        ///     for new or fixed errors.
        /// </summary>
        public void Refresh()
        {
            Errors.Clear();
            _templates.Clear();

            string[] openedTemplates = OpenedTemplates.Keys.ToArray();
            foreach (string templateFile in openedTemplates)
            {
                string templateId = null;
                OpenTemplate(templateFile, out templateId);
            }
        }

        /// <summary>
        ///     Notifies event listeners that the template list has been updated
        /// </summary>
        /// <param name="args">Arguments for the event.</param>
        public void OnTemplateListUpdated(EventArgs args)
        {
            if (TemplateListUpdated != null)
            {
                TemplateListUpdated(this, args);
            }
        }

        /// <summary>
        ///     Returns whether the template manager contains a template
        ///     with given template id.
        /// </summary>
        /// <param name="templateId">The id of the template</param>
        /// <returns>True if such a template exists, false if not.</returns>
        public bool Contains(string templateId)
        {
            return _templates.ContainsKey(templateId);
        }

    }
}
