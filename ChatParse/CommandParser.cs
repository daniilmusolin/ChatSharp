using System.Reflection;
using System.Text;


namespace ChatParse
{
    public class CommandParser {
        private readonly ILogger<CommandParser> _logger;
        private readonly Dictionary<string, MethodInfo> _commandDictionary;

        public CommandParser(
            ILogger<CommandParser> logger) {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandDictionary = new Dictionary<string, MethodInfo>();

            CommandRegistration();
        }

        public void Read(string message) {
            if (message.StartsWith("/")) {
                message = message.Replace("/", "");
                string[]? parameterArray = message.Split(' ');
                string? parameterFirst = parameterArray.FirstOrDefault();
                if (parameterFirst != null && _commandDictionary.TryGetValue
                    (parameterFirst, out MethodInfo? commadMethod)) {
                    if (commadMethod != null) {
                        if (commadMethod.ReflectedType != null) {
                            object? commandAction = Activator.CreateInstance(commadMethod.ReflectedType);
                            ParameterInfo[]? commandParameterInfo = commadMethod?.GetParameters();
                            if (commandParameterInfo != null) {
                                try {
                                    object[] commadArgumentArray = new object[commandParameterInfo.Length];
                                    for (int index = 0; index < commandParameterInfo.Length; index++) {
                                        if (commandParameterInfo[index].ParameterType.IsInteger())
                                            commadArgumentArray[index] = Convert.ToInt32(parameterArray[index + 1]);
                                        else commadArgumentArray[index] = parameterArray[index + 1];
                                    }
                                    commadMethod?.Invoke(commandAction, commadArgumentArray);
                                } catch (Exception) {
                                    throw new Exception("The user entered the data incorrectly");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CommandRegistration() {
            Type parentClass = typeof(Command);
            List<Type> commandClassList = new List<Type>();
            foreach (Type subClass in Assembly.GetExecutingAssembly().GetTypes())
                if (subClass.IsSubclassOf(parentClass))
                    commandClassList.Add(subClass);
            if (commandClassList.Count != 0) {
                foreach (var commandClass in commandClassList) {
                    MethodInfo[]? commandMethodArray = commandClass.GetMethods(BindingFlags.Static | BindingFlags.NonPublic |
                                                                    BindingFlags.Public | BindingFlags.Instance |
                                                                    BindingFlags.IgnoreReturn);
                    for (int index = 0; index < commandMethodArray.Length; index++) {
                        object[]? attributeArray = commandMethodArray[index].GetCustomAttributes(true);
                        foreach (Attribute attribute in attributeArray) {
                            if (attribute is CommandAttribute commandAttribute) {
                                if (_commandDictionary.ContainsKey(commandAttribute.Name))
                                    throw new Exception($"There cannot be multiple implementations of the same command {commandAttribute.Name} in the {commandClass.Name}");
                                _commandDictionary.Add(commandAttribute.Name, commandMethodArray[index]);
                            }
                        }
                    }
                }
            }
        }
    }

    public static class TypeChecking {
        public static bool IsInteger(this Type type) => type == typeof(int);
    }
}
