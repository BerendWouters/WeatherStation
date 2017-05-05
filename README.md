# Arduino Weather Station with Azure IoT Hub

Build status
* WebApp: 
[![Build status](https://ci.appveyor.com/api/projects/status/4kmox89hrm36pheu/branch/master?svg=true)](https://ci.appveyor.com/project/BerendWouters/weatherstation-gxxdh/branch/master)


This repository contains two projects:
* The code for a [Adafruit Huzzah ESP8266](https://www.adafruit.com/product/2471) with onboard a temperature and a humidity sensor (a DHT22). All the components can also be found in the [Azure IoT Starters Kit](https://www.adafruit.com/products/3032). This hardware pushed data to an Azure IoT Hub.
* The code for a ASP.NET MVC5 WebApp contains a simple graphical plot showing the stored data from an Azure TableStorage.
![Screenshot](http://i.imgur.com/9z3wq5q.png)

Not in this repository, but might be added later on: how-to's to configure the Azure services, including:
* Azure IoT Hub
    * Device registration
* Azure TableStorage in a Storage Account
* Azure Streaming Analytics linking the TableStorage with the IoT Hub.
