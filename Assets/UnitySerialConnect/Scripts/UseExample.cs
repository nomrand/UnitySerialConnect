using UnityEngine;
using UnityEngine.UI;

public class UseExample : MonoBehaviour
{
    public Text recievedText;

    public void recieved(string message)
    {
        int lightSensor = 0;
        double tempSensor = 0.0;
        string[] messages = message.Split(',');

        if (messages.Length < 2)
        {
            return;
        }
        lightSensor = int.Parse(messages[0]);
        tempSensor = double.Parse(messages[1]);

        recievedText.text = "light:" + lightSensor + "\n" +
                            "temp:" + tempSensor;

    }

    public void send(string message)
    {
        Debug.Log("SEND:" + message);
        SerialConnectHandler.Write(message);
    }
}
