using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.SpeechRecognition;

namespace CortanaLightController
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync();
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }

            // Register VCD File
            try
            {
                var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///LightListenerCommands.xml"));
                await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(storageFile);
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Installing Voice Commands Failed: " + ex.ToString()); }



            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);


            Frame rootFrame = Window.Current.Content as Frame;
            // Do not repeat app initialization when the Window already has content, 
            // just ensure that the window is active 
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page 
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window 
                Window.Current.Content = rootFrame;
                rootFrame.Navigate(typeof(MainPage));
            }

            Window.Current.Activate();

            //init model
            Type navigateToPageType;
            ViewModel.LightListenerVoiceCommand? lightCommand = null;

            if (args.Kind != Windows.ApplicationModel.Activation.ActivationKind.VoiceCommand) { return; }

            var commandArgs = args as Windows.ApplicationModel.Activation.VoiceCommandActivatedEventArgs;

            Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = commandArgs.Result;

            // Get the name of the voice command and the text spoken
            string voiceCommandName = speechRecognitionResult.RulePath[0];
            string textSpoken = speechRecognitionResult.Text;
            // The commandMode is either "voice" or "text", and it indicates how the voice command was entered by the user.
            // Apps should respect "text" mode by providing feedback in a silent form.
            string commandMode = this.SemanticInterpretation("commandMode", speechRecognitionResult);
            string action = string.Empty;

            switch (voiceCommandName)
            {
                case "switchAllHueLights":
                    // Access the value of the {destination} phrase in the voice command
                    action = speechRecognitionResult.SemanticInterpretation.Properties["action"][0];
                    // Create a navigation parameter string to pass to the page
                    lightCommand = new ViewModel.LightListenerVoiceCommand(voiceCommandName, commandMode, textSpoken, action, string.Empty);
                    // Set the page where to navigate for this voice command
                    navigateToPageType = typeof(MainPage);
                    break;
                case "switchSpecificHueLight":
                    // Access the value of the {destination} phrase in the voice command
                    action = speechRecognitionResult.SemanticInterpretation.Properties["action"][0];
                    string lightname = speechRecognitionResult.SemanticInterpretation.Properties["lightname"][0];
                    // Create a navigation parameter string to pass to the page
                    lightCommand = new ViewModel.LightListenerVoiceCommand(voiceCommandName, commandMode, textSpoken, action, lightname);
                    // Set the page where to navigate for this voice command
                    navigateToPageType = typeof(MainPage);
                    break;

                default:
                    // There is no match for the voice command name. Navigate to MainPage
                    navigateToPageType = typeof(MainPage);
                    break;
            }


            // Since we're expecting to always show a details page, navigate even if  
            // a content frame is in place (unlike OnLaunched). 
            // Navigate to either the main trip list page, or if a valid voice command 
            // was provided, to the details page for that trip. 
            rootFrame.Navigate(navigateToPageType, lightCommand);



        }


        /// <summary> 
        /// Returns the semantic interpretation of a speech result. Returns null if there is no interpretation for 
        /// that key. 
        /// </summary> 
        /// <param name="interpretationKey">The interpretation key.</param> 
        /// <param name="speechRecognitionResult">The result to get an interpretation from.</param> 
        /// <returns></returns> 
        private string SemanticInterpretation(string interpretationKey, SpeechRecognitionResult speechRecognitionResult)
        {
            return speechRecognitionResult.SemanticInterpretation.Properties[interpretationKey].FirstOrDefault();
        }


    }
}
