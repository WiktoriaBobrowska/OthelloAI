using Microsoft.Maui.Graphics;
using System;

namespace Othello;

public partial class MainPage : ContentPage
{
	private OthelloState gameState;
	private OthelloAI othelloAI;
	private bool isWeakAI = true;
	private bool isHintsEnabled = false;
	public MainPage()
	{
		InitializeComponent();
		gameState = new OthelloState();
		othelloAI = new OthelloAI();
		InitializeBoard();
		UpdateBoardUI();
		Console.WriteLine(gameState.CurrentPlayer);
		if (gameState.CurrentPlayer == 1)
		{
            PerformAIMoveAsync();
		}

	}

	private void InitializeBoard()
	{
		for (int x = 0; x < 8; x++)
		{
			for (int y = 0; y < 8; y++)
			{
				var cell = new Button
				{

					BackgroundColor = Colors.Gray,
					BorderColor = Colors.Black,
					BorderWidth = 1,
					CommandParameter = new Tuple<int, int>(x, y)
				};
				cell.Clicked += OnCellTapped;
				BoardGrid.Add(cell, x, y);
			}
		}
	}
	private async void PerformAIMoveAsync()
	{
		await Task.Delay(500);

		(int x, int y)? aiMove;

		if (isWeakAI)
		{
			//Console.WriteLine("losowy ruch");
			aiMove = othelloAI.GetRandomMove(gameState);
		}
		else
		{
			//Console.WriteLine("najlepszy ruch");
			aiMove = othelloAI.GetBestMove(gameState, true);
		}
		if (aiMove.HasValue)
		{
			gameState.ApplyMove(aiMove.Value.x, aiMove.Value.y);
			UpdateBoardUI();

			if (!gameState.HasValidMoves())
			{
				int winner = gameState.GetWinner();
				string message = winner == 0 ? "Remis!" : (winner == 1 ? "Czarny wygrywa!" : "Biały wygrywa!");
				await DisplayAlert("Koniec gry", message, "OK");
			}
		}
	}
	private void OnCellTapped(object sender, EventArgs e)
	{
		var button = (Button)sender;
		var coordinates = (Tuple<int, int>)button.CommandParameter;
		int x = coordinates.Item1;
		int y = coordinates.Item2;

		if (gameState.IsValidMove(x, y))
		{
			gameState.ApplyMove(x, y);
			UpdateBoardUI();

			if (!gameState.HasValidMoves())
			{
				int winner = gameState.GetWinner();
				string message = winner == 0 ? "Remis!" : (winner == 1 ? "Czarny wygrywa!" : "Biały wygrywa!");
				DisplayAlert("Koniec gry", message, "OK");
				return;
			}

			if (gameState.CurrentPlayer == 1)
			{
				PerformAIMoveAsync();
			}
		}
		else
		{
			DisplayAlert("Błąd", "Nieprawidłowy ruch.", "OK");
		}
	}
	private void UpdateBestMoveLabel()
	{
        if (gameState.CurrentPlayer == -1)
        {
            if (isHintsEnabled)
            {
                var bestMove = othelloAI.GetBestMove(gameState, false);
                if (bestMove.HasValue)
                {
                    BestMoveLabel.Text = $"Najlepszy ruch dla Białego: ({bestMove.Value.x}, {bestMove.Value.y})";
                }
                else
                {
                    BestMoveLabel.Text = "Brak dostępnych ruchów dla Białego.";
                }
            }
            else
            {
                BestMoveLabel.Text = "";
            }
        }
	}

	private void UpdateBoardUI()
	{
		for (int x = 0; x < 8; x++)
		{
			for (int y = 0; y < 8; y++)
			{
				var cell = (Button)BoardGrid.Children[x * 8 + y];
				int cellValue = gameState.Board[x, y];
				if (cellValue == 1)
				{
					cell.BackgroundColor = Colors.Black;
				}
				else if (cellValue == -1)
				{
					cell.BackgroundColor = Colors.White;
				}
				else
				{
					cell.BackgroundColor = Colors.Gray;
				}
			}
		}

		CurrentPlayerLabel.Text = gameState.CurrentPlayer == 1 ? "Gracz: Czarny" : "Gracz: Biały";
		UpdateBestMoveLabel();
		UpdateScoreLabel();
	}

	private void OnNewGameClicked(object sender, EventArgs e)
	{
		gameState = new OthelloState();
		UpdateBoardUI();
        if (gameState.CurrentPlayer == 1)
        {
            PerformAIMoveAsync();
        }
    }
	private void UpdateScoreLabel()
	{
		(int blackCount, int whiteCount) = othelloAI.CountDiscs(gameState);
		ScoreLabel.Text = $"Czarny: {blackCount} | Biały: {whiteCount}";
	}
	private void OnAITypeToggled(object sender, ToggledEventArgs e)
	{

		isWeakAI = !e.Value; // true, to ustawiamy isWeakAI na false (silne AI)

		string aiMode = isWeakAI ? "Słabe AI" : "Silne AI";
		DisplayAlert("Tryb AI", $"AI jest teraz ustawione na: {aiMode}", "OK");
	}
	private void OnHintsToggled(object sender, ToggledEventArgs e)
	{
		isHintsEnabled = e.Value;
		string hintMode = isHintsEnabled ? "Wskazówki włączone" : "Wskazówki wyłączone";
		DisplayAlert("Tryb wskazówek", hintMode, "OK");
	}
}