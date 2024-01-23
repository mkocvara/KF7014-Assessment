using IoTHumiditySensorEmulator;


int sleeptime = 3;
Console.WriteLine($"Waiting {sleeptime} seconds for server to start");
sleeptime *= 1000;
Thread.Sleep(sleeptime);

int sensorCount = 30;


for(int i = 1; i<=sensorCount; i++)
{
    new Sensor(i).SendRequests();
    Thread.Sleep(1000);
}

Console.WriteLine("Running all threads. Press any key to exit...");

await new Sensor(sensorCount).SendRequests();
