using System.Collections;
using System.Collections.Generic;
using AgarPlugin;
using DarkRift.Client;
using DarkRift.Client.Unity;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The DarkRift client to communicate on.")]
    UnityClient client;

    [SerializeField]
    [Tooltip("The controllable player prefab.")]
    GameObject controllablePrefab;

    [SerializeField]
    [Tooltip("The network controllable player prefab.")]
    GameObject networkPrefab;
    
    [SerializeField]
    [Tooltip("The network player manager.")]
    NetworkPlayerManager networkPlayerManager;

    void Awake()
    {
        if (client == null)
        {
            Debug.LogError("Client unassigned in PlayerSpawner.");
            Application.Quit();
        }

        if (controllablePrefab == null)
        {
            Debug.LogError("Controllable Prefab unassigned in PlayerSpawner.");
            Application.Quit();
        }

        if (networkPrefab == null)
        {
            Debug.LogError("Network Prefab unassigned in PlayerSpawner.");
            Application.Quit();
        }

        client.MessageReceived += MessageReceived;
    }

    private void SpawnPlayer(MessageReceivedEventArgs e)
    {
        using (var message = e.GetMessage())
        using (var reader = message.GetReader())
        {
            if (message.Tag != Tags.SPAWN_PLAYER)
            {
                return;
            }

            if (message.DataLength % 17 != 0)
            {
                Debug.LogWarning("Received malformed spawn packet.");
                return;
            }

            while (reader.Position < reader.Length)
            {
                var id = reader.ReadUInt16();
                var position = new Vector3(reader.ReadSingle(), reader.ReadSingle());
                var radius = reader.ReadSingle();
                var color = new Color32(
                    reader.ReadByte(),
                    reader.ReadByte(),
                    reader.ReadByte(),
                    255
                );

                GameObject obj;
                
                if (id == client.ID)
                {
                    obj = Instantiate(controllablePrefab, position, Quaternion.identity);

                    var player = obj.GetComponent<Player>();
                    player.Client = client;

                    Camera.main.GetComponent<CameraFollow>().Target = obj.transform;
                }
                else
                {
                    obj = Instantiate(networkPrefab, position, Quaternion.identity);
                }

                var agarObject = obj.GetComponent<AgarObject>();
                agarObject.SetRadius(radius);
                agarObject.SetColor(color);
                
                networkPlayerManager.Add(id, agarObject);
            }
        }
    }
    
    private void MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        switch (e.Tag)
        {
            case Tags.SPAWN_PLAYER:
                SpawnPlayer(e);
                break;

            case Tags.DESPAWN_PLAYER:
                DespawnPlayer(e);
                break;

            default:
                return;
        }
    }

    private void DespawnPlayer(MessageReceivedEventArgs e)
    {
        using (var reader = e.GetMessage().GetReader())
        {
            var id = reader.ReadUInt16();
            networkPlayerManager.Destroy(id);
        }
    }
}