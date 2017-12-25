using BodyNamed.Models;
using BodyNamed.Utils;
using Microsoft.Toolkit.Uwp.Services.OneDrive;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;
using Windows.Media.SpeechSynthesis;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BodyNamed.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public BodyName AddItem = new BodyName();
        public SettingsPage()
        {
            this.InitializeComponent();
            InitializeListboxVoiceChooser();
        }

        private void InitializeListboxVoiceChooser()
        {
            // Get all of the installed voices.
            var voices = SpeechSynthesizer.AllVoices;

            // Get the currently selected voice.
            VoiceInformation currentVoice = voices.FirstOrDefault(x => x.Id == AppSettings.VoiceInformationID);
            if (currentVoice == null)
            {
                currentVoice = voices.FirstOrDefault();
            }
            AppSettings.VoiceInformationID = currentVoice.Id;
            foreach (VoiceInformation voice in voices.OrderBy(p => p.Language))
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Name = voice.DisplayName;
                item.Tag = voice;
                item.Content = voice.DisplayName + " (语言: " + voice.Language + ")";
                voiceList.Items.Add(item);

                // Check to see if we're looking at the current voice and set it as selected in the listbox.
                if (currentVoice != null && currentVoice.Id == voice.Id)
                {
                    item.IsSelected = true;
                    voiceList.SelectedItem = item;
                }
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ts.IsOn = AppSettings.BodyGender == Gender.Male;
            //voiceList.ItemsSource = SpeechSynthesizer.AllVoices;
            InitializeAddItem();
            Group();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //NameList.ItemsSource = null;
            BodyNamesHelper.Instance.Save();
            base.OnNavigatedFrom(e);
        }

        public void InitializeAddItem()
        {
            AddItem.Name = "周天";
            AddItem.Gender = Gender.Male;
            AddItem.Chance = 100;
        }

        public void Group()
        {
            cvs.Source = BodyNamesHelper.Instance.BodyNames.OrderBy(x => x.Gender).GroupBy(x => x.Gender);
        }
        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            BodyNamesHelper.Instance.BodyNames.Remove((sender as Button).DataContext as BodyName);
            Group();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            BodyNamesHelper.Instance.BodyNames.Add(new BodyName() { Name = AddItem.Name, Gender = AddItem.Gender, Chance = AddItem.Chance });
            Group();
            InitializeAddItem();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).Tag == null)
            {
                (sender as ComboBox).Tag = 1;
            }
            else
            {
                Group();
            }
        }

        private void ts_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettings.BodyGender = ts.IsOn ? Gender.Male : Gender.Female;
        }

        private void voiceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)(voiceList.SelectedItem);
            if (item != null)
            {
                VoiceInformation voice = (VoiceInformation)(item.Tag);
                AppSettings.VoiceInformationID = voice.Id;
            }
        }

        //private async void uploadButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var result = await MessageBox.AskAsync("上传本地数据将会覆盖OneDrive数据", "上传数据确认");
        //    if (result == MessageBoxResult.OK)
        //    {
        //        bool initializeOneDriveService = false;
        //        try
        //        {

        //            initializeOneDriveService = OneDriveService.Instance.Initialize();

        //            if (!await OneDriveService.Instance.LoginAsync())
        //            {
        //                if (initializeOneDriveService)
        //                {
        //                   await OneDriveService.Instance.LogoutAsync();
        //                }
        //                await MessageBox.ShowAsync("暂时无法登陆","提示");
        //                return;
        //            }
        //        }
        //        catch(Exception e1)
        //        {

        //        }
        //    }
        //}

        //private void downloadButton_Click(object sender, RoutedEventArgs e)
        //{

        //}
    }
}
