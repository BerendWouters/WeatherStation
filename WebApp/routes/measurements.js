var azure = require('azure-storage');
var async = require('async');

module.exports = Measurements;

function Measurements(measurement) {
    this.measurement = measurement;
}

Measurements.prototype = {
    showMeasurements: function (req, res) {
        self = this;
        var yesterday = new Date(new Date().getTime() - (24 * 60 * 60 * 1000));
        var date = new Date();
        var query = new azure.TableQuery().where("Timestamp >= ?date?", yesterday).select('deviceid', 'time', 'temp', 'humidity');
        self.measurement.find(query, function itemsFound(error, items) {
            res.render('chart', { title: 'My ToDo List ', measurements: items });
        });
    }
}