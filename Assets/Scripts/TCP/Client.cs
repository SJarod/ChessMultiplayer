using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine.SceneManagement;
using System.Text;
using System.Threading.Tasks;
using Networking;

public class Client : MonoBehaviour
{
    private TCPSocket socket;
    private IPEndPoint remoteEP;

    public int port = 30000;

    private bool inGame = false;

    // Update is called once per frame
    private void Update()
    {
        Package pkg = socket.ReadFirstPackage();
        if (pkg == null)
            return;

        string sceneName = Encoding.ASCII.GetString(pkg.data);
        if (!inGame)
        {
            SceneManager.LoadScene(sceneName);
            inGame = true;
        }
    }

    public void ConnectTo(string ip)
    {
        IPHostEntry host = Dns.GetHostEntry(ip);
        IPAddress ipAddress = host.AddressList[0].IsIPv6LinkLocal ? host.AddressList[1] : host.AddressList[0];
        Debug.Log("Creating client to connect to " + ipAddress.ToString());
        socket = new TCPSocket(new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp));
        remoteEP = new IPEndPoint(ipAddress, port);

        socket.Connect(remoteEP);

        socket.ReceivePackage();
    }

    private void OnDestroy()
    {
        socket.Disconnect();
    }
}
