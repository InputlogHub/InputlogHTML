using System;
using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Preprocessing.Filter;
using InputLog.Core.Preprocessing;
using InputLog.Core.IO;

namespace GUI.Tabs.Preprocess.Filters
{
    /// <summary>
    /// This class provides the controls for a EventType Filter.
    /// InputFields:
    /// - event type to filter
    /// - value of the event
    /// - action to undertake with the events of the specified type
    /// </summary>
    public partial class EventType : ProcessControl
    {
        #region fields

        /// <summary>
        /// Enumerates the different known actions.
        /// </summary>
        private enum ActionType { KEEP, REMOVE }

        /// <summary>
        /// Name of this Filter.
        /// </summary>
		public const string NAME = "Event Type Filter";

		/// <summary>
		/// Returns whether this process control can handle multiple files at the same
		/// time or can only process one time at a time.
		/// </summary>
		public override bool MultipleFileCompatible
		{
			get { return true; }
		}

        /// <summary>
        /// Suppported Actions. 
        /// If a new action is added, this is the only place in this class where an update is needed!
        /// </summary>
        private readonly IDictionary<string, ActionType> Actions = new Dictionary<string, ActionType>
        {
            {"Only keep events of this type", ActionType.KEEP},
            {"Remove events of this type", ActionType.REMOVE}
        };

        /// <summary>
        /// Suppported Event Types to Filter. 
        /// If a new event type is added, this is the only place in this class where an update is needed!
        /// </summary>
        private readonly IDictionary<string, string> AvailableEventTypes = new Dictionary<string, string> 
        {
            {"Keyboard", InputLog.Core.Events.EventType.KEYBOARD},
            {"Mouse", InputLog.Core.Events.EventType.MOUSE},
            {"Focus", InputLog.Core.Events.EventType.FOCUS}
        };


        /// <summary>
        /// Configuration parameter names of this filter.
        /// </summary>
        private static class ConfigurationParameters
        {
            public const String EVENT_TYPE = "EventType";
            public const String EVENT_VALUE = "EventValue";
            public const String ACTION = "Action";
        }

        #endregion

        /// <summary>
        /// Constructs the EventType Control.
        /// Populates the action and event type lists.
        /// </summary>
        public EventType() : base("Event Filter", "event")
        {
            InitializeComponent();
            FilterAbbreviation = "event";
            InitHelp(MoreInfoLabel, FilterAbbreviation);

            ActionList.Items.AddRange(Actions.Keys.ToArray());
            ActionList.SelectedIndex = 0;

            EventTypeList.Items.AddRange(AvailableEventTypes.Keys.ToArray());
            EventTypeList.SelectedIndex = 0;
        }

        /// <summary>
        /// Returns the event filter that is represented by this filter control.
        /// </summary>
        /// <returns>The event filter.</returns>
        public override Preprocessor GetPreprocessor(SessionIdentification sessionID)
        {
            bool action = (Actions[ActionList.SelectedItem.ToString()] == ActionType.REMOVE);
            var eventType = AvailableEventTypes[EventTypeList.SelectedItem.ToString()];
            return new EventTypeFilter(NAME, new[] { eventType }, action);
        }

        /// <summary>
        /// Exports the configuration of this EventType Control to a FilterConfiguration.
        /// </summary>
        /// <param name="options">Options specifying whether certain fields should be exported or not.</param>
        /// <returns>A FilterConfiguration that defines the configuration (= the input fields) of this filter control. </returns>
        public override FilterConfiguration Export(FilterExportOptions options)
        {
            var config = new FilterConfiguration(NAME);
            config.Parameters.Add(ConfigurationParameters.EVENT_TYPE, 
                AvailableEventTypes[EventTypeList.SelectedItem.ToString()]);
            config.Parameters.Add(ConfigurationParameters.EVENT_VALUE,
                EventValueList.Text);
            config.Parameters.Add(ConfigurationParameters.ACTION,
                Actions[ActionList.SelectedItem.ToString()].ToString());
            return config;
        }

        /// <summary>
        /// Imports a configuration into this Eventtype Control from an FilterConfiguration.
        /// </summary>
        public override void Import(FilterConfiguration configuration)
        {
            // event type and value. This is a bit of a strange way to import (iterating over a hashmap to find 
            // the key for a given value), but using a dictionary to store event types is cleaner and more flexible
            // than using ListItems (as we did in the past)
            var defaultVal = EventTypeList.Items.Count > 0 ? EventTypeList.Items[0].ToString() : "";
            var importedEventTypeItem = configuration.TryGetParameter(ConfigurationParameters.EVENT_TYPE, defaultVal);

            for (int i = 0; i < EventTypeList.Items.Count; i++)
            {
                var eventTypeItemItem = EventTypeList.Items[i].ToString();
                if (AvailableEventTypes[eventTypeItemItem].Equals(importedEventTypeItem))
                {
                    EventTypeList.SelectedIndex = i;
                    break;
                }
            }

            EventValueList.Text = configuration.TryGetParameter(ConfigurationParameters.EVENT_VALUE, "");
            defaultVal = (ActionList.Items.Count > 0) ? ActionList.Items[0].ToString() : "";
            var importedActionItem = configuration.TryGetParameter(ConfigurationParameters.ACTION, defaultVal);

            for (int i = 0; i < ActionList.Items.Count; i++)
            {
                var actionItem = ActionList.Items[i].ToString();
                if (Actions[actionItem].ToString().Equals(importedActionItem))
                {
                    ActionList.SelectedIndex = i;
                    break;
                }
            }
        }
    }
}