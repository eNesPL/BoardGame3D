using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json.Linq;

public class Client2 : MonoBehaviour
{
    internal Boolean socketReady = false;
    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;
    bool connected = false;
    void Start()
    {
    }
    void Update()
    {
    }

    public bool FindServer()
    {
        while (!connected)
        {
            using (UdpClient listener = new UdpClient(112))
            {
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 112);
                try
                {

                    Debug.Log("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);

                    Debug.Log($"Received broadcast from {groupEP} :");
                    try
                    {
                        setupSocket(groupEP.Address.ToString(), 111);
                        return true;
                    }
                    catch
                    {
                        Debug.Log("No Server");
                        listener.Close();
                        return false;
                    }


                }
                catch (SocketException e)
                {
                    Console.WriteLine(e);
                    listener.Close();
                    return false;
                }
                finally
                {
                    listener.Close();

                }

            }
        }
        return false;
    }

    public void setupSocket(string host,int port)
    {
        try
        {
            mySocket = new TcpClient(host, port);
            theStream = mySocket.GetStream();
            theWriter = new StreamWriter(theStream);
            theReader = new StreamReader(theStream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e);
        }
    }
    public void writeSocket(string theLine)
    {
        if (!socketReady)
            return;
        String foo = theLine + "\r\n";
        theWriter.Write(foo);
        theWriter.Flush();
    }
    public String readSocket()
    {
        if (!socketReady)
            return "";
        try
        {
            return theReader.ReadLine();
        }
        catch (Exception e)
        {
            return "";
        }
    }
    public string SendCommand(string command)
    {
        try
        {
            writeSocket(command);
            string reply = readSocket();
            return reply;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Connection lost");
            return "";
        }
    }

    public void closeSocket()
    {
        if (!socketReady)
            return;
        theWriter.Close();
        theReader.Close();
        mySocket.Close();
        socketReady = false;
    }

    public int GetDice()
    {
        try
        {
            writeSocket("GiveMeDice");
            string reply = readSocket();
            int dice = int.Parse(reply);
            return dice;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Connection lost");
            return 0;
        }
    }

    public JObject ReplyHandler()
    {
        string reply = readSocket();
        var JsonReply = JObject.Parse(reply);
        return JsonReply;
    }

}
