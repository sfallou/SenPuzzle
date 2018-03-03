using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SenPuzzle
{
    public partial class HelpPage : ContentPage
    {
        public HelpPage()
        {
            InitializeComponent();

            TileView = new ContentView
            {
                Padding = new Thickness(1),

                // Get the bitmap for each tile 
                Content = new Image
                {
                    //Source = ImageSource.FromUri(new Uri(UrlPrefix + "Bitmap" + row + col + ".png"))
                    Source = ImageSource.FromResource("SenPuzzle.Bitmaps.Bitmap.png")
                }
            };
            absoluteLayout.Children.Add(TileView);

        }

        private async void OnBackPage(object sender, EventArgs e)
        {
            //await Navigation.PushModalAsync(new SenPuzzlePage());
            await Navigation.PopModalAsync();

        } 
        public View TileView { private set; get; }
    }
}
