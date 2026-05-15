using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using InputLog.Core.Preprocessing.Recode;
using InputLog.Core.Util;

namespace GUI.Tabs.Preprocess.Recoders
{
    public partial class FocusRewriteDialog : Form
    {
        #region Fields

        /// <summary>
        ///     The focusRewriter being used.
        /// </summary>
        public  FocusRewriter _fRewriter;
        /// <summary>
        ///     Allows access to the saved log file in order to fill the panels
        ///     with the old data.
        /// </summary>
        private FocusRewrite _rewrite;

        /// <summary>
        ///     The currently active group.
        /// </summary>
        private FocusRewriter.Group _activeGroup;

        /// <summary>
        ///     Keeps track of the index of the previously selected group.
        /// </summary>
        private int _oldGroup;

        /// <summary>
        /// The ungrouped titles list.
        /// </summary>
        private IEnumerable<string> _titles;
        /// <summary>
        /// Table of main documents in this collection.
        /// </summary>
        private Dictionary<string, string> _mainDocs;

        /// <summary>
        /// True if the list contains main documents, false otherwise
        /// </summary>
        private bool _isMainDocGroup;
        #endregion

        /// <summary>
        ///     Construct the focus rewrite dialog that will be used for the actual manipulating of the groups
        /// </summary>
        /// <param name="focusRewrite"></param>
        /// <param name="focusRewriter">FocusRewriter being used.</param>
        /// <param name="selectedGroup">
        ///     Name of the group that was selected when this window was created.
        ///     Or null if no such group was selected.
        /// </param>
        public FocusRewriteDialog(FocusRewrite focusRewrite, FocusRewriter focusRewriter, string selectedGroup = null)
        {
            InitializeComponent();
            SaveGroupButton.Enabled = false;
            _oldGroup = -1;
            _rewrite = focusRewrite;

            if (FocusRewrite.LoadFromLogFile)
            {
                _fRewriter = focusRewrite.FRewriter;
                SetActiveGroup(new FocusRewriter.Group(_fRewriter));
                FillSavedGroupedList();
                FillSavedUngroupedList();
            }
            else
            {
                _fRewriter = focusRewriter;
                // Set a selected group as active, if needed.
                SetActiveGroup(selectedGroup != null
                    ? _fRewriter.GetGroup(selectedGroup)
                    : new FocusRewriter.Group(_fRewriter));

                // Initialize the list of the groups & ungrouped
                UpdateGroupsList();
                UpdateUngroupedList();
            }
        }

        /// <summary>
        ///     Update the grouped & ungrouped list
        /// </summary>
        private void UpdateLists()
        {
            UpdateGroupsList();
            UpdateUngroupedList();
        }

        /// <summary>
        ///     Update the groups list with the new groups
        /// </summary>
        private void UpdateGroupsList()
        {
            Groups.Items.Clear();
            List<FocusRewriter.Group> groups = _fRewriter.GetGroups();
            foreach (FocusRewriter.Group group in groups)
            {
                var row = new ListViewItem { Text = @group.Name };
                row.SubItems.Add(group.Count.ToString());
                Groups.Items.Add(row);
            }
        }

        /// <summary>
        ///     Update the ungrouped titles list.
        /// </summary>
        private void UpdateUngroupedList()
        {          
            _titles = _fRewriter.GetUngrouped();
            UngroupedFiller();
        }

        /// <summary>
        /// Putting the sources into the 'ungrouped' panel of the FocusRewriteDialog.
        /// </summary>
        private void UngroupedFiller()
        {
            Ungrouped.Items.Clear();
            _mainDocs = _fRewriter.GetMainDocs();
            foreach (string title in _titles)
            {
                if (_mainDocs.ContainsKey(title) || title.ToLowerInvariant().Contains("wordlog") || title.ToLowerInvariant().Contains("maindoc"))
                {
                    var tmpName = "** " + title + " **";
                    _mainDocs[title] = tmpName;
                    Ungrouped.Items.Add(tmpName);
                }
                else
                {
                    Ungrouped.Items.Add(title);
                }
            }
        }

