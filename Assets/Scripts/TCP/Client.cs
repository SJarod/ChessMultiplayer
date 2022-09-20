using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;


public class Client : MonoBehaviour
{
    private Socket socket;
    private IPEndPoint remoteEP;

    public int port = 30000;

    public string ipConnect = "localhost";

    public Client()
    {
        IPHostEntry host = Dns.GetHostEntry(ipConnect);
        IPAddress ipAddress = host.AddressList[0];
        socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        remoteEP = new IPEndPoint(ipAddress, port);
    }

    public void Connect()
    {
        IPHostEntry host = Dns.GetHostEntry(ipConnect);
        IPAddress ipAddress = host.AddressList[0];
        try
        {
            socket.Connect(remoteEP);
            Debug.Log("Connected to server at " + ipAddress.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("Error connecting to server " + e.ToString());
            Disconnect();
        }
    }

    public void Disconnect()
    {
        if (socket != null)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {
                Debug.Log("Error shutting down " + e.ToString());
            }
            finally
            {
                socket.Close();
            }
        }
    }

    public void Start()
    {
        Connect();
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    public void SetIP(string IP) => ipConnect = IP;

}
