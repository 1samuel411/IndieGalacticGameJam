using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SNetwork
{
	public class Logging : MonoBehaviour
	{

		private static Logging _instance;
		private static Logging instance
		{
			get
			{
				if (!_instance)
				{
					// Create
					GameObject newObj = new GameObject();
					newObj.name = "LoggingManager";
					_instance = newObj.AddComponent<Logging>();
				}
				return _instance;
			}
		}

		private List<string> messageList = new List<string>();

        public void OnGUI()
		{
            /*
		    if (Event.current.type == EventType.Layout)
		    {
		        _messageList = this.messageList;
		    }
		    GUILayout.Space(10);
			GUILayout.BeginVertical();
			for (int i = 0; i < _messageList.Count; i++)
			{
				GUILayout.Label(_messageList[i]);
				GUILayout.Space(5);
			}
			GUILayout.EndVertical();
		*/}

		public static void Clear()
		{
			instance.messageList.Clear();
		}

		public void AddLog(string message)
		{
			if(messageList.Count > 50)
				messageList.RemoveAt(messageList.Count -1);

			messageList.Insert(0, message);
		}

		public static void CreateLog(string message)
		{
			Debug.Log(message);
			instance.AddLog(message);
		}
	}
}
