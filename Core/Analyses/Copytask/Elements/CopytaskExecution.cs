using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using InputLog.Core.Events;

namespace InputLog.Core.Analyses.Copytask.Elements
{
    /// <summary>
    /// Class representing the entirety of all the components of the copyTask. Its structure
    /// is built from a copyTask xml description. The Execution is then filled with the events
    /// of the user executing the copyTask.
    /// </summary>
    public class CopytaskExecution
    {
        /*
         * The structure fields are those fields that help with building an in-memory
         * representation of the copyTask that was executed. This structure is 
         * rebuilt using the copyTask XML that is present within the idfx.
         */
        #region Structure Fields
        /// <summary>
        /// Copytask title
        /// </summary>
        public string Title
        {
            get;
            private set;
        }

        /// <summary>
        /// Copytask language.
        /// </summary>
        public string Language
        {
            get;
            private set;
        }

        /// <summary>
        /// The keyboard layout used by the copyTask 
        /// </summary>
        public string Layout
        {
            get;
            private set;
        }

        /// <summary>
        /// The list of the components in the copyTask.
        /// </summary>
        public IReadOnlyList<ComponentStructure> Components
        {
            get
            {
                return _components.AsReadOnly();
            }
        }

        /// <summary>
        /// Private list of the components in the copyTask.
        /// </summary>
        private List<ComponentStructure> _components;
        #endregion

        /// <summary>
        /// An estimate for the minimum number of bigrams we can expect
        /// </summary>
        private const int MIN_BIGRAM_ESTIMATE = 100;


        /*
         * The execution fields are the fields that help with rebuilding the 'execution' of
         * the copyTask. That is ordering the events in their right order, given their
         * positions. That order needs to be constructed because it might differ from the
         * chronological order of the events. (e.g. going back to change something, removing
         * enters, etc.)
         * The positional order is the order that is required though to create the bigrams.
         *
         * NOTE: THE POSITIONS LOGGED BY THE COPYTASK ARE NOT ENTIRELY CORRECT. E.G.: 
         * The first character should be logged at position 0, doclenght 1.
         * However, in the copyTask the positions are logged with position 1, doclength 1. 
         * 
         * So positions in other idfx's correspond to the position of the cursor at the 
         * time of the keypress. The positions of the copyTask idfx's correspond to the
         * position of the cursor _after_ the keypress.
         */
        #region Execution Fields
        /// <summary>
        /// Reference to the componet that is currenlty active.
        /// </summary>
        private ComponentExecution activeComponent;

        /// <summary>
        /// A dictionary mapping a component title to the execution of that
        /// component. Keeps track of all ComponentExecution's already reconstructed.
        /// </summary>
        private Dictionary<string, ComponentExecution> _executedComponents;
        #endregion

        /// <summary>
        /// Create a new CopytaskStructure.
        /// </summary>
        /// <param param name="bigramData">A dictionary containing all the information about
        /// the known bigrams.</param>
        public CopytaskExecution()
        {
            this._components = new List<ComponentStructure>(6);
            this._executedComponents = new Dictionary<string, ComponentExecution>(6);
            this.activeComponent = null;
        }

        #region Execution Logic

        /// <summary>
        /// Add a new event for the copyTask. The events get processed in chronological
        /// order and this class reconstructs the entire process. Every event should be
        /// added to this class in chronological order.
        /// </summary>
        /// <param name="newEvent">The new event to be added.</param>
        public void AddEvent(Event newEvent)
        {
            // If the event is a focus event it means we are changing the 
            // currently active component to the component mentioned in
            // the focus event.
            //
            if (newEvent.Type == EventType.FOCUS)
            {
                Events.WinLog.FocusChange focus =
                    Event.GetFirstEventPart<Events.WinLog.FocusChange>(newEvent);
                string focussedComponent = focus.WindowTitle;

                this.ActivateComponent(focussedComponent);
                return;
            }
            else if (this.activeComponent == null)
            {
                // We get a non-focus event which should be added to a component, but
                // no component has been set yet. Error!
                throw new CopytaskReplayException(
                    "[No active component] New event to be added to component, but no component has been selected yet.",
                    null
                );
            }

            if (newEvent.Type == EventType.KEYBOARD)
            {
                this.activeComponent.AddEvent(newEvent);
                return;
            }
            else
            {
                throw new CopytaskReplayException(
                    "[Unexpected event type] While replaying the copyTask an unexpected event " +
                        "of type \"" + newEvent.Type + "\" was encountered.",
                    null
                );
            }
        }

