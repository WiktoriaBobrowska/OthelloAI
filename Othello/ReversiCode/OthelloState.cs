public class OthelloState : IGameState
{
    private const int BoardSize = 8;
    private const int Empty = 0;
    public const int Black = 1;
    private const int White = -1;

    public int[,] Board { get; private set; }
    public int CurrentPlayer { get; private set; }

    public OthelloState()
    {
        Board = InitializeBoard();
        CurrentPlayer = Black;
    }

    private int[,] InitializeBoard()
    {
        int[,] board = new int[BoardSize, BoardSize];
        board[3, 3] = White;
        board[3, 4] = Black;
        board[4, 3] = Black;
        board[4, 4] = White;
        return board;
    }

    public bool IsValidMove(int x, int y)
    {
        if (Board[x, y] != Empty)
            return false;

        int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

        for (int dir = 0; dir < 8; dir++)
        {
            int nx = x + dx[dir];
            int ny = y + dy[dir];
            bool hasOpponentPiece = false;

            while (nx >= 0 && ny >= 0 && nx < BoardSize && ny < BoardSize)
            {
                if (Board[nx, ny] == -CurrentPlayer)
                {
                    hasOpponentPiece = true;
                }
                else if (Board[nx, ny] == CurrentPlayer)
                {
                    if (hasOpponentPiece)
                        return true;
                    else
                        break;
                }
                else
                {
                    break;
                }

                nx += dx[dir];
                ny += dy[dir];
            }
        }

        return false;
    }

    public void ApplyMove(int x, int y)
    {
        int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

        Board[x, y] = CurrentPlayer;

        for (int dir = 0; dir < 8; dir++)
        {
            int nx = x + dx[dir];
            int ny = y + dy[dir];
            var piecesToFlip = new List<(int, int)>();

            while (nx >= 0 && ny >= 0 && nx < BoardSize && ny < BoardSize)
            {
                if (Board[nx, ny] == -CurrentPlayer)
                {
                    piecesToFlip.Add((nx, ny));
                }
                else if (Board[nx, ny] == CurrentPlayer)
                {
                    foreach (var (fx, fy) in piecesToFlip)
                    {
                        Board[fx, fy] = CurrentPlayer;
                    }
                    break;
                }
                else
                {
                    break;
                }

                nx += dx[dir];
                ny += dy[dir];
            }
        }

        CurrentPlayer = -CurrentPlayer;
    }

    public bool HasValidMoves()
    {
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                if (IsValidMove(x, y))
                    return true;
            }
        }
        return false;
    }

    public int GetWinner()
    {
        int blackCount = 0;
        int whiteCount = 0;

        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                if (Board[x, y] == Black)
                    blackCount++;
                else if (Board[x, y] == White)
                    whiteCount++;
            }
        }

        if (blackCount > whiteCount)
            return Black;
        else if (whiteCount > blackCount)
            return White;
        else
            return Empty; // Remis
    }
    public OthelloState Clone()
    {
        var newState = new OthelloState
        {
            Board = (int[,])this.Board.Clone(),
            CurrentPlayer = this.CurrentPlayer
        };
        return newState;
    }
}
