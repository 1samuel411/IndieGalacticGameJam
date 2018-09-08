using System;
using System.Net.Sockets;

namespace SNetwork
{
    public struct ResponseMessage
    {
        public enum ResponseType
        {
            Success,
            Failure,
            Full
        }

        public ResponseType type;
    }

    [Serializable]
    public class Response
    {
        public ResponseCallback callback;
        public int customCode;
        public int headerCode;
        public int sendCode;

        public Response(ResponseCallback callback, int headerCode, int sendCode, int customCode)
        {
            this.callback = callback;
            this.headerCode = headerCode;
            this.sendCode = sendCode;
            this.customCode = customCode;
        }

        public Response(ResponseCallback callback, int sendCode, int customCode)
        {
            this.callback = callback;
            this.sendCode = sendCode;
            this.customCode = customCode;
            headerCode = 0;
        }
    }

    public delegate void ResponseCallback(byte[] bytes, Socket fromSocket, int fromId);
}