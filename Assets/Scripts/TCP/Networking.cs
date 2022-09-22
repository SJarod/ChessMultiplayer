using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Networking
{
    public static class Constant
    {
        public const int MAX_PACKAGE_SIZE = 1024;
    }

    public class Package
    {
        public byte[] data;

        public Package(byte[] buffer)
        {
            data = new byte[buffer.Length];
            Array.Copy(buffer, data, buffer.Length);
        }
    }

    public class TCPSocket
    {
        public Socket skt;
        public List<Package> receivedPkg = new List<Package>();

        private byte[] rawPkgBuffer = new byte[Constant.MAX_PACKAGE_SIZE];

        public TCPSocket(Socket skt)
        {
            this.skt = skt;
        }

        public void SendPackage(byte[] data)
        {
            try
            {
                byte[] header = BitConverter.GetBytes(data.Length);

                byte[] buff = new byte[1 + header.Length + data.Length];
                buff[0] = 1; // [START OF HEADING]
                Array.Copy(header, 0, buff, 1, header.Length);
                Array.Copy(data, 0, buff, header.Length + 1, data.Length);

                skt.Send(buff);
            }
            catch (Exception e)
            {
                Debug.Log("Error sending package : " + e.ToString());
            }
        }

        public void ReceivePackage()
        {
            skt.BeginReceive(rawPkgBuffer,
                0,
                Constant.MAX_PACKAGE_SIZE,
                SocketFlags.None,
                ReceiveCallback,
                skt);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            skt.EndReceive(ar);

            for (int i = 0; i < rawPkgBuffer.Length; ++i)
            {
                if (rawPkgBuffer[i] == 1) // [START OF HEADING]
                {
                    byte[] header = { rawPkgBuffer[i + 1],
                        rawPkgBuffer[i + 2],
                        rawPkgBuffer[i + 3],
                        rawPkgBuffer[i + 4] };
                    int size = BitConverter.ToInt32(header);

                    byte[] rawPkg = new byte[size];
                    Array.Copy(rawPkgBuffer, i + 5, rawPkg, 0, size);
                    Package pkg = new Package(rawPkg);
                    receivedPkg.Add(pkg);

                    Debug.Log("Package received from " + skt.LocalEndPoint + " : " + Encoding.ASCII.GetString(pkg.data));

                    i += header.Length + size;
                }
                else
                {
                    continue;
                }
            }

            Array.Clear(rawPkgBuffer, 0, Constant.MAX_PACKAGE_SIZE);
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
