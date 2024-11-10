namespace Domain;

public enum ETurnResult
{
    NoneYet,
    //Start of game.
    GameStart,

    //Player played a normal number card.
    PlayedCard,
        
    //Player played a skip card.
    Skip,

    //Player played a draw two card.
    DrawTwo,

    //Player was forced to draw by other player's card.
    Attacked,

    //Player was forced to draw because s/he couldn't match the current discard.
    ForceDraw,

    //Player was forced to draw because s/he couldn't match the current discard, but the drawn card was playable.
    ForceDrawPlay,

    //Player played a regular wild card.
    WildCard,

    //Player played a draw-four wild card.
    WildDrawFour,

    //Player played a reverse card.
    Reversed,
    
    //Player has chosen an invalid card that he/she should not be able to play.
    Invalid,
    
    //Player chose to quit game.
    Quit
}