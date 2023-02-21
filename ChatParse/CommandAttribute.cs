namespace ChatParse
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public string name { get; private set; }

        public CommandAttribute(string name)
        {
            this.name = name;
        }
    }
}
