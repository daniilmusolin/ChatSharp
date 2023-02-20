namespace ChatParse
{
    public class Commands : Command
    {
        [Command("car")]
        public void SpawnCar(int id_car, int x, int y)
        {
            // Some kind of check
            Console.WriteLine($"Id car: {id_car}, x: {x}, y: {y}");
        }

        [Command("registration")]
        public void test(string login, string password)
        {
            // Some kind of check
            Console.WriteLine($"Login: {login}, password: {password}");
        }
    }
}
