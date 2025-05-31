using UnityEngine;

public class CellVisualController : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    Color m_HintColor = Color.yellow;

    public void ShowHintVisual()
    {
        m_SpriteRenderer.gameObject.SetActive(true);
        m_SpriteRenderer.color = m_HintColor;
    }

    public void HideHintVisual()
    {
        m_SpriteRenderer.gameObject.SetActive(false);
    }

    public void SetToken(Occupancy token)
    {
        Debug.Log($"Placing token: {token} at cell: {name}");

        switch (token)
        {
            case Occupancy.None:
                m_SpriteRenderer.gameObject.SetActive(false);
                break;
            case Occupancy.Black:
                m_SpriteRenderer.gameObject.SetActive(true);
                m_SpriteRenderer.color = Color.black;
                break;
            case Occupancy.White:
                m_SpriteRenderer.gameObject.SetActive(true);
                m_SpriteRenderer.color = Color.white;
                break;
            default:
                Debug.LogWarning($"Unknown token type: {token}");
                break;
        }
    }
}
