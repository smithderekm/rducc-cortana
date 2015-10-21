using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace CortanaLightController
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Initialize();

        }

        private void Initialize()
        {
            token = GetSASToken(baseAddress, "CREDENTIAL-NAME", "CREDENTIAL-KEY"); //UPDATE WITH NAME AND KEY FROM YOUR AZURE SERVICE BUS TOPIC
        }

        string baseAddress = "https://<servicebus-namespace>.servicebus.windows.net/"; //INSERT AZURE SERVICE BUS URL HERE
        string topicName = "huecommands"; //NAME OF THE TOPIC DEFINED IN AZURE
        string token;

        /// <summary>
        /// Click event handler for Light On Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LightOnButton_Click(object sender, RoutedEventArgs e)
        {
            //puts message on Azure Service Bus to turn light on
            WriteLog("INFO", "Sending TurnOn Command to Azure ServiceBus");

            Dictionary<string, string> properties = new Dictionary<string, string>();
            string body = "Turn On command from CortanaLightListener";
            properties.Add("action", "on");

            SendMessage(baseAddress, topicName, token, body, properties);

            WriteLog("INFO", "TurnOn Command sent to Azure ServiceBus");

        }

        /// <summary>
        /// Click event handler for Light Off button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LightOffButton_Click(object sender, RoutedEventArgs e)
        {
            //puts message on Azure Service Bus to turn light off
            WriteLog("INFO", "Sending TurnOff Command to Azure ServiceBus");
            Dictionary<string, string> properties = new Dictionary<string, string>();

            string body = "Turn Off command from CortanaLightListener";
            properties.Add("action", "off");

            SendMessage(baseAddress, topicName, token, body, properties);

            WriteLog("INFO", "TurnOff Command sent to Azure ServiceBus");
        }


        /// <summary>
        /// Turn the Kitchen light on or off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleSwitchKitchen_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleLight(sender,"kitchen");

        }

        /// <summary>
        /// Turns the Office light on or off.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleSwitchOffice_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleLight(sender, "office");

        }

        /// <summary>
        /// Turns the Bedroom light on or off.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleSwitchBedroom_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleLight(sender, "bedroom");

        }

        /// <summary>
        /// Posts a command for a specific light to toggle it on or off.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="lightname"></param>
        private void ToggleLight(object sender, string lightname)
        {
            //puts message on Azure Service Bus to turn light off
            WriteLog("INFO", string.Format("Sending {0} Command to Azure ServiceBus", lightname));
            Dictionary<string, string> properties = new Dictionary<string, string>();

            string body = "Turn On command from CortanaLightListener";

            if (((ToggleSwitch)sender).IsOn)
            {
                properties.Add("action", "on");
                WriteLog("INFO", string.Format("Turn {0} On command sent to Azure ServiceBus", lightname));
            }
            else
            {
                properties.Add("action", "off");
                WriteLog("INFO", string.Format("Turn {0} Off command sent to Azure ServiceBus", lightname));
            }

            properties.Add("lightname", lightname);

            SendMessage(baseAddress, topicName, token, body, properties);
        }

        /// <summary>
        /// Handler for inbound navigation.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (e.Parameter != null)
            {
                //process voice command
                ViewModel.LightListenerVoiceCommand command = (ViewModel.LightListenerVoiceCommand)e.Parameter;

                VoiceCommandReceived(command);
            }
        }

        /// <summary>
        /// Executes action triggered by a voice command
        /// </summary>
        /// <param name="command"></param>
        private void VoiceCommandReceived(ViewModel.LightListenerVoiceCommand command)
        {
            WriteLog("INFO", "Voice command received");
            WriteLog("INFO", string.Format("Voice command: action:{0}", command.action));

            if (!string.IsNullOrEmpty(command.lightName))
            {
                WriteLog("INFO", string.Format("Voice command: lightname:{0}", command.lightName));
            }


            //puts message on Azure Service Bus to change light state
            WriteLog("INFO", "Sending Command to Azure ServiceBus");
            Dictionary<string, string> properties = new Dictionary<string, string>();

            string body = "Command from CortanaLightListener";
            properties.Add("action", command.action);

            if (!string.IsNullOrEmpty(command.lightName))
            {
                properties.Add("lightname", command.lightName);
            }

            SendMessage(baseAddress, topicName, token, body, properties);

            WriteLog("INFO", "Command sent to Azure ServiceBus");
        }


        /// <summary>
        /// Submits a message to an Azure Service Bus queue
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="topicName"></param>
        /// <param name="token"></param>
        /// <param name="body"></param>
        /// <param name="properties"></param>
        private async void SendMessage(string baseAddress, string topicName, string token, string body, Dictionary<string, string> properties)
        {
            string fullAddress = baseAddress + topicName + "/messages" + "?timeout=60&api-version=2013-08 ";

            HttpResponseMessage response = await SendViaHttp(token, body, properties, fullAddress, HttpMethod.Post);
            WriteLog("INFO", string.Format("Result: {0}/{1}", response.StatusCode, response.ReasonPhrase));

        }

        /// <summary>
        /// Posts a message to Azure Service Bus REST api
        /// </summary>
        /// <param name="token"></param>
        /// <param name="body"></param>
        /// <param name="properties"></param>
        /// <param name="fullAddress"></param>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        private static async System.Threading.Tasks.Task<HttpResponseMessage> SendViaHttp(string token, string body, IDictionary<string, string> properties, string fullAddress, HttpMethod httpMethod)
        {
            HttpClient webClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri(fullAddress),
                Method = httpMethod,
            };


            webClient.DefaultRequestHeaders.Add("Authorization", token);

            if (properties != null)
            {
                foreach (string property in properties.Keys)
                {
                    request.Headers.Add(property, properties[property]);
                }
            }

            request.Content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("", body) });

            HttpResponseMessage response = await webClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {

                string error = string.Format("{0} : {1}", response.StatusCode, response.ReasonPhrase);

                throw new Exception(error);

            }

            return response;

        }


        /// <summary>
        /// Writes log message.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        private void WriteLog(string level, string message)
        {
            if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(level))
            {

                var messageToPost = new TextBlock
                {
                    Text = string.Format("{0} - {1}: {2}", DateTime.Now.ToString(), level, message),
                    FontSize = 18,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(10, 3, 10, 0),
                };

                outputContent.Children.Add(messageToPost);

                outputScroller.Measure(outputScroller.RenderSize);
                outputScroller.ChangeView(0.0f, double.MaxValue, 1.0f);
            }
        }

        /// <summary>
        /// Initializes authentication token for Azure Service Bus
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="SASKeyName"></param>
        /// <param name="SASKeyValue"></param>
        /// <returns></returns>
        private string GetSASToken(string baseAddress, string SASKeyName, string SASKeyValue)

        {
            TimeSpan fromEpochStart = DateTime.UtcNow - new DateTime(1970, 1, 1);
            string expiry = Convert.ToString((int)fromEpochStart.TotalSeconds + 3600);
            string stringToSign = WebUtility.UrlEncode(baseAddress) + "\n" + expiry;
            //string hmac = GetSHA256Key(Encoding.UTF8.GetBytes(SASKeyValue), stringToSign);
            string hash = HmacSha256(SASKeyValue, stringToSign);

            string sasToken = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}", WebUtility.UrlEncode(baseAddress), WebUtility.UrlEncode(hash), expiry, SASKeyName);

            return sasToken;

        }



        public string HmacSha256(string secretKey, string value)
        {

            // Move strings to buffers.
            var key = CryptographicBuffer.ConvertStringToBinary(secretKey, BinaryStringEncoding.Utf8);
            var msg = CryptographicBuffer.ConvertStringToBinary(value, BinaryStringEncoding.Utf8);

            // Create HMAC.
            var objMacProv = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);
            var hash = objMacProv.CreateHash(key);

            hash.Append(msg);

            return CryptographicBuffer.EncodeToBase64String(hash.GetValueAndReset());

        }

    }
}
