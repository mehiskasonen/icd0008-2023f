namespace MenuSystem;
using static System.Console;

public class Menu
{
    private int SelectedIndex;
    public string? Title { get; set; }
    public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    
    
    private void DisplayOptions()
    {
        WriteLine(Title);
        int index = 0;
        foreach (var menuItem in MenuItems)
        {
            string currentOption = menuItem.MenuLabelFunction != null
                ? menuItem.MenuLabelFunction()
                : menuItem.MenuLabel;

            string prefix = (index == SelectedIndex) ? "*" : " ";

            Console.ForegroundColor = (index == SelectedIndex) ? ConsoleColor.Black : ConsoleColor.White;
            Console.BackgroundColor = (index == SelectedIndex) ? ConsoleColor.White : ConsoleColor.Black;
            
            WriteLine($"{prefix} << {currentOption} >>");
            
            Console.ResetColor();
            index++;
        }
    }
    
    private void UpdateSelectedIndex(ConsoleKey keyPressed, EMenuLevel menuLevel)
    {
        int maxIndex = MenuItems.Count - 1;

        if (keyPressed == ConsoleKey.UpArrow)
        {
            SelectedIndex = (SelectedIndex == 0) ? maxIndex : SelectedIndex - 1;
        }
        else if (keyPressed == ConsoleKey.DownArrow)
        {
            SelectedIndex = (SelectedIndex == maxIndex) ? 0 : SelectedIndex + 1;
        }

        // Adjust the selected index based on the menuLevel
        if (menuLevel != EMenuLevel.First)
        {
            // Customize the adjustment based on your specific requirements
            switch (menuLevel)
            {
                case EMenuLevel.Second:
                    // Add logic for adjusting SelectedIndex in the second menu level
                    break;
                case EMenuLevel.Other:
                    // Add logic for adjusting SelectedIndex in the other menu levels
                    break;
                // Add more cases if needed
            }
        }
    }
    
    private const string MenuSeparator = "=======================";
    public Menu(string? title, List<MenuItem> menuItems)
    {
        Title = title;
        MenuItems = menuItems;
    }
    
    public string? Run(EMenuLevel menuLevel = EMenuLevel.First)
    {
        ConsoleKey keyPressed;
        do
        {
            Clear();
            DisplayOptions();
            ConsoleKeyInfo keyInfo = ReadKey(true);
            keyPressed = keyInfo.Key;
            // Update SelectedIntex based on arrow keys.
            UpdateSelectedIndex(keyPressed, menuLevel);
        } while (keyPressed != ConsoleKey.Enter);

        if (SelectedIndex >= 0 && SelectedIndex < MenuItems.Count)
        {
            var menuItem = MenuItems[SelectedIndex];

            if (menuItem.SubMenuToRun != null)
            {
                return menuLevel == EMenuLevel.First
                    ? menuItem.SubMenuToRun!(EMenuLevel.Second)
                    : menuItem.SubMenuToRun!(EMenuLevel.Other);
            }
            else if (menuItem.MethodToRun != null)
            {
                var result = menuItem.MethodToRun!();
                return result?.ToLower() == "x" ? "Exit" : null;
            }
        }
        else
        {
            Console.WriteLine("Undefined shortcut....");
        }

        return null;
    }
    //result = menuItem.SubMenuToRun!(EMenuLevel.Second);
}