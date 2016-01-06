using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;

namespace MyUWPToolkit.Common
{
    public  class SpeechHelper
    {
        private bool _recognizerInitialized;

        private SpeechRecognizer _defaultRecognizer = null;
        private SpeechRecognizer _recognizer = null;
        private SpeechRecognizer Recognizer
        {
            get
            {
                if (_recognizer == null)
                {
                    return _defaultRecognizer;
                }

                return _recognizer;
            }

            set
            {
                _recognizer = value;
            }
        }

        public static SpeechHelper Instance { private set; get; }
        static SpeechHelper()
        {
            Instance = new SpeechHelper();
            Instance._defaultRecognizer = Instance.GetNewSpeechRecognizer();
        }

        private SpeechHelper()
        {
            this.InitDefaultRecognizerAsync();
        }

        public async Task<string> StartListeningAsync()
        {
            try
            {
                Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = await this.Recognizer.RecognizeWithUIAsync();
                // If successful, display the recognition result.
                if (speechRecognitionResult.Status == Windows.Media.SpeechRecognition.SpeechRecognitionResultStatus.Success)
                {
                    return speechRecognitionResult.Text;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private async void InitDefaultRecognizerAsync()
        {
            if (!_recognizerInitialized)
            {
                var webSearchGrammar = new Windows.Media.SpeechRecognition.SpeechRecognitionTopicConstraint(Windows.Media.SpeechRecognition.SpeechRecognitionScenario.WebSearch, "webSearch");
                Recognizer.Constraints.Add(webSearchGrammar);
                await Recognizer.CompileConstraintsAsync();
                _recognizerInitialized = true;
            }
        }

        private SpeechRecognizer GetNewSpeechRecognizer()
        {
            SpeechRecognizer speechRecognizer = new SpeechRecognizer(new Language("zh-CN"));
            speechRecognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(5.0);
            speechRecognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(0.15);
            speechRecognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(0.0);
            speechRecognizer.UIOptions.IsReadBackEnabled = false;
            speechRecognizer.UIOptions.ShowConfirmation = false;
            speechRecognizer.UIOptions.ExampleText = @"请说一些东西";

            return speechRecognizer;
        }
    }
}
