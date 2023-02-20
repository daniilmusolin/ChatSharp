using System.Reflection;
using System.Text;


namespace ChatParse
{
    public class Chat
    {
        private readonly Dictionary<string, MethodInfo> commands = new Dictionary<string, MethodInfo>();
        private readonly StringBuilder builder = new StringBuilder();

        public Chat()
        {
            // The command registration method will be called when initializing this class. Initialize it
            // in the same place as the server (or whatever you have there)
            RegistrationCommand();
        }

        // The command that parses the user's message
        public void ParseCommand(string message)
        {
            // Is this a command at all or a regular message?
            if (message.StartsWith("/"))
            {
                message = message.Replace("/", "");
                string[]? objects = message.Split(' ');
                foreach (var command in commands)
                {
                    // If in the list of registered commands, there is one that the user entered, then we go further
                    if (objects[0].ToString() == command.Key)
                    {
                        try
                        {
                            Type type = Type.GetType(command.Value.ReflectedType.FullName);
                            object action = Activator.CreateInstance(type);

                            ParameterInfo[] parameter = command.Value.GetParameters();
                            object[] objects_ = new object[objects.Length - 1];
                            for (int i = 0; i < parameter.Length; i++)
                            {
                                // In string[], all values are string, which is logical, but what if the variable takes
                                // an int value, for this I created a new array object[] and converted the value of
                                // string[] to object[], the type that this method accepts
                                if (parameter[i].ParameterType == typeof(int))
                                    objects_[i] = Convert.ToInt32(objects[i + 1]);
                                else objects_[i] = objects[i + 1];
                            }
                            // If an error pops up here, it means that the variables that should be entered do not match
                            command.Value.Invoke(action, objects_);
                        }
                        catch (Exception)
                        {
                            builder.Clear();
                            builder.Append("/");
                            builder.Append(objects[0].ToString());
                            foreach (var method in command.Value.GetParameters())
                            {
                                builder.Append($" ({method.ParameterType.Name}:{method.Name})");
                            }
                            // We warn the user what the command should look like in the correct format. Here you can send
                            // this message to the user in person if you use a chat on the server
                            Console.WriteLine(builder.ToString());
                        }
                        objects = null;
                        break;
                    }
                }
            }
        }

        // Searches recursively for all commands of this assembly
        private void RegistrationCommand()
        {
            Type parentClass = typeof(Command);
            Assembly parentAssembly = parentClass.Assembly;
            List<Type> commands_class = new List<Type>();
            // We find all classes that are inherited from "Command"
            foreach (Type currType in parentAssembly.GetTypes())
                if (currType.IsSubclassOf(parentClass))
                    commands_class.Add(currType);
            if (commands_class.Count != 0)
            {
                foreach (var class_ in commands_class)
                {
                    MethodInfo[]? methods = class_.GetMethods(BindingFlags.Static | BindingFlags.NonPublic |
                                                              BindingFlags.Public | BindingFlags.Instance  |
                                                              BindingFlags.IgnoreReturn);
                    for (int i = 0; i < methods.GetLength(0); i++)
                    {
                        object[]? attributesArray = methods[i].GetCustomAttributes(true);

                        foreach (Attribute item in attributesArray)
                        {
                            if (item is CommandAttribute)
                            {
                                // If we found the "CommandAttribute" attribute, then we
                                // register this method using the metadata "name" - the key, and MethodInfo - the value
                                CommandAttribute? attributeObject = (CommandAttribute)item;
                                commands.Add(attributeObject.name, methods[i]);
                            }
                        }
                    }
                }
            }
        }
    }
}
