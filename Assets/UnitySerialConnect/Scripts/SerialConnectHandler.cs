using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class SerialConnectHandler : MonoBehaviour
{
    [System.Serializable]
    public class RecievedEvent : UnityEvent<string> { }

    [SerializeField]
    public RecievedEvent receivedEvent;


    public string portName = "COM1";
    public int baudRate = 9600;

    private SerialPort serialPort_;

    static private SerialPort static_serialPort_;

    private Thread thread_;
    private bool isRunning_ = false;

    private string message_;
    private bool isNewMessageReceived_ = false;

    // Start is called before the first frame update
    void Start()
    {
        serialPort_ = new SerialPort(portName, baudRate);
        serialPort_.Open();

        static_serialPort_ = serialPort_;
        isRunning_ = true;

        thread_ = new Thread(Read);
        thread_.Start();

        Debug.Log("Open Serial Port(" + portName + ")  baudRate=" + baudRate);
    }

    void Update()
    {
        if (isNewMessageReceived_)
        {
            receivedEvent.Invoke(message_);
        }
        isNewMessageReceived_ = false;
    }

    void OnApplicationQuit()
    {
        isRunning_ = false;
        static_serialPort_ = null;
        Debug.Log("Close Serial Port(" + portName + ")");

        if (thread_ != null && thread_.IsAlive)
        {
            thread_.Join();
        }

        if (serialPort_ != null && serialPort_.IsOpen)
        {
            serialPort_.Close();
            serialPort_.Dispose();
        }
    }

    private void Read()
    {
        while (isRunning_ && serialPort_ != null && serialPort_.IsOpen)
        {
            try
            {
                string line = serialPort_.ReadLine();
                message_ = line;
                isNewMessageReceived_ = true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }

    static public void Write(string message)
    {
        try
        {
            static_serialPort_.Write(message);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}
