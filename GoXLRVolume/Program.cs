using System;
using System.Configuration;
using NAudio.CoreAudioApi;
using System.Linq;

namespace GoXLRVolume
{
    class Program
    {
        static bool doTerminate = false;
       static void errorAndTerminate(String errorMessage)
        {
            doTerminate = true;
            Console.WriteLine("{0}\nPress enter to exit",errorMessage);
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            String deviceName = args[0];
            String volumePercent = args[1];

            bool converted = float.TryParse(volumePercent, out float volumeInt);
            if (!volumePercent.All(char.IsNumber) || !converted) errorAndTerminate("Volume must be a valid number between 0-100\nPress enter to exit");
                           
            if(volumeInt > 100 || volumeInt < 0) errorAndTerminate("Volume must be a valid number between 0-100\nPress enter to exit");
          
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();

            var device = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active)
                .Where<MMDevice>(d => d.DeviceFriendlyName.Equals("TC-Helicon GoXLR"))
                .FirstOrDefault<MMDevice>(i => i.FriendlyName.ToLower().StartsWith(deviceName));

            if(device == null) errorAndTerminate("Could not find device match for input '" + deviceName + "'");

            if (doTerminate) return;

            Console.WriteLine("Setting {0} to volume {1}", device.FriendlyName, volumeInt / 100);            
            device.AudioEndpointVolume.MasterVolumeLevelScalar = volumeInt / 100;
            
        }
    }
}
