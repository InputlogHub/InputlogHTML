using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.IO.Tobii;
using System.Linq.Expressions;
using System.Reflection;

namespace InputLog.Core.Events.EyeTracking.SubParts
{
	public class EyePositionPart: ISubPart
	{
		//
		// standard tobii information
		//
		public string EyePosLeftX_ADCSmm { private get; set; }
		public string EyePosLeftY_ADCSmm { private get; set; }
		public string EyePosLeftZ_ADCSmm { private get; set; }
		public string EyePosRightX_ADCSmm { private get; set; }
		public string EyePosRightY_ADCSmm { private get; set; }
		public string EyePosRightZ_ADCSmm { private get; set; }
		public string DistanceLeft { private get; set; }
		public string DistanceRight { private get; set; }

		// 
		// calculated fields
		//
		public string EyePosLeftX_ADCSmm_MIN 
		{ 
			get { return InterpretData().EyePosLeftX_ADCSmm_MIN.ToString(); }
			set { InterpretData().EyePosLeftX_ADCSmm_MIN = double.Parse(value); }
		}
		public string EyePosLeftY_ADCSmm_MIN 
		{ 
			get { return InterpretData().EyePosLeftY_ADCSmm_MIN.ToString(); }
			set { InterpretData().EyePosLeftY_ADCSmm_MIN = double.Parse(value); }
		}
		public string EyePosLeftZ_ADCSmm_MIN 
		{ 
			get { return InterpretData().EyePosLeftZ_ADCSmm_MIN.ToString(); }
			set { InterpretData().EyePosLeftZ_ADCSmm_MIN = double.Parse(value); }
		}
		public string EyePosRightX_ADCSmm_MIN 
		{ 
			get { return InterpretData().EyePosRightX_ADCSmm_MIN.ToString(); }
			set { InterpretData().EyePosRightX_ADCSmm_MIN = double.Parse(value); }
		}
		public string EyePosRightY_ADCSmm_MIN 
		{ 
			get { return InterpretData().EyePosRightY_ADCSmm_MIN.ToString(); }
			set { InterpretData().EyePosRightY_ADCSmm_MIN = double.Parse(value); }
		}
		public string EyePosRightZ_ADCSmm_MIN 
		{ 
			get { return InterpretData().EyePosRightZ_ADCSmm_MIN.ToString(); }
			set { InterpretData().EyePosRightZ_ADCSmm_MIN = double.Parse(value); }
		}
		public string DistanceLeft_MIN 
		{ 
			get { return InterpretData().DistanceLeft_MIN.ToString(); }
			set { InterpretData().DistanceLeft_MIN = double.Parse(value); }
		}
		public string DistanceRight_MIN 
		{ 
			get { return InterpretData().DistanceRight_MIN.ToString(); }
			set { InterpretData().DistanceRight_MIN = double.Parse(value); }
		}
		public string EyePosLeftX_ADCSmm_MAX 
		{ 
			get { return InterpretData().EyePosLeftX_ADCSmm_MAX.ToString(); }
			set { InterpretData().EyePosLeftX_ADCSmm_MAX = double.Parse(value); }
		}
		public string EyePosLeftY_ADCSmm_MAX 
		{ 
			get { return InterpretData().EyePosLeftY_ADCSmm_MAX.ToString(); }
			set { InterpretData().EyePosLeftY_ADCSmm_MAX = double.Parse(value); }
		}
		public string EyePosLeftZ_ADCSmm_MAX 
		{ 
			get { return InterpretData().EyePosLeftZ_ADCSmm_MAX.ToString(); }
			set { InterpretData().EyePosLeftZ_ADCSmm_MAX = double.Parse(value); }
		}
		public string EyePosRightX_ADCSmm_MAX 
		{ 
			get { return InterpretData().EyePosRightX_ADCSmm_MAX.ToString(); }
			set { InterpretData().EyePosRightX_ADCSmm_MAX = double.Parse(value); }
		}
		public string EyePosRightY_ADCSmm_MAX 
		{ 
			get { return InterpretData().EyePosRightY_ADCSmm_MAX.ToString(); }
			set { InterpretData().EyePosRightY_ADCSmm_MAX = double.Parse(value); }
		}
		public string EyePosRightZ_ADCSmm_MAX 
		{ 
			get { return InterpretData().EyePosRightZ_ADCSmm_MAX.ToString(); }
			set { InterpretData().EyePosRightZ_ADCSmm_MAX = double.Parse(value); }
		}
		public string DistanceLeft_MAX 
		{ 
			get { return InterpretData().DistanceLeft_MAX.ToString(); }
			set { InterpretData().DistanceLeft_MAX = double.Parse(value); }
		}
		public string DistanceRight_MAX 
		{ 
			get { return InterpretData().DistanceRight_MAX.ToString(); }
			set { InterpretData().DistanceRight_MAX = double.Parse(value); }
		}

