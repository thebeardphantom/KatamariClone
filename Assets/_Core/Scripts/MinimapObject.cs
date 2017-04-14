using System;
using System.Collections;
using System.Collections.Generic;
using UnityCommonLibrary.Messaging;
using UnityEngine;

public class MinimapObject : MonoBehaviour
{
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private Color color = Color.white;
    [SerializeField]
    private Vector3 scale = Vector3.one;

    public SpriteRenderer spriteRenderer { get; private set; }

    private void Awake()
    {
        spriteRenderer = new GameObject("MinimapObject").AddComponent<SpriteRenderer>();
        spriteRenderer.gameObject.layer = LayerMask.NameToLayer("UI");
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
        spriteRenderer.transform.SetParent(transform, true);
        spriteRenderer.transform.localPosition = Vector3.zero;
        spriteRenderer.transform.forward = Vector3.down;
        spriteRenderer.transform.localScale = Vector3.Scale(spriteRenderer.transform.localScale, scale);
    }
}