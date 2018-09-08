using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace SNetwork
{
    public class ResponseManager
    {

        private static ResponseManager _instance;

        public static ResponseManager instance
        {
            get
            {
                if(_instance == null)
                    _instance = new ResponseManager();

                return _instance;
            }
        }

        public List<Response> responses = new List<Response>();

        public void HandleResponse(byte[] bytes, int headerCode, int sendCode, int customCode, Socket fromSocket, int fromId)
        {
            // TODO: Optimize this?s
            responses.ForEach(x =>
            {
                if (x.callback != null)
                {
                    if (x.headerCode == headerCode && headerCode != 0 && customCode == 0)
                    {
                        x.callback(bytes, fromSocket, fromId);
                    }
                    if (x.customCode == customCode && customCode != 0 && headerCode == 0)
                    {
                        x.callback(bytes, fromSocket, fromId);
                    }
                }
            });
        }

        /// <summary>
        /// Adds a response to the list to choose from
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="customCode"></param>
        /// <param name="sendCode"></param>
        public void AddCustomResponse(ResponseCallback callback, int customCode)
        {
            responses.Add(new Response(callback, 0, customCode));
        }

        /// <summary>
        /// Adds a response to the list to choose from
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="headerCode"></param>
        /// <param name="sendCode"></param>
        public void AddServerResponse(ResponseCallback callback, int headerCode)
        {
            responses.Add(new Response(callback, headerCode, 0, 0));
        }

        public void Clear()
        {
            responses.Clear();
        }

        public void ClearCustomResponses()
        {
            responses.ForEach(x =>
            {
                if (x.headerCode == 0) responses.Remove(x);
            });
        }
    }
}