		// Helper variable
		private double AverageSampleValidity;

		private Interpret Interpreter;

		// 
		// constructors & standard functions
		//
		public EyePositionPart()
		{
			Interpreter = new Interpret(this);
		}

		public Interpret InterpretData()
		{
			return Interpreter;
		}

		//
		// Implementation of ISubPart interface.
		//
		#region ISubPart Members

		/// <summary>
		/// Initialize the StudioEventPart based on the data array we get
		/// and the indexmap
		/// </summary>
		/// <param name="data">Data array containing the information</param>
		/// <param name="index">Indexmap mapping the different Tobii tags to the right indices 
		/// in the data array.</param>
		public void Initialize(string[] data, Dictionary<string, int> index)
		{
			if (index.ContainsKey(TAGS.ET_EyePosLeftX_ADCSmm) && !String.IsNullOrEmpty(data[index[TAGS.ET_EyePosLeftX_ADCSmm]]) &&
				index.ContainsKey(TAGS.ET_ValidityLeft) && !String.IsNullOrEmpty(data[index[TAGS.ET_ValidityLeft]]))
			{
				AverageSampleValidity = (ulong.Parse(data[index[TAGS.ET_ValidityLeft]]) + ulong.Parse(data[index[TAGS.ET_ValidityRight]])) / 2;

				if (AverageSampleValidity < 2)
				{
					EyePosLeftX_ADCSmm = data[index[TAGS.ET_EyePosLeftX_ADCSmm]];
					EyePosLeftY_ADCSmm = data[index[TAGS.ET_EyePosLeftY_ADCSmm]];
					EyePosLeftZ_ADCSmm = data[index[TAGS.ET_EyePosLeftZ_ADCSmm]];
					EyePosRightX_ADCSmm = data[index[TAGS.ET_EyePosRightX_ADCSmm]];
					EyePosRightY_ADCSmm = data[index[TAGS.ET_EyePosRightY_ADCSmm]];
					EyePosRightZ_ADCSmm = data[index[TAGS.ET_EyePosRightZ_ADCSmm]];
					DistanceLeft = data[index[TAGS.ET_DistanceLeft]];
					DistanceRight = data[index[TAGS.ET_DistanceRight]];
				}
				else
				{
					EyePosLeftX_ADCSmm = "";
					EyePosLeftY_ADCSmm = "";
					EyePosLeftZ_ADCSmm = "";
					EyePosRightX_ADCSmm = "";
					EyePosRightY_ADCSmm = "";
					EyePosRightZ_ADCSmm = "";
					DistanceLeft = "";
					DistanceRight = "";
				}
			}
		}

