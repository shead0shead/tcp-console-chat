namespace client;

public class Translate
{ 
    public string text(int id, string language)
    {
        Params @params = new Params();
        string text = "";
        switch (id)
        {
            case 1:
                if (language.ToLower() == "english") text = "Write IP address for connection: ";
                else if (language.ToLower() == "russian") text = "Введите IP для подключения: ";
                break;
            case 2:
                if (language.ToLower() == "english") text = "Write your name: ";
                else if (language.ToLower() == "russian") text = "Введите свое имя: ";
                break;
            case 3:
                if (language.ToLower() == "english") text = "Choose name color: ";
                else if (language.ToLower() == "russian") text = "Выберите цвет имени: ";
                break;
            case 4:
                if (language.ToLower() == "english") text = $"Welcome, {@params.userName}";
                else if (language.ToLower() == "russian") text = $"Добро пожаловать, {@params.userName}";
                break;
            case 5:
                if (language.ToLower() == "english") text = "To send messages, type the message and press Enter";
                else if (language.ToLower() == "russian") text = "Для отправки сообщений введите сообщение и нажмите Enter";
                break;
            case 6:
                if (language.ToLower() == "english") text = "logged";
                else if (language.ToLower() == "russian") text = "вошел в чат";
                break;
            case 7:
                if (language.ToLower() == "english") text = "logged out";
                else if (language.ToLower() == "russian") text = "покинул чат";
                break;
        } return text;
    }
}
