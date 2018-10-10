using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;
using SNetwork;
using SNetwork.Server;
using MasterServerProj.DataModels;

namespace SNetwork
{
    public class ByteParser
    {
        public static string ConvertToASCII(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public static byte[] ConvertASCIIToBytes(string data)
        {
            Regex.Replace(data, @"\s+", "");
            return Encoding.UTF8.GetBytes(data);
        }

        public static MasterNetworkPlayer[] ConvertToNetworkPlayers(byte[] data)
        {
            string jsonString = ConvertToASCII(data);
            return JsonConvert.DeserializeObject<MasterNetworkPlayer[]>(jsonString);
        }

        public static byte[] ConvertNetworkPlayersToBytes(MasterNetworkPlayer[] list)
        {
            string jsonString = JsonConvert.SerializeObject(list);
            return ConvertASCIIToBytes(jsonString);
        }

        public static MasterNetworkPlayer ConvertToNetworkPlayer(byte[] data)
        {
            string jsonString = ConvertToASCII(data);
            if(jsonString.EndsWith("}") == false)
            {
                jsonString += "}";
            }
            return JsonConvert.DeserializeObject<MasterNetworkPlayer>(jsonString);
        }

        public static byte[] ConvertNetworkPlayerToBytes(MasterNetworkPlayer list)
        {
            string jsonString = JsonConvert.SerializeObject(list);
            return ConvertASCIIToBytes(jsonString);
        }

        public static byte[] ConvertKeyValuePairsToData(KeyValuePairs[] list)
        {
            string jsonString = JsonConvert.SerializeObject(list);
            return ConvertASCIIToBytes(jsonString);
        }

        public static byte[] ConvertKeyValuePairToData(KeyValuePairs data)
        {
            string jsonString = JsonConvert.SerializeObject(data);
            return ConvertASCIIToBytes(jsonString);
        }

        public static KeyValuePairs[] ConvertDataToKeyValuePairs(byte[] data)
        {
            string jsonString = ConvertToASCII(data);
            return JsonConvert.DeserializeObject<KeyValuePairs[]>(jsonString);
        }

        public static KeyValuePairs ConvertDataToKeyValuePair(byte[] data)
        {
            string jsonString = ConvertToASCII(data);
            if (jsonString.EndsWith("}") == false)
            {
                jsonString += "}";
            }
            return JsonConvert.DeserializeObject<KeyValuePairs>(jsonString);
        }

        public static Room ConvertDataToRoom(byte[] data)
        {
            string jsonString = ConvertToASCII(data);
            return JsonConvert.DeserializeObject<Room>(jsonString);
        }

        public static byte[] ConvertRoomToData(Room room)
        {
            if (room != null)
                room.Refresh();
            string jsonString = JsonConvert.SerializeObject(room);
            return ConvertASCIIToBytes(jsonString);
        }

        public static ControllerInput ConvertDataToInput(byte[] data)
        {
            ControllerInput input = new ControllerInput();
            input.set = true;
            input.change = BitConverter.ToInt32(data, 0);
            return input;
        }

        public static byte[] ConvertInputToData(ControllerInput input)
        {
            byte[] buffer = BitConverter.GetBytes(input.change);
            return buffer;
        }

        public static object[] ConvertDataToObjects(byte[] data)
        {
            string jsonString = ConvertToASCII(data);
            return JsonConvert.DeserializeObject<object[]>(jsonString);
        }

        public static object ConvertDataToObject(byte[] data)
        {
            string jsonString = ConvertToASCII(data);
            return JsonConvert.DeserializeObject<object>(jsonString);
        }

        public static byte[] ConvertObjectToBytes(object Object)
        {
            string jsonString = JsonConvert.SerializeObject(Object);
            return ConvertASCIIToBytes(jsonString);
        }
    }
}
