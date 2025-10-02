namespace InputLog.Core.Reporting.ReportTemplate
{
    /// <summary>
    ///     A single value outputted as a part of an element. A value is one value in a line
    ///     of text. The value has a:
    ///     - value_id: which identifies which analysis to retrieve the value from
    ///     - label: which specifies the text directly before the value. This may be bold or not.
    ///     - bold: specifies whether the label is bold or not.
    ///     - prepend: a prepended text before the label (simple prepend, simple text only)
    ///     - append: an appended text after the value (simple append, simple text only)
    /// </summary>
    public class ValueTemplate
    {
        /// <summary>
        ///     Label to be used for when outputting this element. (optional)
        ///     If this is not set, the default label is used.
        /// </summary>
        public string Label
        {
            get;
            protected set;
        }

        /// <summary>
        ///     Target ID of the element. This determines where teh value for this 
        ///     element will come from. It's bound by the resource files to an analysis
        ///     and a reporting method. And it's also the link to the default outputting
        ///     information such as label, and introduction.
        /// </summary>
        public string TargetID
        {
            get;
            protected set;
        }

        /// <summary>
        ///     The resource information coupled to this Elements target ID.
        /// </summary>
        public Report.Report.ReportResource Resource => Report.Report.GetResource(this.TargetID);

        /// <summary>
        ///     A single string presenting a piece of text to be prepended before
        ///     the label of the value.
        /// </summary>
        public string Append;

        /// <summary>
        ///     A single string presenting a piece of text to be appended behind
        ///     the label of the value.
        /// </summary>
        public string Prepend;

        /// <summary>
        ///     Set to true if the label (and only the label) should be set to bold, 
        ///     this is the default value. False if it should not be set to bold.
        /// </summary>
        public bool IsBold;

        /// <summary>
        ///     Create a new value template. The only mandatory
        ///     information required to create this class is the targetID
        ///     which is the key to the ReportResource for this value.
        /// </summary>
        protected ValueTemplate() 
        {
            IsBold = true;
        }

        /// <summary>
        ///     Import a value from a template, using the given importer.
        /// </summary>
        /// <param name="importer">Importer to use</param>
        /// <returns>A value template constructed by the information given by the importer.</returns>
        internal static ValueTemplate ImportFrom(Import.TemplateImporter importer)
        {
            importer.StartValue();
            ValueTemplate value = new ValueTemplate
            {
                TargetID = importer.CurrentValueValueID,
                Label = importer.CurrentValueLabel,
                Prepend = importer.CurrentValuePrepend,
                Append = importer.CurrentValueAppend,
                IsBold = importer.CurrentValueLabelIsBold
            };


            importer.EndValue();
            return value;
        }
    }
}
