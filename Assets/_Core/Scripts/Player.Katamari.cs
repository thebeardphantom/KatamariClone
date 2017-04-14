using UnityEngine;
using UnityCommonLibrary.Colliders;
using UnityCommonLibrary.Utility;
using System;
using System.Collections.Generic;
using UnityCommonLibrary.Messaging;
using DG.Tweening;

public partial class Player : MonoBehaviour
{
    [Header("Katamari")]
    [SerializeField]
    private float centerOfMassResetSpeed = 0.002f;
    [SerializeField]
    private float triggerRadiusAdd;
    [SerializeField]
    private AudioCue objPickup;
    [SerializeField]
    private SphereCollider catchTrigger;
    private Vector3 cachedVelocity;
    private Vector3 cachedAngularVelocity;
    public readonly List<KatamariObject> attachedObjects = new List<KatamariObject>();

    public Bounds katamariBounds { get; private set; }
    public float katamariMinRadius { get; private set; }
    public float katamariMaxRadius { get; private set; }
    public float katamariAverageRadius { get; private set; }

    private void OnCollisionEnter(Collision collision)
    {
        var obj = collision.gameObject.GetComponentInParent<KatamariObject>();
        if (obj != null)
        {
            if (!obj.AttachToKatamari(this, collision) || attachedObjects.Contains(obj))
            {
                return;
            }
            attachedObjects.Add(obj);
            rigidbody.mass += obj.massAdd;
            /*
             * Can't use Triggers for detecting objects, not accurate enough.
             * Side effect: small hitches in the early game when collecting objects.
             */
            rigidbody.velocity = cachedVelocity;
            rigidbody.angularVelocity = cachedAngularVelocity;
            objPickup.PlayOneShot(obj.transform.position);
            Services.locator.Get<ILogger>().LogFormat(LogType.Log, "Picked up object: {0}", obj.name);
            UpdateKatamariBounds(obj);
        }
    }
    private void StartKatamari()
    {
        UpdateKatamariBounds(null);
    }
    private void UpdateKatamari()
    {
        // Move bounds to updated center
        var b = katamariBounds;
        b.center = transform.position;
        katamariBounds = b;
        cachedVelocity = rigidbody.velocity;
        cachedAngularVelocity = rigidbody.angularVelocity;
        // Slowly move center of mass back to actual center
        rigidbody.centerOfMass = Vector3.MoveTowards(rigidbody.centerOfMass, transform.InverseTransformPoint(katamariBounds.center), Time.deltaTime * centerOfMassResetSpeed);
    }
    private void UpdateKatamariBounds(KatamariObject obj)
    {
        var bounds = new Bounds(transform.position, Vector3.one * 2f);
        for (int i = 0; i < attachedObjects.Count; i++)
        {
            bounds.Encapsulate(attachedObjects[i].bounds);
        }
        katamariMinRadius = Mathf.Max(katamariMinRadius, Mathf.Min(bounds.extents.x, bounds.extents.y, bounds.extents.z));
        katamariMaxRadius = Mathf.Max(katamariMaxRadius, bounds.extents.x, bounds.extents.y, bounds.extents.z);
        katamariAverageRadius = (katamariMinRadius + katamariMaxRadius) / 2f;
        katamariBounds = bounds;
        sphere.radius = katamariMinRadius;
        Messaging<GameMessage>.Broadcast(
            GameMessage.KatamariBoundsUpdated,
            obj == null ? null : new GenericMessageData<KatamariObject>(obj)
        );
    }
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        Gizmos.DrawWireCube(katamariBounds.center, katamariBounds.size);
        var color = Gizmos.color;
        Gizmos.color = Color.HSVToRGB(Mathf.Repeat(Time.time, 1f), 1f, 1f);
        Gizmos.DrawSphere(rigidbody.worldCenterOfMass, 0.5f);
        Gizmos.color = color;
    }
}
