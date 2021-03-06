﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.Forms;


namespace SenPuzzle
{
    public partial class SenPuzzlePage : ContentPage
    {
        // Number of tiles horizontally and vertically,
        //  fixed based on available bitmaps
        static readonly int NUM = 4;
        public int compt = 0;
        public bool flag = true;
        public bool flag_game_over = false;

        // Array of tiles
        Tile[,] tiles = new Tile[NUM, NUM];

        // Empty row and column
        int emptyRow = 0;
        int emptyCol = 3;

        double tileSize;
        bool isBusy;

        public SenPuzzlePage()
        {
            InitializeComponent();
            // Loop through the rows and columns.
            for (int row = 0; row < NUM; row++)
            {
                for (int col = 0; col < NUM; col++)
                {
                    // But skip the last one!
                    if (row == 0 && col == 3)
                        break;

                    // Create the tile
                    Tile tile = new Tile(row, col);

                    // Add tap recognition.
                    TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += OnTileTapped;
                    tile.TileView.GestureRecognizers.Add(tapGestureRecognizer);

                    // Add the tile to the array and the AbsoluteLayout.
                    tiles[row, col] = tile;
                    absoluteLayout.Children.Add(tile.TileView);
                }
            }
        }

        void OnContainerSizeChanged(object sender, EventArgs args)
        {
            View container = (View)sender;
            double width = container.Width;
            double height = container.Height;

            if (width <= 0 || height <= 0)
                return;

            // Orient StackLayout based on portrait/landscape mode.
            stackLayout.Orientation = (width < height) ? StackOrientation.Vertical :
                                                         StackOrientation.Horizontal;

            // Calculate tile size and position based on ContentView size.
            tileSize = Math.Min(width, height) / NUM;
            absoluteLayout.WidthRequest = NUM * tileSize;
            absoluteLayout.HeightRequest = NUM * tileSize;

            foreach (View fileView in absoluteLayout.Children)
            {
                Tile tile = Tile.Dictionary[fileView];

                // Set tile bounds.
                AbsoluteLayout.SetLayoutBounds(fileView, new Rectangle(tile.Col * tileSize,
                                                                       tile.Row * tileSize,
                                                                       tileSize,
                                                                       tileSize));
            }
        }

        async void OnTileTapped(object sender, EventArgs args)
        {
            if (isBusy)
                return;

            isBusy = true;

            View tileView = (View)sender;
            Tile tappedTile = Tile.Dictionary[tileView];
            await ShiftIntoEmpty(tappedTile.Row, tappedTile.Col, tappedTile.MyImage);
            isBusy = false;
        }

        async Task ShiftIntoEmpty(int tappedRow, int tappedCol, String tappedImg, uint length = 100)
        {
            // Shift columns.
            if (tappedRow == emptyRow && tappedCol != emptyCol)
            {
                int inc = Math.Sign(tappedCol - emptyCol);
                int begCol = emptyCol + inc;
                int endCol = tappedCol + inc;

                for (int col = begCol; col != endCol; col += inc)
                {
                    await AnimateTile(emptyRow, col, emptyRow, emptyCol, length);
                }

            }
            // Shift rows.
            else if (tappedCol == emptyCol && tappedRow != emptyRow)
            {
                int inc = Math.Sign(tappedRow - emptyRow);
                int begRow = emptyRow + inc;
                int endRow = tappedRow + inc;

                for (int row = begRow; row != endRow; row += inc)
                {
                    await AnimateTile(row, emptyCol, emptyRow, emptyCol, length);
                }
            }
        }

