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
        }

        private async void OnBackPage(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new SenPuzzlePage());
            //await Navigation.PopAsync();

        }  
    }
}
