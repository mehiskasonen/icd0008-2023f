// See https://aka.ms/new-console-template for more information

using System.Text.Json.Serialization;

namespace Domain;

public class GameCard
{
    
    public enum EEffect
    {
        Skip,
        Reverse,
        DrawTwo,
        DrawFour,
        None
    }

    public EColor CardColor { get; set; }
    public EValue CardValue { get; set; }
    // public EType CardType { get; set; }
    public EEffect CardEffect { get; set; }

    [JsonConstructor]
    public GameCard(EColor cardColor, EValue cardValue)
        //, EEffect cardEffect = EEffect.None
    {
        CardColor = cardColor;
        CardValue = cardValue;
    }
    
    
    public override string ToString()
    {
        if (CardValue == EValue.Wild)
        {
            return CardValue.ToString();
        }
        return CardColor + " " + CardValue;
    }
    
}