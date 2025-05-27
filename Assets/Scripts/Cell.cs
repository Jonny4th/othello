using UnityEngine;

public class Cell : MonoBehaviour
{
    SpriteRenderer m_Renderer;

    public Bounds m_bound;

    private void Awake()
    {
        m_bound = m_Renderer.bounds;
    }
}