        /// <summary>
        /// Activates the component with given name. It sets that component as active
        /// and any following events will be added to the given component, until another
        /// reason comes along to change the active component to a different one.
        /// </summary>
        /// <param name="focussedComponent">The name of the component to be activated.</param>
        private void ActivateComponent(string focussedComponent)
        {
            ComponentStructure currentActiveCmpStr =
                this._components.SingleOrDefault(component => component.Title == focussedComponent);

            if (currentActiveCmpStr == null)
            {
                throw new CopytaskReplayException(
                    "[Component does not exist] Focus event with component given component title (\"" +
                        focussedComponent + "\") references non-existing component.",
                    null
                );
            }

            if (!this._executedComponents.ContainsKey(focussedComponent))
            {
                ComponentExecution activeExecution = new ComponentExecution(currentActiveCmpStr);
                this._executedComponents.Add(focussedComponent, activeExecution);
            }


            // If the new component is found it is set as the active component.
            this.activeComponent = this._executedComponents[focussedComponent];
        }

        /// <summary>
        /// Retrieve the list of all the bigrams found, in their context.
        /// </summary>
        /// <param name="nrOfBigramsEstimate">This is an estimate of the number of bigrams that may be found.
        /// Providing a more accurate estimate might imporve the performance. The value is optional.</param>
        /// <param name="bigramData">Dictionary containing the information about all the known bigrams.</param>
        /// <returns></returns>
        public List<BigramContext> RetrieveBigrams(Dictionary<string, Bigrams.Bigram> bigramData, int nrOfBigramsEstimate = MIN_BIGRAM_ESTIMATE)
        {
            List<BigramContext> bigrams = new List<BigramContext>(nrOfBigramsEstimate);

            foreach (ComponentExecution component in this._executedComponents.Values)
            {
                if (!component.IsExample)
                {
                    List<BigramContext> componentBigrams = component.RetrieveBigrams(bigramData);
                    bigrams.AddRange(componentBigrams);
                }
            }

            return bigrams;
        }
        #endregion


        #region Building the copytask structure
        /// <summary>
        /// Load the copytask structure information based on 
        /// the copyTask xml description.
        /// </summary>
        /// <param name="xml">The root xml node of the copytask XML.</param>
        public void LoadStructure(XmlNode xml)
        {
            XmlNode titleNode = xml.SelectSingleNode("/copytask/title");
            this.Title = titleNode.InnerText;

            XmlNode languageNode = xml.SelectSingleNode("/copytask/language");
            this.Language = languageNode.InnerText;

            //XmlNode layoutNode = xml.SelectSingleNode("/copytask/layout");
            //this.Layout = layoutNode.InnerText;

            foreach (XmlNode taskNode in xml.SelectNodes("//task"))
            {
                ComponentStructure component = CreateComponent(taskNode);
                this._components.Add(component);
            }
        }