		public void Merge(ISubPart other)
		{
			EyePositionPart otherPart = other as EyePositionPart;
			if (otherPart == null)
			{
				throw new ArgumentException("EyePositionPart can only be merged with other EyePositionParts. Parameter type was \""
					+ other.GetType().ToString() + "\"", "other");
			}

			Interpreter.Merge(otherPart.InterpretData());

			// First merge, then copy the values.
			if (otherPart.AverageSampleValidity < 2)
			{
				EyePosLeftX_ADCSmm = otherPart.EyePosLeftX_ADCSmm;
				EyePosLeftY_ADCSmm = otherPart.EyePosLeftY_ADCSmm;
				EyePosLeftZ_ADCSmm = otherPart.EyePosLeftZ_ADCSmm;
				EyePosRightX_ADCSmm = otherPart.EyePosRightX_ADCSmm;
				EyePosRightY_ADCSmm = otherPart.EyePosRightY_ADCSmm;
				EyePosRightZ_ADCSmm = otherPart.EyePosRightZ_ADCSmm;
				DistanceLeft = otherPart.DistanceLeft;
				DistanceRight = otherPart.DistanceRight;
			}
			else
			{
				// Do not overwerite this data, it will lead to wrong results.
				/*
				EyePosLeftX_ADCSmm = "";
				EyePosLeftY_ADCSmm = "";
				EyePosLeftZ_ADCSmm = "";
				EyePosRightX_ADCSmm = "";
				EyePosRightY_ADCSmm = "";
				EyePosRightZ_ADCSmm = "";
				DistanceLeft = "";
				DistanceRight = "";
				 */
			}
		}

		public bool ContainsData()
		{
			// if fwe get data, we initialize the interpreter in the above code, so if it is set true, it contains data.
			return !String.IsNullOrEmpty(EyePosLeftX_ADCSmm) || !String.IsNullOrEmpty(DistanceLeft);
		}

		public bool AllowsDataMerge()
		{
			return true;
		}

		#endregion

		public class Interpret
		{
			//
			// Standard tobii fields
			//
			private EyePositionPart Source;
			public double EyePosLeftX_ADCSmm { get { return string.IsNullOrEmpty(Source.EyePosLeftX_ADCSmm) ? 0.0d : double.Parse(Source.EyePosLeftX_ADCSmm); } }
			public double EyePosLeftY_ADCSmm { get { return string.IsNullOrEmpty(Source.EyePosLeftY_ADCSmm) ? 0.0d : double.Parse(Source.EyePosLeftY_ADCSmm); } }
			public double EyePosLeftZ_ADCSmm { get { return string.IsNullOrEmpty(Source.EyePosLeftZ_ADCSmm) ? 0.0d : double.Parse(Source.EyePosLeftZ_ADCSmm); } }
			public double EyePosRightX_ADCSmm { get { return string.IsNullOrEmpty(Source.EyePosRightX_ADCSmm) ? 0.0d : double.Parse(Source.EyePosRightX_ADCSmm); } }
			public double EyePosRightY_ADCSmm { get { return string.IsNullOrEmpty(Source.EyePosRightY_ADCSmm) ? 0.0d : double.Parse(Source.EyePosRightY_ADCSmm); } }
			public double EyePosRightZ_ADCSmm { get { return string.IsNullOrEmpty(Source.EyePosRightZ_ADCSmm) ? 0.0d : double.Parse(Source.EyePosRightZ_ADCSmm); } }
			public double DistanceLeft { get { return string.IsNullOrEmpty(Source.DistanceLeft) ? 0.0d : double.Parse(Source.DistanceLeft); } }
			public double DistanceRight { get { return string.IsNullOrEmpty(Source.DistanceRight) ? 0.0d : double.Parse(Source.DistanceRight); } }

			//
			// Calculated fields
			//
			#region calculated fields

			//
			// Private placeholder variables
			//
			private double? _EyePosLeftX_ADCSmm_MIN = null;
			private double? _EyePosLeftY_ADCSmm_MIN = null;
			private double? _EyePosLeftZ_ADCSmm_MIN = null;
			private double? _EyePosRightX_ADCSmm_MIN = null;
			private double? _EyePosRightY_ADCSmm_MIN = null;
			private double? _EyePosRightZ_ADCSmm_MIN = null;
			private double? _DistanceLeft_MIN = null;
			private double? _DistanceRight_MIN = null;
			private double? _EyePosLeftX_ADCSmm_MAX = null;
			private double? _EyePosLeftY_ADCSmm_MAX = null;
			private double? _EyePosLeftZ_ADCSmm_MAX = null;
			private double? _EyePosRightX_ADCSmm_MAX = null;
			private double? _EyePosRightY_ADCSmm_MAX = null;
			private double? _EyePosRightZ_ADCSmm_MAX = null;
			private double? _DistanceLeft_MAX = null;
			private double? _DistanceRight_MAX = null;

