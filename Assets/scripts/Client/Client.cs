using Newtonsoft.Json.Linq;
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


public class Client

{

    bool connected = false;
    byte[] bytes = new byte[1024];
    Socket s = new Socket(AddressFamily.InterNetwork,
    SocketType.Stream,
    ProtocolType.Tcp);
  
    public bool Connect(string host, int port)
    {
        try {
            s = new Socket(AddressFamily.InterNetwork,
    SocketType.Stream,
    ProtocolType.Tcp);
            Debug.Log("Connecting");
            Debug.Log(host +":"+ port);
            s.Connect(host, port);
            connected = true;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void ConnectionTester(Client client)
    {
        while (true)
        {
            if (connected)
            {     
                Debug.Log("Testing");
                try
                {

                    s.Send(Encoding.ASCII.GetBytes("?"));
                }
                catch
                {
                    connected = false;
                }
                Thread.Sleep(2000);
            }
        }

    }

    public void Test()
    {
            try
            {
                int bytesRec = 0;
                byte[] msg = Encoding.ASCII.GetBytes("JustTestMe");
                s.Send(msg);
                bytesRec = s.Receive(bytes);
                while (bytesRec == 0) { }
                string cmd = Encoding.ASCII.GetString(bytes, 0, bytesRec).Replace("?","");
                Debug.Log(cmd);
            }
            catch
            {
                Debug.Log("Connection lost");
            }
    }

    public void ReplyHandler()
    {
        try
        {
            int bytesRec = 0;
            string reply = "";
            if (reply == "")
            {
                bytesRec = s.Receive(bytes);
                while (bytesRec == 0)
                {
                }

                reply = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                reply = reply.Replace("?", "");
                Debug.LogError(reply);
                var JsonReply = JObject.Parse(reply);
                if(JsonReply["Type"].ToString()=="New")
                {
                    Debug.Log("New");
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Connection lost");
        }
    }

    public int GetDice()
    {
        try
        {
            int bytesRec = 0;
            byte[] msg = Encoding.ASCII.GetBytes("GiveMeDice");
            s.Send(msg);
            string reply = "";
            if (reply == "")
            {
                bytesRec = s.Receive(bytes);
                while (bytesRec == 0)
                {
                }

                reply = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                reply = reply.Replace("?", "");
                Debug.LogError(reply);
            }

            int dice = Int32.Parse(reply.Replace("?",""));
            Debug.LogError(dice);
            return dice;
        }
        catch(Exception e)
        {
            Debug.Log(e);
            Debug.Log("Connection lost");
            return 0;
        }
    }


    public bool FindServer(Client client)
    {
        while (true)
        {
            if (!connected)
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
                            listener.Close();
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
                    return false;
                }
            }
        }
    }
}
