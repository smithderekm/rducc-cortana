using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CortanaLightController.ViewModel
{
    public struct LightListenerVoiceCommand
    {
        public string voiceCommand;
        public string commandMode;
        public string textSpoken;
        public string action;
        public string lightName;


        /// <summary> 
        /// Set up the voice command struct with the provided details about the voice command. 
        /// </summary> 
        /// <param name="voiceCommand">The voice command (the Command element in the VCD xml) </param> 
        /// <param name="commandMode">The command mode (whether it was voice or text activation)</param> 
        /// <param name="textSpoken">The raw voice command text.</param> 
        /// <param name="action">The actionparameter.</param> 
        /// <param name="lightName"></param>
        public LightListenerVoiceCommand(string voiceCommand, string commandMode, string textSpoken, string action, string lightName)
        {
            this.voiceCommand = voiceCommand;
            this.commandMode = commandMode;
            this.textSpoken = textSpoken;
            this.action  = action;
            this.lightName = lightName;
        }
    }

}
