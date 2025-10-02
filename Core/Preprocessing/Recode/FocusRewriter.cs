using System.Collections.Generic;
using System.Linq;
using System.Xml;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.IO;

namespace InputLog.Core.Preprocessing.Recode
{
    /// <summary>
    ///     Focusrewriter is a preprocessor that rewrites the values of focus events
    ///     (window titles) according to a user specified grouping. Many different window
    ///     titles may be grouped to an (optionally) different name, as the user likes.
    ///     The focusrewriting may be specified on many IDFX files at the same time, as the grouping
    ///     of the focus-event-values needs not be IDFX bound.
    /// </summary>
    public class FocusRewriter : Preprocessor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FocusRewriter()
        {
            // Initialize Dictionaries
            _titleKeyMap = new SortedList<string, int>();
            _keyTitleMap = new Dictionary<int, string>();
            _idfXtoTitles = new Dictionary<string, List<int>>();
            _idfXtoMainDoc = new Dictionary<string, string>();
            _references = new Dictionary<int, int>();
            _titleToGroup = new Dictionary<int, string>();
            _groups = new Dictionary<string, Group>();

            _newKey = 0;
            PreprocessorName = NAME;

            // Add the IGNORE group.
            _groups.Add("IGNORED", new Group(this, "IGNORED"));
        }

