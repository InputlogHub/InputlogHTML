using System;
using System.Collections.Generic;

namespace InputLog.Core.Views
{
	/// <summary>
	/// Class that keeps track of all views that have already been
	/// created for different idfx files. Once a view has been constructed once
	/// it is kept in memory and does not need to be recalculated in 
	/// successive operations. Only if a view has been deleted in between
	/// different operations must it be reconstructed.
	/// 
	/// Views are identified by their IDFX filename, and their view-name. Upon requesting
	/// a view that has not yet been created it will be constructed and then
	/// returned to the user.
	/// </summary>
	static class ViewManager
	{
		#region private_datamembers

		/// <summary>
		/// Dictionary to keep track of each view per idfx file.
		/// </summary>
		private static readonly Dictionary<string, Dictionary<ViewType, BasicView>> Views
			= new Dictionary<string, Dictionary<ViewType, BasicView>>();

		/// <summary>
		/// 'Lock' used to make functions threadsafe
		/// </summary>
		private static readonly object Door = new Object();
		#endregion


		public static void AddIDFX(string idfxPath)
		{
			// Add idfx BasicView to the list of views 
			lock (Door)
			{
				// Construct the basicView of the idfx, that is the idfx event list.
			}
		}

		public static void RemoveIDFX(string idfxPath)
		{
			// Remove the idfx with all its views from the dictionary of views.
			lock (Door)
			{
				if (Views.ContainsKey(idfxPath))
				{
					Views.Remove(idfxPath);
				}
			}
		}

		public static BasicView GetView(string idfxPath, ViewType type)
		{
			//@Joeri: Als je hier de SNotatieView wilt opvragen, moet je dus gewoon even
			// handmatig de XML file inlezen de S-Notatiestring uitlezen en zorgen dat die in een
			// BasicView of CompoundView zit, zodat vandaar het 'snotatie attribuut' opgevraagd kan worden
			// zegt, en dat is dan de string van de S-Notatie.
			// Die wordt dan verder gebruikt voor de Linguistische analyse. 
			// In elk geval, je mag dat hier dus inhacken, je hoeft zelfs geen gebruik te maken van de factory,
			// het mag echt gehacked worden. Zolang het maar een View teruggeeft, mss zelfs het beste dat je dat
			// uitbreid en een SNotationView maakt. Je hoeft in je implementatie zelfs niet de idfx mee te geven he
			// je geeft gewoon de XML mee, en dan kan je die erin hacken om een view terug te geven, 
			// gezien deze methode momenteel voor niets anders gebruikt gaat worden.
			// Tegen de tijd dat we effectief hier met Views gaan werken en analyses op deze 
			// manier gaan opvragen maken we zelf wel echte views. :)

			// Return the requested view of the idfx if it already exists, 
			// or construct the view if it doesn't exist yet.
			lock (Door)
			{
				if (Views.ContainsKey(idfxPath))
				{
					// The view exists already, return it.
					if (Views[idfxPath].ContainsKey(type))
					{
						return Views[idfxPath][type];
					}
				    BasicView tmpView = ViewFactory.Create(idfxPath, type);
				    if (tmpView != null)
				    {
				        Views[idfxPath].Add(type, tmpView);
				        return tmpView;
				    }
				    // Exception... at some point...
				    return null;
				}
			    // We'll write an exception for this at some point...
			    return null;
			}
		}

		public static void RemoveView(string idfxPath, ViewType type)
		{
			// Remove one view from the dictionary of views of an idfx.
			lock (Door)
			{
				if (Views.ContainsKey(idfxPath) && Views[idfxPath].ContainsKey(type))
				{
					Views[idfxPath].Remove(type);
				}
			}
		}
	}
}
