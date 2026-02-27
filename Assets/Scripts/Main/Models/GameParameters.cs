using Core;
using System;

namespace Main.Models
{
    [Serializable]
    public struct GameParameters
    {
        public Faction PlayerFaction;
        public bool IsBotUsed;
        public string BlackPlayer;
        public string WhitePlayer;

        public GameParameters(Faction faction, bool isBotUsed, string blackPlayer, string whitePlayer)
        {
            PlayerFaction = faction;
            IsBotUsed = isBotUsed;
            BlackPlayer = blackPlayer;
            WhitePlayer = whitePlayer;
        }
    }
}