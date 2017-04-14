using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class VisualsVariants : MonoBehaviour
{
    [SerializeField]
    private Texture2D[] textureVariants;
    [SerializeField]
    private bool useColors;
    [SerializeField]
    private Gradient colorVariants = new Gradient();

    private void Awake()
    {
        var renderer = GetComponent<Renderer>();
        var material = renderer.material;
        if (textureVariants.Length > 0)
        {
            material.mainTexture = textureVariants[Random.Range(0, textureVariants.Length)];
        }
        if (useColors)
        {
            material.color = colorVariants.Evaluate(Random.value);
        }
    }
}
