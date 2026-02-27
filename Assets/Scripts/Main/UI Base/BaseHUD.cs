using Core;

public abstract class BaseHUD : BaseUI
{
    public abstract void SetPlayerName(string black, string white);
    public abstract void SetScore(int black, int white);
    public abstract void SetPlayerTurn(Faction currentPlayer);
}
