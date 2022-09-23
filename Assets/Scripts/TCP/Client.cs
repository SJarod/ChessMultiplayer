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
using MyObjSerial;
using UnityEditor;

public class Client : MonoBehaviour
{
    public TCPSocket socket;
    private IPEndPoint remoteEP;

    public int port = 30000;

    private bool inGame = false;
    public bool player1 = true;
    private bool doOnce = false;

    // Update is called once per frame
    private void Update()
    {
        socket.ReceivePackage();

        Package pkg = socket.ReadFirstPackage();
        if (pkg != null && !inGame)
        {
            string sceneName = Encoding.ASCII.GetString(pkg.data);
            SceneManager.LoadScene(sceneName);
            inGame = true;
        }
        else if (pkg != null && inGame)
        {
            if (!doOnce)
            {
                player1 = !BitConverter.ToBoolean(pkg.data);
            }

            if (!player1 && !doOnce)
            {
                Vector3 pos = Camera.main.transform.position;
                Camera.main.transform.position = new Vector3(pos.x, pos.y, -pos.z);
                Vector3 rot = Camera.main.transform.rotation.eulerAngles;
                Camera.main.transform.Rotate(new Vector3(2f * (90f - rot.x), 0f, 0f));
                doOnce = true;
            }
            else if (!doOnce)
            {
                doOnce = true;
            }
            else
            {
                ChessGameMgr chessGameMgr = FindObjectOfType<ChessGameMgr>();
                chessGameMgr.PlayTurn((ChessGameMgr.Move)SerializedMgr.ByteArrayToObject(pkg.data), false);
            }
        }
    }

    public void ConnectTo(string ip)
    {
        IPHostEntry host = Dns.GetHostEntry(ip);
        IPAddress ipAddress = host.AddressList[0].IsIPv6LinkLocal ? host.AddressList[1] : host.AddressList[0];
        Debug.Log("Creating client to connect to " + ipAddress.ToString());
        socket = new TCPSocket(new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp));
        remoteEP = new IPEndPoint(ipAddress, port);

        if (!socket.Connect(remoteEP))
            Destroy(gameObject);

        socket.ReceivePackage();
    }

    private void OnDestroy()
    {
        socket.Disconnect();
    }
}
