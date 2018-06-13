using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using DarkRift;
using DarkRift.Client.Unity;
using UnityEngine;

public class Player : MonoBehaviour
{
    const byte MOVEMENT_TAG = 1;
    const ushort MOVE_SUBJECT = 0;

    [SerializeField]
    [Tooltip("The distance we can move before we send a position update.")]
    float moveDistance = 0.05f;

    public UnityClient Client { get; set; }

    Vector3 lastPosition;

    void Awake()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(lastPosition, transform.position) > moveDistance)
        {
            using (var writer = DarkRiftWriter.Create())
            using (var message = Message.Create(MOVEMENT_TAG, writer))
            {
                writer.Write(transform.position.x);
                writer.Write(transform.position.y);

                Client.SendMessage(message, SendMode.Unreliable);
            }

            lastPosition = transform.position;
        }
    }
}