using Core;
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

    public void SetToken(Faction token)
    {
        Debug.Log($"Placing token: {token} at cell: {name}");

        switch (token)
        {
            case Faction.None:
                m_SpriteRenderer.gameObject.SetActive(false);
                break;
            case Faction.Black:
                m_SpriteRenderer.gameObject.SetActive(true);
                m_SpriteRenderer.color = Color.black;
                break;
            case Faction.White:
                m_SpriteRenderer.gameObject.SetActive(true);
                m_SpriteRenderer.color = Color.white;
                break;
            default:
                Debug.LogWarning($"Unknown token type: {token}");
                break;
        }
    }
}
