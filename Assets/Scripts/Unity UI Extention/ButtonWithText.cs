using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonWithText : Button
{
    private TMP_Text m_Text;
    
    public string Text
    {
        get => m_Text.text;
        set => m_Text.text = value;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (m_Text == null)
        {
            m_Text = GetComponentInChildren<TMP_Text>();
            if (m_Text == null)
            {
                Debug.LogError("ButtonWithText requires a TMP_Text component in its children. Please assign it in the inspector or ensure it exists in the hierarchy.");
            }
        }
    }
}
