using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.tvOS;

namespace Networking
{
    public static class Constant
    {
        public const int MAX_PACKAGE_SIZE = 1024;
    }

    public class Package
    {
        public byte[] data = new byte[Constant.MAX_PACKAGE_SIZE];

        public Package(byte[] buffer)
        {
            Array.Copy(buffer, data, Constant.MAX_PACKAGE_SIZE);
        }
    }

    public class TCPSocket
    {
        public Socket skt;
        public List<Package> receivedPkg = new List<Package>();

        private byte[] tempPkgBuffer = new byte[Constant.MAX_PACKAGE_SIZE];

        public TCPSocket(Socket skt)
        {
            this.skt = skt;
        }

        public void SendPackage(byte[] data)
        {
            try
            {
                skt.Send(data);
            }
            catch (Exception e)
            {
                Debug.Log("Error sending package : " + e.ToString());
            }
        }

        public void ReceivePackage()
        {
            skt.BeginReceive(tempPkgBuffer,
                0,
                Constant.MAX_PACKAGE_SIZE,
                SocketFlags.None,
                ReceiveCallback,
                skt);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            skt.EndReceive(ar);
            Package pkg = new Package(tempPkgBuffer);
            Array.Clear(tempPkgBuffer, 0, Constant.MAX_PACKAGE_SIZE);
            receivedPkg.Add(pkg);

            Debug.Log("Package received from " + skt.LocalEndPoint);
        }

        public Package ReadFirstPackage()
        {
            if (receivedPkg.Count == 0)
                return null;

            Package pkg = receivedPkg[0];
            receivedPkg.RemoveAt(0);
            return pkg;
        }

        public void Connect(IPEndPoint ep)
        {
            try
            {
                skt.Connect(ep);
                Debug.Log("Connected to " + ep.Address.ToString());
            }
            catch (Exception e)
            {
                Debug.Log("Error connecting to EndPoint : " + e.ToString());
                Disconnect();
            }
        }

        public void Disconnect(bool shutdown = true)
        {
            if (skt != null)
            {
                try
                {
                    if (shutdown)
                        skt.Shutdown(SocketShutdown.Both);
                }
                catch (Exception e)
                {
                    Debug.Log("Error shutting socket down : " + e.ToString());
                }
                finally
                {
                    skt.Close();
                }
            }
        }
    }
}
