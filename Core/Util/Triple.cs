namespace InputLog.Core.Util
{
    public class Triple<T, U, V>
    {
        public T First { get; set; }
        public U Second { get; set; }
        public V Third { get; set; }
        
        public Triple()
        {
        }

        public Triple(T first, U second, V third)
        {
            First = first;
            Second = second;
            Third = third;
        }

        public override string ToString()
        {
            return First + "," + Second + "," + Third;
        }
    };
}
