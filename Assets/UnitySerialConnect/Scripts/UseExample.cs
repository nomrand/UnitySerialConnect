using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is a example of using "SerialConnectHandler"
/// </summary>
public class UseExample : MonoBehaviour
{
    public Text recievedText;

    /// <summary>
    /// Data Recieve
    /// This method must be set in "Recieved Event" in "SerialConnectHandler"
    /// (as a Dinamic Parameter Method)
    /// </summary>
    /// <param name="message">Recieved Data</param>
    public void recieved(string message)
    {
        int lightSensor = 0;
        double tempSensor = 0.0;

        // In this example,
        // Recieved Message must be "xxxx,yyyy"
        // xxxx is a int value of Light Sensor
        // yyyy is a double value of Temperature Sensor

        // To use "SerialConnectHandler" correctly,
        // you must check Recieved Data.
        // Because, Serial Port can be used as many purposes,
        // So, this data may not be a true data which you want.
        string[] messages = message.Split(',');
        if (messages.Length < 2)
        {
            // True data has a ',' 
            // and at least 2 parts of values (Light & Temperature)
            // If not correct, it is not a true data
            return;
        }
        // recieved data is a string value,
        // So, you may convert string to numbers
        lightSensor = int.Parse(messages[0]);
        tempSensor = double.Parse(messages[1]);

        recievedText.text = "light:" + lightSensor + "\n" +
                            "temp:" + tempSensor;
    }

    /// <summary>
    /// Data Send
    /// </summary>
    /// <param name="message">Send Data</param>
    public void send(string message)
    {
        Debug.Log("SEND:" + message);
        SerialConnectHandler.Write(message);
    }
}
