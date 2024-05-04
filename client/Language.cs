namespace client;

public class Language
{
    public string language;
    public void menu()
    {
        int[] item = [1, 2];
        language = "english";

        menu:
        Console.ResetColor();
        Console.CursorVisible = false;
        Console.Clear();

        Console.WriteLine("Choose your language:");
        Console.SetCursorPosition(3, 2);
        Console.WriteLine("English");
        Console.SetCursorPosition(3, 3);
        Console.WriteLine("Russian");
        Console.SetCursorPosition(1, item[0] + 1);
        Console.WriteLine(">");

        Console.ForegroundColor = ConsoleColor.Black;
        ConsoleKey key = Console.ReadKey().Key;
        if (key == ConsoleKey.W || key == ConsoleKey.UpArrow)
        {
            if (item[0] == 1) item[0] = item[1];
            else item[0]--;
            goto menu;
        }
        else if (key == ConsoleKey.S || key == ConsoleKey.DownArrow)
        {
            if (item[0] == item[1]) item[0] = 1;
            else item[0]++;
            goto menu;
        }
        else if (key == ConsoleKey.E || key == ConsoleKey.Enter)
        {
            Console.Clear();
            Console.CursorVisible = true;
        }
        else goto menu;

        if (item[0] == 1) language = "english";
        else if (item[0] == 2) language = "russian";

        Console.ResetColor();
    }
    public void typing()
    {
        string[] langs = ["english", "russian"];
        
        Console.WriteLine("Choose your language");
        Console.WriteLine();
        Console.WriteLine("Available langs:");
        Console.WriteLine("1. English");
        Console.WriteLine("2. Russian");
        Console.WriteLine();
        Console.Write("Write number: ");

        byte num = byte.Parse(Console.ReadLine());
        language = langs[num-1];
        
        Console.Clear();
    }
}