        /// <summary>
        /// Construct the component based on the task description in the copyTask
        /// xml description.
        /// </summary>
        /// <param name="taskNode">The xml description of the task.</param>
        /// <returns>A component with the same characteristics as the described task in
        /// the copyTask xml description.</returns>
        private ComponentStructure CreateComponent(XmlNode taskNode)
        {
            // Fetch title
            string taskTitle = "unknown_title";
            XmlAttribute taskTitleAttr = taskNode.Attributes["title"];
            if (taskTitleAttr != null && !String.IsNullOrEmpty(taskTitleAttr.Value))
            {
                taskTitle = taskTitleAttr.Value;
            }

            // Fetch timelimit
            int timelimit = ComponentStructure.NO_TIMELIMIT;
            XmlAttribute timelimitAttr = taskNode.Attributes["timelimit"];
            if (timelimitAttr != null)
            {
                try
                {
                    timelimit = int.Parse(timelimitAttr.Value);
                }
                catch (FormatException fexc)
                {
                    throw new CopytaskFormatException(
                        "[Component \"" + taskTitle +
                            "\"] Timelimit value (\"" + timelimitAttr.Value +
                            "\") is not a valid integer.",
                        fexc
                    );
                }
            }

            // Fetch repetitions
            int repetitions = ComponentStructure.NO_REPETITIONS;
            XmlAttribute repetitionAttr = taskNode.Attributes["repetition"];
            if (repetitionAttr != null)
            {
                try
                {
                    repetitions = int.Parse(repetitionAttr.Value);
                }
                catch (FormatException fexc)
                {
                    throw new CopytaskFormatException(
                        "[Component \"" + taskTitle +
                            "\"] Repetition value (\"" + repetitionAttr.Value +
                            "\") is not a valid integer.",
                        fexc
                    );
                }
            }

            // Fetch isExample
            bool example = false;
            XmlAttribute exampleAttr = taskNode.Attributes["example"];
            if (exampleAttr != null)
            {
                try
                {
                    example = bool.Parse(exampleAttr.Value);
                }
                catch (FormatException fexc)
                {
                    throw new CopytaskFormatException(
                        "[Component \"" + taskTitle +
                            "\"] Example value (\"" + exampleAttr.Value +
                            "\") is not a valid boolean (\"true\" or \"false\").",
                        fexc
                    );
                }
            }

            // Fetch isUnlimited
            bool unlimited = false;
            XmlAttribute unlimitedAttr = taskNode.Attributes["unlimited"];
            if (unlimitedAttr != null)
            {
                try
                {
                    unlimited = bool.Parse(unlimitedAttr.Value);
                }
                catch (FormatException fexc)
                {
                    throw new CopytaskFormatException(
                        "[Component \"" + taskTitle +
                            "\"] Unlimited value (\"" + unlimitedAttr.Value +
                            "\") is not a valid boolean (\"true\" or \"false\").",
                        fexc
                    );
                }
            }

            // Fetch target
            string target = String.Empty;
            XmlNode targetNode = taskNode.SelectSingleNode("target");
            if (targetNode != null)
            {
                target = targetNode.InnerText;
            }

            // Fetch Synthesize
            bool synthesize = false;
            XmlAttribute synthesizeAttr = taskNode.Attributes["synthesize"];
            if (synthesizeAttr != null)
            {
                try
                {
                    synthesize = bool.Parse(synthesizeAttr.Value);
                }
                catch (FormatException fexc)
                {
                    throw new CopytaskFormatException(
                        "[Component \"" + taskTitle +
                            "\"] synthesize value (\"" + synthesizeAttr.Value +
                            "\") is not a valid boolean (\"true\" or \"false\").",
                        fexc
                    );

                }
            }

            // Fetch IsPractice
            bool practice = false;
            XmlAttribute practiceAttr = taskNode.Attributes["practice"];
            if (practiceAttr != null)
            {
                try
                {
                    practice = bool.Parse(practiceAttr.Value);
                }
                catch (FormatException fexc)
                {
                    throw new CopytaskFormatException(
                        "[Component \"" + taskTitle +
                            "\"] pratice value (\"" + practiceAttr.Value +
                            "\") is not a valid boolean (\"true\" or \"false\").",
                        fexc
                    );

                }
            }


            ComponentStructure component = new ComponentStructure(taskTitle, target, example, unlimited, synthesize, practice , repetitions, timelimit);
            return component;
        }
        #endregion
    }


    /// <summary>
    /// Exception thrown when the copyTask structure can not be correctly loaded.
    /// </summary>
    public class CopytaskFormatException : Exception
    {
        public CopytaskFormatException(string message, Exception innerException) :
            base(message, innerException) { }
    }

    public class CopytaskReplayException : Exception
    {
        public CopytaskReplayException(string message, Exception innerException) :
            base(message, innerException) { }
    }
}
