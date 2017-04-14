using System;
using System.Collections;
using System.Collections.Generic;
using UnityCommonLibrary.Messaging;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    public new Rigidbody rigidbody { get; private set; }
    public SphereCollider sphere { get; private set; }

    private void OnSessionEnded(MessageData abstrData)
    {
        GameInput.enabled = false;
    }
    private void OnSessionStarted(MessageData abstData)
    {
        GameInput.enabled = true;
    }
    private void Awake()
    {
        Services.locator.Get<IGameService>().player = this;
        rigidbody = GetComponent<Rigidbody>();
        sphere = GetComponentInChildren<SphereCollider>();
        AwakeMovement();
        AwakeCamera();
        AwakeAnimation();
    }
    private void Start()
    {
        StartKatamari();
    }
    private void OnEnable()
    {
        Messaging<GameMessage>.Register(GameMessage.SessionEnded, OnSessionEnded);
        Messaging<GameMessage>.Register(GameMessage.SessionStarted, OnSessionStarted);
        EnableAnimation();
    }
    private void OnDisable()
    {
        DisableAnimation();
    }
    private void Update()
    {
        UpdateMovement();
        UpdateKatamari();
    }
    private void LateUpdate()
    {
        LateUpdateCamera();
        LateUpdateAnimation();
    }
}