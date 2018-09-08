using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace SNetwork
{
	public struct ResponseMessage
	{
		public enum ResponseType
		{
			Success,
			Failure,
            Full
		};

		public ResponseType type;
	}

    [Serializable]
    public class Response
    {
        public int headerCode;
        public int sendCode;
        public int customCode;

        public ResponseCallback callback;

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