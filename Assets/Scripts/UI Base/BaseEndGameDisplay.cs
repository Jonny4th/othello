using UnityEngine.Events;

namespace Main.UIs
{
    public abstract class BaseEndGameDisplay : BaseUI
    {
        public UnityEvent OnRestartButtonPressed;

        public abstract void SetWinner(string winner);
        public abstract void SetScore(string score);
    }
}