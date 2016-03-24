using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;
using Windows.UI.Popups;

namespace MyUWPToolkit.Util
{
    /// <summary>
    /// 语音服务
    /// </summary>
    public class SpeechService : IDisposable
    {
        /// <summary>
        /// This HResult represents the scenario where a user is prompted to allow in-app speech, but 
        /// declines. This should only happen on a Phone device, where speech is enabled for the entire device,
        /// not per-app.
        /// </summary>
        private const uint HResultPrivacyStatementDeclined = 0x80045509;
        private SpeechRecognizer _speechRecognizer;
        private IAsyncOperation<SpeechRecognitionResult> _recognitionOperation;
        private Task _initialization;

        /// <summary>
        /// 语音识别
        /// </summary>
        /// <returns>识别文本</returns>
        public async Task<string> RecognizeAsync()
        {
            if (_initialization == null || _initialization.IsFaulted)
                _initialization = InitializeRecognizer(SpeechRecognizer.SystemSpeechLanguage);

            await _initialization;

            CancelRecognitionOperation();

            // Start recognition.
            try
            {
                _recognitionOperation = _speechRecognizer.RecognizeWithUIAsync();
                SpeechRecognitionResult speechRecognitionResult = await _recognitionOperation;
                // If successful, return the recognition result.
                if (speechRecognitionResult.Status == SpeechRecognitionResultStatus.Success)
                    return speechRecognitionResult.Text;
                else
                    throw new Exception($"Speech recognition failed. Status: {speechRecognitionResult.Status}");
            }
            catch (Exception ex) when ((uint)ex.HResult == HResultPrivacyStatementDeclined)
            {
                // Handle the speech privacy policy error.
                var messageDialog = new MessageDialog("您没有同意语音识别隐私声明，请同意后重试");
                await messageDialog.ShowAsync();
                throw;
            }
        }

        /// <summary>
        /// Initialize Speech Recognizer and compile constraints.
        /// </summary>
        /// <param name="recognizerLanguage">Language to use for the speech recognizer</param>
        /// <returns>Awaitable task.</returns>
        private async Task InitializeRecognizer(Language recognizerLanguage)
        {
            MicrophoneAccessStatus status = await AudioCapturePermissions.RequestMicrophoneAccessAsync();
            if (status != MicrophoneAccessStatus.Allowed)
            {
                string prompt = status == MicrophoneAccessStatus.NoCaptureDevices ?
                    "没有检测到音频捕获设备，请检查设备后重试" :
                    "您没有允许本应用访问麦克风，请在 设置 -> 隐私 -> 麦克风 中设置";
                var messageDialog = new MessageDialog(prompt);
                await messageDialog.ShowAsync();
                throw new Exception($"Request microphone access failed. Status: {status}");
            }

            Dispose();

            // Create an instance of SpeechRecognizer.
            _speechRecognizer = new SpeechRecognizer(recognizerLanguage);

            // Add a web search topic constraint to the recognizer.
            var webSearchGrammar = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.WebSearch, "webSearch");
            _speechRecognizer.Constraints.Add(webSearchGrammar);

            // RecognizeWithUIAsync allows developers to customize the prompts.    
            _speechRecognizer.UIOptions.AudiblePrompt = "请说出您想搜索的东西";
            _speechRecognizer.UIOptions.ExampleText = "例如：“你好，美女”";

            // Compile the constraint.
            SpeechRecognitionCompilationResult compilationResult = await _speechRecognizer.CompileConstraintsAsync();

            // Check to make sure that the constraints were in a proper format and the recognizer was able to compile it.
            if (compilationResult.Status != SpeechRecognitionResultStatus.Success)
                throw new Exception($"Unable to compile grammar. Status: {compilationResult.Status}");
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_speechRecognizer != null)
            {
                CancelRecognitionOperation();

                _speechRecognizer.Dispose();
                _speechRecognizer = null;
            }
        }

        private void CancelRecognitionOperation()
        {
            if (_speechRecognizer.State != SpeechRecognizerState.Idle)
            {
                if (_recognitionOperation != null)
                {
                    _recognitionOperation.Cancel();
                    _recognitionOperation = null;
                }
            }
        }
    }
}