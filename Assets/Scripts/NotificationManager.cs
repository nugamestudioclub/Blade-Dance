using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine;

public class Notification
{
    public string message;
    public float createdTime;
    public Color color;
    public GameObject notificationObject;

    public Notification(string message, Color color)
    {
        this.message = message;
        this.createdTime = Time.time;
        this.color = color;
        this.notificationObject = null;
    }
}

public static class NotificationManager
{
    private static List<Notification> notifications = new List<Notification>();
    private static float notificationLifetimeSeconds = 4.0f;
    private static int spacing_x_distance = 200;

    private static GameObject notificationPrefab = null;

    public static void Init(GameObject prefab)
    {
        notificationPrefab = prefab;
    }

    /**
     * Displays an ERROR notification with the given message.
     * @param message The message to display in the notification.
     */
    public static void NotifyError(string message)
    {
        notifications.Add(new Notification(message, Color.red));
    }

    /**
     * Displays a WARNING notification with the given message.
     * @param message The message to display in the notification.
     */
    public static void NotifyWarning(string message)
    {
        notifications.Add(new Notification(message, Color.yellow));
    }

    /**
     * Displays an INFO notification with the given message.
     * @param message The message to display in the notification.
     */
    public static void NotifyInfo(string message)
    {
        notifications.Add(new Notification(message, Color.green));
    }

    /**
     * Updates the notification system.
     * This shouldn't be called manually.
     */
    public static void Update()
    {
        if (notificationPrefab == null)
        {
            Debug.LogError("NotificationManager not initialized with prefab!");
            return;
        }

        // Ensure there is a place to put notifications
        bool notifAreaExists = false;
        GameObject notifArea = null;
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            notifArea = canvas.transform.Find("NotificationArea").gameObject;
            if (notifArea != null)
            {
                notifAreaExists = true;
            }
        }

        // Handle notification gameobjects
        int index = 0;
        foreach (var notification in notifications)
        {
            // If the notification object has not been created yet, create it. Otherwise, check if it should be destroyed.
            if (notification.notificationObject == null)
            {
                // Create new notification gameobject from prefab
                if (notifAreaExists)
                {
                    // Create notification object and set text & color
                    notification.notificationObject = UnityEngine.Object.Instantiate(notificationPrefab, notifArea.transform);
                    notification.notificationObject.transform.Find("Mainbg").transform.Find("Text").GetComponent<TMP_Text>().text = notification.message;
                    notification.notificationObject.GetComponent<Image>().color = notification.color;

                    // Position notification object based on index
                    RectTransform notifRect = notification.notificationObject.GetComponent<RectTransform>();
                    notifRect.anchoredPosition = new Vector2(-notifRect.rect.width / 2, (notifRect.rect.height / 2) + index * spacing_x_distance);
                }
            }
            else
            {
                // Reposition existing notification gameobject based on index
                if (notifAreaExists)
                {
                    RectTransform notifRect = notification.notificationObject.GetComponent<RectTransform>();
                    notifRect.anchoredPosition = new Vector2(-notifRect.rect.width / 2, (notifRect.rect.height / 2) + index * spacing_x_distance);
                }

                // Delete old notification gameobjects
                if (Time.time - notification.createdTime > notificationLifetimeSeconds)
                {
                    UnityEngine.Object.Destroy(notification.notificationObject);
                    notification.notificationObject = null;
                }
            }

            index++;
        }

        // Remove old notifications from list
        notifications.RemoveAll(n => Time.time - n.createdTime > notificationLifetimeSeconds);
    }
}
