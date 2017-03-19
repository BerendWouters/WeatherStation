using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class GraphController : Controller
    {
        // GET: Graph
        public ActionResult Index(string span = null)
        {
            var timeSpan = -1;
            switch (span)
            {
                case "2d":
                    timeSpan = -2;
                    break;
                case "1w":
                    timeSpan = -7;
                    break;
                case "1d":
                default:
                    timeSpan = -1;
                    break;
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
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "device"),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, previousTime)));
            var entities = table.ExecuteQuery(query);
            var jsonData = entities.OrderBy(x => x.Timestamp).Select(e => new DataDTO()
            {
                Temp = e.temp,
                Timestamp = e.Timestamp.ToLocalTime().ToString("O"),
                Humidity = e.humidity
            });
            var json = JsonConvert.SerializeObject(jsonData);
            ViewBag.Data = json;
            return View();
        }
         
    }
    
    

    public class DataDTO
    {
        public double Humidity { get; set; }
        public string Timestamp { get; set; }
        public double Temp { get; set; }
    }
}
