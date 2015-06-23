using System;
using Xamarin.Forms;

namespace XFApp
{
	public class DinnerCell : ViewCell
	{
		public DinnerCell ()
		{
			var lblTitle = new Label () {
                TextColor = Color.Gray,
				HorizontalOptions=LayoutOptions.StartAndExpand					
			};

			lblTitle.SetBinding(Label.TextProperty,"Title");

			var lblAddress = new Label(){
				TextColor = Color.Gray,
				FontAttributes = FontAttributes.Italic,
				HorizontalOptions=LayoutOptions.StartAndExpand					
			};

			lblAddress.SetBinding(Label.TextProperty,"Address");

			View = new StackLayout () {
				Children = { lblTitle, lblAddress },
				Orientation = StackOrientation.Vertical
			};
		}
	}
}