			// 
			// the public properties
			// They have been copy-pasted so that he property usage is more easy to understand, 
			// from outside the class, as opposed to using reflection and the Get/Set property methods. Because
			// in that case the fields would still have to be public, yet, the user should be using them. That is confusing.
			//
			public double EyePosLeftX_ADCSmm_MIN
			{
				set
				{
					_EyePosLeftX_ADCSmm_MIN = value;
				}
				get
				{
					if (!_EyePosLeftX_ADCSmm_MIN.HasValue)
					{
						_EyePosLeftX_ADCSmm_MIN = EyePosLeftX_ADCSmm;
					}
					return _EyePosLeftX_ADCSmm_MIN.Value;
				} 
			}
			public double EyePosLeftY_ADCSmm_MIN 
			{
				set
				{
						_EyePosLeftY_ADCSmm_MIN = value;
				}
				get
				{
					if (!_EyePosLeftY_ADCSmm_MIN.HasValue)
					{
						_EyePosLeftY_ADCSmm_MIN = EyePosLeftY_ADCSmm;
					}
					return _EyePosLeftY_ADCSmm_MIN.Value;
				} 
			}
			public double EyePosLeftZ_ADCSmm_MIN 
			{
				set
				{
					_EyePosLeftZ_ADCSmm_MIN = value;
				}
				get
				{
					if (!_EyePosLeftZ_ADCSmm_MIN.HasValue)
					{
						_EyePosLeftZ_ADCSmm_MIN = EyePosLeftZ_ADCSmm;
					}
					return _EyePosLeftZ_ADCSmm_MIN.Value;
				} 
			}
			public double EyePosRightX_ADCSmm_MIN 
			{
				set
				{
					_EyePosRightX_ADCSmm_MIN = value;
				}
				get
				{
					if (!_EyePosRightX_ADCSmm_MIN.HasValue)
					{
						_EyePosRightX_ADCSmm_MIN = EyePosRightX_ADCSmm;
					}
					return _EyePosRightX_ADCSmm_MIN.Value;
				} 
			}
			public double EyePosRightY_ADCSmm_MIN
			{
				set
				{
					_EyePosRightY_ADCSmm_MIN = value;
				}
				get
				{
					if (!_EyePosRightY_ADCSmm_MIN.HasValue)
					{
						_EyePosRightY_ADCSmm_MIN = EyePosRightY_ADCSmm;
					}
					return _EyePosRightY_ADCSmm_MIN.Value;
				} 
			}
			public double EyePosRightZ_ADCSmm_MIN
			{
				set
				{
					_EyePosRightZ_ADCSmm_MIN = value;
				}
				get
				{
					if (!_EyePosRightZ_ADCSmm_MIN.HasValue)
					{
						_EyePosRightZ_ADCSmm_MIN = EyePosRightZ_ADCSmm;
					}
					return _EyePosRightZ_ADCSmm_MIN.Value;
				} 
			}
			public double DistanceLeft_MIN
			{
				set
				{
					_DistanceLeft_MIN = value;
				}
				get
				{
					if (!_DistanceLeft_MIN.HasValue)
					{
						_DistanceLeft_MIN = DistanceLeft;
					}
					return _DistanceLeft_MIN.Value;
				} 
			}
			public double DistanceRight_MIN
			{
				set
				{
					_DistanceRight_MIN = value;
				}
				get
				{
					if (!_DistanceRight_MIN.HasValue)
					{
						_DistanceRight_MIN = DistanceRight;
					}
					return _DistanceRight_MIN.Value;
				} 
			}
			public double EyePosLeftX_ADCSmm_MAX
			{
				set
				{
					_EyePosLeftX_ADCSmm_MAX = value;
				}
				get
				{
					if (!_EyePosLeftX_ADCSmm_MAX.HasValue)
					{
						_EyePosLeftX_ADCSmm_MAX = EyePosLeftX_ADCSmm;
					}
					return _EyePosLeftX_ADCSmm_MAX.Value;
				} 
			}
			public double EyePosLeftY_ADCSmm_MAX
			{
				set
				{
					_EyePosLeftY_ADCSmm_MAX = value;
				}
				get
				{
					if (!_EyePosLeftY_ADCSmm_MAX.HasValue)
					{
						_EyePosLeftY_ADCSmm_MAX = EyePosLeftY_ADCSmm;
					}
					return _EyePosLeftY_ADCSmm_MAX.Value;
				} 
			}
			public double EyePosLeftZ_ADCSmm_MAX
			{
				set
				{
					_EyePosLeftZ_ADCSmm_MAX = value;
				}
				get
				{
					if (!_EyePosLeftZ_ADCSmm_MAX.HasValue)
					{
						_EyePosLeftZ_ADCSmm_MAX = EyePosLeftZ_ADCSmm;
					}
					return _EyePosLeftZ_ADCSmm_MAX.Value;
				} 
			}
			public double EyePosRightX_ADCSmm_MAX
			{
				set
				{
					_EyePosRightX_ADCSmm_MAX = value;
				}
				get
				{
					if (!_EyePosRightX_ADCSmm_MAX.HasValue)
					{
						_EyePosRightX_ADCSmm_MAX = EyePosRightX_ADCSmm;
					}
					return _EyePosRightX_ADCSmm_MAX.Value;
				} 
			}
			public double EyePosRightY_ADCSmm_MAX
			{
				set
				{
					_EyePosRightY_ADCSmm_MAX = value;
				}
				get
				{
					if (!_EyePosRightY_ADCSmm_MAX.HasValue)
					{
						_EyePosRightY_ADCSmm_MAX = EyePosRightY_ADCSmm;
					}
					return _EyePosRightY_ADCSmm_MAX.Value;
				} 
			}
			public double EyePosRightZ_ADCSmm_MAX
			{
				set
				{
					_EyePosRightZ_ADCSmm_MAX = value;
				}
				get
				{
					if (!_EyePosRightZ_ADCSmm_MAX.HasValue)
					{
						_EyePosRightZ_ADCSmm_MAX = EyePosRightZ_ADCSmm;
					}
					return _EyePosRightZ_ADCSmm_MAX.Value;
				} 
			}
			public double DistanceLeft_MAX
			{
				set
				{
					_DistanceLeft_MAX = value;
				}
				get
				{
					if (!_DistanceLeft_MAX.HasValue)
					{
						_DistanceLeft_MAX = DistanceLeft;
					}
					return _DistanceLeft_MAX.Value;
				} 
			}
			public double DistanceRight_MAX
			{
				set
				{
					_DistanceRight_MAX = value;
				}
				get
				{
					if (!_DistanceRight_MAX.HasValue)
					{
						_DistanceRight_MAX = DistanceRight;
					}
					return _DistanceRight_MAX.Value;
				} 
			}

