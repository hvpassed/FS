using System.Collections;
using System.Collections.Generic;
using FS.Manager;
using TMPro;
using UnityEngine;

public class PacketStatisticsView : MonoBehaviour
{
    // Start is called before the first frame update

    private NetworkManager _networkManager;
    private TextMeshProUGUI packetInfo;
    void Start()
    {
        _networkManager = NetworkManager.Instance;
        packetInfo = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        packetInfo.text = $"Recv Packet:{_networkManager.receivedPacket}\nSend Packet:{_networkManager.sendedPacket}";
    }
}
