using System;
using System.Collections.Generic;

namespace GUI.Wizard
{
    /// <summary>
    /// A class that takes the data gathered by WizardPages and processes it all
    /// according to whatever the wizard is supposed to do upon completion.
    /// </summary>
    internal interface WizardProcessor
    {
        /// <summary>
        /// Execute whatever the wizard is supposed to do upon 'completion'.
        /// </summary>
        /// <param name="data">A dictionary of all the data gathered while the user
        /// completed the wizard.</param>
        void Execute(Dictionary<string, Object> data);
    }
}