			#endregion

			public Interpret(EyePositionPart source)
			{
				Source = source;
			}

			/// <summary>
			/// Merge the interpeter parts.
			/// </summary>
			/// <param name="other">The other's interpreters part.</param>
			internal void Merge(Interpret other)
			{
				if (other.Source.AverageSampleValidity < 2)
				{
					DistanceLeft_MAX = Math.Max(DistanceLeft_MAX, other.DistanceLeft_MAX);
					DistanceLeft_MIN = Math.Min(DistanceLeft_MIN, other.DistanceLeft_MIN);
					EyePosLeftX_ADCSmm_MAX = Math.Max(EyePosLeftX_ADCSmm_MAX, other.EyePosLeftX_ADCSmm_MAX);
					EyePosLeftX_ADCSmm_MIN = Math.Min(EyePosLeftX_ADCSmm_MIN, other.EyePosLeftX_ADCSmm_MIN);
					EyePosLeftY_ADCSmm_MAX = Math.Max(EyePosLeftY_ADCSmm_MAX, other.EyePosLeftY_ADCSmm_MAX);
					EyePosLeftY_ADCSmm_MIN = Math.Min(EyePosLeftY_ADCSmm_MIN, other.EyePosLeftY_ADCSmm_MIN);
					EyePosLeftZ_ADCSmm_MAX = Math.Max(EyePosLeftZ_ADCSmm_MAX, other.EyePosLeftZ_ADCSmm_MAX);
					EyePosLeftZ_ADCSmm_MIN = Math.Min(EyePosLeftZ_ADCSmm_MIN, other.EyePosLeftZ_ADCSmm_MIN);
					EyePosRightX_ADCSmm_MAX = Math.Max(EyePosRightX_ADCSmm_MAX, other.EyePosRightX_ADCSmm_MAX);
					EyePosRightX_ADCSmm_MIN = Math.Min(EyePosRightX_ADCSmm_MIN, other.EyePosRightX_ADCSmm_MIN);
					EyePosRightY_ADCSmm_MAX = Math.Max(EyePosRightY_ADCSmm_MAX, other.EyePosRightY_ADCSmm_MAX);
					EyePosRightY_ADCSmm_MIN = Math.Min(EyePosRightY_ADCSmm_MIN, other.EyePosRightY_ADCSmm_MIN);
					EyePosRightZ_ADCSmm_MAX = Math.Max(EyePosRightZ_ADCSmm_MAX, other.EyePosRightZ_ADCSmm_MAX);
					EyePosRightZ_ADCSmm_MIN = Math.Min(EyePosRightZ_ADCSmm_MIN, other.EyePosRightZ_ADCSmm_MIN);
				}
			}

