using System;
using System.Collections.Generic;
using UIKit;

namespace mPassword.iOS
{
	public class PasswordExpiredDurationModelView : UIPickerViewModel
	{
		public event EventHandler<EventArgs> ValueChanged;

		/// <summary>
		/// The items to show up in the picker
		/// </summary>
		public List<string> Items { get; private set; }

		/// <summary>
		/// The current selected item
		/// </summary>
		public string SelectedItem
		{
			get { return Items[selectedIndex]; }
		}

		int selectedIndex;

		public PasswordExpiredDurationModelView()
		{
			Items = new List<string>();
			Items.Add("---Select---");
			Items.Add("10 days");
			Items.Add("20 days");
			Items.Add("30 days");
			Items.Add("60 days");
			Items.Add("90 days");
		}

		/// <summary>
		/// Called by the picker to determine how many rows are in a given spinner item
		/// </summary>
		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return Items.Count;
		}

		/// <summary>
		/// called by the picker to get the text for a particular row in a particular
		/// spinner item
		/// </summary>
		public override string GetTitle(UIPickerView pickerView, nint row, nint component)
		{
			return Items[(int)row];
		}

		/// <summary>
		/// called by the picker to get the number of spinner items
		/// </summary>
		public override nint GetComponentCount(UIPickerView pickerView)
		{
			return 1;
		}

		/// <summary>
		/// called when a row is selected in the spinner
		/// </summary>
		public override void Selected(UIPickerView pickerView, nint row, nint component)
		{
			selectedIndex = (int)row;
			if (ValueChanged != null)
			{
				ValueChanged(this, new EventArgs());
			}
		}
	}
}

