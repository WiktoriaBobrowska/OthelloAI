public interface IGameState
{
    int[,] Board { get; }
    int CurrentPlayer { get; }
    bool IsValidMove(int x, int y);
    void ApplyMove(int x, int y);
    bool HasValidMoves();
    int GetWinner();
}