			//public delegate double MathFunction(double a, double b);

			/*
			/// <summary>
			/// Set the property of the 
			/// </summary>
			/// <param name="field"></param>
			/// <param name="value"></param>
			/// <param name="func"></param>
			public void SetProperty(Expression<Func<double?>> field, double value, MathFunction func)
			{
				 
				 // Place the field info check only in the debug mode. Because this is less
				 // efficient. Plus, if we correctly pass through this at debug time, the code is 
				 // not changing, so then we do not have to waste precious CPU cycles form a 
				 // 'many-times-called'-method at runtime. 
				var fieldInfo = (field.Body as MemberExpression).Member as FieldInfo;

				#if DEBUG
				if (fieldInfo == null ||
					this.GetType().GetField(fieldInfo.Name) == null)
				{
					throw new ArgumentException("The lambda expression 'field' must reference a valid field on this Interpeter");
				}	
				#endif

				double? fieldValue = ((double?)fieldInfo.GetValue(this));
				if (fieldValue.HasValue)
				{
					double newValue = func.Invoke(fieldValue.Value, value);
					fieldInfo.SetValue(this, newValue);
				}
				else
				{
					fieldInfo.SetValue(this, value);
				}
			}

			public double GetProperty(Expression<Func<double?>> field)
			{
				/// 
				 // Place the field info check only in the debug mode. Because this is less
				 // efficient. Plus, if we correctly pass through this at debug time, the code is 
				 // not changing, so then we do not have to waste precious CPU cycles form a 
				 // 'many-times-called'-method at runtime.
				 ///
				var fieldInfo = (field.Body as MemberExpression).Member as FieldInfo;

				#if DEBUG
				if (fieldInfo == null ||
					this.GetType().GetField(fieldInfo.Name) == null)
				{
					throw new ArgumentException("The lambda expression 'field' must reference a valid field on this Interpeter");
				}
				#endif

				double? fieldValue = ((double?)fieldInfo.GetValue(this));
				if (!fieldValue.HasValue)
				{
					string sourceFieldName = fieldInfo.Name.Substring(0, fieldInfo.Name.LastIndexOf('_'));
					PropertyInfo sourcePropertyInfo = Source.GetType().GetProperty(sourceFieldName);
					#if DEBUG
					if (sourcePropertyInfo == null)
					{
						throw new ArgumentException("The source eyeposition does not contain a field: \"" + sourceFieldName + "\"");
					}
					#endif

					string stringValue = (string)sourcePropertyInfo.GetValue(Source,null);
					double newValue = double.Parse(stringValue);
					fieldInfo.SetValue(this, newValue);
				}
				return ((double?)fieldInfo.GetValue(this)).Value;
			}*/

		}
	}
}
