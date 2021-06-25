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

    public int GetDice()
    {
        try
        {
            int bytesRec = 0;
            byte[] msg = Encoding.ASCII.GetBytes("GiveMeDice");
            s.Send(msg);
            bytesRec = s.Receive(bytes);
            while (bytesRec == 0) { }
            int dice = Int32.Parse(Encoding.ASCII.GetString(bytes, 0, bytesRec));
            return dice;
        }
        catch
        {
            Debug.Log("Connection lost");
            return 0;
        }
    }


    public void FindServer(Client client)
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
                        }
                        catch
                        {
                            Debug.Log("No Server");
                            listener.Close();
                        }


                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine(e);
                        listener.Close();
                    }
                    finally
                    {
                        listener.Close();
                    }
                }
            }
        }
    }
}