        /// <summary>
        /// Ungrouped files from a saved log file.
        /// </summary>
        private void FillSavedUngroupedList()
        {
            _titles = _rewrite.UngroupedSources;
            UngroupedFiller();
        }

        /// <summary>
        /// Grouped files from a saved log file.
        /// </summary>
        private void FillSavedGroupedList()
        {
            Groups.Items.Clear();
            List<KeyValuePair<string, List<string>>> savedGroups = _rewrite.GroupedSources;
            foreach (var group in savedGroups)
            {
                var row = new ListViewItem { Text = group.Key };
                row.SubItems.Add(group.Value.Count.ToString());
                Groups.Items.Add(row);
            }
        }


        /// <summary>
        ///     Change the active group in the FocusRewriteDialog and update the control
        ///     with the new information.
        /// </summary>
        /// <param name="group"></param>
        private void SetActiveGroup(FocusRewriter.Group group)
        {
            // Set active group, update save button and clear current list.
            _activeGroup = group;
            ActiveGroupListbox.Items.Clear();

            // Fill in information of the new active group.
            GroupNameField.Text = _activeGroup.Name;
            if (_activeGroup.Count > 0)
            {
                KeyValuePair<string, List<string>> groupContent = _activeGroup.VerboseGroup();
                foreach (string title in groupContent.Value)
                {
                    ActiveGroupListbox.Items.Add(title);
                }
            }

            // Select the active group in the Groups list to the right.
            foreach (ListViewItem groupInList in Groups.Items)
            {
                if (groupInList.Text.Equals(_activeGroup.Name) && !groupInList.Selected)
                {
                    groupInList.Selected = true;
                }
            }

            ActiveGroupChanged(false);
        }

        /// <summary>
        ///     If the active group has been changed since it's last been saved the save group button
        ///     will be enabled for clicking.
        /// </summary>
        private void ActiveGroupChanged(bool changed = true)
        {
            SaveGroupButton.Enabled = changed;
        }

        /// <summary>
        ///     Returns true if there are changes to the current group pending, false if not.
        /// </summary>
        /// <returns>True if there are pending changes, false if not.</returns>
        private bool PendingChanges()
        {
            return SaveGroupButton.Enabled;
        }

        /// <summary>
        ///     Move the selected items (if any) from the ungrouped list to the
        ///     active group's listbox.
        /// </summary>
        private void UngroupedToActiveListbox()
        {
            _isMainDocGroup = false;
            if (Ungrouped.SelectedItems.Count > 0)
            {
                // Copying content of the enumerator
                var ungroupedSelected = Ungrouped.SelectedItems.Cast<string>().ToList();
                // Changing the lists.
                foreach (string entry in ungroupedSelected)
                {
                    if (_mainDocs.ContainsKey(entry) || entry.ToLowerInvariant().Contains("wordlog") || entry.ToLowerInvariant().Contains("maindoc"))
                    {
                        _isMainDocGroup = true;
                        ActiveGroupListbox.Items.Add(_mainDocs.FirstOrDefault(x => x.Value == entry).Key);
                    }
                    else
                    {
                        ActiveGroupListbox.Items.Add(entry);
                    }
                    Ungrouped.Items.Remove(entry);
                }
                ActiveGroupChanged();
            }
        }

        /// <summary>
        /// Search the ungrouped focus titles. Wildcard enabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBtnClick(object sender, EventArgs e)
        {
            Ungrouped.ClearSelected();
            // Expand query to capture text before and after the search term. 
            string pattern = '*' + SearchTxtBx.Text.ToLower() + '*';
            foreach (string t in _titles)
            {
                if (StringSearch.IsMatch(t, pattern))
                {
                    Ungrouped.SelectedItem = t;
                }
            }
            if (Ungrouped.SelectedItems.Count == 0)
            {
                SearchTxtBx.Text = "- No Results -";
            }
        }

