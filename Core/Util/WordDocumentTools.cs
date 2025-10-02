using System;
using System.Globalization;
using System.Reflection;
using Wrd = Microsoft.Office.Interop.Word;

namespace InputLog.Core.Util
{
    public static class WordDocumentTools
    {
        /// <summary>
        /// Opens the logged document and extracts the text content.
        /// </summary>
        /// <param name="path">location of the file</param>
        /// <returns>string with text content</returns>
        public static string OpenDoc(string path)
        {
            var output = "User document not available.";
            object missing = Missing.Value;
            try
            {
                var word = new Wrd.Application();
                var doc = word.Documents.Open(path);
                var rnge = doc.Range();
                rnge.WholeStory();
                output = rnge.Text;
                doc.Close(ref missing, ref missing, ref missing);
            }
            catch
            {
                // Fail silently
            }
            return output;
        }

        /// <summary>
        /// initialize word with the preset configuration as demanded by inputlog.
        /// </summary>
        public static void DisableUnwantedOptions(Wrd.Application thisWord)
        {
            // Disable unwanted options   
            thisWord.AutoCorrect.CorrectCapsLock = false;
            thisWord.AutoCorrect.CorrectDays = false;
            thisWord.AutoCorrect.CorrectHangulAndAlphabet = false;
            thisWord.AutoCorrect.CorrectInitialCaps = false;
            thisWord.AutoCorrect.CorrectKeyboardSetting = false;
            thisWord.AutoCorrect.CorrectSentenceCaps = false;
            thisWord.AutoCorrect.CorrectTableCells = false;
            thisWord.AutoCorrect.ReplaceText = false;
            thisWord.AutoCorrect.ReplaceTextFromSpellingChecker = false;
            thisWord.Options.CheckGrammarAsYouType = false;
            thisWord.Options.CheckSpellingAsYouType = false;
            thisWord.Options.AllowDragAndDrop = false;
            thisWord.Options.SmartCutPaste = false;
            thisWord.Options.PasteAdjustParagraphSpacing = false;
            thisWord.Options.PasteAdjustTableFormatting = false;
            thisWord.Options.PasteAdjustWordSpacing = false;
            thisWord.Options.SmartParaSelection = false;
            thisWord.ActiveDocument.AutoHyphenation = false;
            DisableAutoFormatting(thisWord);
            // SaveInterval property is set to 0 to turn off AutoRecovery, 
            // preventing a 'Save as' dialog to pop up.
            if (thisWord.ActiveDocument.ReadOnly.Equals(false)) thisWord.Options.SaveInterval = 0;
            
            // Only for Word 2013 and newer
            var provider = new NumberFormatInfo {NumberDecimalSeparator = "."};
            var wVersion = Convert.ToDouble(thisWord.Version, provider);
            if (wVersion > 14.0)
            {
                thisWord.Options.ShowDevTools = false;
                thisWord.Options.DisplayPasteOptions = false;
            }

            // Commented out: causes System.NotimplementedException in certain user settings.
            // thisWord.AutoCaptions.CancelAutoInsert();

            // Unloads all loaded add-ins and optionally removes them from the AddIns collection.
            thisWord.AddIns.Unload(Settings.WordLogDisableWordAddins);
        }

        /// <summary>
        /// Disables AutoFormattingAsYouType (e.g. bulleted lists, ...).
        /// </summary>
        private static void DisableAutoFormatting(Wrd.Application thisWord)
        {
            thisWord.Options.AutoFormatAsYouTypeApplyBorders = false;
            thisWord.Options.AutoFormatAsYouTypeApplyBulletedLists = false;
            thisWord.Options.AutoFormatAsYouTypeApplyClosings = false;
            thisWord.Options.AutoFormatAsYouTypeApplyDates = false;
            thisWord.Options.AutoFormatAsYouTypeApplyFirstIndents = false;
            thisWord.Options.AutoFormatAsYouTypeApplyHeadings = false;
            thisWord.Options.AutoFormatAsYouTypeApplyNumberedLists = false;
            thisWord.Options.AutoFormatAsYouTypeApplyTables = false;
            thisWord.Options.AutoFormatAsYouTypeAutoLetterWizard = false;
            thisWord.Options.AutoFormatAsYouTypeDefineStyles = false;
            thisWord.Options.AutoFormatAsYouTypeDeleteAutoSpaces = false;
            thisWord.Options.AutoFormatAsYouTypeFormatListItemBeginning = false;
            thisWord.Options.AutoFormatAsYouTypeInsertClosings = false;
            thisWord.Options.AutoFormatAsYouTypeInsertOvers = false;
            thisWord.Options.AutoFormatAsYouTypeMatchParentheses = false;
            thisWord.Options.AutoFormatAsYouTypeReplaceFarEastDashes = false;
            thisWord.Options.AutoFormatAsYouTypeReplaceFractions = false;
            thisWord.Options.AutoFormatAsYouTypeReplaceHyperlinks = false;
            thisWord.Options.AutoFormatAsYouTypeReplaceOrdinals = false;
            thisWord.Options.AutoFormatAsYouTypeReplacePlainTextEmphasis = false;
            thisWord.Options.AutoFormatAsYouTypeReplaceQuotes = false;
            thisWord.Options.AutoFormatAsYouTypeReplaceSymbols = false;
            thisWord.Options.AutoFormatDeleteAutoSpaces = false;
            thisWord.Options.AutoFormatMatchParentheses = false;

        }
    }
}