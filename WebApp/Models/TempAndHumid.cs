using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace WebApp.Models
{
    public class TempAndHumid : TableEntity
    {
        public string deviceid { get; set; }
        public double temp { get; set; }
        public double humidity { get; set; }
    }
}