        /// <summary>
        ///     Move the selected items (if any) from the active listbox to the
        ///     list of ungrouped items. Or, if the flag for clearing the box is
        ///     set to true, every item in the active listbox is put in the ungrouped
        ///     list regardless of active selections.
        /// </summary>
        private void ActiveListboxToUngrouped(bool clearBox = false)
        {
            // Copy the list of (possibly selected) items to a temp variable so we can 
            // enumerate over the list without using an enumerator
            //
            var selected = new List<string>();
            if (clearBox)
            {
                ListBox.ObjectCollection items = ActiveGroupListbox.Items;
                selected.AddRange(items.Cast<string>());
            }
            else
            {
                ListBox.SelectedObjectCollection items = ActiveGroupListbox.SelectedItems;
                selected.AddRange(items.Cast<string>());
            }

            // Remove the items in the list.
            foreach (string entry in selected)
            {
                ActiveGroupListbox.Items.Remove(entry);

                if (_mainDocs.ContainsKey(entry) || entry.ToLowerInvariant().Contains("wordlog") || entry.ToLowerInvariant().Contains("maindoc"))
                {
                    var tmpName = "** " + entry + " **";
                    _mainDocs[entry] = tmpName;
                    Ungrouped.Items.Add(tmpName);
                }
                else
                {
                    Ungrouped.Items.Add(entry);
                }
            }
            ActiveGroupChanged();
        }

        /// <summary>
        ///     Event handler for the 'AddToGroupButton'. Adds items from the list
        ///     of ungrouped items to the list of items in the current group.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddToGroupButtonClick(object sender, EventArgs e)
        {
            UngroupedToActiveListbox();
        }

        /// <summary>
        ///     Add all selected items in the current groups list back to the list
        ///     of ungrouped items (thus removing them from the current groups list).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveFromGroupButtonClick(object sender, EventArgs e)
        {
            ActiveListboxToUngrouped();
        }

