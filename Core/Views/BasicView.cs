using System.Collections.Generic;

namespace InputLog.Core.Views
{
	abstract class BasicView
	{
		// Publicly declared data members
		#region public_data_members

		/// <summary>
		/// The type of the view, this should be overwritten in every 
		/// subclass.
		/// </summary>
		public virtual ViewType TYPE
		{
			get { return ViewType.BASIC_VIEW; }
		}
		#endregion

		// Datamembers inherited by all other views
		#region protected_data_members

		#endregion

		// Datamembers privately owned by this view
		#region private_data_members
		/// <summary>
		/// The mapping of attributes to their values, in the basic 
		/// view.
		/// </summary>
		private IDictionary<string, object> Attributes { get; set; }
		#endregion

		/// <summary>
		/// Construct a basic view. A basic view has a name that describes what
		/// kind of view this is.
		/// </summary>
		protected BasicView()
		{
			Attributes = new Dictionary<string, object>();
		}

		/// <summary>
		/// Sets an attribute with given key and value. If an attribute
		/// with given key already existed, it is overwritten with the new
		/// value.
		/// </summary>
		/// <param name="key">Key of the attribute</param>
		/// <param name="value">Value of the attribute</param>
		public void Set(string key, object value)
		{
			Attributes[key] = value;
		}

		/// <summary>
		/// Try and get an attributes value by its key. If an attribute
		/// with given key does not exist, null is returned.
		/// </summary>
		/// <param name="key">Key of the attribute.</param>
		/// <returns>The value of the attribute, or null if no attribute with
		/// given key exists</returns>
		public object TryAndGet(string key)
		{
			object value;
			Attributes.TryGetValue(key, out value);
			return value;
		}
	}
}
