public class BoardStateCreator
{
    private Token LastPlayer = Token.None;
    private Coordinates LastCoordinates = new(-1,-1);
    private Token[,] Cells = new Token[8,8];

    public BoardStateCreator()
    {
        Cells[3, 3] = Token.White;
        Cells[4, 4] = Token.White;
        Cells[3, 4] = Token.Black;
        Cells[4, 3] = Token.Black;
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
        Cells = new Token[width, height];
        return this;
    }

    public BoardStateCreator SetToken(int x, int y, Token token)
    {
        Cells[x,y] = token;
        return this;
    }

    public BoardStateCreator SetLastPlayer(Token player)
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
