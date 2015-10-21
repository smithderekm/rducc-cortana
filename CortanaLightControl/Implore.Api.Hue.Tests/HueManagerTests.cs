using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Implore.Api.Hue.Tests
{
    [TestClass]
    public class HueManagerTest
    {
        [TestMethod]
        public void CanInitialize()
        {

            Hue.HueManager manager = new HueManager("http://192.168.1.130/", "d4f04873f3e04ff45ad6773caf58ef");

        }

        [TestMethod]
        public void CanTurnOn()
        {
            Hue.HueManager manager = new HueManager("http://192.168.1.130/", "d4f04873f3e04ff45ad6773caf58ef");

            manager.TurnOn(manager.Lights["office lamp"]);
        }

        [TestMethod]
        public void CanTurnOff()
        {
            Hue.HueManager manager = new HueManager("http://192.168.1.130/", "d4f04873f3e04ff45ad6773caf58ef");

            manager.TurnOff(manager.Lights["office lamp"]);
        }
        [TestMethod]
        public void CanSetColor()
        {
            Hue.HueManager manager = new HueManager("http://192.168.1.130/", "d4f04873f3e04ff45ad6773caf58ef");

            manager.SetColor(manager.Lights["office lamp"], 0, 255, 0);
        }
        [TestMethod]
        public void CanSetEffect()
        {
            Hue.HueManager manager = new HueManager("http://192.168.1.130/", "d4f04873f3e04ff45ad6773caf58ef");

            manager.TurnOn(manager.Lights["office lamp"]);

            manager.SetEffect(manager.Lights["office lamp"], Effect.ColorLoop);

            System.Threading.Thread.Sleep(60 * 1000);

            manager.SetEffect(manager.Lights["office lamp"], Effect.None);

        }

    }
}
