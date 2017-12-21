using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BodyNamed.Utils
{
    public static class NavigationService
    {
        public static event EventHandler<BackRequestedEventArgs> BackRequested;
        public static event EventHandler RootPageNavigated;

        private static Frame _navigationFrame;
        public static Frame Frame
        {
            get
            {
                if (_navigationFrame != null)
                {
                    return _navigationFrame;
                }

                return Window.Current.Content as Frame;
            }
        }

        public static IList<PageStackEntry> BackStack
        {
            get
            {
                return Frame.BackStack;
            }
        }

        public static bool CanGoBack
        {
            get
            {
                return Frame.CanGoBack;
            }
        }

        public static bool CanGoForward
        {
            get
            {
                return Frame.CanGoForward;
            }
        }

        public static void GoBack(bool fireRootPageNavigatedEvent = true)
        {
            if (CanGoBack)
            {
                Frame.GoBack();
                FireRootPageNavigatedEvent(fireRootPageNavigatedEvent);
            }
        }

        public static void FireRootPageNavigatedEvent(bool fireRootPageNavigatedEvent = true)
        {
            if (Frame.BackStackDepth == 0 && RootPageNavigated != null && fireRootPageNavigatedEvent)
            {
                RootPageNavigated(Frame, EventArgs.Empty);
            }
        }

        public static void GoBackToRootPage(bool fireRootPageNavigatedEvent = true)
        {
            while (CanGoBack)
            {
                GoBack(fireRootPageNavigatedEvent);
            }
        }

        public static void GoForward()
        {
            if (CanGoForward)
            {
                Frame.GoForward();
            }
        }

        public static void NavigateInNewFrame(Type pageType, object parameter = null)
        {
            var frame = new Frame() { IsTextScaleFactorEnabled = false };
            Window.Current.Content = frame;
            frame.Navigate(pageType, parameter);
            Window.Current.Activate();
        }

        public static void Navigate(Type pageType, object parameter = null)
        {
            Frame.Navigate(pageType, parameter);
        }

        public static void CleanNavigate(Type sourcePageType, object parameter = null)
        {
            int index = Frame.BackStack.FindIndex(entry => entry.SourcePageType == sourcePageType);
            if (index > -1)
            {
                for (int i = Frame.BackStack.Count - index; i >= 0; i--)
                {
                    Frame.GoBack();
                }
            }
            else if (Frame.CurrentSourcePageType == sourcePageType && Frame.CanGoBack)
            {
                Frame.GoBack();
            }

            Frame.Navigate(sourcePageType, parameter);
        }

        public static void RemoveBackEntry()
        {
            BackStack.RemoveAt(BackStack.Count - 1);
        }

        public static bool ExistedInBackEntry(Type pageType)
        {
            var page = BackStack.FirstOrDefault(entry => entry.SourcePageType == pageType);
            return page != null;
        }

        public static void RemoveBackEntry(Type pageType)
        {
            IList<PageStackEntry> entries = BackStack.Where(entry => entry.SourcePageType == pageType).ToList();
            foreach (PageStackEntry entry in entries)
            {
                BackStack.Remove(entry);
            }
        }

        public static void RegisterCustomFrame(this Frame frame, string sessionStateKey)
        {
            if (!ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
            {
                frame.Navigated += (s, e) =>
                {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = frame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
                };
               
            }

            _navigationFrame = frame;
            //SuspensionManager.RegisterFrame(frame, sessionStateKey);

            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) =>
            {
                e.Handled = true;
                var handler = BackRequested;
                if (handler != null)
                {
                    BackRequested(s, e);
                }
                else if (Frame.CanGoBack)
                {
                    Frame.GoBack();

                    FireRootPageNavigatedEvent();
                }
                //else
                //{

                //}

                //e.Handled = true;
            };
        }

        public static void UnregisterCustomFrame(this Frame frame, string sessionStateKey)
        {
            _navigationFrame = null;
            //SuspensionManager.UnregisterFrame(frame);
        }
    }
}
