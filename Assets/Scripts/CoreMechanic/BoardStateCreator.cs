namespace Core
{
    public class BoardStateCreator
    {
        private Faction LastPlayer = Faction.None;
        private Coordinates LastCoordinates = new(-1, -1);
        private Faction[,] Cells = new Faction[8, 8];

        public BoardStateCreator()
        {
            Cells[3, 3] = Faction.Black;
            Cells[4, 4] = Faction.Black;
            Cells[3, 4] = Faction.White;
            Cells[4, 3] = Faction.White;
        }

        public BoardState Build()
        {
            return new BoardState()
            {
                LastPlacedDiscCoordinates = LastCoordinates,
                Cells = Cells
            };
        }

        public BoardStateCreator SetSize(int width, int height)
        {
            Cells = new Faction[width, height];
            return this;
        }

        public BoardStateCreator SetToken(int x, int y, Faction token)
        {
            Cells[x, y] = token;
            return this;
        }

        public BoardStateCreator SetLastPlayer(Faction player)
        {
            LastPlayer = player;
            return this;
        }

        public BoardStateCreator SetLastCoordinates(int x, int y)
        {
            LastCoordinates = new(x, y);
            return this;
        }
    }
}