public class OthelloAI
{
    private const int MaxDepth = 5;

    public (int x, int y)? GetBestMove(OthelloState state, bool isBlack)
    {
        int bestScore = isBlack ? int.MinValue : int.MaxValue;
        (int x, int y)? bestMove = null;

        foreach (var move in GetValidMoves(state))
        {
            OthelloState newState = state.Clone();
            newState.ApplyMove(move.x, move.y);
            int score = Minimax(newState, MaxDepth, int.MinValue, int.MaxValue, !isBlack);

            if (isBlack && score > bestScore || !isBlack && score < bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private int Minimax(OthelloState state, int depth, int alpha, int beta, bool maximizingPlayer)
    {
        if (depth == 0 || !state.HasValidMoves())
        {
            return EvaluateBoard(state);
        }

        if (maximizingPlayer)
        {
            int maxEval = int.MinValue;
            foreach (var move in GetValidMoves(state))
            {
                OthelloState newState = state.Clone();
                newState.ApplyMove(move.x, move.y);
                int eval = Minimax(newState, depth - 1, alpha, beta, false);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha)
                    break;
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (var move in GetValidMoves(state))
            {
                OthelloState newState = state.Clone();
                newState.ApplyMove(move.x, move.y);
                int eval = Minimax(newState, depth - 1, alpha, beta, true);
                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);
                if (beta <= alpha)
                    break;
            }
            return minEval;
        }
    }

    private List<(int x, int y)> GetValidMoves(OthelloState state)
    {
        var moves = new List<(int x, int y)>();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (state.IsValidMove(x, y))
                {
                    moves.Add((x, y));
                }
            }
        }
        return moves;
    }

    private int EvaluateBoard(OthelloState state)
    {
        int score = 0;

        (int blackDiscs, int whiteDiscs) = CountDiscs(state);

        int totalDiscs = blackDiscs + whiteDiscs;

        int[,] weights = GetWeightsBasedOnPhase(totalDiscs);

        // Zliczanie oceny planszy
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                score += state.Board[x, y] * weights[x, y];
            }
        }

        int stabilityScore = EvaluateStability(state);
        int mobilityScore = EvaluateMobility(state);

        score += stabilityScore * 1;
        score += mobilityScore * 1;

        return score;
    }
    private int EvaluateStability(OthelloState state)
    {
        int stabilityScore = 0;

        // Ocena rogów
        if (state.Board[0, 0] == 1) stabilityScore += 100; // Lewy górny róg
        if (state.Board[0, 7] == 1) stabilityScore += 100; // Prawy górny róg
        if (state.Board[7, 0] == 1) stabilityScore += 100; // Lewy dolny róg
        if (state.Board[7, 7] == 1) stabilityScore += 100; // Prawy dolny róg

        if (state.Board[0, 0] == -1) stabilityScore -= 100; // Lewy górny róg
        if (state.Board[0, 7] == -1) stabilityScore -= 100; // Prawy górny róg
        if (state.Board[7, 0] == -1) stabilityScore -= 100; // Lewy dolny róg
        if (state.Board[7, 7] == -1) stabilityScore -= 100; // Prawy dolny róg

        // Ocena krawędzi
        for (int i = 0; i < 8; i++)
        {
            if (state.Board[0, i] == 1 || state.Board[7, i] == 1 || state.Board[i, 0] == 1 || state.Board[i, 7] == 1)
            {
                stabilityScore += 10; // Stabilne pionki na krawędzi
            }
            if (state.Board[0, i] == -1 || state.Board[7, i] == -1 || state.Board[i, 0] == -1 || state.Board[i, 7] == -1)
            {
                stabilityScore -= 10; // Przeciwnik na krawędzi
            }
        }

        return stabilityScore;
    }

    private int EvaluateMobility(OthelloState state)
    {
        int mobilityScore = 0;

        var validMovesForPlayer = GetValidMoves(state);
        mobilityScore = validMovesForPlayer.Count;

        // Gracz, który ma więcej ruchów, jest w lepszej pozycji
        return mobilityScore;
    }
    private int[,] GetWeightsBasedOnPhase(int totalDiscs)
    {
        if (totalDiscs < 20)
        {
            return new int[8, 8]  // Wagi dla wczesnej fazy gry
            {
            { 100, -20,  10,   5,   5,  10, -20, 100 },
            { -20, -50,  -2,  -2,  -2,  -2, -50, -20 },
            {  10,  -2,   1,   1,   1,   1,  -2,  10 },
            {   5,  -2,   1,   1,   1,   1,  -2,   5 },
            {   5,  -2,   1,   1,   1,   1,  -2,   5 },
            {  10,  -2,   1,   1,   1,   1,  -2,  10 },
            { -20, -50,  -2,  -2,  -2,  -2, -50, -20 },
            { 100, -20,  10,   5,   5,  10, -20, 100 }
            };
        }
        else
        {
            return new int[8, 8]  // Wagi dla późniejszej fazy gry
            {
            { 200, -40,  20,  10,  10,  20, -40, 200 },
            { -40, -100, -5,  -5,  -5,  -5, -100, -40 },
            { 20,  -5,   3,   3,   3,   3,  -5,  20 },
            { 10,  -5,   3,   3,   3,   3,  -5,  10 },
            { 10,  -5,   3,   3,   3,   3,  -5,  10 },
            { 20,  -5,   3,   3,   3,   3,  -5,  20 },
            { -40, -100, -5,  -5,  -5,  -5, -100, -40 },
            { 200, -40,  20,  10,  10,  20, -40, 200 }
            };
        }
    }
    public (int, int) CountDiscs(OthelloState state)
    {
        int blackDiscs = 0;
        int whiteDiscs = 0;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (state.Board[x, y] == 1) // Czarny pionek
                {
                    blackDiscs++;
                }
                else if (state.Board[x, y] == -1) // Biały pionek
                {
                    whiteDiscs++;
                }
            }
        }

        return (blackDiscs, whiteDiscs);
    }
    public (int x, int y)? GetRandomMove(OthelloState state)
    {
        var availableMoves = new List<(int, int)>();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (state.IsValidMove(x, y))
                {
                    availableMoves.Add((x, y));
                }
            }
        }

        if (availableMoves.Count > 0)
        {
            var random = new Random();
            var randomMove = availableMoves[random.Next(availableMoves.Count)];
            return randomMove;
        }

        return null;
    }

}
