
namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var tm = new TaskManager();
            tm.StartServer();
        }
    }
}
