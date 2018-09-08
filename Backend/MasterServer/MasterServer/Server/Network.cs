using System;
using System.Net.Sockets;

namespace SNetwork
{
    public class Network
    {
        public static bool IsConnected(Socket socket)
        {
            bool connected;
            try
            {
                connected = !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException e)
            {
                connected = false;
            }
            catch(ObjectDisposedException e)
            {
                connected = false;
            }

            if (socket == null)
                return connected;

            return connected;
        }
    }
}