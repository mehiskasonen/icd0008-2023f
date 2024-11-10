using DAL;
using Domain;
using GameEngine;
using MenuSystem;
using Microsoft.EntityFrameworkCore;
using static MenuSystem.EGameType;

namespace ConsoleApp;

public static class ProgramMenus
{
    public static Menu GetOptionsMenu(GameOptions gameOptions) =>
        new Menu("Options", new List<MenuItem>()
        {
            new MenuItem()
            {
                MenuLabelFunction = () => "Deck size - " + gameOptions.DeckSize,
                MethodToRun = () => OptionsChanger.ConfigureHandSize(gameOptions)
            },
            new MenuItem()
            {
                MenuLabelFunction = () => "Use new wild cards - " + (gameOptions.UniqueWildCards ? "yes" : "no"),
                MethodToRun = () => OptionsChanger.ConfigureSmallCards(gameOptions)
            },
        });

    public static Menu GetMainMenu(GameOptions gameOptions,
        Menu optionsMenu,
        Func<string?> newGameMethod,
        Func<string?> loadGameMethod,
        Func<string?> changeRepository,
        Func<string?> aboutMethod) => 
        new Menu($">> U N O !<<", new List<MenuItem>()
        {
            new MenuItem()
            {
                MenuLabel = "Start a new game: ",
                MenuLabelFunction = () => "Start a new game: " + gameOptions,
                MethodToRun = newGameMethod
            },
            new MenuItem()
            {
                MenuLabel = "Load game",
                MethodToRun = loadGameMethod
            },
            new MenuItem()
            {
                MenuLabel = "Options",
                MethodToRun = () => optionsMenu.Run(EMenuLevel.Second)
            },
            new MenuItem()
            {
                MenuLabel = "Change Repository",
                MethodToRun = changeRepository
            },
            new MenuItem()
            {
                MenuLabel = "About",
                //MethodToRun = () => optionsMenu.Run(EMenuLevel.Second)
                MethodToRun = aboutMethod
            },
        });
    
    public static void ChoosePlayType(UnoGameEngine gameEngine)
    {
        var maxPlayers = gameEngine.GetMaxAmountOfPlayers();
        var menuItems = new List<MenuItem>
        {
            new MenuItem { MenuLabel = "1. Human vs Human", MethodToRun = () => PlayerSetup.SetPlayerCount(gameEngine, EPlayerType.Human, EPlayerType.Human) },
            new MenuItem { MenuLabel = "2. Human vs AI", MethodToRun = () => PlayerSetup.SetPlayerCount(gameEngine, EPlayerType.AI, EPlayerType.Human) },
            new MenuItem { MenuLabel = "3. AI vs AI", MethodToRun = () => PlayerSetup.SetPlayerCount(gameEngine, EPlayerType.AI, EPlayerType.AI) }
        };

        var playersMenu = new Menu("Configure Players", menuItems);
        playersMenu.Run();
    }
}