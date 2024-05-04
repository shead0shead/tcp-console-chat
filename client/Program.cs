using System.Net.Sockets;
using client;

Params @params = new Params();
Language language = new Language();
Translate translate = new Translate();

language.typing();

string host = "127.0.0.1";
Console.Write(translate.text(1, language.language));
host = Console.ReadLine();
int port = 8888;
using TcpClient client = new TcpClient();

Console.Write(translate.text(2, language.language));
@params.userName = Console.ReadLine();

Console.Write(translate.text(3, language.language));
string? nameColor = Console.ReadLine().ToLower();

Console.Clear();
Console.WriteLine(translate.text(4, language.language));
StreamReader? Reader = null;
StreamWriter? Writer = null;

try
{
    client.Connect(host, port);
    Reader = new StreamReader(client.GetStream());
    Writer = new StreamWriter(client.GetStream());
    if (Writer is null || Reader is null) return;
    Task.Run(() => ReceiveMessageAsync(Reader));
    await SendMessageAsync(Writer);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
Writer?.Close();
Reader?.Close();

async Task SendMessageAsync(StreamWriter writer)
{
    await writer.WriteLineAsync(@params.userName);
    await writer.FlushAsync();
    await writer.WriteLineAsync(nameColor);
    await writer.FlushAsync();
    Console.WriteLine(translate.text(5, language.language));
    Console.WriteLine();

    while (true)
    {
        string? message = Console.ReadLine();
        await writer.WriteLineAsync(message);
        await writer.FlushAsync();
    }
}
async Task ReceiveMessageAsync(StreamReader reader)
{
    while (true)
    {
        try
        {
            string? userName = await reader.ReadLineAsync();
            string? nameColorA = await reader.ReadLineAsync();
            ConsoleColor nameColor = ConsoleColor.Gray;
            if (nameColorA.ToLower() == "red") nameColor = ConsoleColor.Red; 
            else if (nameColorA.ToLower() == "green") nameColor = ConsoleColor.Green;
            else if (nameColorA.ToLower() == "blue") nameColor = ConsoleColor.Blue;
            else if (nameColorA.ToLower() == "yellow") nameColor = ConsoleColor.Yellow;
            else if (nameColorA.ToLower() == "magenta") nameColor = ConsoleColor.Magenta;
            else if (nameColorA.ToLower() == "cyan") nameColor = ConsoleColor.Cyan;
            string? message = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(message)) continue;
            Console.ForegroundColor = nameColor;
            Console.Write(userName);
            Console.ResetColor();
            if (message == "вошел в чат" || message == "logged") message = $" {translate.text(6, language.language)}";
            else if (message == "покинул чат" || message == "logged out") message = $" {translate.text(7, language.language)}";
            else message = $": {message}";
            Console.Write(message);
            Console.WriteLine();
        }
        catch
        {
            break;
        }
    }
}
void Print(string message)
{
    if (OperatingSystem.IsWindows())
    {
        var position = Console.GetCursorPosition();
        int left = position.Left;
        int top = position.Top;
        Console.MoveBufferArea(0, top, left, 1, 0, top + 1);
        Console.SetCursorPosition(0, top);
        Console.Write(message);
        Console.SetCursorPosition(left, top + 1);
    }
    else Console.WriteLine(message);
}
class Params
{
    public string userName;
}