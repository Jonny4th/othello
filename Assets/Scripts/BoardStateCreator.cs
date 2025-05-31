public class BoardStateCreator
{
    private Occupancy LastPlayer = Occupancy.None;
    private Coordinates LastCoordinates = new(-1,-1);
    private Occupancy[,] Cells = new Occupancy[8,8];

    public BoardStateCreator()
    {
        Cells[3, 3] = Occupancy.White;
        Cells[4, 4] = Occupancy.White;
        Cells[3, 4] = Occupancy.Black;
        Cells[4, 3] = Occupancy.Black;
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
        Cells = new Occupancy[width, height];
        return this;
    }

    public BoardStateCreator SetToken(int x, int y, Occupancy token)
    {
        Cells[x,y] = token;
        return this;
    }

    public BoardStateCreator SetLastPlayer(Occupancy player)
    {
        LastPlayer = player;
        return this;
    }

    public BoardStateCreator SetLastCoordinates(int x, int y)
    {
        LastCoordinates = new(x,y);
        return this;
    }
}
