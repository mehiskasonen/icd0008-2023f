using System.Data;
using Domain;

namespace GameEngine;

public class UnoGameEngine
{
    private readonly Random _rnd = new Random();

    private readonly GameOptions _gameOptions;
    
    public GameState State { get; set; } = new GameState();

    public UnoGameEngine(GameOptions gameOptions)
    {
        _gameOptions = gameOptions;
    }

    public void MakePlayerMove(int playerChoice)
    {
        //PlayerTurn currentTurn = new PlayerTurn();
        PlayerTurn currentTurn = new PlayerTurn()
        {
            Result = ETurnResult.GameStart,
            Card = State.DiscardPile.DiscardedCards.Last(),
            //DeclaredColor = turn.DeclaredColor
            DeclaredColor = State.DiscardPile.DiscardedCards.Last()!.CardColor
        };
        var cardsToRemove = new List<GameCard>();
        
        State.DiscardPile.DiscardedCards.Add(
            State.Players[State.ActivePlayerNo].PlayerHand[playerChoice - 1]
        );
        cardsToRemove.Add(State.Players[State.ActivePlayerNo].PlayerHand[playerChoice - 1]);
        
        cardsToRemove.ForEach(g => State.Players[State.ActivePlayerNo].PlayerHand.Remove(g));
        State.PreviousTurn = currentTurn;
    }
    
    public void SkipPlayerMove()
    {
        //PlayerTurn currentTurn = new PlayerTurn();
        PlayerTurn currentTurn = new PlayerTurn()
        {
            Result = ETurnResult.ForceDraw,
            Card = State.DiscardPile.DiscardedCards.Last(),
            DeclaredColor = State.DiscardPile.DiscardedCards.Last()!.CardColor
        };
        /*var cardsToRemove = new List<GameCard>();
        State.DiscardPile.DiscardedCards.Add(
            State.Players[State.ActivePlayerNo].PlayerHand[playerChoice - 1]
        );
        cardsToRemove.Add(State.Players[State.ActivePlayerNo].PlayerHand[playerChoice - 1]);
        cardsToRemove.ForEach(g => State.Players[State.ActivePlayerNo].PlayerHand.Remove(g));*/
        State.PreviousTurn = currentTurn;
    }

    
    public bool ValidatePlayerMove(int playerChoice)
    {
        var currentPlayer = State.Players[State.ActivePlayerNo];
        var playableCards = GetCardsPlayerCanPlay(currentPlayer);
        
            if (playerChoice > currentPlayer.PlayerHand.Count || playerChoice < 1) return false;

            if (!playableCards.Contains(currentPlayer.PlayerHand[playerChoice - 1])) return false;
        
        return true;
    }

    public bool CheckWildCard(int playerChoice)
    {
        var currentPlayerHand = State.Players[State.ActivePlayerNo].PlayerHand;
        //var playableCards = GetCardsPlayerCanPlay(currentPlayer);

        for (int i = 0; i <= currentPlayerHand.Count; i++)
        {
            if (i == playerChoice - 1 && currentPlayerHand[i].CardColor == EColor.Wild)
            {
                return true;
            }
        }
        return false;
    }
    
    public void UpdatePlayerNickname(Guid playerId, string newNickname)
    {
        var player = State.Players.FirstOrDefault(p => p.Id == playerId);
        if (player != null)
        {
            player.NickName = newNickname;
            // Optionally, you might need to trigger events or methods to inform the game logic about the change.
        }
    }

    public void NextPlayerTurn()
    {
        foreach (var player in State.Players)
        {
            if (player.PlayerHand.Count == 0)
            {
                //Handle win condition
                continue;
            }
            
            player.PlayerHand = player.PlayerHand
                .OrderBy(c => c.CardValue)
                .ThenBy(c => c.CardEffect)
                .ToList();
        }
        State.ActivePlayerNo++;
        if (State.ActivePlayerNo >= State.Players.Count) State.ActivePlayerNo = 0;
    }
    
