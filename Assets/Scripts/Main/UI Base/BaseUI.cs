using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup m_CanvasGroup;

    protected CanvasGroup CanvasGroup => m_CanvasGroup;

    public virtual void Show()
    {
        m_CanvasGroup.alpha = 1;
        m_CanvasGroup.blocksRaycasts = true;
        m_CanvasGroup.interactable = true;
    }

    public virtual void Hide()
    {
        m_CanvasGroup.alpha = 0;
        m_CanvasGroup.blocksRaycasts = false;
        m_CanvasGroup.interactable = false;
    }
}
