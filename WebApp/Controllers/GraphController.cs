using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.Azure.Devices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using WebApp.Models;
using WebApp.DTOs;

namespace WebApp.Controllers
{
    public class GraphController : Controller
    {
        // GET: Graph
        public async Task<ActionResult> Index(string span = null, string selectedDeviceName = null)
        {
            var availableTimeSpans = new List<TimeSpanOptionsDto>()
            {
                new TimeSpanOptionsDto()
                {
                    ShortCode = "1d",
                    TimeSpan ="1 Day",
                    TimeSpanValue = -1
                },
                new TimeSpanOptionsDto()
                {
                    ShortCode = "2d",
                    TimeSpan = "2 Days",
                    TimeSpanValue = -2
                },
                new TimeSpanOptionsDto()
                {
                    ShortCode = "1w",
                    TimeSpan = "1 Week",
                    TimeSpanValue = -7
                }
            };


            var deviceManager =
                RegistryManager.CreateFromConnectionString(CloudConfigurationManager.GetSetting("IoTHubConnectionString"));
            var devices = await deviceManager.GetDevicesAsync(5);

            var allDevices = devices as IList<Device> ?? devices.ToList();
            var activeCount = allDevices.Count(x => x.Status == DeviceStatus.Enabled);

            var availableDevicesItems = allDevices.Where(x => x.Status == DeviceStatus.Enabled).Select(t => new DeviceDTO()
            {
                Name = t.Id,
                Status = t.Status
            });

            var timeSpan = availableTimeSpans.SingleOrDefault(x => x.ShortCode == span)?.TimeSpanValue ?? -1;
            
            if (string.IsNullOrEmpty(selectedDeviceName))
            {
                if (activeCount == 1)
                {
                    selectedDeviceName = availableDevicesItems.Single().Name;
                }else if (activeCount > 1)
                {
                    selectedDeviceName = availableDevicesItems.First().Name;
                }
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("tempandhumid");
            var previousTime = Helpers.DateTimeHelpers.GetUnixTimeStamp(DateTimeOffset.Now.AddDays(timeSpan).Date).ToString();


            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<TempAndHumid> query = new TableQuery<TempAndHumid>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, selectedDeviceName),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, previousTime)));
            var entities = table.ExecuteQuery(query);
            var jsonData = entities.OrderBy(x => x.Timestamp).Select(e => new TempAndHumidDTO()
            {
                Temp = e.temp,
                Timestamp = e.Timestamp.ToLocalTime().ToString("O"),
                Humidity = e.humidity
            });
            var json = JsonConvert.SerializeObject(jsonData);
            ViewBag.Data = json;
            ViewBag.TimeSpans = availableTimeSpans;
            ViewBag.Devices = availableDevicesItems;
            return View();
        }         
    }
}
