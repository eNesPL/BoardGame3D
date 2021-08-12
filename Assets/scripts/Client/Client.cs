﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbag;

public class Client
{

    bool connected = false;
    byte[] bytes = new byte[1024];
    Socket s = new Socket(AddressFamily.InterNetwork,
    SocketType.Stream,
    ProtocolType.Tcp);
    public event Action<DiceData> OnGetDiceValue = delegate { };

    public bool Connect(string host, int port)
    {
        try {
            Debug.Log("Connecting");
            Debug.Log(host +":"+ port);
            this.s.Connect(host, port);
            Debug.LogError(this.s.Connected);
            connected = true;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public JObject ReplyHandler()
    {
        try
        {
            int bytesRec = 0;
            string reply = "";
            if (reply == "")
            {
                bytesRec = this.s.Receive(bytes);
                while (bytesRec == 0)
                {
                }

                reply = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                reply = reply.Replace("?", "");
                Debug.Log(reply);
                var JsonReply = JObject.Parse(reply);
                return JsonReply;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Connection lost");
            return null;
        }
        return null;
    }

    public string SendCommand(string command)
    {
        try
        {
            int bytesRec = 0;
            byte[] msg = Encoding.ASCII.GetBytes(command);
            this.s.Send(msg);
            string reply = "";
            if (reply == "")
            {
                bytesRec = this.s.Receive(bytes);
                while (bytesRec == 0)
                {
                }

                reply = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                reply = reply.Replace("?", "");
                Debug.LogError(reply);
            }

            Debug.LogError(reply);
            return reply;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Connection lost");
            return "";
        }
    }

    public void GetDice()
    {
        Debug.LogError(this.s.Connected);
        try
        {
            int bytesRec = 0;
            byte[] msg = Encoding.ASCII.GetBytes("GiveMeDice");
            this.s.Send(msg);
            
            string reply = "";
            if (reply == "")
            {
                bytesRec = this.s.Receive(bytes);
                while (bytesRec == 0)
                {
                }

                reply = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                reply = reply.Replace("?", "");
                Debug.LogError(reply);
            }

            int dice = Int32.Parse(reply.Replace("?",""));
            Debug.LogError(dice);
            DiceData data = new DiceData(dice);
            Dispatcher.Invoke(() => OnGetDiceValue?.Invoke(data));
        }
        catch(Exception e)
        {
            Debug.Log(e);
            Debug.Log("Connection lost");
            DiceData data = new DiceData(0);
            Dispatcher.Invoke(() => OnGetDiceValue?.Invoke(data));
        }
        
    }


    public bool FindServer(Client client)
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
                            client.Connect(groupEP.Address.ToString(), 111);
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

}