        /// <summary>
        ///     Create a new group.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewGroupButtonClick(object sender, EventArgs e)
        {
            if (PendingChanges())
            {
                if (MessageBox.Show("Are you sure you want to close the window? Any unsaved progress will be lost.",
                                    "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }
            CreateNewGroup();
        }

        /// <summary>
        ///     Create a new group. This creates a new 'ActiveGroup' and erases any information
        ///     currently filled in, in the ActiveGroup part of the UI.
        /// </summary>
        private void CreateNewGroup()
        {
            // Clear the text fields and create a new ActiveGroup
            GroupNameField.Text = "";
            _isMainDocGroup = false;

            // Undo any unsaved changes.
            UndoChanges();

            // Change the active group.
            SetActiveGroup(new FocusRewriter.Group(_fRewriter));
            ActiveGroupChanged(false);

            // Remove Current Selections
            Groups.ItemSelectionChanged -= GroupedSelectionChangeEvent;
            foreach (ListViewItem item in Groups.Items)
            {
                item.Selected = false;
            }
            Groups.ItemSelectionChanged += GroupedSelectionChangeEvent;

            _oldGroup = -1;
        }

        /// <summary>
        ///     Attempts to select the group with 'groupName' as the new active
        ///     group in the GroupsList.
        /// </summary>
        /// <param name="groupName"></param>
        private void SelectActiveGroup(string groupName)
        {
            foreach (ListViewItem item in Groups.Items)
            {
                if (item.Text == groupName && !item.Selected)
                {
                    item.Selected = true;

                    // Get the actual group and load it's information.
                    SetActiveGroup(_fRewriter.GetGroup(groupName));

                    break;
                }
            }
        }

        private void DeleteGroupButtonClick(object sender, EventArgs e)
        {
            if (_activeGroup != null)
            {
                if (_activeGroup.Name != FocusRewriter.STANDARD)
                {
                    // If the current group exists, remove it.
                    if (_fRewriter.GroupExists(_activeGroup))
                    {
                        _fRewriter.DeleteGroup(_activeGroup.Name);
                        UpdateLists();
                    }               
                    ActiveGroupListbox.Items.Clear();
                    GroupNameField.Text = string.Empty;
                }
                else
                {
                    MessageBox.Show(
                        "Group name [" + _activeGroup.Name + "] is a standard group that can not be removed.", "Error",
                        MessageBoxButtons.OK);
                }
            }
            else
            {
                MessageBox.Show("No group selected to delete. Please select a group first.", "Error",
                                MessageBoxButtons.OK);
            }
        }

        private void SaveGroupButtonClick(object sender, EventArgs e)
        {
            // Must have a name.
            if (GroupNameField.Text != "")
            {
                // Check if a group is the IGNORE group or an already existing group.
                // - Already existing: GroupExists and the groupName is not equal to the selectedName in the list.
                // - Rename of ignored group: SelectedName = IGNORED, new GroupName is different and, ActiveGroupName == IGNORED
                if (Groups.SelectedItems.Count > 0)
                {
                    if (_activeGroup != null)
                    {
                        if (_activeGroup.Name != Groups.SelectedItems[0].Text
                            && _fRewriter.GroupExists(GroupNameField.Text))
                        {
                            MessageBox.Show("Group name [" + GroupNameField.Text + "] already exists.", "Error",
                                MessageBoxButtons.OK);
                            return;
                        }

                        if (FocusRewriter.STANDARD == Groups.SelectedItems[0].Text
                            && GroupNameField.Text != FocusRewriter.STANDARD
                            && _activeGroup.Name == FocusRewriter.STANDARD)
                        {
                            MessageBox.Show("Group name [" + FocusRewriter.STANDARD + "] cannot be renamed.", "Error",
                                MessageBoxButtons.OK);
                            GroupNameField.TextChanged -= GroupNameFieldTextChanged;
                            GroupNameField.Text = FocusRewriter.STANDARD;
                            GroupNameField.TextChanged += GroupNameFieldTextChanged;
                            return;
                        }
                    }
                }

                // The main document group name should contain 'wordlog' or 'maindoc'. (unnecessary)
                //if (_isMainDocGroup && !GroupNameField.Text.ToLowerInvariant().Contains("wordlog") && !GroupNameField.Text.ToLowerInvariant().Contains("maindoc"))
                //{
                //    MessageBox.Show("The new name for the main documents must include the word 'wordlog' or 'maindoc'" +
                //                  " to avoid confusion with other documents.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    GroupNameField.Text = "Wordlog_" + GroupNameField.Text;
                //}

                // If there's currently an error on the ErrorProvider we can't save the group. 
                if (!string.IsNullOrEmpty(ErrorProvider.GetError(GroupNameField)))
                {
                    MessageBox.Show(
                        "Group name [" + GroupNameField.Text + "] is invalid. Please change the group name" +
                        " before saving the group.", "Error", MessageBoxButtons.OK);
                    return;
                }

                // - Alteration of group: GroupName equals SelectedName in list -> save any alterations
                // If the group currently exists, we remove it
                if (_activeGroup == null) return;
                if (_activeGroup.Name != null)
                {
                    _fRewriter.DeleteGroup(_activeGroup.Name);
                }

                // Set the new group name.
                _activeGroup.Name = GroupNameField.Text;

                // Save the group first, then add the items.
                _fRewriter.SaveNewGroup(_activeGroup);

                // Remove old items.
                _activeGroup.EmptyGroup();

                // Insert the new items
                foreach (string windowTitle in ActiveGroupListbox.Items)
                {
                    _activeGroup.AddWindowTitle(windowTitle);
                }

                // Set ActiveGroupChagned to false now
                ActiveGroupChanged(false);
                UpdateLists();

                // Select the active group in the list of groups.
                SelectActiveGroup(_activeGroup.Name);
            }
            else
            {
                MessageBox.Show("You must specify a name before saving the group.", "Error", MessageBoxButtons.OK);
            }
        }

        private void GroupNameFieldTextChanged(object sender, EventArgs e)
        {
            ActiveGroupChanged();

            // The name may:
            // . not be whitespace
            // . may not be equal to an existing group, unless it is the EXACT same group as the already existing one.
            //
            if (_fRewriter.GroupExists(GroupNameField.Text)
                && _fRewriter.GetGroup(GroupNameField.Text) != _activeGroup)
            {
                ErrorProvider.SetError(GroupNameField, "Group name cannot be empty or a duplicate.");
            }
            else
            {
                ErrorProvider.Clear();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseForm(object sender, FormClosingEventArgs e)
        {
            if (PendingChanges()) // changes are pending.
            {
                DialogResult result = MessageBox.Show("Are you sure you want to close the window? " +
                                                      "Any unsaved progress will be lost.",
                                                      "Warning", MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        ///     Undo the changes to the activeList and the UngroupedList, done during the construction
        ///     of the current ActiveGroup. Any differences with the ActiveGroups windowTitle list and the
        ///     active and ungrouped lists have to be restored.
        /// </summary>
        private void UndoChanges()
        {
            List<string> originalItems = _activeGroup.VerboseGroup().Value;

            // Run over the ungrouped list. Any items, that are also in OriginalItems get removed.
            foreach (string entry in originalItems)
            {
                if (_mainDocs.ContainsKey(entry) || entry.ToLowerInvariant().Contains("wordlog") || entry.ToLowerInvariant().Contains("maindoc"))
                {
                    var tmpName = "** " + entry + " **";
                    _mainDocs[entry] = tmpName;
                    Ungrouped.Items.Remove(tmpName);
                }
                else
                {
                    Ungrouped.Items.Remove(entry);
                }
            }

            // Run over the activeList, any items that are not in the original list get moved back to 
            // ungrouped list.
            foreach (string entry in ActiveGroupListbox.Items)
            {
                if (!originalItems.Contains(entry))
                {
                    if (_mainDocs.ContainsKey(entry) || entry.ToLowerInvariant().Contains("wordlog") || entry.ToLowerInvariant().Contains("maindoc"))
                    {
                        var tmpName = "** " + entry + " **";
                        _mainDocs[entry] = tmpName;
                        Ungrouped.Items.Add(tmpName);
                    }
                    else
                    {
                        Ungrouped.Items.Add(entry);
                    }
                }
            }
        }

        private void GroupedSelectionChangeEvent(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // Only handle changes where an actual selection is made
            if (e.IsSelected)
            {
                // Unregister handler. 
                Groups.ItemSelectionChanged -= GroupedSelectionChangeEvent;

                if (PendingChanges()) // Pending changes
                {
                    DialogResult result = MessageBox.Show("You have pending changes in your currently active group. " +
                                                          "If you change the active group now you will lose your selections. " +
                                                          "Still Continue?",
                                                          "Warning",
                                                          MessageBoxButtons.YesNo);
                    if (result != DialogResult.Yes)
                    {
                        if (_oldGroup != -1 && _oldGroup < Groups.Items.Count)
                        {
                            Groups.Items[e.ItemIndex].Selected = false;
                            Groups.Items[_oldGroup].Selected = true;
                        }

                        // Register handler again
                        Groups.ItemSelectionChanged += GroupedSelectionChangeEvent;
                        return;
                    }
                    // Undo the changes to the Active and the UngroupedList.
                    UndoChanges();
                }

                if (e.ItemIndex != _oldGroup)
                {
                    var groupName = Groups.SelectedItems[0].Text;
                    if (_activeGroup.Name != groupName)
                    {
                        SetActiveGroup(_fRewriter.GetGroup(groupName));
                    }
                }

                // Reregister handler
                Groups.ItemSelectionChanged += GroupedSelectionChangeEvent;
            }
            _oldGroup = e.ItemIndex;
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}