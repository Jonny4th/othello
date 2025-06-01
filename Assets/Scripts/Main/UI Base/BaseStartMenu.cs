using Main.Models;
using UnityEngine.Events;

public abstract class BaseStartMenu : BaseUI
{
    public UnityEvent<GameParameters> OnPlayGameRequest;
}
