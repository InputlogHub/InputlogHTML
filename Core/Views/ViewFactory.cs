
namespace InputLog.Core.Views
{
	/// <summary>
	/// The viewfactory creates views on demand, based on the type of view that is required.
	/// </summary>
	class ViewFactory
	{
		/// <summary>
		/// Create the requested view.
		/// </summary>
		/// <param name="idfx">filename of the idfx the view is based on</param>
		/// <param name="type">viewtype requested for the given idfx</param>
		/// <returns>The requested viewtype if it could be constructed, null if it could
		/// not be constructed, the type was not recognized or the idfx has not yet
		/// been added to the ViewManager</returns>
		public static BasicView Create(string idfx, ViewType type)
		{
			switch (type)
			{
				case ViewType.BASIC_VIEW:
					break;

				case ViewType.COMPOUND_VIEW:
					break;
			}

			return null;
		}
	}
}
