using UnityEngine;

public class TEst : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;

    public Bounds Bounds;

    private void Start()
    {
        Bounds = spriteRenderer.bounds;
    }
}
