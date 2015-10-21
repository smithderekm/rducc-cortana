using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System.Threading;

namespace CortanaLightController
{
    class Program
    {
        static string AzureServiceBusTopicName = "";
        static string AzureServiceBusSubscriptionName = "";
        static string connectionString;

        static private Implore.Api.Hue.HueManager _HueManager;
        static public Implore.Api.Hue.HueManager HueManager
        {
            get
            {
                if (_HueManager == null)
                {
                    WriteLog("INFO", string.Format("Initializing Hue API manager"));
                    _HueManager = new Implore.Api.Hue.HueManager();
                }
                return _HueManager;
            }
        }

        static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {

            //initialize, then poll Azure Service Bus for new messages
            Initialize();

            //initialize Service Bus Queue
            WriteLog("INFO", string.Format("Connecting to service bus topic {0}", AzureServiceBusTopicName.ToString()));
            SubscriptionClient sbClient = SubscriptionClient.CreateFromConnectionString(connectionString, AzureServiceBusTopicName, AzureServiceBusSubscriptionName);

            OnMessageOptions option = new OnMessageOptions();
            option.AutoComplete = false; //we'll manually pop message out of queue after processing it
            option.AutoRenewTimeout = TimeSpan.FromSeconds(10);
            option.MaxConcurrentCalls = 1; //only pull one message at a time

            //prep for continuous run
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            // wait for incoming message
            sbClient.OnMessage((message) =>
            {
                try
                {
                    WriteLog("INFO", "New message received.");
                    foreach (var prop in message.Properties)
                    {
                        WriteLog("INFO", string.Format("Message Property: {0}:{1}", prop.Key, prop.Value));

                    }

                    //process call to API
                    ProcessMessage(message);

                    //remove message from subscription
                    message.Complete();
                }
                catch (Exception ex)
                {
                    //leave message in queue
                    message.Abandon();

                    WriteLog("ERROR", ex.Message);
                }
            }, option);

            //wait
            WriteLog("INFO", "Polling....");

            _quitEvent.WaitOne();

            // cleanup/shutdown and quit
            if (!sbClient.IsClosed) { sbClient.Close(); }
        }

        /// <summary>
        /// Interprets and processes a command message.  
        /// </summary>
        /// <param name="message"></param>
        private static void ProcessMessage(BrokeredMessage message)
        {
            //message body will contain
            string action = message.Properties["action"].ToString().ToLower();
            bool hasLightName = message.Properties.ContainsKey("lightname");
            
            switch (action)
            {
                case "on":

                    WriteLog("INFO", "Hue TurnOn message received.");

                    if (hasLightName)
                    {
                        var light = HueManager.Lights[message.Properties["lightname"].ToString().ToLower()];
                        WriteLog("INFO", string.Format("Turning on light {0}...", light.Name.ToString()));
                        HueManager.TurnOn(light);
                    }
                    else
                    {
                        //call Hue API
                        foreach (var light in HueManager.Lights)
                        {
                            WriteLog("INFO", string.Format("Turning on light {0}...", light.Value.Name.ToString()));
                            HueManager.TurnOn(light.Value);
                        }


                    }
                    break;
                case "off":

                    if (hasLightName)
                    {
                        var light = HueManager.Lights[message.Properties["lightname"].ToString().ToLower()];
                        WriteLog("INFO", string.Format("Turning on light {0}...", light.Name.ToString()));
                        HueManager.TurnOff(light);

                    }
                    else
                    {
                        //call hue API
                        foreach (var light in HueManager.Lights)
                        {
                            WriteLog("INFO", string.Format("Turning off light {0}...", light.Value.Name.ToString()));
                            HueManager.TurnOff(light.Value);
                        }

                    }
                    break;
                case "red":
                    if (hasLightName)
                    {
                        var light = HueManager.Lights[message.Properties["lightname"].ToString().ToLower()];
                        WriteLog("INFO", string.Format("Turning light {0} to red...", light.Name.ToString()));
                        HueManager.SetColor(light, 255, 0, 0);

                    }
                    else
                    {
                        //call hue API
                        foreach (var light in HueManager.Lights)
                        {
                            WriteLog("INFO", string.Format("Turning light {0} red...", light.Value.Name.ToString()));
                            HueManager.SetColor(light.Value, 255, 0, 0);
                        }

                    }
                    break;
                case "green":
                    if (hasLightName)
                    {
                        var light = HueManager.Lights[message.Properties["lightname"].ToString().ToLower()];
                        WriteLog("INFO", string.Format("Turning light {0} to green...", light.Name.ToString()));
                        HueManager.SetColor(light, 0, 255, 0);

                    }
                    else
                    {
                        //call hue API
                        foreach (var light in HueManager.Lights)
                        {
                            WriteLog("INFO", string.Format("Turning light {0} green...", light.Value.Name.ToString()));
                            HueManager.SetColor(light.Value, 0, 255, 0);
                        }

                    }
                    break;
                case "blue":
                    if (hasLightName)
                    {
                        var light = HueManager.Lights[message.Properties["lightname"].ToString().ToLower()];
                        WriteLog("INFO", string.Format("Turning light {0} to blue...", light.Name.ToString()));
                        HueManager.SetColor(light, 0, 0, 255);

                    }
                    else
                    {
                        //call hue API
                        foreach (var light in HueManager.Lights)
                        {
                            WriteLog("INFO", string.Format("Turning light {0} green...", light.Value.Name.ToString()));
                            HueManager.SetColor(light.Value, 0, 0, 255);
                        }

                    }
                    break;
            }
            //else if (message.Properties["action"].ToString().ToLower() == "party")
            //{
            //    //call hue API
            //    foreach (var light in HueManager.Lights)
            //    {
            //        WriteLog("INFO", string.Format("Light {0} is starting to party...", light.Value.Name.ToString()));
            //        HueManager.SetEffect(light.Value, Implore.Api.Hue.Effect.ColorLoop);
            //    }

            //    //wait 60 sec
            //    Thread.Sleep(10 * 1000);
            //    //call hue API
            //    foreach (var light in HueManager.Lights)
            //    {
            //        WriteLog("INFO", string.Format("Light {0} has left the party...", light.Value.Name.ToString()));
            //        HueManager.SetEffect(light.Value, Implore.Api.Hue.Effect.None);
            //    }

            //}
        }

        /// <summary>
        /// Writes a message to the console.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        private static void WriteLog(string level, string message)
        {
            if (level.ToUpper().Equals("ERROR")) Console.ForegroundColor = ConsoleColor.Red;
            else Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(string.Format("{0} - {1}:{2}", DateTime.Now.ToString(), level, message));
        }

        /// <summary>
        /// Initializes settings from configuration file.
        /// </summary>
        static void Initialize()
        {
            AzureServiceBusTopicName = System.Configuration.ConfigurationManager.AppSettings["AzureServiceBusTopicName"];
            AzureServiceBusSubscriptionName = System.Configuration.ConfigurationManager.AppSettings["AzureServiceBusSubscriptionName"];

            connectionString = System.Configuration.ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];

        }
    }
}
