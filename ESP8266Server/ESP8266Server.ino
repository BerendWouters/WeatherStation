#include <NTPClient.h>
#include <TimeLib.h>
#include <Time.h>
#include <ArduinoJson.h>
#include <ESP8266WiFi.h>
#include <WiFiClientSecure.h>
#include <DHT.h>
#include <PubSubClient.h>
#include <WiFiUdp.h>

#define DHTTYPE DHT22
#define DHTPIN  2
#define SENSORDATA_JSON_SIZE (JSON_OBJECT_SIZE(4))

const char* ssid = "Graaf_en_Gravin";
const char* password = "Noorwegen2015";
const char* mqtt_server = "BrixelIoT.azure-devices.net";
const char* deviceId = "device";
const char* devicePath = "brixeliot.azure-devices.net/device";
const char* deviceSAS = "SharedAccessSignature sr=BrixelIoT.azure-devices.net%2Fdevices%2Fdevice&sig=PJig7zogX4sc9NquTAB87zcM2A4gi%2F%2B8V4kcEd5i74I%3D&se=1511107123";
const char* publishPath = "devices/device/messages/events/";
const char* subscriptionPath = "devices/device/messages/devicebound/#";


DHT dht(DHTPIN, DHTTYPE);
WiFiClientSecure espClient;
PubSubClient client(espClient);
WiFiUDP ntpUDP;
NTPClient timeClient(ntpUDP);

float humidity, temp_f;  // Values read from sensor
String webString = "";     // String to display
						   // Generally, you should use "unsigned long" for variables that hold time
unsigned long previousMillisForTemp = 0;        // will store last temp was read
unsigned long previousMillisForHumid = 0;
const long interval = 1000*60;              // interval at which to read sensor

const int led = 0;
long lastMsg = 0;
char msg[50];
int value = 0;

void setup_wifi() {

	delay(10);
	// We start by connecting to a WiFi network
	Serial.println();
	Serial.print("Connecting to ");
	Serial.println(ssid);

	WiFi.begin(ssid, password);

	while (WiFi.status() != WL_CONNECTED) {
		delay(500);
		Serial.print(".");
	}

	randomSeed(micros());

	Serial.println("");
	Serial.println("WiFi connected");
	Serial.println("IP address: ");
	Serial.println(WiFi.localIP());
}
void reconnect() {
	// Loop until we're reconnected
	while (!client.connected()) {
		Serial.print("Attempting MQTT connection...");
		// Attempt to connect
		if (client.connect(deviceId, devicePath, deviceSAS)) {
			client.publish(publishPath, "hello world");
			client.subscribe(subscriptionPath);
		}
		else {
			Serial.print("failed, rc=");
			Serial.print(client.state());
			Serial.println(" try again in 5 seconds");
			// Wait 5 seconds before retrying
			delay(5000);
		}
	}
}
void callback(char* topic, byte* payload, unsigned int length) {
	Serial.print("Message arrived [");
	Serial.print(topic);
	Serial.print("] ");
	for (int i = 0; i < length; i++) {
		Serial.print((char)payload[i]);
	}
	Serial.println();

	// Switch on the LED if an 1 was received as first character
	if ((char)payload[0] == '1') {
		digitalWrite(BUILTIN_LED, LOW);   // Turn the LED on (Note that LOW is the voltage level
										  // but actually the LED is on; this is because
										  // it is acive low on the ESP-01)
	}
	else {
		digitalWrite(BUILTIN_LED, HIGH);  // Turn the LED off by making the voltage HIGH
	}

}

void setup(void) {
	pinMode(led, OUTPUT);
	digitalWrite(led, 0);
	Serial.begin(115200);
	dht.begin();           // initialize temperature sensor
	Serial.println("");
	setup_wifi();
	client.setServer(mqtt_server, 8883);
	client.setCallback(callback);
}

float gethumidity() {
	// Wait at least 2 seconds seconds between measurements.
	// if the difference between the current time and last time you read
	// the sensor is bigger than the interval you set, read the sensor
	// Works better than delay for things happening elsewhere also
	unsigned long currentMillisForHumid = millis();
	// Reading temperature for humidity takes about 250 milliseconds!
	// Sensor readings may also be up to 2 seconds 'old' (it's a very slow sensor)
	humidity = dht.readHumidity();          // Read humidity (percent)
										// Check if any reads failed and exit early (to try again).
	Serial.println(humidity);
	if (isnan(humidity)) {
		Serial.println("Failed to read from DHT sensor!");
		return NULL;
	}
	return humidity;
}

char* serialize(float temp, float humidity, unsigned long unixTime)
{
	char buffer[256];
	Serial.println("Starting serialization");
	StaticJsonBuffer<SENSORDATA_JSON_SIZE> jsonBuffer;
	JsonObject& root = jsonBuffer.createObject();
	root["temp"] = temp;
	root["humidity"] = humidity;
	root["deviceId"] = deviceId;
	root["time"] = unixTime;
	root.prettyPrintTo(Serial);
	root.printTo(buffer, sizeof(buffer));
	return buffer;
}
float gettemperature() {
	// Reading temperature for humidity takes about 250 milliseconds!
	// Sensor readings may also be up to 2 seconds 'old' (it's a very slow sensor)
	temp_f = dht.readTemperature();     // Read temperature as Fahrenheit
										// Check if any reads failed and exit early (to try again).
	Serial.println(temp_f);
	if (isnan(temp_f)) {
		Serial.println("Failed to read from DHT sensor!");
		return NULL;
	}
	return temp_f;
}

void loop(void) {
	if (!client.connected()) {
		reconnect();
	}
	client.loop();
	timeClient.update();
	long now = millis();
	if (now - lastMsg > interval) {
		lastMsg = now;
		
		char tempStr[100];
		char* json = serialize(gettemperature(), gethumidity(), timeClient.getEpochTime());
		Serial.print("Publish message: ");
		Serial.println(json);
		client.publish(publishPath, json);
	}
}