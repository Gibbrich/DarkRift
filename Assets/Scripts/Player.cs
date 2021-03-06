﻿using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using AgarPlugin;
using DarkRift;
using DarkRift.Client.Unity;
using UnityEngine;

public class Player : MonoBehaviour
{
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
            {
                writer.Write(transform.position.x);
                writer.Write(transform.position.y);

                using (var message = Message.Create(Tags.MOVE_PLAYER, writer))
                {
                    Client.SendMessage(message, SendMode.Unreliable);
                }
            }

            lastPosition = transform.position;
        }
    }
}