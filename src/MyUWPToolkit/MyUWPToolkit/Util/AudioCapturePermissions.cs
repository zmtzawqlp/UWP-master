using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;

namespace MyUWPToolkit.Util
{
    public static class AudioCapturePermissions
    {
        // If no recording device is attached, attempting to get access to audio capture devices will throw 
        // a System.Exception object, with this HResult set.
        private const int NoCaptureDevicesHResult = -1072845856;

        /// <summary>
        /// On desktop/tablet systems, users are prompted to give permission to use capture devices on a 
        /// per-app basis. Along with declaring the microphone DeviceCapability in the package manifest,
        /// this method tests the privacy setting for microphone access for this application.
        /// Note that this only checks the Settings->Privacy->Microphone setting, it does not handle
        /// the Cortana/Dictation privacy check, however (Under Settings->Privacy->Speech, Inking and Typing).
        /// 
        /// Developers should ideally perform a check like this every time their app gains focus, in order to 
        /// check if the user has changed the setting while the app was suspended or not in focus.
        /// </summary>
        /// <returns>true if the microphone can be accessed without any permissions problems.</returns>
        public async static Task<MicrophoneAccessStatus> RequestMicrophoneAccessAsync()
        {
            try
            {
                // Request access to the microphone only, to limit the number of capabilities we need
                // to request in the package manifest.
                var settings = new MediaCaptureInitializationSettings();
                settings.StreamingCaptureMode = StreamingCaptureMode.Audio;
                settings.MediaCategory = MediaCategory.Speech;

                var capture = new MediaCapture();
                await capture.InitializeAsync(settings);
            }
            catch (TypeLoadException)
            {
                // On SKUs without media player (eg, the N SKUs), we may not have access to the Windows.Media.Capture
                // namespace unless the media player pack is installed. Handle this gracefully.
                return MicrophoneAccessStatus.UnavailableMediaPlayerComponents;
            }
            catch (UnauthorizedAccessException)
            {
                // The user has turned off access to the microphone. If this occurs, we should show an error, or disable
                // functionality within the app to ensure that further exceptions aren't generated when 
                // recognition is attempted.
                return MicrophoneAccessStatus.Denied;
            }
            catch (Exception ex) when (ex.HResult == NoCaptureDevicesHResult)
            {
                // This can be replicated by using remote desktop to a system, but not redirecting the microphone input.
                // Can also occur if using the virtual machine console tool to access a VM instead of using remote desktop.
                return MicrophoneAccessStatus.NoCaptureDevices;
            }
            catch
            {
                return MicrophoneAccessStatus.Unspecified;
            }
            return MicrophoneAccessStatus.Allowed;
        }
    }

    public enum MicrophoneAccessStatus
    {
        Unspecified,
        Allowed,
        Denied,
        UnavailableMediaPlayerComponents,
        NoCaptureDevices
    }
}
