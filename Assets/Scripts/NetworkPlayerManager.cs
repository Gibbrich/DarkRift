using System.Collections;
using System.Collections.Generic;
using AgarPlugin;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using UnityEngine;

public class NetworkPlayerManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The DarkRift client to communicate on.")]
    UnityClient client;

    Dictionary<ushort, AgarObject> networkPlayers = new Dictionary<ushort, AgarObject>();

    public void Add(ushort id, AgarObject player)
    {
        networkPlayers.Add(id, player);
    }

    private void Awake()
    {
        client.MessageReceived += MessageReceived;
    }

    private void MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        if (e.Tag != Tags.MOVE_PLAYER)
        {
            return;
        }

//        if (e.GetMessage().DataLength != 12)
//        {
//            Debug.LogWarning("Received malformed spawn packet.");
//            return;
//        }
        
        using (DarkRiftReader reader = e.GetMessage().GetReader())
        {
            ushort id = reader.ReadUInt16();
            Vector3 newPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), 0);

            networkPlayers[id].SetMovePosition(newPosition);
            networkPlayers[id].SetRadius(reader.ReadSingle());
        }
    }
}