        public void ToXml(string filename)
        {
            var settings = new XmlWriterSettings { Indent = true, CheckCharacters = false, CloseOutput = true };
            var writer = XmlWriter.Create(filename, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("Preprocessing");

            writer.WriteStartElement("Files");
            foreach (var s in _idfXtoTitles.Keys)
            {
                writer.WriteElementString("file", s);
            }
            writer.WriteEndElement();

            foreach (var group in _groups.Values)
            {
                var groupInfo = group.VerboseGroup();

                writer.WriteStartElement("FocusGroup");
                writer.WriteAttributeString("ID", groupInfo.Key);
                // Create the extraInfo.
                var count = 1;
                foreach (var windowTitle in groupInfo.Value)
                {
                    writer.WriteElementString("entry_" + count, windowTitle);
                    count++;
                }
                writer.WriteEndElement();
            }

            writer.WriteStartElement("Ungrouped");
            foreach (var k in _titleToGroup.Keys)
            {
                if (_titleToGroup[k] == null)
                {
                    writer.WriteElementString("entry", _keyTitleMap[k]);
                }
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            
        }


        //---------------------------------------------------------------------
        // Inherited Members
        //
        /// <summary>
        ///     Process the eventList. This renames the value of any focus events according
        ///     to the mapping rules specified by the user in the preprocessor (as per
        ///     grouping). <br />
        ///     THIS ALTERS THE ORIGINAL LIST OF EVENTS.
        /// </summary>
        /// <param name="inputEvents">The list of events to be processed.</param>
        /// <param name="sessionId"></param>
        public override List<Event> Process(List<Event> inputEvents, SessionIdentification sessionId)
        {
            var i = 0;
            var previousFocus = "";

            while (i < inputEvents.Count)
            {
                var skipCounterIncrease = false;

                var e = inputEvents[i];
                if (e.Type == "focus")
                {
                    var focus = e.Parts.OfType<FocusChange>().Single();
                    var newTitle = _titleToGroup[_titleKeyMap[focus.WindowTitle]];

                    // Focus event is in a group -> replace name and if it's a main document, 
                    // replace the name in the SessionIdentification as well.
                    if (!string.IsNullOrEmpty(newTitle))
                    {
                        if (newTitle != STANDARD)
                        {
                            var oldTitle = _keyTitleMap[_titleKeyMap[focus.WindowTitle]];
                            if (oldTitle.ToLowerInvariant().Contains("wordlog"))
                            {
                                if (!newTitle.ToLowerInvariant().Contains("wordlog"))
                                {
                                    newTitle = "Wordlog_" + newTitle;
                                }
                                RenameMainDocSessionId(sessionId, newTitle);
                            }
                            ((FocusChange)e.Parts[e.Parts.IndexOf(focus)]).WindowTitle = newTitle;
                        }
                    }
                    else
                    {
                        newTitle = ((FocusChange)e.Parts[e.Parts.IndexOf(focus)]).WindowTitle;
                    }

                    // If the previous name is the same as the current name we remove the current focus event.
                    // Or if the newTitle is a FocusWriter.STANDARD (=IGNORED) focus event, we remove it as well.
                    if (previousFocus == newTitle || newTitle == STANDARD)
                    {
                        inputEvents.Remove(e);
                        skipCounterIncrease = true;
                    }

                    // If we removed an event, we skip the counter increase and don't change the name of
                    // the previous focus event.
                    if (skipCounterIncrease) continue;

                    // Update name of previous focus event.
                    previousFocus = newTitle;
                }
                i++;
            }

            return inputEvents;
        }

        /// <summary>
        ///     Returns an extra info object that contains some extra information about this preprocessor.
        ///     This can be used to inform the user of involved parameters of the preprocessors.
        /// </summary>
        public override IDictionary<string, IDictionary<string, object>> GetExtraInfo()
        {
            var info = new Dictionary<string, IDictionary<string, object>>();

            foreach (var group in _groups.Values)
            {
                var groupInfo = group.VerboseGroup();

                // Create the extraInfo.
                var infoTitle = "Focus Group: " + groupInfo.Key;
                info.Add(infoTitle, new Dictionary<string, object>(groupInfo.Value.Count));
                var count = 1;
                foreach (var windowTitle in groupInfo.Value)
                {
                    info[infoTitle].Add("Entry " + count, windowTitle);
                    count++;
                }
            }

            return info;
        }

        //---------------------------------------------------------------------
        // Public functions
        //

        /// <summary>
        ///     Add an IDFX file with a list of all its focus events to the FocusRewriters
        ///     in memory collection of focus events. The focusEvents are bound to the IDFX file.
        /// </summary>
        /// <param name="idfxName">
        ///     Identifier of the IDFX file, this is the key to which the
        ///     focus events are bound.
        /// </param>
        /// <param name="focusEvents">
        ///     List of all the window titles of the focus events in the
        ///     specified IDFX file.
        /// </param>
        public void AddIdfx(string idfxName, List<string> focusEvents)
        {
            // Check whether the IDFX is already added, if not add it.
            if (!_idfXtoTitles.ContainsKey(idfxName))
            {
                // Map windowKey list to IDFX file.
                var windowKeys = new List<int>();
                _idfXtoTitles.Add(idfxName, windowKeys);

                // Fill windowKey list.
                foreach (var windowTitle in focusEvents)
                {
                    var key = AddWindowTitle(windowTitle);
                    if (!windowKeys.Contains(key))
                    {
                        windowKeys.Add(key);
                    }
                }
            }
        }

        /// <summary>
        ///     Add an IDFX file with the name of its main document to the FocusRewriters
        /// </summary>
        /// <param name="idfxName">
        ///     Identifier of the IDFX file, this is the key of the main document.
        /// </param>
        /// <param name="mainDoc">The document name.</param>
        public void AddMainDocs(string idfxName, string mainDoc)
        {
            // Check whether the IDFX is already added, if not add it.
            if (!_idfXtoMainDoc.ContainsKey(idfxName))
            {
                _idfXtoMainDoc.Add(idfxName, mainDoc);
            }
        }

        /// <summary>
        ///     Remove an IDFX with all its focusEvents from the FocusRewriters in memory
        ///     collection of focus events. This implies:
        ///     - If a focusEvent is only specified in one IDFX, then removing the IDFX from
        ///     the collection will remove the focusEvent.
        ///     - If a focusEvent is specified in multiple IDFXs it shall only be removed from
        ///     the collection when the last IDFX bound to the focus event has been removed.
        ///     - If a focus event was in a group and the removal of an IDFX file it was bound to
        ///     results in its removal from the overall collection of focus events, the focus event
        ///     will be removed from the group, possibly resulting in an empty group!
        /// </summary>
        /// <param name="idfxName">Identifier of the IDFX from which file to remove focus events.</param>
        public void RemoveIdfx(string idfxName)
        {
            var keys = _idfXtoTitles[idfxName];

            // Remove every key once.
            foreach (var key in keys)
            {
                RemoveWindowTitle(_keyTitleMap[key]);
            }
        }

        /// <summary>
        /// Renames the main document with its new name. The new main document name should contain 'wordlog'
        /// in order to facilitate the interaction with different analyses.
        /// </summary>
        /// <param name="sessionId">The session identification to change.</param>
        /// <param name="newMainDocTitle">The new main document name.</param>
        private static void RenameMainDocSessionId(SessionIdentification sessionId, string newMainDocTitle)
        {
            sessionId.SetMainDocument(newMainDocTitle);
        }

        /// <summary>
        ///     Return the list of all groups currently defined.
        /// </summary>
        /// <returns>The list of groups currenlty defined.</returns>
        public List<Group> GetGroups()
        {
            return _groups.Values.ToList();
        }

        /// <summary>
        ///     Return the list of ungrouped items.
        /// </summary>
        /// <returns>The list of ungrouped items.</returns>
        public IEnumerable<string> GetUngrouped()
        {
            return (from key in _titleToGroup.Keys
                    where string.IsNullOrEmpty(_titleToGroup[key])
                    select _keyTitleMap[key]).ToList();
        }

        /// <summary>
        ///     Return a dictionary of main documents.
        /// </summary>
        /// <returns>Dictionary with the main document names as key.</returns>
        public Dictionary<string, string> GetMainDocs()
        {
            Dictionary<string, string> tmpDict = new Dictionary<string, string>();
            foreach (var value in _idfXtoMainDoc.Values.Where(value => !tmpDict.ContainsKey(value)))
            {
                tmpDict.Add(value, "");
            }
            return tmpDict;
        }

        /// <summary>
        ///     Get a group by name
        /// </summary>
        /// <param name="name">Name of the group</param>
        /// <returns>The group, or null if no such group exists.</returns>
        public Group GetGroup(string name)
        {
            if (_groups.ContainsKey(name))
            {
                return _groups[name];
            }
            return null;
        }

        /// <summary>
        ///     Save a new group. This only saves the name and any window keys that may already
        ///     be set. But if the group has no windowKeys set yet, it is saved as an empty group.
        /// </summary>
        /// <param name="group">
        ///     The new group. The groupname may not be empty
        ///     or a duplicate of an already existing groupname
        /// </param>
        /// <returns>True if the group was successfully added (name-wise), false if not.</returns>
        public bool SaveNewGroup(Group group)
        {
            if (GroupExists(group))
            {
                return false;
            }
            if (!string.IsNullOrWhiteSpace(group.Name))
            {
                _groups.Add(group.Name, group);
                group.UpdateUngrouped();
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Returns whether the group already exists in the list or not.
        /// </summary>
        /// <param name="group">Group to check for.</param>
        /// <returns>True if the group already exists, false if not.</returns>
        public bool GroupExists(Group group)
        {
            return _groups.ContainsKey(group.Name);
        }

        /// <summary>
        ///     Returns whether the group already exists in the list or not.
        /// </summary>
        /// <param name="groupName">Group to check for.</param>
        /// <returns>True if the group already exists, false if not.</returns>
        public bool GroupExists(string groupName)
        {
            return _groups.ContainsKey(groupName);
        }

        /// <summary>
        ///     Delete a group with given name.
        /// </summary>
        /// <param name="groupName">Name of the group to delete</param>
        public void DeleteGroup(string groupName)
        {
            if (_groups.ContainsKey(groupName))
            {
                var group = GetGroup(groupName);
                group.EmptyGroup();
                _groups.Remove(groupName);
            }
        }

        //---------------------------------------------------------------------
        // Private Helpers
        //

        /// <summary>
        ///     Add a windowTitle to the FocusRewriter. If the windowTitle already existed the
        ///     reference count is increased and the key returned. Otherwise the windowTitle is
        ///     first created in all the dictionaries, an initial reference added and lastly
        ///     the key is returned.
        /// </summary>
        /// <param name="title">WindowTitle to be added, the windowTitle may already exist.</param>
        /// <returns>The key of the windowTitle.</returns>
        private int AddWindowTitle(string title)
        {
            if (_titleKeyMap.ContainsKey(title))
            {
                AddReference(_titleKeyMap[title]);
                return _titleKeyMap[title];
            }
            var key = _newKey++;
            _titleKeyMap.Add(title, key);
            _keyTitleMap.Add(key, title);
            _titleToGroup.Add(key, null);
            AddReference(key);
            return key;
        }

        /// <summary>
        ///     Remove a windowTitle once from the list of windowTitles. If there are still references
        ///     to the windowTitle nothing changes except for the reference count. If this is the last
        ///     reference being removed the key will be removed from all datastructures and possibly
        ///     any groups it is in.
        /// </summary>
        /// <param name="title">WindowTitle to remove.</param>
        private void RemoveWindowTitle(string title)
        {
            var key = _titleKeyMap[title];

            if (RemoveReference(key))
            {
                // There are no more references
                _titleKeyMap.Remove(title);
                _keyTitleMap.Remove(key);
                _titleToGroup.Remove(key);
                RemoveTitleFromGroups(key);
            }
        }

        /// <summary>
        ///     Add an extra reference to a windowTitle. If the reference does not exist yet
        ///     it will be created.
        /// </summary>
        /// <param name="key">Key of the windowTitle that is being referenced.</param>
        private void AddReference(int key)
        {
            if (_references.ContainsKey(key))
            {
                _references[key]++;
            }
            else
            {
                _references.Add(key, 1);
            }
        }

        /// <summary>
        ///     Remove a reference from windowTitle, if that was the last reference, then the key is
        ///     also deleted from the dictionary.
        /// </summary>
        /// <param name="key">Key of the windowTitle whoms reference is being removed</param>
        /// <returns>
        ///     True if no more references to the key exist, false if there are still references
        ///     to the key.
        /// </returns>
        private bool RemoveReference(int key)
        {
            if (_references[key] == 1)
            {
                _references.Remove(key);
                return true;
            }
            _references[key]--;
            return false;
        }

        /// <summary>
        ///     Remove a key associated to a windowTitle from any groups it may be in.
        /// </summary>
        /// <param name="key">Key associated with the windowTitle to be removed.</param>
        private void RemoveTitleFromGroups(int key)
        {
            foreach (var group in _groups.Values)
            {
                group.RemoveWindowTitle(key);
            }
        }

        /// <summary>
        ///     Inner class that handles the construction and maintenance of groups
        ///     on its own. The adding and deleting of windowTitles is all based on the
        ///     key values of the windowTitles.
        ///     By passing along an instance of the FocusRewriter we may ask the Group
        ///     to provide us with a 'verbose' way of the groups windowTitle-grouping.
        /// </summary>
        public class Group
        {
            /// <summary>
            ///     Construct a group. The name is the name of the group of
            ///     windowTitles
            /// </summary>
            /// <param name="creator">
            ///     The focusrewriter that created the group. Will be used for the verbose
            ///     presentation of the group.
            /// </param>
            public Group(FocusRewriter creator)
            {
                _fRewriter = creator;
                Name = "";
            }

            /// <summary>
            ///     Construct a group. The name is the name of the group of
            ///     windowTitles
            /// </summary>
            /// <param name="creator">
            ///     The focusrewriter that created the group. Will be used for the verbose
            ///     presentation of the group.
            /// </param>
            /// <param name="name">Name for the group of windowTitles.</param>
            public Group(FocusRewriter creator, string name)
            {
                Name = name;
                _fRewriter = creator;
            }

            /// <summary>
            ///     Add the key of a windowtitle to the group.
            /// </summary>
            /// <param name="windowKey">Key of the windowTitle</param>
            private bool AddWindowTitle(int windowKey)
            {
                if (!_windowKeys.Contains(windowKey) && string.IsNullOrEmpty(_fRewriter._titleToGroup[windowKey]))
                {
                    _windowKeys.Add(windowKey);
                    UpdateUngrouped(windowKey);
                    return true;
                }
                return false;
            }

            /// <summary>
            ///     Add a windowtitle to this group.
            /// </summary>
            /// <param name="windowTitle">Window title to add to this group.</param>
            public bool AddWindowTitle(string windowTitle)
            {
                return AddWindowTitle(_fRewriter._titleKeyMap[windowTitle]);
            }

            /// <summary>
            ///     Remove the key of the windowtitle from this group.
            /// </summary>
            /// <param name="windowKey">Key of the windowTitle</param>
            internal bool RemoveWindowTitle(int windowKey)
            {
                if (_windowKeys.Contains(windowKey) && !string.IsNullOrEmpty(_fRewriter._titleToGroup[windowKey]))
                {
                    _windowKeys.Remove(windowKey);
                    UpdateUngrouped(windowKey);
                    return true;
                }
                return false;
            }

            /// <summary>
            ///     Remove the key of the windowtitle from this group.
            /// </summary>
            /// <param name="windowTitle">windowTitle to remove from this group</param>
            internal bool RemoveWindowTitle(string windowTitle)
            {
                return RemoveWindowTitle(_fRewriter._titleKeyMap[windowTitle]);
            }

            /// <summary>
            ///     Empty the group. This removes any items that are currently in this group.
            /// </summary>
            public void EmptyGroup()
            {
                var keys = _windowKeys.ToArray();
                foreach (var key in keys)
                {
                    RemoveWindowTitle(key);
                }
            }

            /// <summary>
            ///     Add a list of window titles to the group.
            /// </summary>
            /// <param name="items">List of window titles to add to the group</param>
            public void AddWindows(ICollection<string> items)
            {
                foreach (var title in items)
                {
                    AddWindowTitle(title);
                }
            }

            /// <summary>
            ///     Check whether this group is empty or not.
            /// </summary>
            /// <returns>Returns true if the group is empty, false if not.</returns>
            public bool IsEmpty()
            {
                VerboseGroup();
                return _windowKeys.Count == 0;
            }

            /// <summary>
            ///     Returns a verbose presentation of this group. This is a KeyValuePair that
            ///     uses the GroupName as key, and a list of all the windowTitles in the group as value.
            /// </summary>
            /// <returns>A keyValuePair with groupName as key and a List of the windowTitles as value.</returns>
            public KeyValuePair<string, List<string>> VerboseGroup()
            {
                var titles = new List<string>(_windowKeys.Count);
                titles.AddRange(_windowKeys.Select(key => _fRewriter._keyTitleMap[key]));

                // Get the titles from the FRewriter.
                var verbose = new KeyValuePair<string, List<string>>(Name, titles);

                return verbose;
            }

            /// <summary>
            ///     Update the grouped or ungrouped status of the currently ungrouped
            ///     windowtitles.
            /// </summary>
            internal void UpdateUngrouped()
            {
                foreach (var key in _windowKeys)
                {
                    _fRewriter._titleToGroup[key] = Name;
                }
            }

            /// <summary>
            ///     Update the grouped or ungrouped status of the currently ungrouped
            ///     windowtitles.
            /// </summary>
            /// <param name="key">
            ///     Update the grouped or ungrouped status of the windowtitle
            ///     with given key.
            /// </param>
            private void UpdateUngrouped(int key)
            {
                if (_windowKeys.Contains(key))
                {
                    _fRewriter._titleToGroup[key] = Name;
                }
                else
                {
                    _fRewriter._titleToGroup[key] = null;
                }
            }

            #region GroupFields

            /// <summary>
            ///     Title of the group.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            ///     Refernce to the focusRewriter that the group was created in. This is
            ///     used to construct the verbose presentation of the group.
            /// </summary>
            private readonly FocusRewriter _fRewriter;

            /// <summary>
            ///     A list that keeps track of all the windowTitleKeys
            ///     that are associated with this group.
            /// </summary>
            private readonly List<int> _windowKeys = new List<int>();


            /// <summary>
            ///     Return the number of windowtitles currently in this group.
            /// </summary>
            public int Count => _windowKeys.Count;

            #endregion
        }

        #region Fields

        /// <summary>
        ///     Name of the focus rewrite preprocessor.
        /// </summary>
        private const string NAME = "Focus Rewriter";

        /// <summary>
        ///     Dictionary that maps a windowTitle to an integer used as its key.
        ///     This mapping is done bijectively (see keyTitleMap) for efficiency reasons.
        /// </summary>
        private readonly SortedList<string, int> _titleKeyMap;

        /// <summary>
        ///     Dictionary that maps the integer key to its corresponding windowTitle.
        ///     This mapping is done bijectively (see keyTitleMap) for efficiency reasons.
        /// </summary>
        private readonly Dictionary<int, string> _keyTitleMap;

        /// <summary>
        ///     Map that maps every IDFX file to the keys of all the windowTitles in that IDFX
        ///     file.
        /// </summary>
        private readonly Dictionary<string, List<int>> _idfXtoTitles;

        /// <summary>
        /// Map with the IDFX as key and the window title of the main document as value.
        /// </summary>
        private readonly Dictionary<string, string> _idfXtoMainDoc;

        /// <summary>
        ///     Map that maps each windowTitleKey to the number of references in the different
        ///     IDFX files that that the windowTitle has.
        /// </summary>
        private readonly Dictionary<int, int> _references;

        /// <summary>
        ///     Map that tracks whether a windowTitle with given key is already grouped or not.
        ///     If the string is null the windowTitle is not grouped, if it is
        /// </summary>
        private readonly Dictionary<int, string> _titleToGroup;

        /// <summary>
        ///     A dictionary that maps the name of a group to the GroupObject that holds all the
        ///     information of that group.
        /// </summary>
        private readonly Dictionary<string, Group> _groups;

        /// <summary>
        ///     Keeps track of the last used key. Keys always increment.
        /// </summary>
        private int _newKey;

        /// <summary>
        ///     Static string that represents the name of the one group that is always
        ///     in the groups list automatically and that can not be removed.
        /// </summary>
        public const string STANDARD = "IGNORED";

        #endregion
    }
}