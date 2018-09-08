using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using SNetwork;
using PirateGame.Managers;

namespace SNetwork.Client
{
    public class MasterClient : MonoBehaviour
    {

        public Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public bool connecting;
        public bool disconnecting;
        private int bufferSize = 256000;
        private int timeOut = 5000;

        public int ourId = 0;

        public Room room;
        public Invite[] invites;
        public MasterNetworkPlayer[] networkPlayers;
        public KeyValuePairs[] serverData;
        public Match match;
        
        private bool updating;

        public int port;
        public string ip;

        public MasterClientSettingsScriptableObject clientSettings;

        public void Connect(string ip = "127.0.0.1", int port = 100, Action<ResponseMessage> Callback = null, Action CallbackClosed = null)
        {
            if (IsConnectedClient())
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            this.CallbackClosed = CallbackClosed;
            this.ip = ip;
            this.port = port;
            if (connecting && IsConnectedClient() == false)
                return;

            StartCoroutine(ConnectCoroutine(Callback));

            if (!updating)
            {
                updating = true;
            }
        }

        private IEnumerator ConnectCoroutine(Action<ResponseMessage> Callback)
        {
            connecting = true;

            Logging.CreateLog("[SNetworking] Connecting: " + ip + ", " + port);
            int attempts = 0;

            bool failed = false;

            while (!IsConnectedClient())
            {
                failed = false;

                if (attempts >= clientSettings.maxConnAttempts)
                {
                    Logging.CreateLog("[SNetworking] Exceeded Attempts! Canceling connection");
                    break;
                }

                try
                {
                    attempts++;

                    clientSocket.Connect(IPAddress.Parse(ip), port);
                    clientSocket.Connect(IPAddress.Parse(ip), port);
                }
                catch (SocketException)
                {
                    failed = true;
                    Logging.CreateLog("[SNetworking] Failed attempts: " + attempts.ToString() + ". Connecting in " + (clientSettings.retryTime) + " s.");
                }

                if (failed)
                    yield return new WaitForSeconds((clientSettings.retryTime) * 0.001f);

                yield return null;
            }

            connecting = false;

            if (IsConnectedClient())
            {
                ConnectCallback(Callback);
                Logging.CreateLog("[SNetworking] Connected to server");
            }
            else
            {
                Logging.CreateLog("[SNetworking] Connection failed");
                yield break;
            }

            StartCoroutine(Recieve());

            clientSocket.ReceiveTimeout = timeOut;
        }

        public void ConnectCallback(Action<ResponseMessage> Callback)
        {
            ResponseMessage response = new ResponseMessage();
            response.type = ResponseMessage.ResponseType.Success;
            Callback(response);
        }

        public void Disconnect(Action Callback = null)
        {
            CallbackDisconnect = Callback;
            Logging.CreateLog("[SNetworking] Disconnecting");
            disconnecting = true;
            if (IsConnectedClient())
            {
                SendString("/leave");
            }
            DisconnectCallback();
        }

        private Action CallbackDisconnect;
        private Action CallbackClosed;

        private void DisconnectCallback()
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            if (CallbackDisconnect != null)
                CallbackDisconnect();
            Logging.CreateLog("[SNetworking] Disconnected Successfully");
            disconnecting = false;
        }

        public void SendString(string data, int sendCode = 2, int customCode = 0)
        {
            if (!IsConnectedClient())
                return;

            MasterMessaging.instance.SendFinal(Encoding.ASCII.GetBytes(data), 21, sendCode, ourId, customCode, clientSocket);
        }

        public IEnumerator Recieve()
        {
            byte[] recievedBuf = new byte[bufferSize];

            int rec = 0;
            while (IsConnectedClient())
            {
                if (clientSocket.Available > 0)
                {
                    rec = clientSocket.Receive(recievedBuf);
                    byte[] dataByte = new byte[rec];
                    Array.Copy(recievedBuf, dataByte, rec);

                    int id = Convert.ToInt32(dataByte[2]);

                    byte[] customCode = new byte[2];
                    customCode[0] = dataByte[3];
                    customCode[1] = dataByte[4];

                    if (UIManager.instance)
                    {
                        UIManager.instance.RecieveMasterServerCall();
                    }

                    ResponseManager.instance.HandleResponse(dataByte.Skip(5).Take(BitConverter.ToInt16(customCode, 0)).ToArray(), Convert.ToInt32(dataByte[0]), Convert.ToInt32(dataByte[1]), 0, clientSocket, id);
                }
                yield return null;
            }
        }

        public bool IsConnectedClient()
        {
            bool connected = clientSocket.Connected;

            if(connected)
                connected = Network.IsConnected(clientSocket);

            if (!connected && !connecting)
            {
                if (CallbackClosed != null && !disconnecting)
                {
                    CallbackClosed();
                    CallbackClosed = null;
                }
            }

            return connected;
        }

        private MasterNetworkPlayer[] GetNetworkPlayers()
        {
            return networkPlayers;
        }

    }
}