    public List<GameCard> GetCardsPlayerCanPlay(Player player)
    {
        List<GameCard> cardsPlayerCanPlay = new List<GameCard>();
        GameCard? lastCardInDiscardPile = State.DiscardPile.DiscardedCards.LastOrDefault();
        //Console.WriteLine(currentTurn.DeclaredColor);
        if (player.PlayerHand != null)
            foreach (var card in player.PlayerHand)
            {
                if (lastCardInDiscardPile != null && lastCardInDiscardPile.CardColor == EColor.Wild)
                {
                    if (card.CardColor == lastCardInDiscardPile.CardColor ||
                        //card.CardColor == declaredColor
                        //card.CardColor == currentTurn.DeclaredColor ||
                        card.CardColor == EColor.Wild)
                    {
                        cardsPlayerCanPlay.Add(card);
                    }
                } else if (lastCardInDiscardPile != null && lastCardInDiscardPile.CardColor != EColor.Wild)
                {
                    if (
                        card.CardColor == lastCardInDiscardPile?.CardColor ||
                        card.CardValue == lastCardInDiscardPile?.CardValue ||
                        //card.CardColor == currentTurn.DeclaredColor ||
                        card.CardColor == EColor.Wild)
                    {
                        cardsPlayerCanPlay.Add(card);
                    }
                }
                
            }
        return cardsPlayerCanPlay;
    }


    public void ShuffleAndDistributeCards()
    {
        Console.WriteLine("StartingGameManager");
        DeckBuilder();

        Console.WriteLine("Players count: " + State.Players.Count);
        Console.WriteLine("UnoDeck count: " + State.DrawDeck.Count);
        
        //DeckBuilder drawPile = new DeckBuilder(State.UnoDeck);
        ShuffleCards(State);
        DealCards(State);
        //Add a single card to the discard pile
        InitialDiscard(State);
    }
    
    private void InitialDiscard(GameState state)
    {
        GameCard initialDiscard;
        do
        {
            initialDiscard = DrawCard(state)!;
        } while (initialDiscard.CardValue == EValue.Wild 
                 || initialDiscard.CardValue == EValue.Skip
                 || initialDiscard.CardValue == EValue.Reverse
                 || initialDiscard.CardValue == EValue.DrawTwo           
                 || initialDiscard.CardValue == EValue.DrawFour);
        // while (initialDiscard.CardType == Card.EType.Wild || initialDiscard.CardEffect != Card.EEffect.None);
  
        state.DiscardPile.DiscardedCards.Add(initialDiscard);
    }
    
    public int GetMaxAmountOfPlayers()
    {
        /*var cardsInDeck = _gameOptions.UniqueWildCards ? 4 * 13 : 4 * 9;
        var remainingCards =
            cardsInDeck - 1;*/ // we need the trump card on table at least. Or use any other minimum? Make it an option.
        //return remainingCards / _gameOptions.DeckSize;
        return 7;
    }
    
    public Player GetActivePlayer()
    {
        return State.Players[State.ActivePlayerNo];
    }

    public bool CheckWinner(Player activePlayer)
    {
        return activePlayer.PlayerHand.Count == 0;
    }
    
    public List<GameCard> BuildUnoDeck()
    {
        State.DrawDeck = new List<GameCard>();
        
        
        foreach (EColor color in Enum.GetValues(typeof(EColor)))
        {
            if (color != EColor.Wild)
            {
                for (int i = 1; i <= 9; i++)
                {
                    State.DrawDeck.Add(new GameCard(color, (EValue)i));
                    State.DrawDeck.Add(new GameCard(color, (EValue)i));
                }

                State.DrawDeck.Add(new GameCard(color, EValue.Skip));
                State.DrawDeck.Add(new GameCard(color, EValue.Skip));
                State.DrawDeck.Add(new GameCard(color, EValue.Reverse));
                State.DrawDeck.Add(new GameCard(color, EValue.Reverse));
                State.DrawDeck.Add(new GameCard(color, EValue.DrawTwo));
                State.DrawDeck.Add(new GameCard(color, EValue.DrawTwo));

            }

            //return State.DrawDeck;
        }

        for (int i = 0; i < 4; i++)
        {
            State.DrawDeck.Add(new GameCard(EColor.Wild, EValue.Wild));
            State.DrawDeck.Add(new GameCard(EColor.Wild, EValue.DrawFour));
        }

        return State.DrawDeck;
    }
    
    public List<GameCard> DeckBuilder()
    {
        return BuildUnoDeck();
    }
    
