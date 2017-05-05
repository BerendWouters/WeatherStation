using Microsoft.Azure.Devices;

namespace WebApp.DTOs
{
    public class DeviceDTO
    {
        public DeviceStatus Status { get; set; }
        public string Name { get; set; }
    }
}