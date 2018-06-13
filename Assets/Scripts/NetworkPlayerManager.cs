using System.Collections;
using System.Collections.Generic;
using AgarPlugin;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using UnityEngine;

public class NetworkPlayerManager : MonoBehaviour
{
    [SerializeField] [Tooltip("The DarkRift client to communicate on.")]
    UnityClient client;

    Dictionary<ushort, AgarObject> networkPlayers = new Dictionary<ushort, AgarObject>();

    public void Add(ushort id, AgarObject player)
    {
        networkPlayers.Add(id, player);
    }

    public void Destroy(ushort id)
    {
        networkPlayers.Remove(id);
    }
}