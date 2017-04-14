using System.Collections;
using System.Collections.Generic;
using UnityCommonLibrary;
using UnityCommonLibrary.Messaging;
using UnityCommonLibrary.Utility;
using UnityEngine;

public class KatamariObject : MonoBehaviour
{
    /// <summary>
    /// Simple struct used for previewing object in UI
    /// </summary>
    public struct ObjectPreview
    {
        public readonly Mesh mesh;
        public readonly Material material;

        public ObjectPreview(Mesh mesh, Material material)
        {
            this.mesh = mesh;
            this.material = material;
        }
    }

    public string displayName;
    public float massAdd = 0.05f;
    [Range(1f, 10f)]
    public float pickupSizeRequirement = 1f;
    [SerializeField]
    private bool affectsKatamariShape;
    [SerializeField]
    private bool isAttachPoint = true;
    private new Renderer renderer;
    private List<Collider> colliders = new List<Collider>();
    private new Rigidbody rigidbody;
    private MinimapObject minimap;
    /// <summary>
    /// Cache rigidbody null check
    /// </summary>
    private bool hasRigidbody;

    public ObjectPreview preview { get; private set; }
    public Bounds bounds
    {
        get
        {
            return renderer.bounds;
        }
    }

    public bool AttachToKatamari(Player player, Collision collision)
    {
        if (!CanBePickedUp(player))
        {
            return false;
        }
        if (hasRigidbody && !rigidbody.IsSleeping())
        {
            return false;
        }
        // Move to point of impact
        transform.position = collision.contacts[0].thisCollider.ClosestPoint(collision.contacts[0].point);

        if (!isAttachPoint)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                colliders[i].enabled = false;
            }
        }
        // Functional, but requires balancing. Not used by any objects.
        if (affectsKatamariShape)
        {
            gameObject.SetLayerRecursive(LayerMask.NameToLayer("Player"));
            // Offset center of mass as much as possible
            var minDist = Vector3.Distance(bounds.min, player.transform.position);
            var maxDist = Vector3.Distance(bounds.max, player.transform.position);
            var center = minDist > maxDist ? player.transform.InverseTransformPoint(bounds.min) : player.transform.InverseTransformPoint(bounds.max);
            player.rigidbody.centerOfMass = center;
            // After 10 seconds, don't collide with world anymore
            Jobs.ExecuteDelayed(10f, () => gameObject.layer = LayerMask.NameToLayer("StuckObject"));
        }
        else
        {
            transform.up = player.transform.position - transform.position;
            gameObject.SetLayerRecursive(LayerMask.NameToLayer("StuckObject"));
        }
        if (hasRigidbody)
        {
            // Can't make Kinematic, only choice is to destroy rigidbody
            Destroy(rigidbody);
        }
        transform.SetParent(player.sphere.transform, true);
        minimap.spriteRenderer.gameObject.SetActive(false);
        return true;
    }
    private bool CanBePickedUp(Player player)
    {
        return player.katamariMaxRadius >= pickupSizeRequirement;
    }
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        minimap = GetComponent<MinimapObject>();
        hasRigidbody = rigidbody != null;
        gameObject.layer = LayerMask.NameToLayer("Object");

        GetComponentsInChildren(colliders);
        colliders.RemoveAll(c => c.isTrigger);
    }
    private void Start()
    {
        preview = new ObjectPreview(GetComponent<MeshFilter>().sharedMesh, renderer.material);
    }
    private void OnObjectPickedUp(MessageData abstData)
    {
        var game = Services.locator.Get<IGameService>();
        minimap.spriteRenderer.color = CanBePickedUp(game.player) ? Color.green : Color.red;
    }
    private void OnEnable()
    {
        Messaging<GameMessage>.Register(GameMessage.KatamariBoundsUpdated, OnObjectPickedUp);
        Messaging<GameMessage>.Register(GameMessage.SessionStarted, OnObjectPickedUp);
    }
    private void OnDisable()
    {
        Messaging<GameMessage>.RemoveAll(this);
    }
}
