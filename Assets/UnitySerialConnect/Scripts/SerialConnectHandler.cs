/// Copyright [2019] [https://github.com/nomrand]
/// 
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
/// 
///     http://www.apache.org/licenses/LICENSE-2.0
/// 
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Control serial connection in Unity
/// This script is designed to be used by attaching into a Game Object in the Scene.
/// (If the Scene has a Game Object which is attached by this script,
/// it will automatically open/close the Serial Port)
/// Before used, must set the properties in the inspector in Unity.
/// </summary>
/// <example>
/// For Sending (Just call the static method like below)
/// <code>
/// SerialConnectHandler.Write(message);
/// </code>
///
/// For Recieving
/// 1. Make a method with a string parameter like below
/// <code>
/// public void recieved(string message)
/// {
/// 	// Do something ...
/// }
/// </code>
/// 2. Then, set the "Recieved Event" property of the Game Object
/// which is attached by this script
/// (set as a "Dinamic" method, not "Static Parameter" method)
/// </example>
public class SerialConnectHandler : MonoBehaviour
{

    /// <summary>
    /// Event handler which can be called when Serial Connection Data received
    /// (It can be set on inspector window in Unity)
    /// </summary>
    [SerializeField]
    public RecievedEvent receivedEvent;
    [System.Serializable]
    public class RecievedEvent : UnityEvent<string> { }


    /// <summary>
    /// Port name of Serial Connection<br>
    /// Windows : Such as COM1/COM2/... (It can be checked by "Device Manager"<br>
    /// Mac : Such as /dev/tty.usbmodem1421<br>
    /// Linux : Such as /dev/ttyUSB0<br>
    /// </summary>
    public string portName = "COM1";

    /// <summary>
    /// BaudRate of Serial Connection (It must be same with connection destination)
    /// </summary>
    public int baudRate = 9600;


    /// <summary>
    /// Opened Serial Port (It is static for convenience of sending message)
    /// </summary>
    static private SerialPort serialPort_;

    /// <summary>
    /// Recieved Message (When not recieved yet, it must be null)
    /// </summary>
    private string message_;

    private Thread thread_;
    private bool isRunning_ = false;


    /// <summary>
    /// Initialize (open port, etc)
    /// </summary>
    void Start()
    {
        SerialPort serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();

        serialPort_ = serialPort;
        isRunning_ = true;

        // start "Message Read Thread"
        thread_ = new Thread(Read);
        thread_.Start();

        Debug.Log("Open Serial Port(" + portName + ")  baudRate=" + baudRate);
    }

    /// <summary>
    /// Call registered handler method
    /// ("Invoke" can not be called in "Message Read Thread", but main-thread can)
    /// </summary>
    void Update()
    {
        if (message_ != null)
        {
            // message_ is only set when data recieved
            receivedEvent.Invoke(message_);
        }
        message_ = null;
    }

    /// <summary>
    /// Finalize (close port, etc)<br>
    /// This method will be called when "Stop" (in Unity Editor), or stop the application
    /// </summary>
    void OnApplicationQuit()
    {
        isRunning_ = false;

        if (thread_ != null && thread_.IsAlive)
        {
            // if connection is blocking, and not return
            // operation would stop here...
            thread_.Join();
        }

        if (serialPort_ != null && serialPort_.IsOpen)
        {
            serialPort_.Close();
            serialPort_.Dispose();
        }
        serialPort_ = null;
        Debug.Log("Close Serial Port(" + portName + ")");
    }

    /// <summary>
    /// Read from Serial Connection (by "Message Read Thread")
    /// </summary>
    private void Read()
    {
        while (isRunning_ && serialPort_ != null && serialPort_.IsOpen)
        {
            try
            {
                string line = serialPort_.ReadLine();

                // In this "Message Read Thread",
                // we can not control Game Objects in Scene.
                // So, recieved message should be set temporary.
                // And pass to the handler methods indirectly.
                message_ = line;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }

    /// <summary>
    /// Write to Serial Connection (with Line Feed)
    /// </summary>
    static public void Write(string message)
    {
        try
        {
            // In connection destination,
            // Line Feed (end of the line) will be used to understand one chunk of data
            serialPort_.WriteLine(message);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}
