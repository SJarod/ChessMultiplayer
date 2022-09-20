using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine.SceneManagement;
using System.Text;
using System.Threading.Tasks;

public class Client : MonoBehaviour
{
    private Socket socket;
    private IPEndPoint remoteEP;

    public int port = 30000;

    private bool inGame = false;
    private byte[] sceneNamePackage = new byte[1024];

    // Update is called once per frame
    private void Update()
    {
        string sceneName = Encoding.ASCII.GetString(sceneNamePackage);
        if (!inGame && sceneName.ToCharArray()[0] != '\0')
        {
            SceneManager.LoadScene(sceneName);
            inGame = true;
        }
    }

    public void Connect(string ip)
    {
        IPHostEntry host = Dns.GetHostEntry(ip);
        IPAddress ipAddress = host.AddressList[0].IsIPv6LinkLocal ? host.AddressList[1] : host.AddressList[0];
        Debug.Log("Creating client to connect to " + ipAddress.ToString());
        socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        remoteEP = new IPEndPoint(ipAddress, port);

        try
        {
            socket.Connect(remoteEP);
            Debug.Log("Connected to server at " + remoteEP.Address.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("Error connecting to server " + e.ToString());
            Disconnect();
        }

        socket.BeginReceive(sceneNamePackage, 0, 1024, SocketFlags.None, new AsyncCallback(ReadString), socket);
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

    public bool ReceiveBool()
    {
        try
        {
            byte[] data = new byte[2];
            socket.Receive(data);
            return BitConverter.ToBoolean(data);
        }
        catch (Exception e)
        {
            Debug.Log("Error receiving bool : " + e.ToString());
        }

        return false;
    }

    public void ReadString(IAsyncResult ar)
    {
        socket.EndReceive(ar);
    }

    private void OnDestroy()
    {
        Disconnect();
    }
}
