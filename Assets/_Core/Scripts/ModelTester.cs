using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityCommonLibrary;
using UnityCommonLibrary.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Debug class not used in final product.
/// Used to control ModelTester scene.
/// </summary>
public class ModelTester : MonoBehaviour
{
    public new Camera camera;
    public Slider fovSlider;
    private Vector3 defaultCameraPosition;

    public void ResetCamera()
    {
        camera.transform.DOMove(defaultCameraPosition, 0.5f);
        fovSlider.DOValue(60f, 0.5f);
    }
    public void MoveCamera(BaseEventData bData)
    {
        var data = bData as PointerEventData;
        var delta = -data.delta * Time.deltaTime;
        camera.transform.Translate(delta.x, 0f, delta.y, Space.World);
    }
    private IEnumerator Start()
    {
        camera.enabled = false;
        var prefabs = Resources.LoadAll<GameObject>("Prefabs/Objects");
        var instances = new Renderer[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            instances[i] = Instantiate(prefabs[i], Vector3.zero, Quaternion.identity).GetComponent<Renderer>();
        }
        Array.Sort(instances, (i1, i2) => GetScreenArea(i1).CompareTo(GetScreenArea(i2)));
        var lastSize = 0f;
        var lastPosition = Vector3.zero;
        defaultCameraPosition = Vector3.zero;
        for (int i = 0; i < instances.Length; i++)
        {
            var obj = instances[i];
            if (i > 0)
            {
                obj.transform.position =
                    lastPosition +
                    Vector3.right *
                    (obj.bounds.size.x + lastSize);
            }
            lastSize = obj.bounds.size.x;
            lastPosition = obj.transform.position;
            defaultCameraPosition += lastPosition;
            defaultCameraPosition.y += GetLargestSize(obj);
            yield return null;
        }
        defaultCameraPosition.y /= 2.0f * Mathf.Tan(0.5f * camera.fieldOfView * Mathf.Deg2Rad);
        defaultCameraPosition.x /= prefabs.Length;
        defaultCameraPosition.z /= prefabs.Length;
        camera.transform.position = defaultCameraPosition;
        camera.transform.forward = Vector3.down;
        camera.enabled = true;
    }
    private float GetScreenArea(Renderer r)
    {
        return r.bounds.size.x * r.bounds.size.z;
    }
    private float GetLargestSize(Renderer r)
    {
        return Mathf.Max(r.bounds.size.x, r.bounds.size.y, r.bounds.size.z);
    }
}