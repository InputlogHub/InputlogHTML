
namespace InputLog.Core.Views
{
	/// <summary>
	/// Enumerates the different types of views there are are. Each view subclassing a basic or
	/// compound view has its own viewtype that will be used by the viewFactory and viewManager
	/// to decide which views have already been created and which should still be constructed.
	/// </summary>
	enum ViewType
	{
		BASIC_VIEW,
		COMPOUND_VIEW,
	}
}