    public void ShuffleCards(GameState state)
    {
        Random rand = new Random();
        int n = state.DrawDeck.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);
            GameCard temp = state.DrawDeck[i];
            state.DrawDeck[i] = state.DrawDeck[j];
            state.DrawDeck[j] = temp;
        }
        
    }
    
    public List<GameCard> Draw(int count)
    {
        var drawnCards = State.DrawDeck.Take(count).ToList();
        State.DrawDeck.RemoveAll(x => drawnCards.Contains(x));

        return drawnCards;
    }

    public GameCard? DrawCard(GameState state)
    {
        try
        {
            if (state.DrawDeck.Count > 0)
            {
                GameCard drawnCard = state.DrawDeck[0];
                state.DrawDeck.Remove(drawnCard);
                return drawnCard;
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Exception in OnPostDrawCard: {ex.Message}");
        }
        return null;
    }

    public void DealCards(GameState state)
    {
        foreach (Player player in state.Players)
        {
            for (int i = 0; i < 7; i++)
            {
                GameCard? card = DrawCard(state);
                player.PlayerHand?.Add(card);
            }
        }
    } 
    
//================================= AI LOGIC ===========================================================
private PlayerTurn ProcessAttack(GameCard currentDiscard, List<GameCard> drawPile)
    {
        PlayerTurn turn = new PlayerTurn
         
        {
            Result = ETurnResult.Attacked,
            Card = currentDiscard,
            DeclaredColor = currentDiscard.CardColor
        };
        var currentPlayer = GetActivePlayer();


        if (currentDiscard.CardEffect == GameCard.EEffect.Skip)
        {
            Console.WriteLine("Player " + State.ActivePlayerNo.ToString() + "was skipped!");
            //Console.WriteLine("Player " + GameState.ActivePlayerNo.ToString() + "was skipped!");
        }
        else if (currentDiscard.CardEffect == GameCard.EEffect.DrawTwo)
        {
            Console.WriteLine("Player " + State.ActivePlayerNo.ToString() + "must draw two cards!");
            currentPlayer.PlayerHand?.AddRange(Draw(2));
        } else if (currentDiscard.CardEffect == GameCard.EEffect.DrawFour)
        {
            Console.WriteLine("Player " + State.ActivePlayerNo.ToString() + "must draw four cards!");
            currentPlayer.PlayerHand?.AddRange(Draw(4));
        }

        return turn;
    }
    
    //Player is going on the offencive
    public PlayerTurn PlayTurn(PlayerTurn previousTurn, List<GameCard> drawPile)
    {
        PlayerTurn turn = new PlayerTurn();
        
        if (previousTurn.Result == ETurnResult.Skip
            || previousTurn.Result == ETurnResult.DrawTwo
            || previousTurn.Result == ETurnResult.WildDrawFour)
        {
            if (previousTurn.Card != null) turn = ProcessAttack(previousTurn.Card, drawPile);
        }
        else if ((previousTurn.Result == ETurnResult.WildCard
                 || previousTurn.Result == ETurnResult.Attacked
                 || previousTurn.Result == ETurnResult.ForceDraw) && previousTurn.Card?.CardColor == EColor.Wild
                 && HasMatch((EColor)previousTurn.DeclaredColor))
        {
            //Player is going on the offencive
            turn = PlayMatchingCard((EColor)previousTurn.DeclaredColor);
        }
        else if (HasMatch(previousTurn.Card))
        {
            if (previousTurn.Card != null) turn = PlayMatchingCard(previousTurn.Card);
        }
        else
        {
            turn = DrawCard(previousTurn, drawPile);
        }

        //DisplayTurn(turn);
        return turn;
    }
    
    private PlayerTurn PlayMatchingCard(EColor previousTurnDeclaredColor)
    {
        var turn = new PlayerTurn();
        turn.Result = ETurnResult.PlayedCard;
        var currentPlayer = GetActivePlayer();

        var matching = currentPlayer.PlayerHand.Where(x => x.CardColor == previousTurnDeclaredColor 
                                                || x.CardColor == EColor.Wild).ToList();
        ////We cannot play wild draw four unless there are no other matches.  But if we can play it, we must.
        if (matching.All(x => x.CardEffect == GameCard.EEffect.DrawFour))
        {
            turn.Card = matching.First();
            turn.DeclaredColor = SelectDominantColor();
            turn.Result = ETurnResult.WildCard;
            currentPlayer.PlayerHand.Remove(matching.First());

            return turn;
        }
        //Otherwise, we play the card that would cause the most damage to the next player.
        if (matching.Any(x => x.CardEffect == GameCard.EEffect.DrawTwo))
        {
            turn.Card = matching.First(x => x.CardEffect == GameCard.EEffect.DrawTwo);
            turn.Result = ETurnResult.DrawTwo;
            turn.DeclaredColor = turn.Card.CardColor;
            currentPlayer.PlayerHand.Remove(turn.Card);

            return turn;
        }
        
        if (matching.Any(x => x.CardEffect == GameCard.EEffect.Skip))
        {
            turn.Card = matching.First(x => x.CardEffect == GameCard.EEffect.Skip);
            turn.Result = ETurnResult.Skip;
            turn.DeclaredColor = turn.Card.CardColor;
            currentPlayer.PlayerHand.Remove(turn.Card);

            return turn;
        }
        
        if (matching.Any(x => x.CardEffect == GameCard.EEffect.Reverse))
        {
            turn.Card = matching.First(x => x.CardEffect == GameCard.EEffect.Reverse);
            turn.Result = ETurnResult.Reversed;
            turn.DeclaredColor = turn.Card.CardColor;
            currentPlayer.PlayerHand.Remove(turn.Card);

            return turn;
        }
        //If we cannot play an "attacking" card, we play any number card
        var matchOnColor = matching.Where(x => x.CardColor == previousTurnDeclaredColor);
        var onColor = matchOnColor as GameCard[] ?? matchOnColor.ToArray();
        if (onColor.Any())
        {
            turn.Card = onColor.First();
            turn.DeclaredColor = turn.Card.CardColor;
            currentPlayer.PlayerHand.Remove(onColor.First());

            return turn;
        }
        //We only play a regular Wild card if we have no other matches
        if (matching.Any(x => x.CardColor == EColor.Wild))
        {
            turn.Card = matching.First(x => x.CardColor == EColor.Wild);
            turn.DeclaredColor = SelectDominantColor();
            turn.Result = ETurnResult.WildCard;
            currentPlayer.PlayerHand.Remove(turn.Card);

            return turn;
        }
        //This should never happen
        turn.Result = ETurnResult.ForceDraw;
        return turn;
    }
    
    //Matching a regular card
    //
    private PlayerTurn PlayMatchingCard(GameCard currentDiscard)
    {
        var currentPlayer = GetActivePlayer();
        
        var turn = new PlayerTurn();
        turn.Result = ETurnResult.PlayedCard;
        var matching = currentPlayer.PlayerHand.Where(x => x.CardColor == currentDiscard.CardColor 
                                                || x.CardValue == currentDiscard.CardValue).ToList();
                                             // || x.CardType == Card.EType.Wild).ToList();

        ////We cannot play wild draw four unless there are no other matches.  But if we can play it, we must.
        if (matching.All(x => x.CardEffect == GameCard.EEffect.DrawFour))
        {
            turn.Card = matching.First();
            turn.DeclaredColor = SelectDominantColor();
            turn.Result = ETurnResult.WildCard;
            currentPlayer.PlayerHand.Remove(matching.First());

            return turn;
        }
        //Otherwise, we play the card that would cause the most damage to the next player.
        if (matching.Any(x => x.CardEffect == GameCard.EEffect.DrawTwo))
        {
            turn.Card = matching.First(x => x.CardEffect == GameCard.EEffect.DrawTwo);
            turn.Result = ETurnResult.DrawTwo;
            turn.DeclaredColor = turn.Card.CardColor;
            currentPlayer.PlayerHand.Remove(turn.Card);

            return turn;
        }
        
        if(matching.Any(x => x.CardEffect == GameCard.EEffect.Skip))
        {
            turn.Card = matching.First(x => x.CardEffect == GameCard.EEffect.Skip);
            turn.Result = ETurnResult.Skip;
            turn.DeclaredColor = turn.Card.CardColor;
            currentPlayer.PlayerHand.Remove(turn.Card);

            return turn;
        }
        
        if(matching.Any(x => x.CardEffect == GameCard.EEffect.Reverse))
        {
            turn.Card = matching.First(x => x.CardEffect == GameCard.EEffect.Reverse);
            turn.Result = ETurnResult.Reversed;
            turn.DeclaredColor = turn.Card.CardColor;
            currentPlayer.PlayerHand.Remove(turn.Card);

            return turn;
        }
        // Assuming he has a match on color AND a match on value 
        // (with none of the matches being attacking cards), 
        // he can choose which to play.  We'll assume 
        // that playing the match with MORE possible matches from his hand 
        // is the better option.
        var matchOnColor = matching.Where(x => x.CardColor == currentDiscard.CardColor);
        var matchOnValue = matching.Where(x => x.CardValue == currentDiscard.CardValue);
        var onColor = matchOnColor as GameCard[] ?? matchOnColor.ToArray();
        var onValue = matchOnValue as GameCard[] ?? matchOnValue.ToArray();
        if (onColor.Any() && onValue.Any())
        {
            if (onColor.Count() > onValue.Count())
            {
                turn.Card = onColor.First();
                turn.DeclaredColor = turn.Card.CardColor;
                currentPlayer.PlayerHand.Remove(onColor.First());

                return turn;
            }
            else
            {
                turn.Card = onValue.First();
                turn.DeclaredColor = turn.Card.CardColor;
                currentPlayer.PlayerHand.Remove(onValue.First());

                return turn;
            }
        }
        
        //Play regular wilds last.  If a wild becomes our last card, we win on the next turn!
        if (matching.Any(x => x.CardValue == EValue.Wild))
        {
            turn.Card = matching.First(x => x.CardValue == EValue.Wild);
            turn.DeclaredColor = SelectDominantColor();
            turn.Result = ETurnResult.WildCard;
            currentPlayer.PlayerHand.Remove(turn.Card);

            return turn;
        }
        
        /*if (matching.Any(x => x.CardType == Card.EType.Wild))
        {
            turn.Card = matching.First(x => x.CardType == Card.EType.Wild);
            turn.DeclaredColor = SelectDominantColor();
            turn.Result = ETurnResult.WildCard;
            PlayerHand.Remove(turn.Card);

            return turn;
        }*/
        //This should never happen
        turn.Result = ETurnResult.ForceDraw;
        return turn;

    }

    /*private bool HasMatch(Card? previousTurnCard)
    {
        return PlayerHand != null 
               && PlayerHand.Any(x => x.CardColor == previousTurnCard?.CardColor 
                                                         || x.CardValue == previousTurnCard?.CardValue 
                                                         || x.CardType == Card.EType.Wild 
                                                         && previousTurnCard?.CardType == Card.EType.Wild);
    }*/
    
    private bool HasMatch(GameCard? previousTurnCard)
    {
        var currentPlayer = GetActivePlayer();

        return currentPlayer.PlayerHand != null 
               && currentPlayer.PlayerHand.Any(x => x.CardColor == previousTurnCard?.CardColor 
                                      || x.CardValue == previousTurnCard?.CardValue 
                                      || x.CardValue == EValue.Wild 
                                      && previousTurnCard?.CardValue == EValue.Wild);
    }
    
    private bool HasMatch(EColor previousTurnDeclaredColor)
    {
        var currentPlayer = GetActivePlayer();

        return currentPlayer.PlayerHand != null 
               && currentPlayer.PlayerHand.Any(x => x.CardColor == previousTurnDeclaredColor || x.CardColor == EColor.Wild);
    }
    
    //Determine the dominant color that a player should choose to declare when they have a Wild card in hand.
    private EColor SelectDominantColor()
    {
     Player currentPlayer = GetActivePlayer();
     if (currentPlayer.PlayerHand == null) throw new ArgumentNullException(nameof(Player.PlayerHand));
     if (currentPlayer.PlayerHand!.Any())
     {
      return EColor.Wild; //player can choose any color.
     }
     var colors = currentPlayer.PlayerHand!.GroupBy(x => x.CardColor)
      .OrderByDescending(x => x.Count());
     return colors.First().First().CardColor;
    }
    
    private PlayerTurn DrawCard(PlayerTurn previousTurn, List<GameCard> deck)
    {
     var currentPlayer = GetActivePlayer();
     PlayerTurn turn = new PlayerTurn();
     var drawnCard = Draw(1);
     currentPlayer.PlayerHand!.AddRange(drawnCard);

     if (HasMatch(previousTurn.Card))
     {
      if (previousTurn.Card != null) turn = PlayMatchingCard(previousTurn.Card);
      turn.Result = ETurnResult.ForceDrawPlay;
     }
     else
     {
      turn.Result = ETurnResult.ForceDraw;
      turn.Card = previousTurn.Card;
     }

     return turn;
    }
    
}