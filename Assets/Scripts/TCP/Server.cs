using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;

public class Server : MonoBehaviour
{
    static int NbPlayers = 0;

    private Socket serverSkt;
    private List<Socket> clientSkts = new List<Socket>();
    public string serverIP = "127.0.0.1";
    public int port = 30000;
    private IPEndPoint localEP;

    public string gameScene;
    public int maxPlayer = 2;

    private bool waiting = false;

    // Update is called once per frame
    void Update()
    {
        if (waiting)
        {
            WaitForConnection();

            if (clientSkts.Count >= 2)
            {
                foreach (Socket clientSkt in clientSkts)
                {
                    SendStringTo(clientSkt, gameScene);
                }
                waiting = false;
            }
        }
        NbPlayers = clientSkts.Count;
    }

    private void OnDestroy()
    {
        Shutdown();
    }

    public void WaitForConnection()
    {
        //Debug.Log("Waiting for connection");

        try
        {
            Socket skt = serverSkt.Accept();
            clientSkts.Add(skt);
            Debug.Log(skt.LocalEndPoint + " joined the server");
        }
        catch (Exception)
        {
            // no connection attempt
        }
    }

    public void Startup()
    {
        IPHostEntry host = Dns.GetHostEntry(serverIP);
        IPAddress ipAddress = host.AddressList[0].IsIPv6LinkLocal ? host.AddressList[1] : host.AddressList[0];
        Debug.Log("Starting server at " + ipAddress.ToString());
        serverSkt = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        localEP = new IPEndPoint(ipAddress, port);

        try
        {
            serverSkt.Blocking = false;
            serverSkt.Bind(localEP);
            serverSkt.Listen(2);

            waiting = true;
        }
        catch (Exception e)
        {
            Debug.Log("Error starting server " + e.ToString());
            Shutdown();
        }
    }

    public void Shutdown()
    {
        waiting = false;

        foreach (Socket clientSkt in clientSkts)
        {
            if (clientSkt != null)
            {
                try
                {
                    clientSkt.Shutdown(SocketShutdown.Both);
                }
                catch (Exception e)
                {
                    Debug.Log("Error shutting down client socket " + e.ToString());
                }

                clientSkt.Close();
            }
        }

        if (serverSkt != null)
        {
            // no need to shut down server socket
            serverSkt.Close();
        }
    }

    public void SendBoolTo(Socket skt, bool b)
    {
        try
        {
            skt.Send(BitConverter.GetBytes(b));
        }
        catch (Exception e)
        {
            Debug.Log("Error sending bool : " + e.ToString());
        }
    }

    public void SendStringTo(Socket skt, string str)
    {
        try
        {
            byte[] data = Encoding.ASCII.GetBytes(str);
            skt.Send(data);
        }
        catch (Exception e)
        {
            Debug.Log("Error sending string : " + e.ToString());
        }
    }

    public static int GetNbPlayers() => NbPlayers;
}
