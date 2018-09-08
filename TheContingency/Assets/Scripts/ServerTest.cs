using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerTest : MonoBehaviour
{

    public string ip;
    public int port;


    void Start()
    {
        MasterClientManager.instance.Connect(ip, port);

    }

    void Update()
    {

    }
}
