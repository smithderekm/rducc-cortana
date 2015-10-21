using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Implore.Api.Hue
{
    public class HueManager

    {
        private Uri _bridgeApiBase;

        public Dictionary<string, Light> Lights { get; set; }

        public HueManager()
        {
            string bridgeUrl = System.Configuration.ConfigurationManager.AppSettings["HueBridgeUrl"].ToString();
            string bridgeUserName = ConfigurationManager.AppSettings["HueBridgeUserName"].ToString();

            Initialize(bridgeUrl, bridgeUserName);

       }

        public HueManager(string url, string username)
        {
            Initialize(url, username);

        }

       
        public void TurnOn(Light light)
        {
            LightCommand newState = new LightCommand();
            newState.On = true;

            string command = JsonConvert.SerializeObject(newState, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            SendApiCommand(light, command);

        }

        public void TurnOff(Light light)
        {
            LightCommand newState = new LightCommand();
            newState.On = false;

            string command = JsonConvert.SerializeObject(newState, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            SendApiCommand(light, command);

        }

        public void SetColor(Light light, int red, int green, int blue)
        {
            int hueValue = GetHueFromRGB(red, green, blue);

            string command = JsonConvert.SerializeObject(new { hue = hueValue});
            SendApiCommand(light, command);
        }

        private int GetHueFromRGB(int red, int green, int blue)
        {
            float min = Math.Min(Math.Min(red, green), blue);
            float max = Math.Max(Math.Max(red, green), blue);

            float hue = 0f;
            if (max == red)
            {
                hue = (green - blue) / (max - min);

            }
            else if (max == green)
            {
                hue = 2f + (blue - red) / (max - min);
       
            }
            else
            {
                hue = 4f + (red - green) / (max - min);
            }

            hue = hue * 60;
            if (hue < 0) hue = hue + 360;

            return (int)Math.Round(hue);

        }

        public void SetEffect(Light light, Effect effect)
        {
            LightCommand newState = new LightCommand();
            newState.Effect = effect;

            string command = JsonConvert.SerializeObject(newState, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            SendApiCommand(light, command);
        }
        /// <summary>
        /// Posts a API command for a single light to the Hue Bridge.  Used to change the State of a light.
        /// </summary>
        /// <param name="light"></param>
        /// <param name="command"></param>
        private void SendApiCommand(Light light, string command)
        {
            try
            {
                HttpResponseMessage response = new HttpResponseMessage();

                // Create a Http Call for Access Token
                HttpClient client = new HttpClient();
                client.BaseAddress = _bridgeApiBase;

                
                client.PutAsync(_bridgeApiBase + "/lights/" + light.Id.ToString() + "/state", new StringContent(command)).ContinueWith(
                    (getTask) =>
                    {
                        if (getTask.IsCanceled) { return; }
                        if (getTask.IsFaulted) { throw getTask.Exception; }
                        response = getTask.Result;

                        response.EnsureSuccessStatusCode();
                    }).Wait();

                string result = response.Content.ReadAsStringAsync().Result.ToString();


            }
            catch (Exception ex)
            {
                throw new Exception("Error sending command.  Check inner exception for details.", ex);
            }
        }

       

        /// <summary>
        /// Initializes connection to Hue bridge
        /// </summary>
        /// <param name="url"></param>
        /// <param name="username"></param>
        public void Initialize(string url, string username)
        {
            if (url.LastIndexOf('/') == url.Length - 1) { url = url.Substring(0, url.Length - 1); }

            //connect to bridge using IP address from config file

            //assume connected app and username from config file

            //ping bridge to ensure connectivity

            _bridgeApiBase = new Uri(string.Format("{0}/api/{1}", url, username));

            try
            {
                HttpResponseMessage response = new HttpResponseMessage();

                // Create a Http Call for Access Token
                HttpClient client = new HttpClient();
                client.BaseAddress = _bridgeApiBase;
                
                client.GetAsync(_bridgeApiBase + "/lights").ContinueWith(
                    (getTask) =>
                    {
                        if (getTask.IsCanceled) { return; }
                        if (getTask.IsFaulted) { throw getTask.Exception; }
                        response = getTask.Result;

                        response.EnsureSuccessStatusCode();
                    }).Wait();

                string result = response.Content.ReadAsStringAsync().Result.ToString();

                this.Lights = new Dictionary<string, Light>();

                JToken token = JToken.Parse(result);
                if (token.Type == JTokenType.Object)
                {
                    var lightsJSON = (JObject)token;
                    foreach (var prop in lightsJSON.Properties())
                    {
                        Light newLight = JsonConvert.DeserializeObject<Light>(prop.Value.ToString());
                        newLight.Id = prop.Name.ToString();
                        this.Lights.Add(newLight.Name.ToLower(), newLight);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing HueManager.  Check inner exception for details.", ex);
            }

        }

    }
}
