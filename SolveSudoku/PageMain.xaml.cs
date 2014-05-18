using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SolveSudoku
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class PageMain : SolveSudoku.Common.LayoutAwarePage
    {
        int[,] grid = new int[9, 9];
        int temp = 0;

        TextBox[] textBoxes; // = new TextBox[] { textBox_11, textBox_12 };

        #region Setup Page

        public PageMain()
        {
            this.InitializeComponent();

            textBoxes = new TextBox[] {textBox_11,
                                       textBox_12,
                                       textBox_13,
                                       textBox_14,
                                       textBox_15,
                                       textBox_16,
                                       textBox_17,
                                       textBox_18,
                                       textBox_19,
                                       textBox_21,
                                       textBox_22,
                                       textBox_23,
                                       textBox_24,
                                       textBox_25,
                                       textBox_26,
                                       textBox_27,
                                       textBox_28,
                                       textBox_29,
                                       textBox_31,
                                       textBox_32,
                                       textBox_33,
                                       textBox_34,
                                       textBox_35,
                                       textBox_36,
                                       textBox_37,
                                       textBox_38,
                                       textBox_39,
                                       textBox_41,
                                       textBox_42,
                                       textBox_43,
                                       textBox_44,
                                       textBox_45,
                                       textBox_46,
                                       textBox_47,
                                       textBox_48,
                                       textBox_49,
                                       textBox_51,
                                       textBox_52,
                                       textBox_53,
                                       textBox_54,
                                       textBox_55,
                                       textBox_56,
                                       textBox_57,
                                       textBox_58,
                                       textBox_59,
                                       textBox_61,
                                       textBox_62,
                                       textBox_63,
                                       textBox_64,
                                       textBox_65,
                                       textBox_66,
                                       textBox_67,
                                       textBox_68,
                                       textBox_69,
                                       textBox_71,
                                       textBox_72,
                                       textBox_73,
                                       textBox_74,
                                       textBox_75,
                                       textBox_76,
                                       textBox_77,
                                       textBox_78,
                                       textBox_79,
                                       textBox_81,
                                       textBox_82,
                                       textBox_83,
                                       textBox_84,
                                       textBox_85,
                                       textBox_86,
                                       textBox_87,
                                       textBox_88,
                                       textBox_89,
                                       textBox_91,
                                       textBox_92,
                                       textBox_93,
                                       textBox_94,
                                       textBox_95,
                                       textBox_96,
                                       textBox_97,
                                       textBox_98,
                                       textBox_99
            };

            // Register for the window resize event
            Window.Current.SizeChanged += WindowSizeChanged; 
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void WindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (Windows.UI.ViewManagement.ApplicationView.Value.ToString() == "Snapped") // If window is snapped to side.
            {
                foreach (TextBox tbIter in textBoxes)
                {
                    tbIter.Style = pageRoot.Resources["TextBoxStyleSnapped"] as Style;
                }

                var brush = new ImageBrush();
                brush.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:/Assets/background-clouds-320w.jpg"));
                rootFrame.Background = brush;

                //rootFrame.Background = (new ImageBrush()).imagesource("Assets/background-clouds-320w.jpg");
            }
            else // If window is not snapped to side.
            {
                foreach (TextBox tbIter in textBoxes)
                {
                    tbIter.Style = pageRoot.Resources["TextBoxStyle"] as Style;
                }

                var brush = new ImageBrush();
                brush.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:/Assets/background-clouds.jpg"));
                rootFrame.Background = brush;
                //rootFrame.Background  = imagesource("Assets/background-clouds.jpg");
            }
        }

        #endregion

        #region Button Events

        private void button_SolveSudoku_Click(object sender, RoutedEventArgs e)
        {
            readAPuzzle(grid);

            if (!isValid(grid))
            {
                button_SolveSudoku.Content = "Invalid";
            }
            else if (search(grid))
            {
                button_SolveSudoku.Content = "Solved";
                printGrid(grid);
            }
            else
            {
                button_SolveSudoku.Content = "No Solution";
            }
        }

        private void button_ClearBoard_Click(object sender, RoutedEventArgs e)
        {
            clearBoard();
            button_SolveSudoku.Content = "Solve";
        }

        #endregion

        #region Solve Sudoku Algorithms

        void readAPuzzle(int[,] grid)
        {
            int i = 0;
            foreach (TextBox tbIter in textBoxes)
            {
                int.TryParse(tbIter.Text, out temp);
                grid[i / 9, i % 9] = temp;

                i++;
            }
        }

        int getFreeCellList(int[,] grid, int[,] freeCellList)
        {
            int numberOfFreeCells = 0;

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (grid[i, j] == 0)
                    {
                        freeCellList[numberOfFreeCells, 0] = i;
                        freeCellList[numberOfFreeCells, 1] = j;
                        numberOfFreeCells++;
                    }
            return numberOfFreeCells;
        }

        void printGrid(int[,] grid)
        {
            int i = 0;
            foreach (TextBox tbIter in textBoxes)
            {
                tbIter.Text = grid[i / 9, i % 9].ToString();

                i++;
            }
        }

        bool search(int[,] grid)
        {
            int[,] freeCellList = new int[81, 2];
            int numberOfFreeCells = getFreeCellList(grid, freeCellList);

            if (numberOfFreeCells == 0)
                return true;

            int k = 0;
            while (true)
            {
                int i = freeCellList[k, 0];
                int j = freeCellList[k, 1];
                if (grid[i, j] == 0)
                    grid[i, j] = 1;

                if (isValid(i, j, grid))
                {
                    if (k + 1 == numberOfFreeCells)
                    {
                        return true;
                    }
                    else
                    {
                        k++;
                    }
                }
                else if (grid[i, j] < 9)
                {
                    grid[i, j] = grid[i, j] + 1;
                }
                else
                {
                    while (grid[i, j] == 9)
                    {
                        if (k == 0)
                        {
                            return false;
                        }
                        grid[i, j] = 0;
                        k--;
                        i = freeCellList[k, 0];
                        j = freeCellList[k, 1];
                    }

                    grid[i, j] = grid[i, j] + 1;
                }
            }

            return true;
        }

        bool isValid(int i, int j, int[,] grid)
        {
            for (int column = 0; column < 9; column++)
                if (column != j && grid[i, column] == grid[i, j])
                    return false;

            for (int row = 0; row < 9; row++)
                if (row != i && grid[row, j] == grid[i, j])
                    return false;

            for (int row = (i / 3) * 3; row < (i / 3) * 3 + 3; row++)
                for (int col = (j / 3) * 3; col < (j / 3) * 3 + 3; col++)
                    if (row != i && col != j && grid[row, col] == grid[i, j])
                        return false;

            return true;
        }

        bool isValid(int[,] grid)
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (grid[i, j] < 0 || grid[i, j] > 9 || (grid[i, j] != 0 && !isValid(i, j, grid)))
                        return false;
            return true;
        }

