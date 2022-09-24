using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using Networking;
using MyObjSerial;
using System.Net.WebSockets;

public class Server : MonoBehaviour
{
    static int NbPlayers = 0;

    private TCPSocket serverSkt;
    private List<TCPSocket> clientSkts = new List<TCPSocket>();
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

            if (clientSkts.Count >= maxPlayer)
            {
                byte[] sceneName = Encoding.ASCII.GetBytes(gameScene);
                int i = 0;
                foreach (TCPSocket clientSkt in clientSkts)
                {
                    clientSkt.SendPackageOfType(PackageType.STRING, sceneName);
                    byte[] boolByte = new byte[1];
                    boolByte[0] = Convert.ToByte(i++ == 0);
                    clientSkt.SendPackageOfType(PackageType.BOOL, boolByte);
                }
                waiting = false;
        NbPlayers = clientSkts.Count;
            }
        }
        else
        {
            Package pkgP1 = ReadPackageFromSocket(clientSkts[0]);
            Package pkgP2 = ReadPackageFromSocket(clientSkts[1]);

            if (pkgP1 != null)
            {
                clientSkts[1].SendPackageOfType(pkgP1.type, pkgP1.data);
            }
            else if (pkgP2 != null)
            {
                clientSkts[0].SendPackageOfType(pkgP2.type, pkgP2.data);
            }
        }
    }

    public Package ReadPackageFromSocket(TCPSocket skt)
    {
        skt.ReceivePackage();
        Package pkg = skt.ReadFirstPackage();
        return pkg;
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
            Socket skt = serverSkt.skt.Accept();
            clientSkts.Add(new TCPSocket(skt));
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
        serverSkt = new TCPSocket(new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp));
        localEP = new IPEndPoint(ipAddress, port);

        try
        {
            serverSkt.skt.Blocking = false;
            serverSkt.skt.Bind(localEP);
            serverSkt.skt.Listen(maxPlayer);

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

        foreach (TCPSocket clientSkt in clientSkts)
        {
            if (clientSkt != null)
            {
                clientSkt.Disconnect();
            }
        }

        if (serverSkt != null)
        {
            // no need to shut down server socket
            serverSkt.Disconnect(false);
        }
    }

    public static int GetNbPlayers() => NbPlayers;
}
