var azure = require("azure-storage");
var uuid = require("node-uuid");
var entityGen = azure.TableUtilities.entityGenerator;

module.exports = Measurement;

function Measurement(storageClient, tableName, partitionKey) {
    this.storageClient = storageClient;
    this.tableName = tableName;
    this.partitionKey = partitionKey;
    this.storageClient.createTableIfNotExists(tableName, function tableCreated(error){
        if (error) {
            throw error;
        }
    });
};

Measurement.prototype = {
    find: function (query, callback) {
        self = this;
        self.storageClient.queryEntities(this.tableName, query, null, function entitiesQueried(error, result) {
            if (error) {
                callback(error);
            } else {
                callback(null, result.entries);
            }
        });
    }
}