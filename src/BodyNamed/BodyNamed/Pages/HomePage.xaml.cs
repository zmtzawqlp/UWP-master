using BodyNamed.Models;
using BodyNamed.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BodyNamed.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        DispatcherTimer dt = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(0.03) };
        List<BodyName> Names = new List<BodyName>();
        Random rd = new Random();
        int Total = 0;
        BodyName currentBodyName;
        VoiceInformation currentVoice;
        public HomePage()
        {
            this.InitializeComponent();
            dt.Tick += Dt_Tick;
            var voices = SpeechSynthesizer.AllVoices;

            // Get the currently selected voice.
            currentVoice = voices.FirstOrDefault(x => x.Id == AppSettings.VoiceInformationID);
            if (currentVoice == null)
            {
                currentVoice = voices.FirstOrDefault();
            }
            MediaPlayer player = new MediaPlayer();
            player.AutoPlay = false;

            media.SetMediaPlayer(player);
        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            media.MediaPlayer.Pause();
            rootGrid.IsHitTestVisible = true;
            Flyout.Hide();
            Grid_Tapped(null, null);
        }

        private void Dt_Tick(object sender, object e)
        {
            var index = new Random(Guid.NewGuid().GetHashCode()).Next(1, Total);
            //Debug.WriteLine(index);
            foreach (var item in Names)
            {
                if (item.Min <= index && index <= item.Max)
                {
                    tb.Text = item.Name;
                    currentBodyName = item;
                    break;
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            tb.Text = "点击屏幕以开始";
            Names.Clear();
            var names = BodyNamesHelper.Instance.BodyNames.Where(x => x.Gender == AppSettings.BodyGender);
            Total = 1;
            foreach (var item in names)
            {
                item.Min = Total;
                item.Max = Total + (int)item.Chance.Value - 1;
                Total = item.Max + 1;
                Names.Add(item);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            dt.Stop();
            Flyout.Hide();
            media.MediaPlayer.Pause();
            base.OnNavigatedFrom(e);
        }

        bool start;
        private async void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {

            if (start)
            {
                dt.Stop();
                rootGrid.IsHitTestVisible = false;

                await ReadText();
                Flyout.ShowAt(this);
                //grid.IsHitTestVisible = true;
            }
            else
            {
                textToSynthesize.Visibility = Visibility.Collapsed;
                tb.Visibility = Visibility.Visible;
                dt.Start();
            }
            start = !start;
        }

        public async Task ReadText()
        {
            var name = currentBodyName?.Name;
            var introduction = currentBodyName?.Introduction;

            if (introduction != null && name != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(name + "小朋友");
                sb.AppendLine("你已经选择了自己的名字.");
                sb.AppendLine("其寓意为:" + introduction);
                var text = sb.ToString();
                // text = @"仅使用语音，便可以启动程序、打开菜单、单击屏幕上的按钮和其他对象、将文本口述到文档中以及书写和发送电子邮件。只要是可以用键盘和鼠标完成的所有事情，都可以仅用语音来完成。";
                textToSynthesize.Text = text;
                textToSynthesize.Visibility = Visibility.Visible;
                tb.Visibility = Visibility.Collapsed;
                MediaElement mediaplayer = new MediaElement();
                using (var speech = new SpeechSynthesizer())
                {
                    speech.Options.SpeakingRate = 1;
                    speech.Options.IncludeWordBoundaryMetadata = true;
                    //speech.Options.IncludeSentenceBoundaryMetadata = true;
                    speech.Voice = currentVoice;
                    //string ssml = @"<speak version='1.0' " + "xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='zh-cn'>" + text + "</speak>";
                    //SpeechSynthesisStream stream = await speech.SynthesizeSsmlToStreamAsync(ssml);
                    SpeechSynthesisStream stream = await speech.SynthesizeTextToStreamAsync(text);
                    // Create a media source from the stream: 
                    var mediaSource = MediaSource.CreateFromStream(stream, stream.ContentType);

                    //Create a Media Playback Item   
                    var mediaPlaybackItem = new MediaPlaybackItem(mediaSource);

                    // Ensure that the app is notified for cues.  
                    RegisterForWordBoundaryEvents(mediaPlaybackItem);

                    media.Source = mediaPlaybackItem;
                    //media.MediaPlayer.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;
                    media.MediaPlayer.Play();
                }

            }
        }

        //private void PlaybackSession_PositionChanged(MediaPlaybackSession sender, object args)
        //{
        //    Debug.WriteLine("位置：" + sender.Position);
        //}

        private void RegisterForWordBoundaryEvents(MediaPlaybackItem mediaPlaybackItem)
        {
            // If tracks were available at source resolution time, itterate through and register: 
            for (int index = 0; index < mediaPlaybackItem.TimedMetadataTracks.Count; index++)
            {
                RegisterMetadataHandlerForWords(mediaPlaybackItem, index);
                //RegisterMetadataHandlerForSentences(mediaPlaybackItem, index);
            }

            // Since the tracks are added later we will  
            // monitor the tracks being added and subscribe to the ones of interest 
            mediaPlaybackItem.TimedMetadataTracksChanged += (MediaPlaybackItem sender, IVectorChangedEventArgs args) =>
            {
                if (args.CollectionChange == CollectionChange.ItemInserted)
                {
                    RegisterMetadataHandlerForWords(sender, (int)args.Index);
                    //RegisterMetadataHandlerForSentences(mediaPlaybackItem, (int)args.Index);
                }
                else if (args.CollectionChange == CollectionChange.Reset)
                {
                    for (int index = 0; index < sender.TimedMetadataTracks.Count; index++)
                    {
                        RegisterMetadataHandlerForWords(sender, index);
                        //RegisterMetadataHandlerForSentences(mediaPlaybackItem, index);
                    }
                }
            };
        }

        private void RegisterMetadataHandlerForWords(MediaPlaybackItem mediaPlaybackItem, int index)
        {
            var timedTrack = mediaPlaybackItem.TimedMetadataTracks[index];
            //register for only word cues
            if (timedTrack.Id == "SpeechWord")
            {
                timedTrack.CueEntered += metadata_SpeechCueEntered;
                mediaPlaybackItem.TimedMetadataTracks.SetPresentationMode((uint)index, TimedMetadataTrackPresentationMode.ApplicationPresented);
            }
        }

        private async void metadata_SpeechCueEntered(TimedMetadataTrack timedMetadataTrack, MediaCueEventArgs args)
        {
            // Check in case there are different tracks and the handler was used for more tracks 
            if (timedMetadataTrack.TimedMetadataKind == TimedMetadataKind.Speech)
            {
                var cue = args.Cue as SpeechCue;
                if (cue != null)
                {
                    //System.Diagnostics.Debug.WriteLine("Hit Cue with start:" + cue.StartPositionInInput + " and end:" + cue.EndPositionInInput);
                    //System.Diagnostics.Debug.WriteLine("Cue text:[" + cue.Text + "]");
                    // Do something with the cue 
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                     () =>
                     {
                         // Your UI update code goes here!
                         HighlightTextOnScreen(cue.StartPositionInInput, cue.EndPositionInInput);
                         //FillTextBoxes(cue, timedMetadataTrack);
                     });
                }
            }
        }

        private void HighlightTextOnScreen(int? startPositionInInput, int? endPositionInInput)
        {
            if (startPositionInInput != null && endPositionInInput != null)
            {
                //textToSynthesize.Select(startPositionInInput.Value, endPositionInInput.Value - startPositionInInput.Value + 1);
                // Debug.WriteLine(endPositionInInput.Value + 1);
                textToSynthesize.Select(0, endPositionInInput.Value + 1);
            }
        }

    }
}
