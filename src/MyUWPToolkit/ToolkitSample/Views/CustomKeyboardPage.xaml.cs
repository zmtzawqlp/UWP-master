using MyUWPToolkit.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ToolkitSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomKeyboardPage : Page
    {
        private SpeechService _speechService;
        public CustomKeyboardPage()
        {
            this.InitializeComponent();
           
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var inputPane = InputPane.GetForCurrentView();
            if (inputPane != null)
            {
                inputPane.Showing += InputPane_Showing;
            }
        }
        private void InputPane_Showing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            if (!isShowSystemKeyboard)
            {
                sender.TryHide();
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var inputPane = InputPane.GetForCurrentView();
            if (inputPane != null)
            {
                inputPane.Showing -= InputPane_Showing;
                inputPane.TryHide();
            }
            HideCustomKeyboard();
            preSelectionStart = 0;
            base.OnNavigatedFrom(e);
        }

        private bool IsAllNum(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            foreach (char c in str)
            {
                if (!(c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9'))
                    return false;
            }
            return true;
        }

        private bool IsAllAlphabetOrNumber(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            int tempByte = 0;

            foreach (char c in str)
            {
                tempByte = Convert.ToInt32(c);
                if (tempByte < 48 || tempByte > 122)
                    return false;
            }
            return true;
        }

        private async void MicrophoneIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_speechService == null)
                _speechService = new SpeechService();

            var notifier = NotifyTaskCompletion.Create(_speechService.RecognizeAsync());
            await notifier.TaskCompleted;
            if (notifier.IsSuccessfullyCompleted)
                searchBox.Text = notifier.Result;
        }

        #region CustomKeyboard
        bool isButtonClick = false;
        private void ToNumericButtonClick(object sender, RoutedEventArgs e)
        {
            numericKeyboardGrid.Visibility = Visibility.Visible;
            alphabeticalKeyboardGrid.Visibility = Visibility.Collapsed;
            this.searchBox.SelectionStart = preSelectionStart;
            searchBox.Focus(FocusState.Programmatic);
            isButtonClick = true;
        }

        private void ToAlphabeticalButtonClick(object sender, RoutedEventArgs e)
        {
            numericKeyboardGrid.Visibility = Visibility.Collapsed;
            alphabeticalKeyboardGrid.Visibility = Visibility.Visible;
            this.searchBox.SelectionStart = preSelectionStart;
            searchBox.Focus(FocusState.Programmatic);
            isButtonClick = true;
        }

        private void numericButtonClick(object sender, RoutedEventArgs e)
        {
            OnCustomKeyboardInput((sender as Button).Content.ToString());
            isButtonClick = true;
        }

        private void alphabeticalButtonClick(object sender, RoutedEventArgs e)
        {
            OnCustomKeyboardInput((sender as Button).Content.ToString());
            isButtonClick = true;
        }

        public void OnCustomKeyboardInput(string inputchart)
        {
            try
            {
                int index = preSelectionStart + inputchart.Length;
                if (searchBox.Text == null)
                {
                    this.searchBox.Text = inputchart;
                }
                else
                {
                    if (searchBox.SelectionLength > 0)
                    {
                        this.searchBox.Text = this.searchBox.Text.Remove(preSelectionStart, searchBox.SelectionLength);
                    }

                    this.searchBox.Text = this.searchBox.Text.Insert(preSelectionStart, inputchart);

                }

                this.searchBox.SelectionStart = index;

                this.searchBox.Focus(FocusState.Programmatic);
            }
            catch (Exception e)
            {
              
            }

        }
        private bool isShowSystemKeyboard = false;
        private void functionButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Button bt = sender as Button;
                switch (bt.Content.ToString())
                {
                    case "隐藏":
                        HideCustomKeyboard();
                        break;
                    case "系统键盘":
                        ShowSystemKeyboard();
                        break;
                    case "清空":
                        preSelectionStart = 0;
                        searchBox.Text = "";
                        break;
                    case "确定":

                        break;
                    default:
                        if (bt.Tag.ToString() == "删除")
                        {
                            //删除
                            if (searchBox.Text != null && searchBox.Text.Length > 0)
                            {
                                searchBox.Text = searchBox.Text.Remove(searchBox.Text.Length - 1, searchBox.SelectionLength == 0 ? 1 : searchBox.SelectionLength);
                                preSelectionStart -= 1;
                            }
                        }
                        else if (bt.Tag.ToString() == "空格")
                        {
                            OnCustomKeyboardInput(" ");
                            isButtonClick = true;
                            return;
                        }

                        break;
                }

                if (bt.Content.ToString() != "隐藏")
                {
                    this.searchBox.SelectionStart = preSelectionStart;
                    searchBox.Focus(FocusState.Programmatic);
                }
                isButtonClick = true;
            }
            catch (Exception e1)
            {
            }
                
        }


        private void uppercaseButtonClick(object sender, RoutedEventArgs e)
        {
            uppercaseGrid.Visibility = Visibility.Visible;
            lowercaseGrid.Visibility = Visibility.Collapsed;
            this.searchBox.SelectionStart = preSelectionStart;
            searchBox.Focus(FocusState.Programmatic);
            isButtonClick = true;
        }

        private void lowercaseButtonClick(object sender, RoutedEventArgs e)
        {
            uppercaseGrid.Visibility = Visibility.Collapsed;
            lowercaseGrid.Visibility = Visibility.Visible;
            this.searchBox.SelectionStart = preSelectionStart;
            searchBox.Focus(FocusState.Programmatic);
            isButtonClick = true;
        }

        private void searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!isShowSystemKeyboard)
            {
                ShowCustomKeyboard();
            }
        }
        private void searchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            isShowSystemKeyboard = false;
            //HideCustomKeyboard();
        }
        private void ShowCustomKeyboard()
        {
            isShowSystemKeyboard = false;
            if (customKeyboardGrid.Visibility == Visibility.Collapsed)
            {
                HideSystemKeyboard();
                customKeyboardGrid.Visibility = Visibility.Visible;
                numericKeyboardGrid.Visibility = Visibility.Visible;
                alphabeticalKeyboardGrid.Visibility = Visibility.Collapsed;
                showStoryboard.Begin();
            }
        }

        private void HideCustomKeyboard()
        {
            hideStoryboard.Begin();
            hideStoryboard.Completed += HideStoryboard_Completed;
        }

        private void HideStoryboard_Completed(object sender, object e)
        {
            customKeyboardGrid.Visibility = Visibility.Collapsed;
        }

        private void ShowSystemKeyboard()
        {
            isShowSystemKeyboard = true;
            //if (isShowSystemKeyboard)
            //{
            HideCustomKeyboard();
            var inputPane = InputPane.GetForCurrentView();
            if (inputPane != null)
            {
                inputPane.TryShow();
            }
            searchBox.Focus(FocusState.Programmatic);
            //}
        }

        private void HideSystemKeyboard()
        {
            isShowSystemKeyboard = false;
            var inputPane = InputPane.GetForCurrentView();
            if (inputPane != null)
            {
                inputPane.TryHide();
            }
        }

        int preSelectionStart = 0;
        private void searchBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(searchBox.SelectionStart);
            var focusedElement = FocusManager.GetFocusedElement();
            if (focusedElement != null && focusedElement == searchBox)
            {
                preSelectionStart = searchBox.SelectionStart;
            }
        }
        private void customKeyboardGrid_Tapped(object sender, RoutedEventArgs e)
        {
            if (!isButtonClick)
            {
                this.searchBox.SelectionStart = preSelectionStart;
                this.searchBox.Focus(FocusState.Programmatic);
            }
            isButtonClick = false;
        }

        private void LayoutRoot_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var point = e.GetPosition(searchBox);
            var point1 = e.GetPosition(customKeyboardGrid);
            bool hide = true;

            if (point.Y >= 0 && point.X >= 0 && point.X <= searchBox.ActualWidth && point.Y <= searchBox.ActualHeight)
            {
                hide = false;
            }

            if (point1.Y >= 0 && point1.X >= 0 && point1.X <= customKeyboardGrid.ActualWidth && point1.Y <= customKeyboardGrid.ActualHeight)
            {
                hide = false;
            }
            if (hide)
            {
                HideCustomKeyboard();
            }
        }



        #endregion

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {

        }
    }



}
