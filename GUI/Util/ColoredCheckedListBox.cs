using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace GUI.Util
{
	/// <summary>
	/// This is a CheckedListBox that can display a set of Special items with a certain color.
	/// </summary>
	public class ColoredCheckedListBox: CheckedListBox
	{
		#region public_fields
		/// <summary>
		/// Color for all 'SpecialItems'.
		/// </summary>
		public Color SpecialColor;

		/// <summary>
		/// Set of all the indices of all the special items that are
		/// to be drawon in the SpecialColor
		/// </summary>
		public ISet<int> SpecialItems { get; private set; }
		#endregion

		public ColoredCheckedListBox(): base()
		{
			SpecialItems = new HashSet<int>();
		}

		/// <summary>
		/// Overriden draw method that doesn't allow highlighting of the selected item since that obscures the item's text color which has desired meaning.  But the 
		/// selected item is still known to the user by the focus rectangle being displayed.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (this.DesignMode)
			{
				base.OnDrawItem(e);
			}
			else
			{
				Color textColor = SpecialItems.Contains(e.Index) ? SpecialColor : Color.Black;

				DrawItemEventArgs e2 = new DrawItemEventArgs
				   (e.Graphics,
					e.Font,
					new Rectangle(e.Bounds.Location, e.Bounds.Size),
					e.Index,
					// Remove 'selected' state so that the base.OnDrawItem doesn't obliterate the work we are doing here.
					(e.State & DrawItemState.Focus) == DrawItemState.Focus ? DrawItemState.Focus : DrawItemState.None, 
					textColor,
					this.BackColor);

				base.OnDrawItem(e2);
			}
		}

		/// <summary>
		/// Changes original behaviour. items that are in the SpecialItem category can not 
		/// have their check-state changed!
		/// </summary>
		/// <param name="ice">Event parameters.</param>
		protected override void OnItemCheck(ItemCheckEventArgs ice)
		{
			if (SpecialItems.Contains(ice.Index))
			{
				ice.NewValue = ice.CurrentValue;
			}

			base.OnItemCheck(ice);
		}

	}
}