#endregion



        private void AdvanceFocus(object sender, KeyRoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                int currentTabIndex = tb.TabIndex;
                int nextTabFocus = currentTabIndex;

                if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Down || e.Key == Windows.System.VirtualKey.PageDown)
                {
                    nextTabFocus = (currentTabIndex + 9) % 81;
                    if (nextTabFocus == 0) { nextTabFocus = 81; }
                }
                else if (e.Key == Windows.System.VirtualKey.Tab || e.Key == Windows.System.VirtualKey.Right || e.Key == Windows.System.VirtualKey.End)
                {
                    nextTabFocus = currentTabIndex + 1;
                    if (nextTabFocus > 81) { nextTabFocus = 1; }
                }
                else if (e.Key == Windows.System.VirtualKey.Left || e.Key == Windows.System.VirtualKey.Home)
                {
                    nextTabFocus = currentTabIndex - 1;
                    if (nextTabFocus < 1) { nextTabFocus = 81; }
                }
                else if (e.Key == Windows.System.VirtualKey.Up || e.Key == Windows.System.VirtualKey.PageUp)
                {
                    nextTabFocus = (currentTabIndex - 9 + 81) % 81;
                    if (nextTabFocus == 0) { nextTabFocus = 81; }
                }
                else if (e.Key == Windows.System.VirtualKey.Number0 || e.Key == Windows.System.VirtualKey.Number1 || e.Key == Windows.System.VirtualKey.Number2 || e.Key == Windows.System.VirtualKey.Number3 || e.Key == Windows.System.VirtualKey.Number4 || e.Key == Windows.System.VirtualKey.Number5 || e.Key == Windows.System.VirtualKey.Number6 || e.Key == Windows.System.VirtualKey.Number7 || e.Key == Windows.System.VirtualKey.Number8 || e.Key == Windows.System.VirtualKey.Number9
                     || e.Key == Windows.System.VirtualKey.NumberPad0 || e.Key == Windows.System.VirtualKey.NumberPad1 || e.Key == Windows.System.VirtualKey.NumberPad2 || e.Key == Windows.System.VirtualKey.NumberPad3 || e.Key == Windows.System.VirtualKey.NumberPad4 || e.Key == Windows.System.VirtualKey.NumberPad5 || e.Key == Windows.System.VirtualKey.NumberPad6 || e.Key == Windows.System.VirtualKey.NumberPad7 || e.Key == Windows.System.VirtualKey.NumberPad8 || e.Key == Windows.System.VirtualKey.NumberPad9)
                {
                    if (!String.IsNullOrEmpty(tb.Text))
                    {
                        nextTabFocus = currentTabIndex + 1;
                        if (nextTabFocus > 81) { nextTabFocus = 1; }
                    }
                }

                foreach (TextBox tbIter in textBoxes)
                {
                    if (tbIter.TabIndex == nextTabFocus)
                    {
                        tbIter.Focus(FocusState.Programmatic);
                        break;
                    }
                }

                //e.Handled = true;
            }
        }

        private void clearBoard()
        {
            foreach (TextBox tbIter in textBoxes)
            {
                tbIter.Text = "";
            }
        }


        #region TextBox Callbacks

        private void textBox_11_GotFocus(object sender, RoutedEventArgs e) { textBox_11.SelectAll(); }
        private void textBox_12_GotFocus(object sender, RoutedEventArgs e) { textBox_12.SelectAll(); }
        private void textBox_13_GotFocus(object sender, RoutedEventArgs e) { textBox_13.SelectAll(); }
        private void textBox_14_GotFocus(object sender, RoutedEventArgs e) { textBox_14.SelectAll(); }
        private void textBox_15_GotFocus(object sender, RoutedEventArgs e) { textBox_15.SelectAll(); }
        private void textBox_16_GotFocus(object sender, RoutedEventArgs e) { textBox_16.SelectAll(); }
        private void textBox_17_GotFocus(object sender, RoutedEventArgs e) { textBox_17.SelectAll(); }
        private void textBox_18_GotFocus(object sender, RoutedEventArgs e) { textBox_18.SelectAll(); }
        private void textBox_19_GotFocus(object sender, RoutedEventArgs e) { textBox_19.SelectAll(); }

        private void textBox_21_GotFocus(object sender, RoutedEventArgs e) { textBox_21.SelectAll(); }
        private void textBox_22_GotFocus(object sender, RoutedEventArgs e) { textBox_22.SelectAll(); }
        private void textBox_23_GotFocus(object sender, RoutedEventArgs e) { textBox_23.SelectAll(); }
        private void textBox_24_GotFocus(object sender, RoutedEventArgs e) { textBox_24.SelectAll(); }
        private void textBox_25_GotFocus(object sender, RoutedEventArgs e) { textBox_25.SelectAll(); }
        private void textBox_26_GotFocus(object sender, RoutedEventArgs e) { textBox_26.SelectAll(); }
        private void textBox_27_GotFocus(object sender, RoutedEventArgs e) { textBox_27.SelectAll(); }
        private void textBox_28_GotFocus(object sender, RoutedEventArgs e) { textBox_28.SelectAll(); }
        private void textBox_29_GotFocus(object sender, RoutedEventArgs e) { textBox_29.SelectAll(); }

        private void textBox_31_GotFocus(object sender, RoutedEventArgs e) { textBox_31.SelectAll(); }
        private void textBox_32_GotFocus(object sender, RoutedEventArgs e) { textBox_32.SelectAll(); }
        private void textBox_33_GotFocus(object sender, RoutedEventArgs e) { textBox_33.SelectAll(); }
        private void textBox_34_GotFocus(object sender, RoutedEventArgs e) { textBox_34.SelectAll(); }
        private void textBox_35_GotFocus(object sender, RoutedEventArgs e) { textBox_35.SelectAll(); }
        private void textBox_36_GotFocus(object sender, RoutedEventArgs e) { textBox_36.SelectAll(); }
        private void textBox_37_GotFocus(object sender, RoutedEventArgs e) { textBox_37.SelectAll(); }
        private void textBox_38_GotFocus(object sender, RoutedEventArgs e) { textBox_38.SelectAll(); }
        private void textBox_39_GotFocus(object sender, RoutedEventArgs e) { textBox_39.SelectAll(); }

        private void textBox_41_GotFocus(object sender, RoutedEventArgs e) { textBox_41.SelectAll(); }
        private void textBox_42_GotFocus(object sender, RoutedEventArgs e) { textBox_42.SelectAll(); }
        private void textBox_43_GotFocus(object sender, RoutedEventArgs e) { textBox_43.SelectAll(); }
        private void textBox_44_GotFocus(object sender, RoutedEventArgs e) { textBox_44.SelectAll(); }
        private void textBox_45_GotFocus(object sender, RoutedEventArgs e) { textBox_45.SelectAll(); }
        private void textBox_46_GotFocus(object sender, RoutedEventArgs e) { textBox_46.SelectAll(); }
        private void textBox_47_GotFocus(object sender, RoutedEventArgs e) { textBox_47.SelectAll(); }
        private void textBox_48_GotFocus(object sender, RoutedEventArgs e) { textBox_48.SelectAll(); }
        private void textBox_49_GotFocus(object sender, RoutedEventArgs e) { textBox_49.SelectAll(); }

        private void textBox_51_GotFocus(object sender, RoutedEventArgs e) { textBox_51.SelectAll(); }
        private void textBox_52_GotFocus(object sender, RoutedEventArgs e) { textBox_52.SelectAll(); }
        private void textBox_53_GotFocus(object sender, RoutedEventArgs e) { textBox_53.SelectAll(); }
        private void textBox_54_GotFocus(object sender, RoutedEventArgs e) { textBox_54.SelectAll(); }
        private void textBox_55_GotFocus(object sender, RoutedEventArgs e) { textBox_55.SelectAll(); }
        private void textBox_56_GotFocus(object sender, RoutedEventArgs e) { textBox_56.SelectAll(); }
        private void textBox_57_GotFocus(object sender, RoutedEventArgs e) { textBox_57.SelectAll(); }
        private void textBox_58_GotFocus(object sender, RoutedEventArgs e) { textBox_58.SelectAll(); }
        private void textBox_59_GotFocus(object sender, RoutedEventArgs e) { textBox_59.SelectAll(); }

        private void textBox_61_GotFocus(object sender, RoutedEventArgs e) { textBox_61.SelectAll(); }
        private void textBox_62_GotFocus(object sender, RoutedEventArgs e) { textBox_62.SelectAll(); }
        private void textBox_63_GotFocus(object sender, RoutedEventArgs e) { textBox_63.SelectAll(); }
        private void textBox_64_GotFocus(object sender, RoutedEventArgs e) { textBox_64.SelectAll(); }
        private void textBox_65_GotFocus(object sender, RoutedEventArgs e) { textBox_65.SelectAll(); }
        private void textBox_66_GotFocus(object sender, RoutedEventArgs e) { textBox_66.SelectAll(); }
        private void textBox_67_GotFocus(object sender, RoutedEventArgs e) { textBox_67.SelectAll(); }
        private void textBox_68_GotFocus(object sender, RoutedEventArgs e) { textBox_68.SelectAll(); }
        private void textBox_69_GotFocus(object sender, RoutedEventArgs e) { textBox_69.SelectAll(); }

        private void textBox_71_GotFocus(object sender, RoutedEventArgs e) { textBox_71.SelectAll(); }
        private void textBox_72_GotFocus(object sender, RoutedEventArgs e) { textBox_72.SelectAll(); }
        private void textBox_73_GotFocus(object sender, RoutedEventArgs e) { textBox_73.SelectAll(); }
        private void textBox_74_GotFocus(object sender, RoutedEventArgs e) { textBox_74.SelectAll(); }
        private void textBox_75_GotFocus(object sender, RoutedEventArgs e) { textBox_75.SelectAll(); }
        private void textBox_76_GotFocus(object sender, RoutedEventArgs e) { textBox_76.SelectAll(); }
        private void textBox_77_GotFocus(object sender, RoutedEventArgs e) { textBox_77.SelectAll(); }
        private void textBox_78_GotFocus(object sender, RoutedEventArgs e) { textBox_78.SelectAll(); }
        private void textBox_79_GotFocus(object sender, RoutedEventArgs e) { textBox_79.SelectAll(); }

        private void textBox_81_GotFocus(object sender, RoutedEventArgs e) { textBox_81.SelectAll(); }
        private void textBox_82_GotFocus(object sender, RoutedEventArgs e) { textBox_82.SelectAll(); }
        private void textBox_83_GotFocus(object sender, RoutedEventArgs e) { textBox_83.SelectAll(); }
        private void textBox_84_GotFocus(object sender, RoutedEventArgs e) { textBox_84.SelectAll(); }
        private void textBox_85_GotFocus(object sender, RoutedEventArgs e) { textBox_85.SelectAll(); }
        private void textBox_86_GotFocus(object sender, RoutedEventArgs e) { textBox_86.SelectAll(); }
        private void textBox_87_GotFocus(object sender, RoutedEventArgs e) { textBox_87.SelectAll(); }
        private void textBox_88_GotFocus(object sender, RoutedEventArgs e) { textBox_88.SelectAll(); }
        private void textBox_89_GotFocus(object sender, RoutedEventArgs e) { textBox_89.SelectAll(); }

        private void textBox_91_GotFocus(object sender, RoutedEventArgs e) { textBox_91.SelectAll(); }
        private void textBox_92_GotFocus(object sender, RoutedEventArgs e) { textBox_92.SelectAll(); }
        private void textBox_93_GotFocus(object sender, RoutedEventArgs e) { textBox_93.SelectAll(); }
        private void textBox_94_GotFocus(object sender, RoutedEventArgs e) { textBox_94.SelectAll(); }
        private void textBox_95_GotFocus(object sender, RoutedEventArgs e) { textBox_95.SelectAll(); }
        private void textBox_96_GotFocus(object sender, RoutedEventArgs e) { textBox_96.SelectAll(); }
        private void textBox_97_GotFocus(object sender, RoutedEventArgs e) { textBox_97.SelectAll(); }
        private void textBox_98_GotFocus(object sender, RoutedEventArgs e) { textBox_98.SelectAll(); }
        private void textBox_99_GotFocus(object sender, RoutedEventArgs e) { textBox_99.SelectAll(); }

        private void textBox_11_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_12_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_13_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_14_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_15_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_16_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_17_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_18_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_19_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }

        private void textBox_21_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_22_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_23_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_24_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_25_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_26_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_27_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_28_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_29_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }

        private void textBox_31_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_32_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_33_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_34_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_35_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_36_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_37_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_38_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_39_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }

        private void textBox_41_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_42_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_43_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_44_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_45_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_46_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_47_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_48_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_49_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }

        private void textBox_51_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_52_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_53_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_54_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_55_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_56_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_57_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_58_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_59_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }

        private void textBox_61_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_62_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_63_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_64_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_65_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_66_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_67_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_68_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_69_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }

        private void textBox_71_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_72_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_73_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_74_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_75_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_76_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_77_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_78_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_79_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }

        private void textBox_81_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_82_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_83_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_84_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_85_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_86_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_87_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_88_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_89_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }

        private void textBox_91_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_92_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_93_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_94_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_95_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_96_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_97_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_98_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }
        private void textBox_99_KeyUp(object sender, KeyRoutedEventArgs e) { AdvanceFocus(sender, e); }

        #endregion

    }
}
