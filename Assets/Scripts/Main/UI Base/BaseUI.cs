using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup m_CanvasGroup;

    protected CanvasGroup CanvasGroup => m_CanvasGroup;

    public virtual void Show()
    {
        m_CanvasGroup.alpha = 1;
    }

    public virtual void Hide()
    {
        m_CanvasGroup.alpha = 0;
    }
}
