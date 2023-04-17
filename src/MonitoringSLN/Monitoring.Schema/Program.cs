using Monitoring.Schema;
using Newtonsoft.Json;

Console.WriteLine("Loading class and outputting scheme");
var loader = new LoaderViewModel
{
    Computer = Environment.MachineName,
    MyTime = DateTimeOffset.Now,
    AdditionalContext = new CustomLogViewModel
    {
        Name = "Custom counter",
        CounterValue = 40,
        CounterName = "Age"
    }
};

var loaderJson = JsonConvert.SerializeObject(loader);
Console.WriteLine("Json value:::::");
Console.WriteLine(loaderJson);