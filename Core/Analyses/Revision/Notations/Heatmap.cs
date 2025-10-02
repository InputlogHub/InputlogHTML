using System;
using System.Collections.Generic;
using InputLog.Core.Plugin.WordLog;
using InputLog.Core.Analyses.Revision.Revisions;

namespace InputLog.Core.Analyses.Revision.Notations
{
    public class Heatmap
    {
        public class RevisionHMEntry
        {
            public readonly TextRange Range;
            public int Count;
            public int Length;
            public readonly bool IsDel;

            public RevisionHMEntry(TextRange range, int count, int length, bool isDel = false)
            {
                Count = count;
                Range = range;
                Length = length;
                IsDel = isDel;
            }

            internal RevisionHMEntry Split(int pivot, InputlogDocument ilDoc)
            {
                int newLength;
                if (IsDel)
                {
                    Length = Length + (Range.End - pivot);
                    newLength = pivot - Range.End;
                }
                else 
                {
                    Length = Length - (Range.End - pivot);
                    newLength = Range.End - pivot;
                }
                var newRange = new TextRange(pivot, Range.End);
                Range.Shift(0, pivot - (Range.End+1));
                return new RevisionHMEntry(newRange, Count, newLength, IsDel);
            }
        }

        private readonly RevisionMatrixSummary Summary;
        private readonly InputlogDocument IlDoc;
        private readonly List<RevisionHMEntry> Entries;

        public Heatmap(RevisionMatrixSummary summary, InputlogDocument ilDoc)
        {
            Summary = summary;
            IlDoc = ilDoc;
            Entries = new List<RevisionHMEntry>();
        }

        public void Process()
        {
            // Apply all edits in revisions
            foreach (RevisionMatrixEntry entry in Summary.Entries)
            {
                var rev = (AbstractRevision)entry.Revision;
                rev.Update(IlDoc);
            }

            // Add all interesting revisions to Entries,
            // and determine their positions in the final document along the way
            foreach (RevisionMatrixEntry entry in Summary.Entries)
            {
                var rev = (AbstractRevision)entry.Revision;
                if (rev is SelectionChangeRevision) continue;
                var r = new TextRange(rev.StartPos, rev.EndPos);
                var hme = new RevisionHMEntry(r, rev is DeleteRevision ? 2 : 1, rev.Length, rev is DeleteRevision);
                InsertRange(hme);
                Entries.Add(hme);
            }
            SortEntriesByLength();
            int c = 0;
            // Infinite loop since we will be adding entries during processing
            while (true)
            {
                if (c == Entries.Count) break;
                RevisionHMEntry e = Entries[c];
                ComputeRevCount(e);
                c++;
            }
            // Revisions with lowest count get processed first
            Entries.Sort((hme1, hme2) => hme1.Count.CompareTo(hme2.Count));
            // Give all text the basic color
            IlDoc.Range().Range.HighlightColorIndex = GetColor(1);
            foreach (RevisionHMEntry e in Entries)
            {
                ColorRange(e.Range.Start, e.Range.End, e.Count);
            }
            IlDoc.SaveAndClose();
        }

        private void ColorRange(int begin, int end, int c)
        {
            TextRange r = IlDoc.Range(begin, end);
            if (r.Text.Trim().Equals("") && r.Text[0] != ' ')
            {
                ColorRange(r.Start - 1, r.End-1, c);
            }
            else
            {
                r.Range.HighlightColorIndex = GetColor(c);
            }
        }

        private void SortEntriesByLength()
        {
            Entries.Sort(
                delegate(RevisionHMEntry hme1, RevisionHMEntry hme2)
                {
                    int c = Math.Abs(hme2.Length).CompareTo(Math.Abs(hme1.Length));
                    if (c == 0) return hme1.GetHashCode().CompareTo(hme2.GetHashCode());
                    return c;
                }
            );
        }

        private void InsertRange(RevisionHMEntry hme)
        {
            foreach (RevisionHMEntry e in Entries)
            {
                TextRange.OverlapTYPE ol = e.Range.Overlap(hme.Range);

                if (ol == TextRange.OverlapTYPE.DISJUNCT_BEFORE)
                {
                    // Push the other revision backward 
                    e.Range.Shift(hme.Length, hme.Length);
                }
                if (e.IsDel) continue;
                if (ol == TextRange.OverlapTYPE.CONTAINED)
                {
                    // Insertion or deletion in the middle: only end changes
                    e.Range.Shift(0, hme.Length);
                    e.Length += hme.Length;
                }

                if (ol == TextRange.OverlapTYPE.OVERLAP_BEGIN)
                {
                    if (e.IsDel)
                    {
                        // Overlap in the beginning: begin is shifted by length of non-overlapping part, end is shifted fully
                        e.Range.Shift(hme.Length - e.Range.StartOffset(hme.Range), hme.Length);
                        e.Length += e.Range.StartOffset(hme.Range);
                    }
                    else
                    {
                        // Push the other revision backward 
                        e.Range.Shift(hme.Length, hme.Length);
                    }
                }
                if (ol == TextRange.OverlapTYPE.OVERLAP_END)
                {
                    if (e.IsDel)
                    {
                        // Overlap at the end: end is shifted by length of overlapping part
                        e.Range.Shift(0, e.Range.EndOffset(hme.Range));
                        e.Length += e.Range.EndOffset(hme.Range);
                    }
                    else
                    {
                        // Push the other revision backward 
                        e.Range.Shift(0, hme.Length);
                        e.Length += hme.Length;
                    }
                }
            }
        }

        private void ComputeRevCount(RevisionHMEntry hme)
        {
            var splits = new List<RevisionHMEntry>();
            bool pastThis = false;
            foreach (RevisionHMEntry e in Entries)
            {
                if (hme.Equals(e)) 
                {
                    pastThis = true;
                    continue;
                }

                TextRange.OverlapTYPE ol = e.Range.Overlap(hme.Range);
                if (ol == TextRange.OverlapTYPE.CONTAINED)
                {
                    // This part has been edited once more than the surrounding part
                    hme.Count = Math.Max(e.Count + 1, hme.Count);
                }
                if (pastThis) continue;
                if (ol == TextRange.OverlapTYPE.OVERLAP_BEGIN)
                {
                    // Split off part after e begins, this part has been edited once more than e
                    RevisionHMEntry split = hme.Split(e.Range.Start, IlDoc);
                    split.Count = Math.Max(e.Count + 1, split.Count);
                    splits.Add(split);
                }
                if (ol == TextRange.OverlapTYPE.OVERLAP_END)
                {
                    // Split off part after e ends, original part has been edited once more than e
                    RevisionHMEntry split = hme.Split(e.Range.End, IlDoc);
                    hme.Count = Math.Max(e.Count + 1, hme.Count);
                    splits.Add(split);
                }
            }
            // Add splits and sort list again
            foreach (RevisionHMEntry s in splits)
            {
                Entries.Add(s);
            }
            if (splits.Count > 0) SortEntriesByLength();
        }

        public static Microsoft.Office.Interop.Word.WdColorIndex GetColor(int p)
        {
            if (p == 2) return Microsoft.Office.Interop.Word.WdColorIndex.wdBrightGreen;
            if (p == 3) return Microsoft.Office.Interop.Word.WdColorIndex.wdYellow;
            if (p == 4) return Microsoft.Office.Interop.Word.WdColorIndex.wdRed;
            if (p > 4)
                return Microsoft.Office.Interop.Word.WdColorIndex.wdDarkRed;
            return Microsoft.Office.Interop.Word.WdColorIndex.wdWhite;
        }
    }
}
