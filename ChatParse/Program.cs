namespace ChatParse
{
    public class Programm
    {
        public static void Main(string[] args)
        {
            Chat chat = new Chat();
            chat.ParseCommand("/registration User1 1234"); // Login: User1, password: 1234
            chat.ParseCommand("/car 10 23 423"); // Id car: 10, x: 23, y: 423
        }
    }
}