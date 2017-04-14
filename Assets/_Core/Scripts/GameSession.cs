using UnityEngine;
using UnityCommonLibrary.Messaging;
using System;

public class GameSession
{
    public float duration;
    public float targetRadius;
    private float startTime;
    private bool lowTimeLeftEventFired;
    private GameObject objectsContainer;

    public float timeLeft { get; private set; }

    public GameSession(TimeSpan duration, float targetRadius)
    {
        this.duration = (float)duration.TotalSeconds;
        this.targetRadius = targetRadius;
        startTime = Time.time;
        objectsContainer = GameObject.FindWithTag("ObjectsContainer");
        Update();
    }
    public void Update()
    {
        timeLeft = duration - (Time.time - startTime);
        if (!lowTimeLeftEventFired && timeLeft <= 10f)
        {
            lowTimeLeftEventFired = true;
            Messaging<GameMessage>.Broadcast(GameMessage.SessionEndingSoon);
        }
        if (timeLeft <= 0f || objectsContainer.transform.childCount == 0)
        {
            Messaging<GameMessage>.Broadcast(GameMessage.SessionEnded);
        }
    }
}
