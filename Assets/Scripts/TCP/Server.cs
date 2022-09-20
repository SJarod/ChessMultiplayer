using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;

public class Server : MonoBehaviour
{
    private Socket serverSkt;
    private Socket clientSkt;
    public string serverIP = "127.0.0.1";
    public int port = 30000;
    private IPEndPoint localEP;

    private bool ready = false;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        Debug.Log("Starting server");

        IPHostEntry host = Dns.GetHostEntry(serverIP);
        IPAddress ipAddress = host.AddressList[0];
        serverSkt = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        localEP = new IPEndPoint(ipAddress, port);

        try
        {
            serverSkt.Blocking = false;
            serverSkt.Bind(localEP);
            serverSkt.Listen(1);

            Debug.Log("Waiting for a connection");

            ready = true;
        }
        catch (Exception e)
        {
            Debug.Log("Error starting server " + e.ToString());
            Shutdown();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ready && clientSkt == null)
        {
            WaitForConnection();
        }
    }

    private void OnDestroy()
    {
        Shutdown();
    }

    public void WaitForConnection()
    {
        try
        {
            clientSkt = serverSkt.Accept();
            Debug.Log(clientSkt.LocalEndPoint + " joined the server");
        }
        catch (Exception e)
        {
            // no connection attempt
        }
    }

    public void Shutdown()
    {
        ready = false;

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

        if (serverSkt != null)
        {
            // no need to shut down server socket
            serverSkt.Close();
        }
    }
}
