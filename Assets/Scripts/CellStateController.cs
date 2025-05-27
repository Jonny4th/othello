using UnityEngine;

public class CellStateController : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer m_SpriteRenderer;

    public void UpdateToken(Token token)
    {
        Debug.Log($"Placing token: {token} at cell: {name}");

        m_SpriteRenderer.gameObject.SetActive(true);

        switch (token)
        {
            case Token.Black:
                m_SpriteRenderer.color = Color.black;
                break;
            case Token.White:
                m_SpriteRenderer.color = Color.white;
                break;
            default:
                Debug.LogWarning($"Unknown token type: {token}");
                break;
        }
    }
}
