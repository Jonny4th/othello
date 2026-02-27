using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NullEventChannel", menuName = "Scriptable Objects/NullEventChannel")]
public class NullEventChannel : ScriptableObject
{
    private event Action m_Event;

    public void AddListener(Action listener)
    {
        m_Event += listener;
    }

    public void RemoveListener(Action listener)
    {
        m_Event -= listener;
    }

    public void Invoke()
    {
        m_Event?.Invoke();
    }
}
