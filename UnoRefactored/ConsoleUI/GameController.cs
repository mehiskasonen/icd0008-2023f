using DAL;
using Domain;
using GameEngine;

namespace ConsoleUI;

public class GameController
{
    private readonly UnoGameEngine _engine;
    private readonly IGameRepository? _repository;

    public GameController(UnoGameEngine engine, IGameRepository? repository)
    {
        _engine = engine;
        _repository = repository;
    }

    public void Run()
    {
        Console.Clear();
        
        PlayerTurn currentTurn = new PlayerTurn()
        {
            Result = ETurnResult.GameStart,
            Card = _engine.State.DiscardPile.DiscardedCards.Last(),
            DeclaredColor = _engine.State.DiscardPile.DiscardedCards.Last()!.CardColor
        };
        
        while (true)
        {
            Console.WriteLine($"The first card at the beginning is {_engine.State.DiscardPile.DiscardedCards.Last()?.CardValue} {_engine.State.DiscardPile.DiscardedCards.Last()?.CardColor}");
            /*if (_engine.State.DiscardPile.DiscardedCards.Last()?.CardColor != EColor.Wild)
            {
                currentTurn.DeclaredColor = _engine.State.DiscardPile.DiscardedCards.Last()!.CardColor;
            }*/
            // one move in loop
            Console.WriteLine(
                $"Player {_engine.State.ActivePlayerNo + 1} - {_engine.State.Players[_engine.State.ActivePlayerNo].NickName}");
            Console.Write("Your turn, make sure you are alone looking at screen! Press enter to continue...");
            Console.ReadLine();

            Console.Clear();

            Console.WriteLine(
                $"Player {_engine.State.ActivePlayerNo + 1} - {_engine.State.Players[_engine.State.ActivePlayerNo].NickName}");
            ConsoleVisualization.DrawDesk(_engine.State);
            bool hasDrawnCard = false;

            while (true)
            {
                
                if (IsAiPlayerActive())
                {
                    
                    currentTurn = _engine.PlayTurn(currentTurn, _engine.State.DrawDeck);
                    if (currentTurn.Card != null)
                    {
                        _engine.State.DiscardPile.DiscardedCards.Add(currentTurn.Card);
                        //Console.WriteLine(currentTurn.Card.CardColor);
                        _engine.State.Players[_engine.State.ActivePlayerNo].PlayerHand.Remove(currentTurn.Card);
                    }
                    DisplayTurn(currentTurn);
                    break;
                }
                else
                {
                    bool hasPlayableCard =
                    _engine.GetCardsPlayerCanPlay(_engine.State.Players[_engine.State.ActivePlayerNo]).Count > 0;

                ConsoleVisualization.DrawPlayerHand(_engine.State.Players[_engine.State.ActivePlayerNo]);

                if (hasPlayableCard)
                {
                    Console.Write(
                        $"Choose card to play 1-{_engine.State.Players[_engine.State.ActivePlayerNo].PlayerHand.Count}, or 'D' to draw a card:");
                }
                else
                {
                    Console.Write("You don't have playable cards. Press 'D' to draw a card:");
                }

                var playerChoiceStr = Console.ReadLine()?.Trim().ToLower();

                if (playerChoiceStr == "d" && !hasDrawnCard)
                {
                    // Draw a card
                    var drawnCard = _engine.Draw(1).FirstOrDefault();
                    if (drawnCard != null)
                    {
                        Console.WriteLine($"You drew: {drawnCard}");
                        _engine.State.Players[_engine.State.ActivePlayerNo].PlayerHand?.Add(drawnCard);
                        hasDrawnCard = true;

                        if (_engine.GetCardsPlayerCanPlay(_engine.State.Players[_engine.State.ActivePlayerNo])
                            .Contains(drawnCard))
                        {
                            Console.WriteLine($"You can play the drawn card: {drawnCard}");
                            Console.Write(
                                $"Choose card to play 1-{_engine.State.Players[_engine.State.ActivePlayerNo].PlayerHand.Count}");
                            ConsoleVisualization.DrawDesk(_engine.State);
                            var playerChoiceStr2 = Console.ReadLine()?.Trim().ToLower();
                            if (!int.TryParse(playerChoiceStr2, out int playerChoice) ||
                                playerChoice < 1 ||
                                playerChoice > _engine.State.Players[_engine.State.ActivePlayerNo].PlayerHand.Count)
                            {
                                ConsoleVisualization.DrawDesk(_engine.State);
                                Console.WriteLine("Invalid input. Please enter a valid card number.");
                                continue;
                            }

                            var playerChoices = playerChoice ;
                            if (_engine.ValidatePlayerMove(playerChoices) == false)
                            {
                                ConsoleVisualization.DrawDesk(_engine.State);
                                Console.WriteLine("Selected cards are not valid");
                                continue;
                            }
                            
                            //Check if playerChoice card is a wild card. If it is, ask for color input.
                            //Check color input validity
                            //If not valid, show message and ask again.
                            if (_engine.CheckWildCard(playerChoices))
                            {
                                Console.Write("Choose next card color (1 - Yellow, 2 - Green, 3 - Red, 4 - Blue): ");
                                ConsoleVisualization.DrawDesk(_engine.State);
                                var playerChoiceColor = Console.ReadLine()?.Trim().ToLower();
                                if (!int.TryParse(playerChoiceColor, out int outPlayerChoiceColor) ||
                                    outPlayerChoiceColor < 1 ||
                                    outPlayerChoiceColor > 4)
                                {
                                    Console.WriteLine("Invalid input. Please enter a valid color number.");
                                    continue;
                                }
                                if (outPlayerChoiceColor == '1')
                                {
                                    currentTurn.DeclaredColor = EColor.Yellow;
                                }
                                if (outPlayerChoiceColor == '2')
                                {
                                    currentTurn.DeclaredColor = EColor.Green;
                                }
                                if (outPlayerChoiceColor == '3')
                                {
                                    currentTurn.DeclaredColor = EColor.Red;
                                }
                                if (outPlayerChoiceColor == '4')
                                {
                                    currentTurn.DeclaredColor = EColor.Blue;
                                }
                            }
                            
                            _engine.MakePlayerMove(playerChoice);
                            //currentTurn.DeclaredColor = _engine.State.DiscardPile.DiscardedCards.Last()!.CardColor;

                            break;
                        }
                        else
                        {
                            Console.WriteLine("The drawn card cannot be played.");
                            if (hasPlayableCard)
                            {
                                var playableCards =
                                    _engine.GetCardsPlayerCanPlay(_engine.State.Players[_engine.State.ActivePlayerNo]);
                                if (playableCards.Count > 0)
                                {
                                    ConsoleVisualization.DrawPlayerHand(
                                        _engine.State.Players[_engine.State.ActivePlayerNo]);
                                    Console.Write(
                                        $"Choose card to play 1-{_engine.State.Players[_engine.State.ActivePlayerNo].PlayerHand.Count}:");
                                    var playerChoiceStr3 = Console.ReadLine()?.Trim().ToLower();
                                    if (!int.TryParse(playerChoiceStr3, out int playerChoice) ||
                                        playerChoice < 1 ||
                                        playerChoice > _engine.State.Players[_engine.State.ActivePlayerNo].PlayerHand
                                            .Count)
                                    {
                                        ConsoleVisualization.DrawDesk(_engine.State);
                                        Console.WriteLine("Invalid input. Please enter a valid card number.");
                                        continue;
                                    }

                                    var playerChoices = new List<int> { playerChoice };
                                    //currentTurn.DeclaredColor = _engine.State.DiscardPile.DiscardedCards.Last()!.CardColor;
                                    if (_engine.ValidatePlayerMove(playerChoice) == false)
                                    {
                                        ConsoleVisualization.DrawDesk(_engine.State);
                                        Console.WriteLine("Selected cards are not valid");
                                        continue;
                                    }
                                    _engine.MakePlayerMove(playerChoice);
                                    //currentTurn.DeclaredColor = _engine.State.DiscardPile.DiscardedCards.Last()!.CardColor;
                                    break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Skipping turn, no cards to play.");
                                _engine.SkipPlayerMove();
                                break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No more cards in the draw deck.");
                        //RESHUFFLE CARDS?
                        break;
                    }
                }

                else if (playerChoiceStr == "d")
                {
                    Console.WriteLine("You have already drawn a card this turn.");
                }

                else
                {
                    hasDrawnCard = false;
                    if (!int.TryParse(playerChoiceStr, out int playerChoice) ||
                        playerChoice < 1 ||
                        playerChoice > _engine.State.Players[_engine.State.ActivePlayerNo].PlayerHand.Count)
                    {
                        ConsoleVisualization.DrawDesk(_engine.State);
                        Console.WriteLine("Invalid input. Please enter a valid card number.");
                        continue;
                    }

                    var playerChoices = new List<int> { playerChoice };
                    if (_engine.ValidatePlayerMove(playerChoice) == false)
                    {
                        ConsoleVisualization.DrawDesk(_engine.State);

                        Console.WriteLine("Selected cards are not valid");
                        continue;
                    }
                    
                    if (_engine.CheckWildCard(playerChoice))
                    {
                        Console.Write("Choose next card color (1 - Yellow, 2 - Green, 3 - Red, 4 - Blue): ");
                        ConsoleVisualization.DrawDesk(_engine.State);
                        var playerChoiceColor = Console.ReadLine()?.Trim().ToLower();
                        if (!int.TryParse(playerChoiceColor, out int outPlayerChoiceColor) ||
                            outPlayerChoiceColor < 1 ||
                            outPlayerChoiceColor > 4)
                        {
                            Console.WriteLine("Invalid input. Please enter a valid color number.");
                            continue;
                        }
                        if (outPlayerChoiceColor == '1')
                        {
                            currentTurn.DeclaredColor = EColor.Yellow;
                        }
                        if (outPlayerChoiceColor == '2')
                        {
                            currentTurn.DeclaredColor = EColor.Green;
                        }
                        if (outPlayerChoiceColor == '3')
                        {
                            currentTurn.DeclaredColor = EColor.Red;
                        }
                        if (outPlayerChoiceColor == '4')
                        {
                            currentTurn.DeclaredColor = EColor.Blue;
                        }

                        Console.WriteLine($"Declared color is {currentTurn.DeclaredColor}");
                    }
                    _engine.MakePlayerMove(playerChoice);
                    //currentTurn.DeclaredColor = _engine.State.DiscardPile.DiscardedCards.Last()!.CardColor;
                    break;
                    }
                }
            }
            
            ConsoleVisualization.DrawDesk(_engine.State);
            if (_engine.CheckWinner(_engine.GetActivePlayer()))
            {
                Console.WriteLine($"Player {_engine.GetActivePlayer().NickName} has won!");
                break;
            }
            _engine.NextPlayerTurn();
        
            _repository?.SaveGame(_engine.State.Id, _engine.State);

            Console.Write("State saved. Continue (Y/N)[Y]?");
            var continueAnswer = Console.ReadLine()?.Trim().ToLower();

            if (continueAnswer is "n") break;
        }
    }
    
    
    private bool IsAiPlayerActive()
    {
        var activePlayerNo = _engine.State.ActivePlayerNo;
    
        // Check if activePlayerNo is a valid index
        if (activePlayerNo >= 0 && activePlayerNo < _engine.State.Players.Count)
        {
            var activePlayer = _engine.State.Players[activePlayerNo];

            // Check if activePlayer is not null and has PlayerType property
            if (activePlayer != null && (activePlayer.PlayerType == EPlayerType.AI || activePlayer.PlayerType == EPlayerType.Human))
            {
                return activePlayer.PlayerType == EPlayerType.AI;
            }
        }

        // Return false if any check fails
        return false;
    }
    
    public void DisplayTurn(PlayerTurn currentTurn)
    {
        var currentPlayer = _engine.State.Players[_engine.State.ActivePlayerNo];
        if (currentTurn.Result == ETurnResult.ForceDraw)
        {
            Console.WriteLine("Player " + currentPlayer.NickName + " is forced to draw.");
        }
        if(currentTurn.Result == ETurnResult.ForceDrawPlay)
        {
            Console.WriteLine("Player " + currentPlayer.NickName + " is forced to draw AND can play the drawn card!");
        }

        if (currentTurn.Result == ETurnResult.PlayedCard
            || currentTurn.Result == ETurnResult.Skip
            || currentTurn.Result == ETurnResult.DrawTwo 
            || currentTurn.Result == ETurnResult.WildCard
            || currentTurn.Result == ETurnResult.WildDrawFour
            || currentTurn.Result == ETurnResult.Reversed
            || currentTurn.Result == ETurnResult.ForceDrawPlay)
        {
            Console.WriteLine("Player " + currentPlayer.NickName + " plays a " + currentTurn.Card + " card.");
            if(currentTurn.Card?.CardColor == EColor.Wild)
            {
                Console.WriteLine("Player " + currentPlayer.NickName + " declares " + currentTurn.DeclaredColor + " as the new color.");
            }
            if(currentTurn.Result == ETurnResult.Reversed)
            {
                Console.WriteLine("Turn order reversed!");
            }
        }

        if (currentPlayer.PlayerHand?.Count == 1)
        {
            Console.WriteLine("Player " + currentPlayer.NickName + " shouts Uno!");
        }    
        
    }

}
    