        async Task AnimateTile(int row, int col, int newRow, int newCol, uint length)
        {
            // The tile to be animated.
            Tile tile = tiles[row, col];
            View tileView = tile.TileView;

            // The destination rectangle.
            Rectangle rect = new Rectangle(emptyCol * tileSize,
                                           emptyRow * tileSize,
                                           tileSize,
                                           tileSize);

            // Animate it!
            await tileView.LayoutTo(rect, length);

            // Set layout bounds to same Rectangle.
            AbsoluteLayout.SetLayoutBounds(tileView, rect);

            // Set several variables and properties for new layout.
            tiles[newRow, newCol] = tile;
            tile.Row = newRow;
            tile.Col = newCol;

            if(flag_game_over){
                if (CheckGame() == 1)
                {
                    GameOver();
                }
            }
           
            tiles[row, col] = null;
            emptyRow = row;
            emptyCol = col;

                
        }
        int checkRow(int numRow){
            String[] imgPos = new String[4];
            for (int i = 0; i < 4; i++)
            {
                imgPos[i] = tiles[numRow, i].MyImage;
            }
            if (
                imgPos[0].Equals("SenPuzzle.Bitmaps.Bitmap" + numRow + "0.png") &&
                imgPos[1].Equals("SenPuzzle.Bitmaps.Bitmap" + numRow + "1.png") &&
                imgPos[2].Equals("SenPuzzle.Bitmaps.Bitmap" + numRow + "2.png") &&
                imgPos[3].Equals("SenPuzzle.Bitmaps.Bitmap" + numRow + "3.png")
            )
            {
                return 1;
            }
            return 0;
        }
        int checkFirstRow()
        {
            String[] imgPos = new String[2];
            imgPos[0] = tiles[0, 1].MyImage;
            imgPos[1] = tiles[0, 2].MyImage;

            if (
                imgPos[0].Equals("SenPuzzle.Bitmaps.Bitmap01.png") &&
                imgPos[1].Equals("SenPuzzle.Bitmaps.Bitmap02.png")
            )
            {
                return 1;
            }
            return 0;
        }
        int CheckGame(){
            int val3 = checkRow(3);
            int val2 = checkRow(2);
            int val1 = checkRow(1);
            int val0 = checkFirstRow();
            return val2*val3*val1*val0;
        }

        void GameOver(){
            flag = false;
            isBusy = true;
            btnStart.Opacity = 0;
            DisplayAlert("Félicitation", "Game Over ! Vous avez réussi.\n Durée :"+chrono.Text, "OK");
            //chrono.Text += "  Félicitation !";
            flag = false;
            isBusy = true;
        }

        async void OnRandomizeButtonClicked(object sender, EventArgs args)
        {
            Button button = (Button)sender;
            button.IsEnabled = false;
            Random rand = new Random();

            isBusy = true;

            // Simulate some fast crazy taps.
            for (int i = 0; i < 100; i++)
            {
                await ShiftIntoEmpty(rand.Next(NUM), emptyCol,null,25);
                await ShiftIntoEmpty(emptyRow, rand.Next(NUM), null, 25);
            }
           
            button.Text = "Pause";
            button.Clicked -= OnRandomizeButtonClicked;
            button.Clicked += PauseButtonClicked;
            Device.StartTimer(TimeSpan.FromSeconds(1), update_data); 

            button.IsEnabled = true;
            btnNew.Opacity = 1;
            btnNew.Clicked += RestartButtonClicked;
            btnHelp.Opacity = 1;
            btnHelp.Clicked += HelpButtonClicked;
            chrono.Opacity = 1;

            flag_game_over = true;
            isBusy = false;

        }


        // Fonction pour faire la pause
         void PauseButtonClicked(object sender, EventArgs args)
        {
            flag = false;
            isBusy = true;
            btnStart.Clicked -= PauseButtonClicked;
            btnStart.Text = "Reprendre";
            btnStart.Clicked += ResumeButtonClicked;

            //var res = DisplayAlert("Pause", "Pour reprendre la partie cliquer sur Reprendre", "Reprendre");
            //var res = await DisplayAlert ("Dialog Title", "Prompt", "Ok", "Cancel"); 
            //if(res)
            //{
             //   flag = true;
            //}            
        }

        void ResumeButtonClicked(object sender, EventArgs args)
        {
            flag = true;
            isBusy = false;
            btnStart.Clicked -= ResumeButtonClicked;
            btnStart.Text = "Pause";
            btnStart.Clicked += PauseButtonClicked;
            Device.StartTimer(TimeSpan.FromSeconds(1), update_data); 

        }

        // Fonction pour redémarrer la partie
         void RestartButtonClicked(object sender, EventArgs args)
        {
            Navigation.PopModalAsync();
            Navigation.PushModalAsync(new SenPuzzlePage());

        }

        async void HelpButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new HelpPage());

        }
        private async Task WaitAndExecute(int milisec, Action actionToExecute)
        {
            await Task.Delay(milisec);
            actionToExecute();
        }
        public bool update_data()
                {
            compt += 1;
            TimeSpan result = TimeSpan.FromSeconds(compt);//TimeSpan.FromHours(compt);
            string duration = result.ToString("hh':'mm':'ss");
            chrono.Text = duration;
                    //Code to run frequently
                    return flag;
                }
    }
}
