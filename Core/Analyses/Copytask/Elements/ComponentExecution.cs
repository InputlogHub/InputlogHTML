using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Events;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.Analyses.Copytask.Elements
{
    /// <summary>
    /// This class describes the execution of a copyTask component.
    /// It adds typed bigrams to their respective trials, and keeps track
    /// of which trial we are in etc.
    /// 
    /// It also has information about the structure of the component.
    /// </summary>
    class ComponentExecution
    {
        /// <summary>
        /// A Component structure that holds all information about how 
        /// this component is structured.
        /// </summary>
        private ComponentStructure structure;

        /// <summary>
        /// Keeps track of the nr of enters we have already encountered.
        /// This will let us know whether more enters are allowed are not.
        /// </summary>
        private int _nrOfEnters;

        /// <summary>
        /// The number of events that have already been added to the component.
        /// </summary>
        private int _nrOfEvents;

        /// <summary>
        /// An approximation of the actual trials by splitting trials based on enters
        /// and just selecting the (#repetitions) longest ones as the 'trials'. In the order
        /// they have been typed. 
        /// </summary>
        private List<List<Event>> _pseudoTrials;

        /// <summary>
        /// Retrieve the event list of the current trial.
        /// </summary>
        private List<Event> currentPseudoTrial
        {
            get
            {
                if (this._pseudoTrials[this._nrOfEnters] == null)
                {
                    this._pseudoTrials[this._nrOfEnters] = new List<Event>();
                }
                return this._pseudoTrials[this._nrOfEnters];
            }
        }

        /// <summary>
        /// The list containing the indices that have been selected as the final trials.
        /// The first element in the list has the index of the a trial in the _pseudoTrials variable
        /// that will serve as the 'first' actual trial.
        /// </summary>
        private List<int> _selectedTrialsIndices;

        /// <summary>
        /// The dictionary mapping bigram values (e.g. aa, ab, ac, ...) to the 
        /// characteristic information of the bigram. 
        /// </summary>
        private Dictionary<string, Bigrams.Bigram> _bigramData;

        /// <summary>
        /// Return whether this execution is just an example component, and thus
        /// doesn't actually have any execution information.
        /// </summary>
        public bool IsExample
        {
            get
            {
                return this.structure.IsExample;
            }
        }

        /// <summary>
        /// The starttime of the first event in the execution.
        /// </summary>
        private ulong _startTime;

        /// <summary>
        /// The starttime of the last event in the execution
        /// </summary>
        private ulong _endTime;

        /// <summary>
        /// Create a new component execution. The execution uses the structure of the component
        /// in order to determine how to interpret the events processing of the execution 
        /// of the component
        /// </summary>
        /// <param name="structure">Describes the structure of the component and its
        /// characteristics.</param>
        public ComponentExecution(ComponentStructure structure)
        {
            this.structure = structure;
            this._nrOfEnters = 0;
            this._nrOfEvents = 0;
            this._pseudoTrials = new List<List<Event>>();
            this._pseudoTrials.Add(new List<Event>());
            this._selectedTrialsIndices = new List<int>(structure.Repetitions);
            this._startTime = ulong.MaxValue;
            this._endTime = ulong.MinValue;
        }

        /// <summary>
        /// Add a new event to this component. 
        /// </summary>
        /// <param name="newEvent">The new event to add.</param>
        public void AddEvent(Events.Event newEvent)
        {
            if (newEvent.Type != EventType.KEYBOARD)
            {
                throw new CopytaskReplayException(
                    "[Unexpected event type] While replaying the copyTask an unexpected event " +
                        "of type \"" + newEvent.Type + "\" was encountered. " +
                        "Only keyboard events should be handled by components.",
                    null
                );
            }

            // Get the data about the keyboard event
            var winlogKey = newEvent.GetKeypressFromEvent();
            var wordlogKey = Event.GetFirstEventPart<InputLog.Core.Events.WordLog.Keypress>(newEvent);

            if (winlogKey == null || wordlogKey == null)
            {
                throw new CopytaskReplayException(
                    "[Invalid Keyboard Event] Event with id #" + newEvent.GetId().ToString() + " is missing a mandatory 'part'.",
                    null
                );
            }


            // Retrieve positional information
            string output = winlogKey.Value;
            int position = wordlogKey.Position;
            int docLength = wordlogKey.DocumentLength;

            // Check if the keyboard event is alphanumeric, and a single character.
            if (this._isNormalOutput(output))
            {
                this._addEvent(newEvent, output, position, docLength);
            }
            else
            {
                // This key requires special action. We only recognize to special types of actions, 
                // Delete and backspace, return
                switch (winlogKey.Key)
                {
                    case KeysEx.VK_BACK:
                        this._handleBackSpace(position, docLength);
                        break;
                    case KeysEx.VK_DELETE:
                        this._handleDelete(position, docLength);
                        break;
                    case KeysEx.VK_RETURN:
                        this._handleReturn(newEvent, position, docLength);
                        break;
                    default:
                        // Other actions such as VK_TAB, VK_SHIFT, VK_ESC, are just ignored.
                        break;
                }
            }
        }

        /// <summary>
        /// Add a new event to the current pseudo trial.
        /// </summary>
        /// <param name="newEvent">The event to be added.</param>
        /// <param name="output"></param>
        /// <param name="position"></param>
        /// <param name="docLength"></param>
        private void _addEvent(Event newEvent, string output, int position, int docLength)
        {
            ulong time = newEvent.GetKeypressFromEvent().StartTime;
            this._startTime = Math.Min(time, this._startTime);
            this._endTime = Math.Max(time, this._endTime);

            this.currentPseudoTrial.Add(newEvent);
            this._nrOfEvents += 1;
        }

        /// <summary>
        /// Handle the occurence of an enter key. This brings us to the following pseudo trial.
        /// </summary>
        /// <param name="newEvent"></param>
        /// <param name="position"></param>
        /// <param name="docLength"></param>
        private void _handleReturn(Event newEvent, int position, int docLength)
        {
            this.currentPseudoTrial.Add(newEvent);
            this._nrOfEvents += 1;

            if (this._structureHasTrials())
            {
                this._nrOfEnters += 1;
                this._pseudoTrials.Add(new List<Event>());
            }
        }

        private void _handleBackSpace(int position, int docLength)
        {
            /* This method could be used if you wished to make an exact rebuild of 
             * the writing process. That way you could very accurately determine which bigrams
             * belong to which trial. In that case, you would build a positional reconstruction of
             * the writing process. Deleting characters from the 'output' as users delete them, and
             * in the end you could split based on the enters to reconstruct the exact trials.
             * 
             * Because we will use an estimate approach that is much simpler to implement,
             * these methods are not needed.
             */
        }

        private void _handleDelete(int position, int docLength)
        {
            /* This method could be used if you wished to make an exact rebuild of 
             * the writing process. That way you could very accurately determine which bigrams
             * belong to which trial. In that case, you would build a positional reconstruction of
             * the writing process. Deleting characters from the 'output' as users delete them, and
             * in the end you could split based on the enters to reconstruct the exact trials.
             * 
             * Because we will use an estimate approach that is much simpler to implement,
             * these methods are not needed.
             */
        }

        /// <summary>
        /// Checks whether the output of the event signifies it to be a
        /// normal output event or a 'special event' such as delete or backspace.
        /// </summary>
        /// <param name="output">The output of the keyboard event under inspection.</param>
        /// <returns>True if the output signifies this to be normal output, 
        /// false if this is not normal output and requires special handling.</returns>
        private bool _isNormalOutput(string output)
        {
            bool isSingleCharacter = output.Length == 1;
            if (!isSingleCharacter)
            {
                return false;
            }
            bool isNormalCharacter = char.IsLetter(output.ElementAt(0));
            bool isWhitespaceCharacter = char.IsWhiteSpace(output.ElementAt(0));

            return isNormalCharacter || isWhitespaceCharacter;
        }

        /// <summary>
        /// Create the list of bigrams for this component. The bigram context takes into 
        /// account the trials in which the events have been added.
        /// </summary>
        /// <param name="bigramData">Dictionary containing the information about all the known bigrams.</param>
        /// <returns>The list of all the bigrams.</returns>
        public List<BigramContext> RetrieveBigrams(Dictionary<string, Bigrams.Bigram> bigramData)
        {
            this._bigramData = bigramData;
            this._selectTrialsToKeep();
            // Nr of events is an upper limit to the number of bigrams
            List<BigramContext> bigrams = new List<BigramContext>(this._nrOfEvents);

            for (int i = 0; i < this._selectedTrialsIndices.Count; i++)
            {
                // Trials start to count from 1
                int trialNumber = i + 1;

                List<Event> activeTrial = this._pseudoTrials[this._selectedTrialsIndices[i]];
                this.AddBigramsInTrial(bigrams, activeTrial, trialNumber);
            }

            return bigrams;
        }

        /// <summary>
        /// Add the bigrams that can be found in the current trial to the list
        /// of bigrams we have already discovered.
        /// </summary>
        /// <param name="bigrams">The list of bigrams we will append the detected bigrams too.</param>
        /// <param name="activeTrial">The trial we are looking at for detecting bigrams.</param>
        /// <param name="trialNumber">The number identifier of the trial (1 to #Repetitions)</param>
        private void AddBigramsInTrial(List<BigramContext> bigrams, List<Event> activeTrial, int trialNumber)
        {
            Event previousKey = null;
            Event currentKey = null;

            foreach (Event key in activeTrial)
            {
                currentKey = key;
                if (previousKey == null)
                {
                    // If the previousKey is not set we can't have a bigram, skip this cycle.
                    previousKey = currentKey;
                    continue;
                }

                BigramContext bigram = this._convertToBigram(previousKey, currentKey, trialNumber);
                if (bigram != null)
                {
                    bigrams.Add(bigram);
                }
                previousKey = currentKey;
            }
        }

        /// <summary>
        /// Convert the sequence of keystrokes to a Bigram, if a bigram can be constructed
        /// from these keystrokes. 
        /// - Capital Letters are ignored in bigrams, and thus can not be part of a bigram
        /// - Spaces are ignored in bigrams
        /// - Anything that is not 'normal production' is ignored in bigrams
        /// 
        /// The context of the bigram is also set, and the BigramContext with it's bigram information
        /// inside is what will be returned.
        /// </summary>
        /// <param name="firstKey">The first key in the sequence that might be part form a bigram.</param>
        /// <param name="secondKey">The second key in the sequence that mihgt be part in a bigram.</param>
        /// <param name="trialNumber">The trial to which the bigram would belong.</param>
        /// <returns>The bigramcontext if a bigram could be constructed from the two keys, or
        /// returns null if no valid bigram could be formed from the two keystrokes.</returns>
        private BigramContext _convertToBigram(Event firstKey, Event secondKey, int trialNumber)
        {
            // First check whether we have all the necessary parts.
            if (firstKey == null || secondKey == null)
            {
                return null;
            }

            // Get the data about the keyboard events
            Events.WinLog.KeyPress firstKeyInfo = firstKey.GetKeypressFromEvent();
            Events.WinLog.KeyPress secondKeyInfo = secondKey.GetKeypressFromEvent();

            // Output value conditions:
            bool bothKeysHaveNormalOutput = this._isNormalOutput(firstKeyInfo.Value) &&
                                            this._isNormalOutput(secondKeyInfo.Value);
            bool bothKeysAreLowerCase = char.IsLower(firstKeyInfo.Value.ElementAt(0)) &&
                                        char.IsLower(secondKeyInfo.Value.ElementAt(0));

            // Timing conditions: second event after first event.
            bool timeFirstKeyBeforeSecondKey = firstKeyInfo.StartTime <= secondKeyInfo.StartTime;


            BigramContext bigramContext = null;

            // If all conditions are met, this is a valid bigram.
            if (bothKeysAreLowerCase &&
                bothKeysHaveNormalOutput &&
                timeFirstKeyBeforeSecondKey)
            {
                string bigramValue = firstKeyInfo.Value + secondKeyInfo.Value;

                //Debug.Assert(this._bigramData.ContainsKey(bigramValue));
                if (this._bigramData.ContainsKey(bigramValue))
                {
                    Bigrams.Bigram bigram = this._bigramData[bigramValue];
                    bigramContext = new BigramContext(bigram, this.structure, trialNumber);
                    bigramContext.PauseTime = secondKeyInfo.StartTime - firstKeyInfo.StartTime;
                    bigramContext.StartTime = firstKeyInfo.StartTime;
                    bigramContext.SetExecutionWindow(this._startTime, this._endTime);
                }
            }

            return bigramContext;
        }

        /// <summary>
        /// Determines which of the _pseudoTrials will be used as the actual trials.
        /// </summary>
        private void _selectTrialsToKeep()
        {
            // If it is unlimited, every pseudo Trial corresponds to an actual trial.
            if (this.structure.IsUnlimited)
            {
                this._selectedTrialsIndices = Enumerable.Range(0, this._pseudoTrials.Count).ToList();
            }
            else if (this.structure.IsRepetitive)
            {
                int maxTrials = this.structure.Repetitions;
                if (this._pseudoTrials.Count <= maxTrials)
                {
                    this._selectedTrialsIndices = Enumerable.Range(0, this._pseudoTrials.Count).ToList();
                }
                else
                {
                    // The indices of the trials we will use as selected Trials. Initially this list contains,
                    // more indices than maxTrials. We iteratively remove the index of the pseudotrial with the smalles
                    // number of events associated with it.
                    // Eventually we are left with the #maxTrials largest trials in the _pseudoTrials list, in
                    // the order of their appearance.
                    //
                    this._selectedTrialsIndices = Enumerable.Range(0, this._pseudoTrials.Count).ToList();
                    List<int> trialSizes = this._pseudoTrials.Select(trial => trial.Count).ToList();

                    // The indices in _selectedTrialsIndices and trialSizes are the same for the same trial.
                    // Trial A in _selectedTrials, will have the same index in the trialSizes list. 

                    while (this._selectedTrialsIndices.Count > maxTrials)
                    {
                        int firstIndexSmallestTrial = trialSizes.IndexOf(trialSizes.Min());

                        // This is the smallest trial, we remove its index from the list of _selectedTrialsIndices
                        // And then we set it's trialSize to int.MaxValue, so that the same 'smallest trial' will
                        // not be selected a second time.
                        this._selectedTrialsIndices.Remove(firstIndexSmallestTrial);
                        trialSizes[firstIndexSmallestTrial] = int.MaxValue;
                    }
                }
            }
            else
            {
                // There is only one trial, the only 'selected trial' is the first trial,
                // which is the first trial in the _pseudoTrials.
                this._selectedTrialsIndices.Add(0);
            }
        }

        /// <summary>
        /// Checks whether the component structure is a component that accomodates
        /// trials or not. Trials don't only appear when a task is repetitive.
        /// </summary>
        /// <returns>True if the component supports trials, false if it does not.</returns>
        private bool _structureHasTrials()
        {
            return (this.structure.IsRepetitive || this.structure.IsUnlimited);
        }

        #region DEBUG HELPERS
        /// <summary>
        /// A debug helper property that shows the contents, as a string,
        /// of the current trial.
        /// </summary>
        private String DEBUG_currentPseudoTrial
        {
            get
            {
                return this.DEBUG_pseudoTrial_to_string(this.currentPseudoTrial);
            }
        }

        private String DEBUG_pseudoTrials
        {
            get
            {
                string[] trials = new string[this._pseudoTrials.Count];
                for (int i = 0; i < this._pseudoTrials.Count; ++i )
                {
                    trials[i] = this.DEBUG_pseudoTrial_to_string(this._pseudoTrials[i]);
                }
                return String.Join("\n", trials);
            }
        }

        /// <summary>
        /// Converst a list of events (pseudoTrial) to a string representation
        /// of the sequence of characters in the trial.
        /// </summary>
        /// <param name="pseudoTrial"></param>
        /// <returns></returns>
        private string DEBUG_pseudoTrial_to_string(List<Event> pseudoTrial)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Event ev in this.currentPseudoTrial)
            {
                var keypress = ev.GetKeypressFromEvent();
                sb.Append(keypress.Value);
            }
            return sb.ToString();
        }

        #endregion
    }
}
