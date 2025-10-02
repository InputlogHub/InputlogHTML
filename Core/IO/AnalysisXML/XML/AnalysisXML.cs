namespace InputLog.Core.IO.AnalysisXML.XML
{
    internal class AnalysisXML
    {
        #region Nested type: Session

        public static class Session
        {
            public const string TAG = "session";

            #region Nested type: Event

            public static class Event
            {
                public const int MAXELEMENTS = 21;
                public const string TAG = "event";

                #region Nested type: ActionTime

                public static class ActionTime
                {
                    public const string TAG = "actionTime";
                }

                #endregion

                #region Nested type: DocLength

                public static class DocLength
                {
                    public const string TAG = "doclength";
                }

                #endregion

                #region Nested type: DocLengthFull

                public static class DocLengthFull
                {
                    public const string TAG = "doclengthFull";
                }

                #endregion

                #region Nested type: CharProduction

                public static class CharProduction
                {
                    public const string TAG = "charProduction";
                }

                #endregion

                #region Nested type: EndClock

                public static class EndClock
                {
                    public const string TAG = "endClock";
                }

                #endregion

                #region Nested type: EndTime

                public static class EndTime
                {
                    public const string TAG = "endTime";
                }
                #endregion

                #region Nested type: Id

                public static class Id
                {
                    public const string TAG = "id";
                }

                #endregion

                #region Nested type: Output

                public static class Output
                {
                    public const string TAG = "output";
                }

                #endregion

                #region Nested type: PauseLocation

                public static class PauseLocation
                {
                    public const string TAG = "pauseLocation";
                }

                #endregion

                #region Nested type: PauseLocationFull

                public static class PauseLocationFull
                {
                    public const string TAG = "pauseLocationFull";
                }

                #endregion

                #region Nested type: PauseTime

                public static class PauseTime
                {
                    public const string TAG = "pauseTime";
                }

                #endregion

                #region Nested type: Position

                public static class Position
                {
                    public const string TAG = "position";
                }

                #endregion

                #region Nested type: PositionFull

                public static class PositionFull
                {
                    public const string TAG = "positionFull";
                }

                #endregion

                #region Nested type: StartClock

                public static class StartClock
                {
                    public const string TAG = "startClock";
                }

                #endregion

                #region Nested type: StartTime

                public static class StartTime
                {
                    public const string TAG = "startTime";
                }

                #endregion

                #region Nested type: Type

                public static class Type
                {
                    public const string TAG = "type";
                }

                #endregion

                #region Nested type: Fixed Number Interval

                public static class FixedNumberInterval
                {
                    public const string TAG = "intervalNumber";
                }

                #endregion

                #region Nested type: Fixed Size Interval

                public static class FixedSizeInterval
                {
                    public const string TAG = "intervalSize";
                }

                #endregion

                #region Nested type: X

                public static class X
                {
                    public const string TAG = "x";
                }

                #endregion

                #region Nested type: Y

                public static class Y
                {
                    public const string TAG = "y";
                }

                #endregion

                #region Nested type: RevisionInfo

                public static class RevisionInfo
                {
                    public const string TAG = "RevisionInfo";
                    public const string NR_TAG = "RevisionNumber";
                    public const string POS_TAG = "RevisionPos";
                    public const string TYPE_TAG = "RevisionType";
                }

                #endregion
            }

            #endregion

            #region Nested type: ExtraInfo

            public static class ExtraInfo
            {
                public const string TAG = "extraInfo";

                #region Nested type: ATTRIBUTES

                public static class ATTRIBUTES
                {
                    public const string TITLE = "title";
                }

                #endregion

                #region Nested type: Entry

                public static class Entry
                {
                    public const string TAG = "entry";

                    #region Nested type: ATTRIBUTES

                    public static class ATTRIBUTES
                    {
                        public const string NAME = "name";
                        public const string VALUE = "value";
                    }

                    #endregion
                }

                #endregion
            }

            #endregion

            #region Nested type: Meta

            public static class Meta
            {
                public const string TAG = "meta";

                #region Nested type: Entry

                public static class Entry
                {
                    public const string TAG = "entry";

                    #region Nested type: ATTRIBUTES

                    public static class ATTRIBUTES
                    {
                        public const string NAME = "name";
                        public const string VALUE = "value";
                    }

                    #endregion
                }

                #endregion
            }

            #endregion

            #region Nested type: Module

            public static class Module
            {
                public const string TAG = "module";

                #region Nested type: ATTRIBUTES

                public static class ATTRIBUTES
                {
                    public const string NAME = "name";
                }

                #endregion

                #region Nested type: Block

                public static class Block
                {
                    public const string TAG = "block";

                    #region Nested type: ATTRIBUTES

                    public static class ATTRIBUTES
                    {
                        public const string NAME = "name";
                        public const string VALUE = "value";
                    }

                    #endregion

                    #region Nested type: Element

                    public static class Element
                    {
                        public const string TAG = "element";

                        #region Nested type: ATTRIBUTES

                        public static class ATTRIBUTES
                        {
                            public const string NAME = "name";
                            public const string VALUE = "value";
                        }

                        #endregion
                    }

                    #endregion
                }

                #endregion
            }

            #endregion

            #region Nested type: SessionIdentification

            public static class SessionIdentification
            {
                public const string TAG = "sessionIdentification";

                #region Nested type: Entry

                public static class Entry
                {
                    public const string TAG = "entry";

                    #region Nested type: ATTRIBUTES

                    public static class ATTRIBUTES
                    {
                        public const string NAME = "name";
                        public const string VALUE = "value";
                    }

                    #endregion
                }

                #endregion
            }

            #endregion

            #region Nested type: Revision

            public static class Revision
            {
                public const int MAXELEMENTS = 13;
                public const string TAG = "revision";

                #region Nested type: RevisionNumber

                public static class RevisionNumber
                {
                    public const string TAG = "revisionNumber";
                }

                #endregion

                #region Nested type: Type

                public static class Type
                {
                    public const string TAG = "type";
                }

                #endregion

                #region Nested type: Content

                public static class Content
                {
                    public const string TAG = "content";
                }

                #endregion

                #region Nested type: Edits

                public static class Edits
                {
                    public const string TAG = "edits";
                }

                #endregion

                #region Nested type: Start

                public static class Start
                {
                    public const string TAG = "start";
                }

                #endregion

                #region Nested type: End

                public static class End
                {
                    public const string TAG = "end";
                }
                #endregion

                #region Nested type: Duration

                public static class Duration
                {
                    public const string TAG = "duration";
                }

                #endregion

                #region Nested type: Length

                public static class Length
                {
                    public const string TAG = "length";
                }

                #endregion

                #region Nested type: BeginPos

                public static class BeginPos
                {
                    public const string TAG = "beginPos";
                }

                #endregion

                #region Nested type: EndPos

                public static class EndPos
                {
                    public const string TAG = "endPos";
                }

                #endregion

                #region Nested type: Chars

                public static class Chars
                {
                    public const string TAG = "chars";
                }

                #endregion

                #region Nested type: CharWithoutSpace

                public static class CharWithoutSpace
                {
                    public const string TAG = "charWithoutSpace";
                }

                #endregion

                #region Nested type: Words

                public static class Words
                {
                    public const string TAG = "words";
                }

                #endregion

            }

            #endregion

            #region Nested type: SNotation

            public static class SNotation
            {
                public const string TAG = "markup";
            }
            #endregion

            #region Nested type: Period

            public static class Period
            {
                public const string TAG = "Period";
                public static class Condensed
                {
                    public const string TAG = "Condensed";
                }
                public static class PeriodID
                {
                    public const string TAG = "period_id";
                }
                public static class PeriodTime
                {
                    public const string TAG = "periodTime";
                }
                public static class PeriodEvent
                {
                    public const string TAG = "PeriodEvent";
                    public static class Value
                    {
                        public const string TAG = "value";
                    }
                }
            }
            #endregion


            #region Nested type: Linguistic

            public static class Linguistic
            {
                public const string TAG = "linguisticProcess";
                public static class InfoTable
                {
                    public const string TAG = "infoTable";
                    public static class Revisions
                    {
                        public const string TAG = "Revisions";
                    }
                    public static class SNotation
                    {
                        public const string TAG = "S-Notation";
                    }
                    public static class CharsProduced
                    {
                        public const string TAG = "CharsProduced";
                    }
                    public static class Token
                    {
                        public const string TAG = "Token";
                    }
                    public static class PoSA
                    {
                        public const string TAG = "PoSA";
                    }
                    public static class PoSB
                    {
                        public const string TAG = "PoSB";
                    }
                    public static class PoSProb
                    {
                        public const string TAG = "PoS-Prob";
                    }
                    public static class Lemma
                    {
                        public const string TAG = "Lemma";
                    }
                    public static class LemmaProb
                    {
                        public const string TAG = "Lemma-Prob";
                    }
                    public static class ChunkA
                    {
                        public const string TAG = "ChunkA";
                    }
                    public static class ChunkB
                    {
                        public const string TAG = "ChunkB";
                    }
                    public static class NE
                    {
                        public const string TAG = "NE";
                    }
                    public static class NEProb
                    {
                        public const string TAG = "NE-Prob";
                    }
                    public static class LogFreq
                    {
                        public const string TAG = "LogFreq";
                    }
                    public static class RelFreq
                    {
                        public const string TAG = "RelFreq";
                    }
                    public static class Syllable
                    {
                        public const string TAG = "Syllable";
                    }
                    public static class StartID
                    {
                        public const string TAG = "StartID";
                    }
                    public static class EndID
                    {
                        public const string TAG = "EndID";
                    }
                    public static class StartTime
                    {
                        public const string TAG = "StartTime";
                    }
                    public static class EndTime
                    {
                        public const string TAG = "EndTime";
                    }
                    public static class BeforeWord2
                    {
                        public const string TAG = "BeforeWord2";
                    }
                    public static class BeforeWord1
                    {
                        public const string TAG = "BeforeWord1";
                    }
                    public static class BetweenPause
                    {
                        public const string TAG = "BetweenPause";
                    }
                    public static class Production
                    {
                        public const string TAG = "Production";
                    }
                    public static class WordPause
                    {
                        public const string TAG = "WordPause";
                    }
                    public static class AfterWordPause
                    {
                        public const string TAG = "AfterWordPause";
                    }
                }
            }
            #endregion

            #region Nested type: WordPauses

            public static class WordPauses
            {
                public const string TAG = "wordPauses";
                public static class InfoTable
                {
                    public const string TAG = "infoTable";
                    public static class Revisions
                    {
                        public const string TAG = "Revisions";
                    }
                    public static class SNotation
                    {
                        public const string TAG = "S-Notation";
                    }
                    public static class CharsProduced
                    {
                        public const string TAG = "CharsProduced";
                    }
                    public static class Token
                    {
                        public const string TAG = "Token";
                    }
                     public static class StartID
                    {
                        public const string TAG = "StartID";
                    }
                    public static class EndID
                    {
                        public const string TAG = "EndID";
                    }
                    public static class StartTime
                    {
                        public const string TAG = "StartTime";
                    }
                    public static class EndTime
                    {
                        public const string TAG = "EndTime";
                    }
                    public static class BeforeWord2
                    {
                        public const string TAG = "BeforeWord2";
                    }
                    public static class BeforeWord1
                    {
                        public const string TAG = "BeforeWord1";
                    }
                    public static class BetweenPause
                    {
                        public const string TAG = "BetweenPause";
                    }
                    public static class Production
                    {
                        public const string TAG = "Production";
                    }
                    public static class WordPause
                    {
                        public const string TAG = "WordPause";
                    }
                    public static class AfterWordPause
                    {
                        public const string TAG = "AfterWordPause";
                    }
                    public static class Target
                    {
                        public const string TAG = "Target";
                    }
                }
            }
            #endregion
        }

        #endregion
    }
}