using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is just a driver for NotificationManager, which is a static class that handles displaying notifications.
public class NotificationUpdateDriver : MonoBehaviour
{
    [SerializeField] private GameObject notificationPrefab;

    // If notifications end up being allowed in gameplay, then this should be uncommented (and NotificationManager.cs should be updated to work there)
    // void Awake()
    // {
    //     DontDestroyOnLoad(gameObject);
    // }

    void Start()
    {
        NotificationManager.Init(notificationPrefab);
    }

    void Update()
    {
        NotificationManager.Update();
    }
}
