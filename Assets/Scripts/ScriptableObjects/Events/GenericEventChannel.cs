using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GenericEventChannel", menuName = "Scriptable Objects/GenericEventChannel")]
public abstract class GenericEventChannel<T> : ScriptableObject
{
    private event Action<T> m_Event;

    public void AddListener(Action<T> listener)
    {
        m_Event += listener; 
    }

    public void RemoveListener(Action<T> listener)
    {
        m_Event -= listener;
    }

    public void Invoke(T value)
    {
        m_Event?.Invoke(value);
    }
}
