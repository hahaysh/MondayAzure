using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Timers;
using Microsoft.Azure.EventHubs;

namespace EventHubsClient
{
    class Program
    {
       private static System.Timers.Timer SensorTimer;

        private const string DeviceConnectionString = "Endpoint=sb://hahaysheventhubs.servicebus.windows.net/;SharedAccessKeyName=fromdevice;SharedAccessKey=9N5AwkmmYtdgF3AvgEKT7q3EH1BMa0f0U7npjumkxN0=;EntityPath=hahayhseventhub1";
        private const string DeviceID = "Device1";

        private static Microsoft.Azure.EventHubs.EventHubClient SensorDevice = null;
        private static DummySensor Sensor = new DummySensor();

        static void Main(string[] args)
        {
            SensorDevice = Microsoft.Azure.EventHubs.EventHubClient.CreateFromConnectionString(DeviceConnectionString);

            SetTimer();

            Console.WriteLine("\nPress the Enter key to exit the application...\n");
            Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
            Console.ReadLine();
            SensorTimer.Stop();
            SensorTimer.Dispose();
        }

        private static void SetTimer()
        {
            SensorTimer = new Timer(2000);
            SensorTimer.Elapsed += SensorTimer_Elapsed;
            SensorTimer.AutoReset = true;
            SensorTimer.Enabled = true;
        }

        private async static void SensorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(Sensor.GetWetherData(DeviceID));
            Console.WriteLine(json);
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}", e.SignalTime);

            EventData eventMessage = new EventData(Encoding.UTF8.GetBytes(json));
            await SensorDevice.SendAsync(eventMessage);
        }
    }
}
