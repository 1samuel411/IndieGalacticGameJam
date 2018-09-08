using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

namespace SNetwork
{
	public class Network
	{

        public static bool IsConnected(Socket socket)
        {
            if (socket == null)
                return false;
            bool part1 = socket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (socket.Available == 0);

            if (part1 && part2)
                return false;
            else
            {
                return true;
            }
        }
    }
}
