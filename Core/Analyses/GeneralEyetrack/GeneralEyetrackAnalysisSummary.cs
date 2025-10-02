using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputLog.Core.Analyses.GeneralEyetrack
{
	public class GeneralEyetrackAnalysisSummary: General.GeneralAnalysisSummary
	{
		public GeneralEyetrackAnalysisSummary() : base() 
		{
		}

		#region Nested type - GeneralEyetrackAnalysisEvent [Inherited from GeneralAnalysisEvent]

		/// <summary>
		/// Create a new Analysis Event.
		/// </summary>
		/// <returns></returns>
		public override General.GeneralAnalysisSummary.GeneralAnalysisEvent GetAnalysisEvent()
		{
			return new GeneralEyetrackAnalysisEvent();
		}

		/// <summary>
		/// Extends the original: GeneralAnalysisEvent
		/// Adds information for the condensed eyetrack analysis
		/// </summary>
		public class GeneralEyetrackAnalysisEvent : General.GeneralAnalysisSummary.GeneralAnalysisEvent
		{
			///////////////////////////////////////////////////////////////////
			// The number of gaze types and their indices.
			///////////////////////////////////////////////////////////////////

			public const int NR_OF_TYPES = 3;
			public const int FIX = 0;
			public const int SAC = 1;
			public const int UNC = 2;

			public const string GAZE_EVENT_TYPE_FIXATION = "Fixation";
			public const string GAZE_EVENT_TYPE_SACCADE = "Saccade";
			public const string GAZE_EVENT_TYPE_UNCLASSIFIED = "Unclassified";

			/// <summary>
			/// Prefixes coupled to the gaze types.
			/// </summary>
			public static readonly Dictionary<int, string> PREFIX = new Dictionary<int,string> 
			{
				{ FIX, "FIX_" },
				{ SAC, "SAC_" },
				{ UNC, "UNC_" },
			};

			///////////////////////////////////////////////////////////////////
			// The variables
			///////////////////////////////////////////////////////////////////

			/// <summary>
			/// Return the index of the gazeType.
			/// </summary>
			/// <param name="gazeType">String name of the gazeEventType</param>
			/// <returns>The index in the array of results</returns>
			private int GTINDEX(string gazeType)
			{
				switch (gazeType)
				{
					case GAZE_EVENT_TYPE_FIXATION:
						return FIX;
					case GAZE_EVENT_TYPE_SACCADE:
						return SAC;
					case GAZE_EVENT_TYPE_UNCLASSIFIED:
						return UNC;
					default:
						return -1;
				}
			}

			/// <summary>
			/// Convert a type index into the full type name.
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public static string INDEX_TO_TYPE(int index)
			{
				switch (index)
				{
					case FIX:
						return GAZE_EVENT_TYPE_FIXATION;
					case SAC:
						return GAZE_EVENT_TYPE_SACCADE;
					case UNC:
						return GAZE_EVENT_TYPE_UNCLASSIFIED;
					default:
						return "";
				}
			}

			// Unique variables (not copied for each gaze type)
			private List<string> _FixationIndices = new List<string>();
			public string Get_FixationIndices()
			{
				return string.Join(", ", _FixationIndices);
			}
			public void Merge_FixationIndices(string index)
			{
				_FixationIndices.Add(index);
			}
			private List<string> _SaccadeIndices = new List<string>();
			public string Get_SaccadeIndices()
			{
				return string.Join(", ", _SaccadeIndices);
			}
			public void Merge_SaccadeIndices(string index)
			{
				_SaccadeIndices.Add(index);
			}


			///////////////////////////////////////////////////////////////////
			// Variables per gaze type.
			///////////////////////////////////////////////////////////////////

			///////////////////////////////////////////////////////////////////
			// Nr of gaze events of a certain type
			//
			private int?[] GazeEventTypeNumberOf = new int?[3];
			public void Merge_GazeEventTypeNumberOf(string type)
			{
				if (!GazeEventTypeNumberOf[GTINDEX(type)].HasValue)
				{
					GazeEventTypeNumberOf[GTINDEX(type)] = 0;
				}
				GazeEventTypeNumberOf[GTINDEX(type)]++;
			}
			public int? Get_GazeEventTypeNumberOf(string type)
			{
				return GazeEventTypeNumberOf[GTINDEX(type)];
			}

			///////////////////////////////////////////////////////////////////
			// Total duration of all gaze events of a certain type
			//
			private int?[] GazeEventDuration = new int?[3];
			public void Merge_GazeEventDuration(string type, int duration)
			{
				if (!GazeEventDuration[GTINDEX(type)].HasValue)
				{
					GazeEventDuration[GTINDEX(type)] = 0;
				}
				GazeEventDuration[GTINDEX(type)] += duration;
			}
			public int? Get_GazeEventDuration(string type)
			{
				return GazeEventDuration[GTINDEX(type)];
			}

			///////////////////////////////////////////////////////////////////
			// AverageGazePointX_ADCSpx
			//
			private double[] _AverageGazePointX_ADCSpx_SUM = new double[3];
			private double?[] _AverageGazePointX_ADCSpx_NRVSMPLES = new double?[3];
			public double? Get_AverageGazePointX_ADCSpx(string type)
			{
				return (_AverageGazePointX_ADCSpx_NRVSMPLES[GTINDEX(type)].HasValue && _AverageGazePointX_ADCSpx_NRVSMPLES[GTINDEX(type)].Value > 0) ?
						_AverageGazePointX_ADCSpx_SUM[GTINDEX(type)] / _AverageGazePointX_ADCSpx_NRVSMPLES[GTINDEX(type)].Value : (double?)null;
			}
			public void Merge_AverageGazePointX_ADCSpx_Merge(string type, double avg, int nrValidSamples)
			{
				if (!_AverageGazePointX_ADCSpx_NRVSMPLES[GTINDEX(type)].HasValue)
				{
					_AverageGazePointX_ADCSpx_NRVSMPLES[GTINDEX(type)] = 0;
				}
				_AverageGazePointX_ADCSpx_NRVSMPLES[GTINDEX(type)] += nrValidSamples;
				_AverageGazePointX_ADCSpx_SUM[GTINDEX(type)] += avg * nrValidSamples;
			}

			///////////////////////////////////////////////////////////////////
			// AverageGazePointY_ADCSpx
			//
			private double[] _AverageGazePointY_ADCSpx_SUM = new double[3];
			private double?[] _AverageGazePointY_ADCSpx_NRVSMPLES = new double?[3];
			public double? Get_AverageGazePointY_ADCSpx(string type)
			{
				return (_AverageGazePointY_ADCSpx_NRVSMPLES[GTINDEX(type)].HasValue && _AverageGazePointY_ADCSpx_NRVSMPLES[GTINDEX(type)].Value > 0) ?
						_AverageGazePointY_ADCSpx_SUM[GTINDEX(type)] / _AverageGazePointY_ADCSpx_NRVSMPLES[GTINDEX(type)].Value : (double?)null;
			}
			public void Merge_AverageGazePointY_ADCSpx_Merge(string type, double avg, int nrValidSamples)
			{
				if (!_AverageGazePointY_ADCSpx_NRVSMPLES[GTINDEX(type)].HasValue)
				{
					_AverageGazePointY_ADCSpx_NRVSMPLES[GTINDEX(type)] = 0;
				}
				_AverageGazePointY_ADCSpx_NRVSMPLES[GTINDEX(type)] += nrValidSamples;
				_AverageGazePointY_ADCSpx_SUM[GTINDEX(type)] += avg * nrValidSamples;
			}

			///////////////////////////////////////////////////////////////////
			// AveragePupilLeft
			//
			private double[] _AveragePupilLeft_SUM = new double[3];
			private double?[] _AveragePupilLeft_NRVSMPLES = new double?[3];
			public double? Get_AveragePupilLeft(string type)
			{
				return (_AveragePupilLeft_NRVSMPLES[GTINDEX(type)].HasValue && _AveragePupilLeft_NRVSMPLES[GTINDEX(type)].Value > 0) ?
					_AveragePupilLeft_SUM[GTINDEX(type)] / _AveragePupilLeft_NRVSMPLES[GTINDEX(type)].Value : (double?)null;
			}
			public void Merge_AveragePupilLeft(string type, double avg, int nrValidSamples)
			{
				if (!_AveragePupilLeft_NRVSMPLES[GTINDEX(type)].HasValue)
				{
					_AveragePupilLeft_NRVSMPLES[GTINDEX(type)] = 0;
				}
				_AveragePupilLeft_NRVSMPLES[GTINDEX(type)] += nrValidSamples;
				_AveragePupilLeft_SUM[GTINDEX(type)] += avg * nrValidSamples;
			}

			///////////////////////////////////////////////////////////////////
			// AveragePupilRight
			//
			private double[] _AveragePupilRight_SUM = new double[3];
			private double?[] _AveragePupilRight_NRVSMPLES = new double?[3];
			public double? Get_AveragePupilRight(string type)
			{
				return (_AveragePupilRight_NRVSMPLES[GTINDEX(type)].HasValue && _AveragePupilRight_NRVSMPLES[GTINDEX(type)].Value > 0) ?
					_AveragePupilRight_SUM[GTINDEX(type)] / _AveragePupilRight_NRVSMPLES[GTINDEX(type)].Value : (double?)null;
			}
			public void Merge_AveragePupilRight(string type, double avg, int nrValidSamples)
			{
				if (!_AveragePupilRight_NRVSMPLES[GTINDEX(type)].HasValue)
				{
					_AveragePupilRight_NRVSMPLES[GTINDEX(type)] = 0;
				}
				_AveragePupilRight_NRVSMPLES[GTINDEX(type)] += nrValidSamples;
				_AveragePupilRight_SUM[GTINDEX(type)] += avg * nrValidSamples;
			}

			///////////////////////////////////////////////////////////////////
			// AverageValidityLeft
			//
			private double[] _AverageValidityLeft_SUM = new double[3];
			private double?[] _AverageValidityLeft_NRVSMPLES = new double?[3];
			public double? Get_AverageValidityLeft(string type)
			{
				return (_AverageValidityLeft_NRVSMPLES[GTINDEX(type)].HasValue && _AverageValidityLeft_NRVSMPLES[GTINDEX(type)].Value > 0) ?
					_AverageValidityLeft_SUM[GTINDEX(type)] / _AverageValidityLeft_NRVSMPLES[GTINDEX(type)].Value : (double?)null;
			}
			public void Merge_AverageValidityLeft(string type, double avg, int nrValidSamples)
			{
				if (!_AverageValidityLeft_NRVSMPLES[GTINDEX(type)].HasValue)
				{
					_AverageValidityLeft_NRVSMPLES[GTINDEX(type)] = 0;
				}
				_AverageValidityLeft_NRVSMPLES[GTINDEX(type)] += nrValidSamples;
				_AverageValidityLeft_SUM[GTINDEX(type)] += avg * nrValidSamples;
			}

			///////////////////////////////////////////////////////////////////
			// AverageValidityRight
			//
			private double[] _AverageValidityRight_SUM = new double[3];
			private double?[] _AverageValidityRight_NRVSMPLES = new double?[3];
			public double? Get_AverageValidityRight(string type)
			{
				return (_AverageValidityRight_NRVSMPLES[GTINDEX(type)].HasValue && _AverageValidityRight_NRVSMPLES[GTINDEX(type)].Value > 0) ?
					_AverageValidityRight_SUM[GTINDEX(type)] / _AverageValidityRight_NRVSMPLES[GTINDEX(type)].Value : (double?)null;
			}
			public void Merge_AverageValidityRight(string type, double avg, int nrValidSamples)
			{
				if (!_AverageValidityRight_NRVSMPLES[GTINDEX(type)].HasValue)
				{
					_AverageValidityRight_NRVSMPLES[GTINDEX(type)] = 0;
				}
				_AverageValidityRight_NRVSMPLES[GTINDEX(type)] += nrValidSamples;
				_AverageValidityRight_SUM[GTINDEX(type)] += avg * nrValidSamples;
			}

			///////////////////////////////////////////////////////////////////
			// OffscreenTime
			//
			private int?[] _OffscreenTime = new int?[3];
			public int? Get_OffscreenTime(string type)
			{
				return _OffscreenTime[GTINDEX(type)];
			}
			public void Merge_OffscreenTime(string type, int offScreenTime)
			{
				if (!_OffscreenTime[GTINDEX(type)].HasValue)
				{
					_OffscreenTime[GTINDEX(type)] = 0;
				}
				_OffscreenTime[GTINDEX(type)] += offScreenTime;
			}

			///////////////////////////////////////////////////////////////////
			// OffscreenTime
			//
			private int?[] _NrOfSamples = new int?[3];
			public int? Get_NrOfSamples(string type)
			{
				return _NrOfSamples[GTINDEX(type)];
			}
			public void Merge_NrOfSamples(string type, int offScreenTime)
			{
				if (!_NrOfSamples[GTINDEX(type)].HasValue)
				{
					_NrOfSamples[GTINDEX(type)] = 0;
				}
				_NrOfSamples[GTINDEX(type)] += offScreenTime;
			}

			///////////////////////////////////////////////////////////////////
			// OffscreenTime
			//
			private int?[] _NrOfValidSamples = new int?[3];
			public int? Get_NrOfValidSamples(string type)
			{
				return _NrOfValidSamples[GTINDEX(type)];
			}
			public void Merge_NrOfValidSamples(string type, int offScreenTime)
			{
				if (!_NrOfValidSamples[GTINDEX(type)].HasValue)
				{
					_NrOfValidSamples[GTINDEX(type)] = 0;
				}
				_NrOfValidSamples[GTINDEX(type)] += offScreenTime;
			}

			///////////////////////////////////////////////////////////////////
			// MinGazePointX_ADCSpx
			//
			private int?[] _MinGazePointX_ADCSpx = new int?[3];
			public int? Get_MinGazePointX_ADCSpx(string type)
			{
				return _MinGazePointX_ADCSpx[GTINDEX(type)];
			}
			public void Merge_MinGazePointX_ADCSpx(string type, int value)
			{
				_MinGazePointX_ADCSpx[GTINDEX(type)] = 
                    Math.Min(value, (_MinGazePointX_ADCSpx[GTINDEX(type)].HasValue 
                    ? _MinGazePointX_ADCSpx[GTINDEX(type)].Value : int.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// MinGazePointX_MCSpx
			//
			private int?[] _MinGazePointX_MCSpx = new int?[3];
			public int? Get_MinGazePointX_MCSpx(string type)
			{
				return _MinGazePointX_MCSpx[GTINDEX(type)];
			}
			public void Merge_MinGazePointX_MCSpx(string type, int value)
			{
				_MinGazePointX_MCSpx[GTINDEX(type)] = 
                    Math.Min(value, (_MinGazePointX_MCSpx[GTINDEX(type)].HasValue 
                    ? _MinGazePointX_MCSpx[GTINDEX(type)].Value : int.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// MinGazePointY_ADCSpx
			//
			private int?[] _MinGazePointY_ADCSpx = new int?[3];
			public int? Get_MinGazePointY_ADCSpx(string type)
			{
				return _MinGazePointY_ADCSpx[GTINDEX(type)];
			}
			public void Merge_MinGazePointY_ADCSpx(string type, int value)
			{
				_MinGazePointY_ADCSpx[GTINDEX(type)] = 
                    Math.Min(value, (_MinGazePointY_ADCSpx[GTINDEX(type)].HasValue 
                    ? _MinGazePointY_ADCSpx[GTINDEX(type)].Value : int.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// MinGazePointY_MCSpx
			//
			private int?[] _MinGazePointY_MCSpx = new int?[3];
			public int? Get_MinGazePointY_MCSpx(string type)
			{
				return _MinGazePointY_MCSpx[GTINDEX(type)];
			}
			public void Merge_MinGazePointY_MCSpx(string type, int value)
			{
				_MinGazePointY_MCSpx[GTINDEX(type)] = 
                    Math.Min(value, (_MinGazePointY_MCSpx[GTINDEX(type)].HasValue 
                    ? _MinGazePointY_MCSpx[GTINDEX(type)].Value : int.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// MaxGazePointX_ADCSpx
			//
			private int?[] _MaxGazePointX_ADCSpx = new int?[3];
			public int? Get_MaxGazePointX_ADCSpx(string type)
			{
				return _MaxGazePointX_ADCSpx[GTINDEX(type)];
			}
			public void Merge_MaxGazePointX_ADCSpx(string type, int value)
			{
				_MaxGazePointX_ADCSpx[GTINDEX(type)] = 
                    Math.Max(value, (_MaxGazePointX_ADCSpx[GTINDEX(type)].HasValue 
                    ? _MaxGazePointX_ADCSpx[GTINDEX(type)].Value : int.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// MaxGazePointX_MCSpx
			//
			private int?[] _MaxGazePointX_MCSpx = new int?[3];
			public int? Get_MaxGazePointX_MCSpx(string type)
			{
				return _MaxGazePointX_MCSpx[GTINDEX(type)];
			}
			public void Merge_MaxGazePointX_MCSpx(string type, int value)
			{
				_MaxGazePointX_MCSpx[GTINDEX(type)] = 
                    Math.Max(value, (_MaxGazePointX_MCSpx[GTINDEX(type)].HasValue 
                    ? _MaxGazePointX_MCSpx[GTINDEX(type)].Value : int.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// MaxGazePointY_ADCSpx
			//
			private int?[] _MaxGazePointY_ADCSpx = new int?[3];
			public int? Get_MaxGazePointY_ADCSpx(string type)
			{
				return _MaxGazePointY_ADCSpx[GTINDEX(type)];
			}
			public void Merge_MaxGazePointY_ADCSpx(string type, int value)
			{
				_MaxGazePointY_ADCSpx[GTINDEX(type)] = 
                    Math.Max(value, (_MaxGazePointY_ADCSpx[GTINDEX(type)].HasValue 
                    ? _MaxGazePointY_ADCSpx[GTINDEX(type)].Value : int.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// MaxGazePointY_MCSpx
			//
			private int?[] _MaxGazePointY_MCSpx = new int?[3];
			public int? Get_MaxGazePointY_MCSpx(string type)
			{
				return _MaxGazePointY_MCSpx[GTINDEX(type)];
			}
			public void Merge_MaxGazePointY_MCSpx(string type, int value)
			{
				_MaxGazePointY_MCSpx[GTINDEX(type)] = 
                    Math.Max(value, (_MaxGazePointY_MCSpx[GTINDEX(type)].HasValue 
                    ? _MaxGazePointY_MCSpx[GTINDEX(type)].Value : int.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// MaxMaxDistanceX
			//
			private int?[] _MaxMaxDistanceX = new int?[3];
			public int? Get_MaxMaxDistanceX(string type)
			{
				return _MaxMaxDistanceX[GTINDEX(type)];
			}
			public void Merge_MaxMaxDistanceX(string type, int distance)
			{
				_MaxMaxDistanceX[GTINDEX(type)] = 
                    Math.Max(distance, (_MaxMaxDistanceX[GTINDEX(type)].HasValue 
                    ? _MaxMaxDistanceX[GTINDEX(type)].Value : int.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// MinMaxDistanceX
			//
			private int?[] _MinMaxDistanceX = new int?[3];
			public int? Get_MinMaxDistanceX(string type)
			{
				return _MinMaxDistanceX[GTINDEX(type)];
			}
			public void Merge_MinMaxDistanceX(string type, int distance)
			{
				_MinMaxDistanceX[GTINDEX(type)] = 
                    Math.Min(distance, (_MinMaxDistanceX[GTINDEX(type)].HasValue 
                    ? _MinMaxDistanceX[GTINDEX(type)].Value : int.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// AvgMaxDistanceX
			//
			private int[] _AvgMaxDistanceX_SUM = new int[3];
			private int?[] _AvgMaxDistanceX_NREVENTS = new int?[3];
			public double? Get_AvgMaxDistanceX(string type)
			{
				return (_AvgMaxDistanceX_NREVENTS[GTINDEX(type)].HasValue 
                    && _AvgMaxDistanceX_NREVENTS[GTINDEX(type)] > 0) 
                    ? ((double)_AvgMaxDistanceX_SUM[GTINDEX(type)]) 
                    / _AvgMaxDistanceX_NREVENTS[GTINDEX(type)].Value : (double?)null;
			}
			public void Merge_AvgMaxDistanceX(string type, int distance)
			{
				if (!_AvgMaxDistanceX_NREVENTS[GTINDEX(type)].HasValue)
				{
					_AvgMaxDistanceX_NREVENTS[GTINDEX(type)] = 0;
				}
				_AvgMaxDistanceX_NREVENTS[GTINDEX(type)]++;
				_AvgMaxDistanceX_SUM[GTINDEX(type)] += distance;
			}

			///////////////////////////////////////////////////////////////////
			// MaxMaxDistanceY
			//
			private int?[] _MaxMaxDistanceY = new int?[3];
			public int? Get_MaxMaxDistanceY(string type)
			{
				return _MaxMaxDistanceY[GTINDEX(type)];
			}
			public void Merge_MaxMaxDistanceY(string type, int distance)
			{
				_MaxMaxDistanceY[GTINDEX(type)] = 
                    Math.Max(distance, (_MaxMaxDistanceY[GTINDEX(type)].HasValue 
                    ? _MaxMaxDistanceY[GTINDEX(type)].Value : int.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// MinMaxDistanceY
			//
			private int?[] _MinMaxDistanceY = new int?[3];
			public int? Get_MinMaxDistanceY(string type)
			{
				return _MinMaxDistanceY[GTINDEX(type)];
			}
			public void Merge_MinMaxDistanceY(string type, int distance)
			{
				_MinMaxDistanceY[GTINDEX(type)] = 
                    Math.Min(distance, (_MinMaxDistanceY[GTINDEX(type)].HasValue 
                    ? _MinMaxDistanceY[GTINDEX(type)].Value : int.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// AvgMaxDistanceY
			//
			private int[] _AvgMaxDistanceY_SUM = new int[3];
			private int?[] _AvgMaxDistanceY_NREVENTS = new int?[3];
			public double? Get_AvgMaxDistanceY(string type)
			{
				return (_AvgMaxDistanceY_NREVENTS[GTINDEX(type)].HasValue 
                    && _AvgMaxDistanceY_NREVENTS[GTINDEX(type)] > 0) ?
					((double)_AvgMaxDistanceY_SUM[GTINDEX(type)]) / _AvgMaxDistanceY_NREVENTS[GTINDEX(type)].Value 
                    : (double?)null;
			}
			public void Merge_AvgMaxDistanceY(string type, int distance)
			{
				if (!_AvgMaxDistanceY_NREVENTS[GTINDEX(type)].HasValue)
				{
					_AvgMaxDistanceY_NREVENTS[GTINDEX(type)] = 0;
				}
				_AvgMaxDistanceY_NREVENTS[GTINDEX(type)]++;
				_AvgMaxDistanceY_SUM[GTINDEX(type)] += distance;
			}

			///////////////////////////////////////////////////////////////////
			// MaxDistanceX
			//
			private int?[] _MaxDistanceX = new int?[3];
			public int? Get_MaxDistanceX(string type)
			{
				return _MaxDistanceX[GTINDEX(type)];
			}
			public void Merge_MaxDistanceX(string type, int distance)
			{
				_MaxDistanceX[GTINDEX(type)] = 
                    Math.Max(distance, (_MaxDistanceX[GTINDEX(type)].HasValue 
                    ? _MaxDistanceX[GTINDEX(type)].Value : int.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// MinDistanceX
			//
			private int?[] _MinDistanceX = new int?[3];
			public int? Get_MinDistanceX(string type)
			{
				return _MinDistanceX[GTINDEX(type)];
			}
			public void Merge_MinDistanceX(string type, int distance)
			{
				_MinDistanceX[GTINDEX(type)] = 
                    Math.Min(distance, (_MinDistanceX[GTINDEX(type)].HasValue 
                    ? _MinDistanceX[GTINDEX(type)].Value : int.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// AvgDistanceX
			//
			private int[] _AvgDistanceX_SUM = new int[3];
			private int?[] _AvgDistanceX_NREVENTS = new int?[3];
			public double? Get_AvgDistanceX(string type)
			{
				return (_AvgDistanceX_NREVENTS[GTINDEX(type)].HasValue && _AvgDistanceX_NREVENTS[GTINDEX(type)] > 0) ?
					((double)_AvgDistanceX_SUM[GTINDEX(type)]) / _AvgDistanceX_NREVENTS[GTINDEX(type)].Value : (double?)null;
			}
			public void Merge_AvgDistanceX(string type, int distance)
			{
				if (!_AvgDistanceX_NREVENTS[GTINDEX(type)].HasValue)
				{
					_AvgDistanceX_NREVENTS[GTINDEX(type)] = 0;
				}
				_AvgDistanceX_NREVENTS[GTINDEX(type)]++;
				_AvgDistanceX_SUM[GTINDEX(type)] += distance;
			}

			///////////////////////////////////////////////////////////////////
			// MaxDistanceY
			//
			private int?[] _MaxDistanceY = new int?[3];
			public int? Get_MaxDistanceY(string type)
			{
				return _MaxDistanceY[GTINDEX(type)];
			}
			public void Merge_MaxDistanceY(string type, int distance)
			{
				_MaxDistanceY[GTINDEX(type)] = Math.Max(distance, (_MaxDistanceY[GTINDEX(type)].HasValue ? _MaxDistanceY[GTINDEX(type)].Value : int.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// MinDistanceY
			//
			private int?[] _MinDistanceY = new int?[3];
			public int? Get_MinDistanceY(string type)
			{
				return _MinDistanceY[GTINDEX(type)];
			}
			public void Merge_MinDistanceY(string type, int distance)
			{
				_MinDistanceY[GTINDEX(type)] = Math.Min(distance, (_MinDistanceY[GTINDEX(type)].HasValue ? _MinDistanceY[GTINDEX(type)].Value : int.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// AvgDistanceY
			//
			private int[] _AvgDistanceY_SUM = new int[3];
			private int?[] _AvgDistanceY_NREVENTS = new int?[3];
			public double? Get_AvgDistanceY(string type)
			{
				return (_AvgDistanceY_NREVENTS[GTINDEX(type)].HasValue && _AvgDistanceY_NREVENTS[GTINDEX(type)] > 0) ?
					((double)_AvgDistanceY_SUM[GTINDEX(type)]) / _AvgDistanceY_NREVENTS[GTINDEX(type)].Value : (double?)null;
			}
			public void Merge_AvgDistanceY(string type, int distance)
			{
				if (!_AvgDistanceY_NREVENTS[GTINDEX(type)].HasValue)
				{
					_AvgDistanceY_NREVENTS[GTINDEX(type)] = 0;
				}
				_AvgDistanceY_NREVENTS[GTINDEX(type)]++;
				_AvgDistanceY_SUM[GTINDEX(type)] += distance;
			}

			///////////////////////////////////////////////////////////////////
			// MinStartGazePointX_ADCSpx
			//
			private int?[] _MinStartGazePointX_ADCSpx = new int?[3];
			public int? Get_MinStartGazePointX_ADCSpx(string type)
			{
				return _MinStartGazePointX_ADCSpx[GTINDEX(type)];
			}
			public void Merge_MinStartGazePointX_ADCSpx(string type, int startGazePoint)
			{
				_MinStartGazePointX_ADCSpx[GTINDEX(type)] = Math.Min(startGazePoint, (_MinStartGazePointX_ADCSpx[GTINDEX(type)].HasValue ? _MinStartGazePointX_ADCSpx[GTINDEX(type)].Value : int.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// MaxStartGazePointX_ADCSpx
			//
			private int?[] _MaxStartGazePointX_ADCSpx = new int?[3];
			public int? Get_MaxStartGazePointX_ADCSpx(string type)
			{
				return _MaxStartGazePointX_ADCSpx[GTINDEX(type)];
			}
			public void Merge_MaxStartGazePointX_ADCSpx(string type, int startGazePoint)
			{
				_MaxStartGazePointX_ADCSpx[GTINDEX(type)] = Math.Max(startGazePoint, (_MaxStartGazePointX_ADCSpx[GTINDEX(type)].HasValue ? _MaxStartGazePointX_ADCSpx[GTINDEX(type)].Value : int.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// MinStartGazePointY_ADCSpx
			//
			private int?[] _MinStartGazePointY_ADCSpx = new int?[3];
			public int? Get_MinStartGazePointY_ADCSpx(string type)
			{
				return _MinStartGazePointY_ADCSpx[GTINDEX(type)];
			}
			public void Merge_MinStartGazePointY_ADCSpx(string type, int startGazePoint)
			{
				_MinStartGazePointY_ADCSpx[GTINDEX(type)] = Math.Min(startGazePoint, (_MinStartGazePointY_ADCSpx[GTINDEX(type)].HasValue ? _MinStartGazePointY_ADCSpx[GTINDEX(type)].Value : int.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// MaxStartGazePointY_ADCSpx
			//
			private int?[] _MaxStartGazePointY_ADCSpx = new int?[3];
			public int? Get_MaxStartGazePointY_ADCSpx(string type)
			{
				return _MaxStartGazePointY_ADCSpx[GTINDEX(type)];
			}
			public void Merge_MaxStartGazePointY_ADCSpx(string type, int startGazePoint)
			{
				_MaxStartGazePointY_ADCSpx[GTINDEX(type)] = Math.Max(startGazePoint, (_MaxStartGazePointY_ADCSpx[GTINDEX(type)].HasValue ? _MaxStartGazePointY_ADCSpx[GTINDEX(type)].Value : int.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// MinEndGazePointX_ADCSpx
			//
			private int?[] _MinEndGazePointX_ADCSpx = new int?[3];
			public int? Get_MinEndGazePointX_ADCSpx(string type)
			{
				return _MinEndGazePointX_ADCSpx[GTINDEX(type)];
			}
			public void Merge_MinEndGazePointX_ADCSpx(string type, int startGazePoint)
			{
				_MinEndGazePointX_ADCSpx[GTINDEX(type)] = Math.Min(startGazePoint, (_MinEndGazePointX_ADCSpx[GTINDEX(type)].HasValue ? _MinEndGazePointX_ADCSpx[GTINDEX(type)].Value : int.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// MaxEndGazePointX_ADCSpx
			//
			private int?[] _MaxEndGazePointX_ADCSpx = new int?[3];
			public int? Get_MaxEndGazePointX_ADCSpx(string type)
			{
				return _MaxEndGazePointX_ADCSpx[GTINDEX(type)];
			}
			public void Merge_MaxEndGazePointX_ADCSpx(string type, int startGazePoint)
			{
				_MaxEndGazePointX_ADCSpx[GTINDEX(type)] = Math.Max(startGazePoint, (_MaxEndGazePointX_ADCSpx[GTINDEX(type)].HasValue ? _MaxEndGazePointX_ADCSpx[GTINDEX(type)].Value : int.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// MinEndGazePointY_ADCSpx
			//
			private int?[] _MinEndGazePointY_ADCSpx = new int?[3];
			public int? Get_MinEndGazePointY_ADCSpx(string type)
			{
				return _MinEndGazePointY_ADCSpx[GTINDEX(type)];
			}
			public void Merge_MinEndGazePointY_ADCSpx(string type, int startGazePoint)
			{
				_MinEndGazePointY_ADCSpx[GTINDEX(type)] = Math.Min(startGazePoint, (_MinEndGazePointY_ADCSpx[GTINDEX(type)].HasValue ? _MinEndGazePointY_ADCSpx[GTINDEX(type)].Value : int.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// MaxEndGazePointY_ADCSpx
			//
			private int?[] _MaxEndGazePointY_ADCSpx = new int?[3];
			public int? Get_MaxEndGazePointY_ADCSpx(string type)
			{
				return _MaxEndGazePointY_ADCSpx[GTINDEX(type)];
			}
			public void Merge_MaxEndGazePointY_ADCSpx(string type, int startGazePoint)
			{
				_MaxEndGazePointY_ADCSpx[GTINDEX(type)] = Math.Max(startGazePoint, (_MaxEndGazePointY_ADCSpx[GTINDEX(type)].HasValue ? _MaxEndGazePointY_ADCSpx[GTINDEX(type)].Value : int.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// CumAbsDistanceX
			//
			private int?[] _CumAbsDistanceX = new int?[3];
			public int? Get_CumAbsDistanceX(string type)
			{
				return _CumAbsDistanceX[GTINDEX(type)];
			}
			public void Merge_CumAbsDistanceX(string type, int distance)
			{
				if (!_CumAbsDistanceX[GTINDEX(type)].HasValue)
				{
					_CumAbsDistanceX[GTINDEX(type)] = 0;
				}
				_CumAbsDistanceX[GTINDEX(type)] += distance;
			}

			///////////////////////////////////////////////////////////////////
			// CumAbsDistanceX_Left
			//
			private int?[] _CumAbsDistanceX_Left = new int?[3];
			public int? Get_CumAbsDistanceX_Left(string type)
			{
				return _CumAbsDistanceX_Left[GTINDEX(type)];
			}
			public void Merge_CumAbsDistanceX_Left(string type, int distance)
			{
				if (!_CumAbsDistanceX_Left[GTINDEX(type)].HasValue)
				{
					_CumAbsDistanceX_Left[GTINDEX(type)] = 0;
				}
				_CumAbsDistanceX_Left[GTINDEX(type)] += distance;
			}

			///////////////////////////////////////////////////////////////////
			// CumAbsDistanceX_Right
			//
			private int?[] _CumAbsDistanceX_Right = new int?[3];
			public int? Get_CumAbsDistanceX_Right(string type)
			{
				return _CumAbsDistanceX_Right[GTINDEX(type)];
			}
			public void Merge_CumAbsDistanceX_Right(string type, int distance)
			{
				if (!_CumAbsDistanceX_Right[GTINDEX(type)].HasValue)
				{
					_CumAbsDistanceX_Right[GTINDEX(type)] = 0;
				}
				_CumAbsDistanceX_Right[GTINDEX(type)] += distance;
			}

			///////////////////////////////////////////////////////////////////
			// CumAbsDistanceY
			//
			private int?[] _CumAbsDistanceY = new int?[3];
			public int? Get_CumAbsDistanceY(string type)
			{
				return _CumAbsDistanceY[GTINDEX(type)];
			}
			public void Merge_CumAbsDistanceY(string type, int distance)
			{
				if (!_CumAbsDistanceY[GTINDEX(type)].HasValue)
				{
					_CumAbsDistanceY[GTINDEX(type)] = 0;
				}
				_CumAbsDistanceY[GTINDEX(type)] += distance;
			}

			///////////////////////////////////////////////////////////////////
			// CumAbsDistanceY_Down
			//
			private int?[] _CumAbsDistanceY_Down = new int?[3];
			public int? Get_CumAbsDistanceY_Down(string type)
			{
				return _CumAbsDistanceY_Down[GTINDEX(type)];
			}
			public void Merge_CumAbsDistanceY_Down(string type, int distance)
			{
				if (!_CumAbsDistanceY_Down[GTINDEX(type)].HasValue)
				{
					_CumAbsDistanceY_Down[GTINDEX(type)] = 0;
				}
				_CumAbsDistanceY_Down[GTINDEX(type)] += distance;
			}

			///////////////////////////////////////////////////////////////////
			// CumAbsDistanceY_Up
			//
			private int?[] _CumAbsDistanceY_Up = new int?[3];
			public int? Get_CumAbsDistanceY_Up(string type)
			{
				return _CumAbsDistanceY_Up[GTINDEX(type)];
			}
			public void Merge_CumAbsDistanceY_Up(string type, int distance)
			{
				if (!_CumAbsDistanceY_Up[GTINDEX(type)].HasValue)
				{
					_CumAbsDistanceY_Up[GTINDEX(type)] = 0;
				}
				_CumAbsDistanceY_Up[GTINDEX(type)] += distance;
			}

			///////////////////////////////////////////////////////////////////
			// DistanceLeft_Max
			//
			private double?[] _DistanceLeft_Max = new double?[3];
			public double? Get_DistanceLeft_Max(string type)
			{
				return _DistanceLeft_Max[GTINDEX(type)];
			}
			public void Merge_DistanceLeft_Max(string type, double distance)
			{
				_DistanceLeft_Max[GTINDEX(type)] = Math.Max(distance, (_DistanceLeft_Max[GTINDEX(type)].HasValue ? _DistanceLeft_Max[GTINDEX(type)].Value : int.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// DistanceLeft_Min
			//
			private double?[] _DistanceLeft_Min = new double?[3];
			public double? Get_DistanceLeft_Min(string type)
			{
				return _DistanceLeft_Min[GTINDEX(type)];
			}
			public void Merge_DistanceLeft_Min(string type, double distance)
			{
				_DistanceLeft_Min[GTINDEX(type)] = Math.Min(distance, (_DistanceLeft_Min[GTINDEX(type)].HasValue ? _DistanceLeft_Min[GTINDEX(type)].Value : int.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// DistanceRight_Max
			//
			private double?[] _DistanceRight_Max = new double?[3];
			public double? Get_DistanceRight_Max(string type)
			{
				return _DistanceRight_Max[GTINDEX(type)];
			}
			public void Merge_DistanceRight_Max(string type, double distance)
			{
				_DistanceRight_Max[GTINDEX(type)] = Math.Max(distance, (_DistanceRight_Max[GTINDEX(type)].HasValue ? _DistanceRight_Max[GTINDEX(type)].Value : int.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// DistanceRight_Min
			//
			private double?[] _DistanceRight_Min = new double?[3];
			public double? Get_DistanceRight_Min(string type)
			{
				return _DistanceRight_Min[GTINDEX(type)];
			}
			public void Merge_DistanceRight_Min(string type, double distance)
			{
				_DistanceRight_Min[GTINDEX(type)] = Math.Min(distance, (_DistanceRight_Min[GTINDEX(type)].HasValue ? _DistanceRight_Min[GTINDEX(type)].Value : int.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// EyePosLeftX_Max
			//
			private double?[] _EyePosLeftX_Max = new double?[3];
			public double? Get_EyePosLeftX_Max(string type)
			{
				return _EyePosLeftX_Max[GTINDEX(type)];
			}
			public void Merge_EyePosLeftX_Max(string type, double distance)
			{
				_EyePosLeftX_Max[GTINDEX(type)] = Math.Max(distance, (_EyePosLeftX_Max[GTINDEX(type)].HasValue ? _EyePosLeftX_Max[GTINDEX(type)].Value : double.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// EyePosLeftX_Min
			//
			private double?[] _EyePosLeftX_Min = new double?[3];
			public double? Get_EyePosLeftX_Min(string type)
			{
				return _EyePosLeftX_Min[GTINDEX(type)];
			}
			public void Merge_EyePosLeftX_Min(string type, double distance)
			{
				_EyePosLeftX_Min[GTINDEX(type)] = Math.Min(distance, (_EyePosLeftX_Min[GTINDEX(type)].HasValue ? _EyePosLeftX_Min[GTINDEX(type)].Value : double.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// EyePosRightX_Max
			//
			private double?[] _EyePosRightX_Max = new double?[3];
			public double? Get_EyePosRightX_Max(string type)
			{
				return _EyePosRightX_Max[GTINDEX(type)];
			}
			public void Merge_EyePosRightX_Max(string type, double distance)
			{
				_EyePosRightX_Max[GTINDEX(type)] = Math.Max(distance, (_EyePosRightX_Max[GTINDEX(type)].HasValue ? _EyePosRightX_Max[GTINDEX(type)].Value : double.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// EyePosRightX_Min
			//
			private double?[] _EyePosRightX_Min = new double?[3];
			public double? Get_EyePosRightX_Min(string type)
			{
				return _EyePosRightX_Min[GTINDEX(type)];
			}
			public void Merge_EyePosRightX_Min(string type, double distance)
			{
				_EyePosRightX_Min[GTINDEX(type)] = Math.Min(distance, (_EyePosRightX_Min[GTINDEX(type)].HasValue ? _EyePosRightX_Min[GTINDEX(type)].Value : double.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// EyePosLeftY_Max
			//
			private double?[] _EyePosLeftY_Max = new double?[3];
			public double? Get_EyePosLeftY_Max(string type)
			{
				return _EyePosLeftY_Max[GTINDEX(type)];
			}
			public void Merge_EyePosLeftY_Max(string type, double distance)
			{
				_EyePosLeftY_Max[GTINDEX(type)] = Math.Max(distance, (_EyePosLeftY_Max[GTINDEX(type)].HasValue ? _EyePosLeftY_Max[GTINDEX(type)].Value : double.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// EyePosLeftY_Min
			//
			private double?[] _EyePosLeftY_Min = new double?[3];
			public double? Get_EyePosLeftY_Min(string type)
			{
				return _EyePosLeftY_Min[GTINDEX(type)];
			}
			public void Merge_EyePosLeftY_Min(string type, double distance)
			{
				_EyePosLeftY_Min[GTINDEX(type)] = Math.Min(distance, (_EyePosLeftY_Min[GTINDEX(type)].HasValue ? _EyePosLeftY_Min[GTINDEX(type)].Value : double.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// EyePosRightY_Max
			//
			private double?[] _EyePosRightY_Max = new double?[3];
			public double? Get_EyePosRightY_Max(string type)
			{
				return _EyePosRightY_Max[GTINDEX(type)];
			}
			public void Merge_EyePosRightY_Max(string type, double distance)
			{
				_EyePosRightY_Max[GTINDEX(type)] = Math.Max(distance, (_EyePosRightY_Max[GTINDEX(type)].HasValue ? _EyePosRightY_Max[GTINDEX(type)].Value : double.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// EyePosRightY_Min
			//
			private double?[] _EyePosRightY_Min = new double?[3];
			public double? Get_EyePosRightY_Min(string type)
			{
				return _EyePosRightY_Min[GTINDEX(type)];
			}
			public void Merge_EyePosRightY_Min(string type, double distance)
			{
				_EyePosRightY_Min[GTINDEX(type)] = Math.Min(distance, (_EyePosRightY_Min[GTINDEX(type)].HasValue ? _EyePosRightY_Min[GTINDEX(type)].Value : double.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// EyePosLeftZ_Max
			//
			private double?[] _EyePosLeftZ_Max = new double?[3];
			public double? Get_EyePosLeftZ_Max(string type)
			{
				return _EyePosLeftZ_Max[GTINDEX(type)];
			}
			public void Merge_EyePosLeftZ_Max(string type, double distance)
			{
				_EyePosLeftZ_Max[GTINDEX(type)] = Math.Max(distance, (_EyePosLeftZ_Max[GTINDEX(type)].HasValue ? _EyePosLeftZ_Max[GTINDEX(type)].Value : double.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// EyePosLeftZ_Min
			//
			private double?[] _EyePosLeftZ_Min = new double?[3];
			public double? Get_EyePosLeftZ_Min(string type)
			{
				return _EyePosLeftZ_Min[GTINDEX(type)];
			}
			public void Merge_EyePosLeftZ_Min(string type, double distance)
			{
				_EyePosLeftZ_Min[GTINDEX(type)] = Math.Min(distance, (_EyePosLeftZ_Min[GTINDEX(type)].HasValue ? _EyePosLeftZ_Min[GTINDEX(type)].Value : double.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// EyePosRightZ_Max
			//
			private double?[] _EyePosRightZ_Max = new double?[3];
			public double? Get_EyePosRightZ_Max(string type)
			{
				return _EyePosRightZ_Max[GTINDEX(type)];
			}
			public void Merge_EyePosRightZ_Max(string type, double distance)
			{
				_EyePosRightZ_Max[GTINDEX(type)] = Math.Max(distance, (_EyePosRightZ_Max[GTINDEX(type)].HasValue ? _EyePosRightZ_Max[GTINDEX(type)].Value : double.MinValue));
			}

			///////////////////////////////////////////////////////////////////
			// EyePosRightZ_Min
			//
			private double?[] _EyePosRightZ_Min = new double?[3];
			public double? Get_EyePosRightZ_Min(string type)
			{
				return _EyePosRightZ_Min[GTINDEX(type)];
			}
			public void Merge_EyePosRightZ_Min(string type, double distance)
			{
				_EyePosRightZ_Min[GTINDEX(type)] = Math.Min(distance, (_EyePosRightZ_Min[GTINDEX(type)].HasValue ? _EyePosRightZ_Min[GTINDEX(type)].Value : double.MaxValue));
			}

			///////////////////////////////////////////////////////////////////
			// MouseEvents
			//
			private List<string>[] _MouseEvents = new List<string>[] { new List<string>(), new List<string>(), new List<string>() };
			public string Get_MouseEvents(string type) // returns a comma-separated string of the MouseEvents
			{
				return string.Join(", ", _MouseEvents[GTINDEX(type)]);
			}
			public void Merge_MouseEvents(string type, string mouseEvents)
			{
				string[] mEvents = mouseEvents.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string @event in mEvents)
				{
					_MouseEvents[GTINDEX(type)].Add(@event);
				}
			}

			///////////////////////////////////////////////////////////////////
			// MouseNumberOfEvents
			//
			public int? Get_MouseNumberOfEvents(string type)
			{
				return _MouseEvents[GTINDEX(type)].Count > 0 ? _MouseEvents[GTINDEX(type)].Count : (int?)null;
			}

			///////////////////////////////////////////////////////////////////
			// KeyboardEvents
			//
			private List<string>[] _KeyboardEvents = new List<string>[] { new List<string>(), new List<string>(), new List<string>() };
			public string Get_KeyboardEvents(string type) // returns a comma-separated string of the mouseEvents
			{
				return string.Join(", ", _KeyboardEvents[GTINDEX(type)]);
			}
			public void Merge_KeyboardEvents(string type, string mouseEvents)
			{
				string[] mEvents = mouseEvents.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string @event in mEvents)
				{
					_KeyboardEvents[GTINDEX(type)].Add(@event);
				}
			}

			///////////////////////////////////////////////////////////////////
			// KeyboardNumberOfEvents
			//
			public int? Get_KeyboardNumberOfEvents(string type)
			{
				return _KeyboardEvents[GTINDEX(type)].Count > 0 ? _KeyboardEvents[GTINDEX(type)].Count : (int?)null;
			}

			///////////////////////////////////////////////////////////////////
			// StudioEvents
			//
			private List<string>[] _StudioEvents = new List<string>[] { new List<string>(), new List<string>(), new List<string>() };
			public string Get_StudioEvents(string type) // returns a comma-separated string of the mouseEvents
			{
				return string.Join(", ", _StudioEvents[GTINDEX(type)]);
			}
			public void Merge_StudioEvents(string type, string mouseEvents)
			{
				string[] mEvents = mouseEvents.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string @event in mEvents)
				{
					_StudioEvents[GTINDEX(type)].Add(@event);
				}
			}

			///////////////////////////////////////////////////////////////////
			// StudioEventValues
			//
			private List<string>[] _StudioEventValues = new List<string>[] { new List<string>(), new List<string>(), new List<string>() };
			public string Get_StudioEventValues(string type) // returns a comma-separated string of the mouseEvents
			{
				return string.Join(", ", _StudioEventValues[GTINDEX(type)]);
			}
			public void Merge_StudioEventValues(string type, string mouseEvents)
			{
				string[] mEvents = mouseEvents.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string @event in mEvents)
				{
					_StudioEventValues[GTINDEX(type)].Add(@event);
				}
			}

			///////////////////////////////////////////////////////////////////
			// ExternalEvents
			//
			private List<string>[] _ExternalEvents = new List<string>[] { new List<string>(), new List<string>(), new List<string>() };
			public string Get_ExternalEvents(string type) // returns a comma-separated string of the mouseEvents
			{
				return string.Join(", ", _ExternalEvents[GTINDEX(type)]);
			}
			public void Merge_ExternalEvents(string type, string mouseEvents)
			{
				string[] mEvents = mouseEvents.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string @event in mEvents)
				{
					_ExternalEvents[GTINDEX(type)].Add(@event);
				}
			}

			///////////////////////////////////////////////////////////////////
			// ExternalEventValues
			//
			private List<string>[] _ExternalEventValues = new List<string>[] { new List<string>(), new List<string>(), new List<string>() };
			public string Get_ExternalEventValues(string type) // returns a comma-separated string of the mouseEvents
			{
				return string.Join(", ", _ExternalEventValues[GTINDEX(type)]);
			}
			public void Merge_ExternalEventValues(string type, string mouseEvents)
			{
				string[] mEvents = mouseEvents.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string @event in mEvents)
				{
					_ExternalEventValues[GTINDEX(type)].Add(@event);
				}
			}

			///////////////////////////////////////////////////////////////////
			// AOIs
			//

			private Dictionary<string, string> _AOIs = new Dictionary<string, string>();
			public Dictionary<string, string> Get_AOIs()
			{
				return _AOIs;
			}
			public void Merge_AOIs(Dictionary<string, string> hits)
			{
				IEnumerable<string> aoiFields = hits.Keys;
				foreach (string aoi in aoiFields)
				{
					if (!_AOIs.Keys.Contains(aoi))
					{
						_AOIs.Add(aoi, hits[aoi]);
					}
					else
					{
						if (_AOIs[aoi] == "0")
						{
							_AOIs[aoi] = hits[aoi];
						}
					}
				}
			}
		}

		#endregion
	}
}
