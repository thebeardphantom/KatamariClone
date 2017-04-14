using System.Collections;
using UnityEngine;

/// <summary>
/// Debug class not used in final product.
/// Randomly (and poorly) spawns objects
/// </summary>
public class JunkSpawner : MonoBehaviour
{
    private const byte SPAWNS_PER_FRAME = 10;

    public GameObject prefab;
    public ushort spawnCount = 10;
    public float radius;

    private IEnumerator Start()
    {
        for (ushort i = 0; i < spawnCount; i++)
        {
            var instance = Instantiate(prefab);
            var position = transform.position;
            var offset = (Random.insideUnitCircle * radius * 0.5f);
            position.x += offset.x;
            position.z += offset.y;
            instance.transform.position = position;
            if (i % SPAWNS_PER_FRAME == 0)
            {
                yield return null;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        var color = Gizmos.color;
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawCube(transform.position, new Vector3(radius, 0f, radius));
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(radius, 0f, radius));
        Gizmos.color = color;
    }
}
