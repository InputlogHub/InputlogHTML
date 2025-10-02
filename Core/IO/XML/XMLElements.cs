namespace InputLog.Core.IO.Xml
{
    public class XmlElements
    {
        public static class Log
        {
            public const string TAG = "log";

            public static class Session
            {
                public const string TAG = "session";

                public static class Entry
                {
                    public const string TAG = "entry";

                    public static class Key
                    {
                        public const string TAG = "key";
                    }

                    public static class Value
                    {
                        public const string TAG = "value";
                    }
                }
            }

            public static class Meta
            {
                public const string TAG = "meta";

                public static class Entry
                {
                    public const string TAG = "entry";

                    public static class Key
                    {
                        public const string TAG = "key";
                    }

                    public static class Value
                    {
                        public const string TAG = "value";
                    }
                }
            }

            public static class Events
            {
                public const string TAG = "events";

                public static class Event
                {
                    public const string TAG = "event";

                    public static class ATTRIBUTES
                    {
                        public static class Type
                        {
                            public const string KEY = "type";

                            public static class VALUES
                            {
                                public const string KEYBOARD = "keyboard";
                                public const string MOUSE = "mouse";
                                public const string FOCUSCHANGE = "focus";
                                public const string SELECTIONCHANGE = "selectionchange";
                                public const string STATISTICS = "statistics";
                                public const string QUESTIONS = "questions";
                                public const string REPLACEMENT = "replacement";
                                public const string INSERT = "insert";
                                public const string EYETRACKER = "eyetrack";
                                public const string DRAGON_NS = "dragonns";
                                public const string AUTHORCOMMENT = "comment";
                            }
                        }

                        public const string ID = "id";
                    }

                    public static class Label
                    {
                        public const string TAG = "label";

                        public static class ATTRIBUTES
                        {
                            public static class Key
                            {
                                public const string KEY = "key";
                            }
                        };
                    }

                    public static class Part
                    {
                        public const string TAG = "part";

                        public static class ATTRIBUTES
                        {
                            public static class Type
                            {
                                public const string KEY = "type";

                                public static class VALUES
                                {
                                    public const string WinLog = "winlog";
                                    public const string WordLog = "wordlog";
                                    public const string Eyetracker = "eyetrack";
                                    public const string DragonNS = "dragonns";
                                }
                            }
                        }

                        public static class DragonNS
                        {
                            public static class ID
                            {
                                public const string TAG = "id";
                            }

                            public static class StartTime
                            {
                                public const string TAG = "startTime";
                            }

                            public static class EndTime
                            {
                                public const string TAG = "endTime";
                            }

                            public static class TotalStartTime
                            {
                                public const string TAG = "totalStartTime";
                            }

                            public static class TotalEndTime
                            {
                                public const string TAG = "totalEndTime";
                            }

                            public static class Guid
                            {
                                public const string TAG = "guid";
                            }

                            public static class Text
                            {
                                public const string TAG = "text";
                            }

                            public static class WavePath
                            {
                                public const string TAG = "wavePath";
                            }
                        }

                        public static class Eyetrack
                        {
                            public static class Media
                            {
                                public const string TAG = "media";

                                public static class MediaName
                                {
                                    public const string TAG = "mediaName";
                                }

                                public static class MediaPosX
                                {
                                    public const string TAG = "mediaPosX_ADCSpx";
                                }

                                public static class MediaPosY
                                {
                                    public const string TAG = "mediaPosY_ADCSpx";
                                }

                                public static class MediaWidth
                                {
                                    public const string TAG = "mediaWidth";
                                }

                                public static class MediaHeight
                                {
                                    public const string TAG = "mediaHeight";
                                }
                            }

                            public static class Segment
                            {
                                public const string TAG = "segmentScene";

                                public static class SegmentName
                                {
                                    public const string TAG = "segmentName";
                                }

                                public static class SegmentStart
                                {
                                    public const string TAG = "segmentStart";
                                }

                                public static class SegmentEnd
                                {
                                    public const string TAG = "segmendEnd";
                                }

                                public static class SegmentDuration
                                {
                                    public const string TAG = "segmentDuration";
                                }

                                public static class SceneName
                                {
                                    public const string TAG = "sceneName";
                                }

                                public static class SceneStart
                                {
                                    public const string TAG = "sceneStart";
                                }

                                public static class SceneEnd
                                {
                                    public const string TAG = "sceneEnd";
                                }

                                public static class SceneDuration
                                {
                                    public const string TAG = "sceneDuration";
                                }
                            }

                            public static class Time
                            {
                                public const string TAG = "time";

                                public static class Start
                                {
                                    public const string TAG = "start";
                                }

                                public static class End
                                {
                                    public const string TAG = "end";
                                }

                                public static class Duration
                                {
                                    public const string TAG = "duration";
                                }

                                public static class FullTimestamp
                                {
                                    public const string TAG = "fullTimestamp";
                                }
                            }

                            public static class MouseEvent
                            {
                                public const string TAG = "mouse";

                                public static class Event
                                {
                                    public const string TAG = "events";
                                }

                                public static class EventNumber
                                {
                                    public const string TAG = "nrEvents";
                                }
                            }

                            public static class KeyboardEvent
                            {
                                public const string TAG = "keyboard";

                                public static class Event
                                {
                                    public const string TAG = "events";
                                }

                                public static class EventNumber
                                {
                                    public const string TAG = "nrEvents";
                                }
                            }

                            public static class StudioEvent
                            {
                                public const string TAG = "studioEvent";

                                public static class Event
                                {
                                    public const string TAG = "event";
                                }

                                public static class EventData
                                {
                                    public const string TAG = "eventData";
                                }
                            }

                            public static class ExternalEvent
                            {
                                public const string TAG = "externalEvent";

                                public static class Event
                                {
                                    public const string TAG = "event";
                                }

                                public static class EventValue
                                {
                                    public const string TAG = "eventValue";
                                }
                            }

                            public static class Gaze
                            {
                                public const string TAG = "gaze";

                                // basic information
                                public static class FixationIndex
                                {
                                    public const string TAG = "fixationIndex";
                                }

                                public static class SaccadeIndex
                                {
                                    public const string TAG = "saccadeIndex";
                                }

                                public static class GazeEventType
                                {
                                    public const string TAG = "gazeEventType";
                                }

                                public static class GazeEventDuration
                                {
                                    public const string TAG = "gazeEventDuration";
                                }

                                // Averages
                                public static class AverageGazePointX
                                {
                                    public const string TAG = "averageGazePointX_ADCSpx";
                                }

                                public static class AverageGazePointY
                                {
                                    public const string TAG = "averageGazePointY_ADCSpx";
                                }

                                public static class AverageValidityLeft
                                {
                                    public const string TAG = "averageValidityLeft";
                                }

                                public static class AverageValidityRight
                                {
                                    public const string TAG = "averageValidityRight";
                                }

                                public static class AveragePupilLeft
                                {
                                    public const string TAG = "averagePupilLeft";
                                }

                                public static class AveragePupilRight
                                {
                                    public const string TAG = "averagePupilRight";
                                }

                                public static class OffscreenTime
                                {
                                    public const string TAG = "offscreenTime";
                                }

                                public static class NrOfSamples
                                {
                                    public const string TAG = "nrOfSamples";
                                }

                                public static class NrOfValidSamples
                                {
                                    public const string TAG = "nrOfValidSamples";
                                }


                                // X calculations
                                public static class MinGazePointX_MDSpx
                                {
                                    public const string TAG = "minGazePointX_MDSpx";
                                }

                                public static class MaxGazePointX_MDSpx
                                {
                                    public const string TAG = "maxGazePointX_MDSpx";
                                }

                                public static class MinGazePointX_ADSCpx
                                {
                                    public const string TAG = "minGazePointX_ADSCpx";
                                }

                                public static class MaxGazePointX_ADSCpx
                                {
                                    public const string TAG = "maxGazePointX_ADSCpx";
                                }

                                public static class StartGazePointX_ADSCpx
                                {
                                    public const string TAG = "startGazePointX_ADSCpx";
                                }

                                public static class EndGazePointX_ADSCpx
                                {
                                    public const string TAG = "endGazePointX_ADSCpx";
                                }

                                public static class MaxDistanceX
                                {
                                    public const string TAG = "maxDistanceX";
                                }

                                public static class DistanceX
                                {
                                    public const string TAG = "distanceX";
                                }

                                public static class CumulativeAbsoluteDistanceX
                                {
                                    public const string TAG = "cumAbsDistanceX";
                                }

                                public static class CumulativeAbsoluteDistanceX_Left
                                {
                                    public const string TAG = "cumAbsDistanceX_Left";
                                }

                                public static class CumulativeAbsoluteDistanceX_Right
                                {
                                    public const string TAG = "cumAbsDistanceX_Right";
                                }

                                // Y Calculations
                                public static class MinGazePointY_MDSpx
                                {
                                    public const string TAG = "minGazePointY_MDSpx";
                                }

                                public static class MaxGazePointY_MDSpx
                                {
                                    public const string TAG = "maxGazePointY_MDSpx";
                                }

                                public static class MinGazePointY_ADSCpx
                                {
                                    public const string TAG = "minGazePointY_ADSCpx";
                                }

                                public static class MaxGazePointY_ADSCpx
                                {
                                    public const string TAG = "maxGazePointY_ADSCpx";
                                }

                                public static class StartGazePointY_ADSCpx
                                {
                                    public const string TAG = "startGazePointY_ADSCpx";
                                }

                                public static class EndGazePointY_ADSCpx
                                {
                                    public const string TAG = "endGazePointY_ADSCpx";
                                }

                                public static class MaxDistanceY
                                {
                                    public const string TAG = "maxDistanceY";
                                }

                                public static class DistanceY
                                {
                                    public const string TAG = "distanceY";
                                }

                                public static class CumulativeAbsoluteDistanceY
                                {
                                    public const string TAG = "cumAbsDistanceY";
                                }

                                public static class CumulativeAbsoluteDistanceY_Up
                                {
                                    public const string TAG = "cumAbsDistanceY_Up";
                                }

                                public static class CumulativeAbsoluteDistanceY_Down
                                {
                                    public const string TAG = "cumAbsDistanceY_Down";
                                }
                            }

                            public static class EyePosition
                            {
                                public const string TAG = "eyeposition_ADCSmm";

                                // minimums
                                public static class EyePosLeftX_MIN
                                {
                                    public const string TAG = "eyeposLeftX_MIN";
                                }

                                public static class EyePosLeftY_MIN
                                {
                                    public const string TAG = "eyeposLeftY_MIN";
                                }

                                public static class EyePosLeftZ_MIN
                                {
                                    public const string TAG = "eyeposLeftZ_MIN";
                                }

                                public static class EyePosRightX_MIN
                                {
                                    public const string TAG = "eyeposRightX_MIN";
                                }

                                public static class EyePosRightY_MIN
                                {
                                    public const string TAG = "eyeposRightY_MIN";
                                }

                                public static class EyePosRightZ_MIN
                                {
                                    public const string TAG = "eyeposRightZ_MIN";
                                }

                                public static class DistanceLeft_MIN
                                {
                                    public const string TAG = "distanceLeft_MIN";
                                }

                                public static class DistanceRight_MIN
                                {
                                    public const string TAG = "distanceRight_MIN";
                                }

                                // maximums
                                public static class EyePosLeftX_MAX
                                {
                                    public const string TAG = "eyeposLeftX_MAX";
                                }

                                public static class EyePosLeftY_MAX
                                {
                                    public const string TAG = "eyeposLeftY_MAX";
                                }

                                public static class EyePosLeftZ_MAX
                                {
                                    public const string TAG = "eyeposLeftZ_MAX";
                                }

                                public static class EyePosRightX_MAX
                                {
                                    public const string TAG = "eyeposRightX_MAX";
                                }

                                public static class EyePosRightY_MAX
                                {
                                    public const string TAG = "eyeposRightY_MAX";
                                }

                                public static class EyePosRightZ_MAX
                                {
                                    public const string TAG = "eyeposRightZ_MAX";
                                }

                                public static class DistanceLeft_MAX
                                {
                                    public const string TAG = "distanceLeft_MAX";
                                }

                                public static class DistanceRight_MAX
                                {
                                    public const string TAG = "distanceRight_MAX";
                                }
                            }

                            public static class AOI
                            {
                                public const string TAG = "aoi";

                                public static class AOI_HIT
                                {
                                    public const string TAG = "hit";

                                    public static class ATTRIBUTES
                                    {
                                        public static class NAME
                                        {
                                            public const string KEY = "name";
                                        }
                                    };
                                }
                            }
                        }

                        public static class FocusChange
                        {
                            public static class WinLog
                            {
                                public static class Title
                                {
                                    public const string TAG = "title";
                                }
                            }
                        }

                        public static class Insert
                        {
                            public static class WordLog
                            {
                                public static class Position
                                {
                                    public const string TAG = "position";
                                }

                                public static class Before
                                {
                                    public const string TAG = "before";
                                }

                                public static class After
                                {
                                    public const string TAG = "after";
                                }
                            }
                        }

                        public static class Keyboard
                        {
                            public static class WinLog
                            {
                                public static class StartTime
                                {
                                    public const string TAG = "startTime";
                                }

                                public static class EndTime
                                {
                                    public const string TAG = "endTime";
                                }

                                public static class Key
                                {
                                    public const string TAG = "key";
                                }

                                public static class Value
                                {
                                    public const string TAG = "value";
                                }

                                public static class KeyboardState
                                {
                                    public const string TAG = "keyboardstate";

                                    public static class Key
                                    {
                                        public const string TAG = "key";
                                    }
                                }
                            }

                            public static class WordLog
                            {
                                public static class Position
                                {
                                    public const string TAG = "position";
                                }

                                public static class DocumentLength
                                {
                                    public const string TAG = "documentLength";
                                }

                                public static class IncludeInReplay
                                {
                                    public const string TAG = "replay";
                                }
                            }
                        }

                        public static class Mouse
                        {
                            public static class WinLog
                            {
                                public static class StartTime
                                {
                                    public const string TAG = "startTime";
                                }

                                public static class EndTime
                                {
                                    public const string TAG = "endTime";
                                }

                                public static class X
                                {
                                    public const string TAG = "x";
                                }

                                public static class Y
                                {
                                    public const string TAG = "y";
                                }

                                public static class Type
                                {
                                    public const string TAG = "type";

                                    public static class VALUES
                                    {
                                        public const string CLICK = "click";
                                        public const string MOVEMENT = "movement";
                                        public const string SCROLL = "scroll";
                                    }
                                }

                                public static class Click
                                {
                                    public static class Button
                                    {
                                        public const string TAG = "button";
                                    }
                                }

                                public static class Scroll
                                {
                                    public static class Orientation
                                    {
                                        public const string TAG = "orientation";
                                    }

                                    public static class Delta
                                    {
                                        public const string TAG = "delta";
                                    }
                                }

                                public static class KeyboardState
                                {
                                    public const string TAG = "keyboardstate";

                                    public static class Key
                                    {
                                        public const string TAG = "key";
                                    }
                                }
                            }
                        }

                        public static class Replacement
                        {
                            public static class WordLog
                            {
                                public static class Start
                                {
                                    public const string TAG = "start";
                                }

                                public static class End
                                {
                                    public const string TAG = "end";
                                }

                                public static class NewText
                                {
                                    public const string TAG = "newtext";
                                }
                            }
                        }

                        public static class SelectionChange
                        {
                            public static class WordLog
                            {
                                public static class Start
                                {
                                    public const string TAG = "start";
                                }

                                public static class End
                                {
                                    public const string TAG = "end";
                                }

                                public static class Type
                                {
                                    public const string TAG = "type";
                                }
                            }
                        }

                        public static class Questions
                        {
                            public static class WinLog
                            {
                                public static class Handedness
                                {
                                    public const string TAG = "handedness";
                                }

                                public static class Computer
                                {
                                    public const string TAG = "computer";
                                }

                                public static class Keyboard
                                {
                                    public const string TAG = "keyboard";
                                }

                                public static class Browser
                                {
                                    public const string TAG = "browser";
                                }

                                public static class Language
                                {
                                    public const string TAG = "language";
                                }

                                public static class Disorder
                                {
                                    public const string TAG = "disorder";
                                }

                                public static class Education
                                {
                                    public const string TAG = "education";
                                }

                                public static class Repetition
                                {
                                    public const string TAG = "repetition";
                                }
                            }
                        }

                        public static class Statistics
                        {
                            public static class WordLog
                            {
                                public static class CharCountWithSpaces
                                {
                                    public const string TAG = "charinclspaces";
                                }

                                public static class CharCountWithoutSpaces
                                {
                                    public const string TAG = "charexclspaces";
                                }

                                public static class FarEastCharCount
                                {
                                    public const string TAG = "fareastcharcount";
                                }

                                public static class LineCount
                                {
                                    public const string TAG = "linecount";
                                }

                                public static class PageCount
                                {
                                    public const string TAG = "pagecount";
                                }

                                public static class ParagraphCount
                                {
                                    public const string TAG = "paragraphcount";
                                }

                                public static class WordCount
                                {
                                    public const string TAG = "wordcount";
                                }

                                public static class StartCharCountWithSpaces
                                {
                                    public const string TAG = "stcharinclspaces";
                                }

                                public static class StartCharCountWithoutSpaces
                                {
                                    public const string TAG = "stcharexclspaces";
                                }

                                public static class StartFarEastCharCount
                                {
                                    public const string TAG = "fareastcharcount";
                                }

                                public static class StartLineCount
                                {
                                    public const string TAG = "stlinecount";
                                }

                                public static class StartPageCount
                                {
                                    public const string TAG = "stpagecount";
                                }

                                public static class StartParagraphCount
                                {
                                    public const string TAG = "stparagraphcount";
                                }

                                public static class StartWordCount
                                {
                                    public const string TAG = "stwordcount";
                                }
                            }
                        }

                        public static class AuthorComment
                        {
                            public static class WordLog
                            {
                                public static class Comment
                                {
                                    public const string TAG = "comment";
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}