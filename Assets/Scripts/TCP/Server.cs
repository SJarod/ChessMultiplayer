using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;

public class Server : MonoBehaviour
{
    static int NbPlayers = 0;

    private Socket serverSkt;
    private List<Socket> clientSkts = new List<Socket>();
    public string serverIP = "127.0.0.1";
    public int port = 30000;
    private IPEndPoint localEP;

    private bool ready = false;

    // Update is called once per frame
    void Update()
    {
        if (ready)
        {
            WaitForConnection();
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
        catch (Exception e)
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

            ready = true;
        }
        catch (Exception e)
        {
            Debug.Log("Error starting server " + e.ToString());
            Shutdown();
        }
    }

    public void Shutdown()
    {
        ready = false;

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

    public static int GetNbPlayers() => NbPlayers;
}
