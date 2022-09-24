using MyObjSerial;
using System;
using System.Collections.Generic;
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

    public class PackageHeader
    {
        public byte[] header;

        public static int size
        {
            get { return 1 + sizeof(int) + sizeof(PackageType); }
        }

        public PackageHeader(int dataSize, PackageType type)
        {
            header = new byte[1 + sizeof(int) + sizeof(PackageType)];

            header[0] = 1; // [START OF HEADING]

            byte[] dataSizeBytes = BitConverter.GetBytes(dataSize);
            Array.Copy(dataSizeBytes, 0, header, 1, dataSizeBytes.Length);

            byte[] typeBytes = BitConverter.GetBytes((int)type);
            Array.Copy(typeBytes, 0, header, 1 + sizeof(int), typeBytes.Length);
        }
    }

    public class Package
    {
        public PackageType type;
        public byte[] data;

        public Package(PackageType type, byte[] buffer)
        {
            this.type = type;
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

        public void SendPackageOfType(PackageType type, byte[] data)
        {
            try
            {
                PackageHeader h = new PackageHeader(data.Length, type);
                byte[] buff = new byte[h.header.Length + data.Length];

                Array.Copy(h.header, 0, buff, 0, h.header.Length);
                Array.Copy(data, 0, buff, h.header.Length, data.Length);

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
                    byte[] sizeB = new byte[sizeof(int)];
                    Array.Copy(rawPkgBuffer, 1, sizeB, 0, sizeof(int));
                    int size = BitConverter.ToInt32(sizeB);

                    byte[] typeB = new byte[sizeof(PackageType)];
                    Array.Copy(rawPkgBuffer, 1 + sizeof(int), typeB, 0, sizeof(PackageType));
                    PackageType type = (PackageType)BitConverter.ToInt32(typeB);

                    byte[] rawPkg = new byte[size];
                    Array.Copy(rawPkgBuffer, i + PackageHeader.size, rawPkg, 0, size);
                    Package pkg = new Package(type, rawPkg);
                    receivedPkg.Add(pkg);

                    Debug.Log("Package received from " + skt.LocalEndPoint + " : " + Encoding.ASCII.GetString(pkg.data));

                    i += PackageHeader.size + rawPkg.Length - 1;
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

        public bool Connect(IPEndPoint ep)
        {
            try
            {
                skt.Connect(ep);
                Debug.Log("Connected to " + ep.Address.ToString());
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("Error connecting to EndPoint : " + e.ToString());
                Disconnect();
                return false;
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
