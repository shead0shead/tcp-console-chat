﻿using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

ServerObject server = new ServerObject();
await server.ListenAsync();

class ServerObject
{
    TcpListener tcpListener = new TcpListener(IPAddress.Any, 8888);
    List<ClientObject> clients = new List<ClientObject>();
    TextObject textObject = new TextObject();
    public bool Wait = true;
    public int[] WindowSize = [Console.WindowWidth, Console.WindowHeight];
    protected internal void RemoveConnection(string id)
    {
        ClientObject? client = clients.FirstOrDefault(c => c.Id == id);
        if (client != null) clients.Remove(client);
        client?.Close();
    }
    protected internal async Task ListenAsync()
    {
        Console.CursorVisible = false;
        try
        {
            tcpListener.Start();
            Console.ForegroundColor = ConsoleColor.Yellow;
            // textObject.PrintCentered("Сервер запущен. Ожидание подключений...");
            textObject.PrintCentered("Server started. Waiting connections...");
            Console.ResetColor();

            Task.Run(() => CheckSizeAsync());
            
            while (true)
            {
                TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                if (Wait)
                {
                    Wait = false;
                    Console.Clear();
                }
                
                ClientObject clientObject = new ClientObject(tcpClient, this);
                clients.Add(clientObject);
                Task.Run(clientObject.ProcessAsync);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Disconnect();
        }
    }

    protected internal async Task BroadcastMessageAsync(string name, string message, string nameColor, string id)
    {
        foreach (var client in clients)
        {
            if (client.Id != id)
            {
                await client.Writer.WriteLineAsync(name);
                await client.Writer.FlushAsync();
                await client.Writer.WriteLineAsync(nameColor);
                await client.Writer.FlushAsync();
                await client.Writer.WriteLineAsync(message);
                await client.Writer.FlushAsync();
            }
        }
    }
    protected internal void Disconnect()
    {
        foreach (var client in clients)
        {
            client.Close();
        }
        tcpListener.Stop();
    }
    protected internal async Task CheckSizeAsync()
    {
        while (Wait)
        {
            if (Console.WindowWidth != WindowSize[0] || Console.WindowHeight != WindowSize[1])
            {
                Console.Clear();
                WindowSize = [Console.WindowWidth, Console.WindowHeight];
                Console.ForegroundColor = ConsoleColor.Yellow;
                // textObject.PrintCentered("Сервер запущен. Ожидание подключений...");
                textObject.PrintCentered("Server started. Waiting connections...");
                Console.ResetColor();
            }
        }
    }
}
class ClientObject
{
    protected internal string Id { get; } = Guid.NewGuid().ToString();
    protected internal StreamWriter Writer { get; }
    protected internal StreamReader Reader { get; }

    TcpClient client;
    ServerObject server;

    public ClientObject(TcpClient tcpClient, ServerObject serverObject)
    {
        client = tcpClient;
        server = serverObject;
        var stream = client.GetStream();
        Reader = new StreamReader(stream);
        Writer = new StreamWriter(stream);
    }

    public async Task ProcessAsync()
    {
        try
        {
            string? userName = await Reader.ReadLineAsync();
            string? nameColorA = await Reader.ReadLineAsync();
            ConsoleColor nameColor = ConsoleColor.Gray;
            if (nameColorA.ToLower() == "red") nameColor = ConsoleColor.Red; 
            else if (nameColorA.ToLower() == "green") nameColor = ConsoleColor.Green;
            else if (nameColorA.ToLower() == "blue") nameColor = ConsoleColor.Blue;
            else if (nameColorA.ToLower() == "yellow") nameColor = ConsoleColor.Yellow;
            else if (nameColorA.ToLower() == "magenta") nameColor = ConsoleColor.Magenta;
            else if (nameColorA.ToLower() == "cyan") nameColor = ConsoleColor.Cyan;
            // string? message = "вошел в чат";
            string? message = "logged";
            await server.BroadcastMessageAsync(userName, message, nameColorA, Id);
            Console.Write("+ ");
            Console.ForegroundColor = nameColor;
            Console.Write(userName);
            Console.ResetColor();
            Console.WriteLine();
            while (true)
            {
                try
                {
                    message = await Reader.ReadLineAsync();
                    if (message == null) continue;
                    Console.ForegroundColor = nameColor;
                    Console.Write(userName);
                    Console.ResetColor();
                    Console.Write(": " + message);
                    Console.WriteLine();
                    await server.BroadcastMessageAsync(userName, message, nameColorA, Id);
                }
                catch
                {
                    // message = "покинул чат";
                    message = "logged out";
                    Console.Write("- ");
                    Console.ForegroundColor = nameColor;
                    Console.Write(userName);
                    Console.ResetColor();
                    Console.WriteLine();
                    await server.BroadcastMessageAsync(userName, message, nameColorA, Id);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            server.RemoveConnection(Id);
        }
    }
    protected internal void Close()
    {
        Writer.Close();
        Reader.Close();
        client.Close();
    }
}
class TextObject
{
    protected internal bool Fade = false;
    protected internal void PrintCentered(string text)
    {
        int length = text.Length;
        int cursor = 0;

        while (length > Console.WindowWidth)
        {
            string newLine = text.Substring(cursor, Console.WindowWidth - 4);
            int lineLength = newLine.LastIndexOf(' ');
            cursor += lineLength;
            text = text.Insert(cursor, "\n");
            length -= lineLength;
        }

        string[] lines = Regex.Split(text, "\r\n|\r|\n");
        int left = 0;
        int top = (Console.WindowHeight / 2) - (lines.Length / 2) - 1;
        int center = Console.WindowWidth / 2;

        for (int i = 0; i < lines.Length; i++)
        {
            left = center - (lines[i].Length / 2);
            Console.SetCursorPosition(left, top);
            if (Fade)
            {
                foreach (char letter in lines[i])
                {
                    Console.CursorVisible = true;
                    Console.Write(letter);
                    Thread.Sleep(15);
                    Console.CursorVisible = false;
                }
                Console.WriteLine();
            }
            else Console.WriteLine(lines[i]);
            top = Console.CursorTop;
        }
    }
}