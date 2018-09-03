using System;
using System.Reflection;

using log4net;
using log4net.Repository;

namespace UnitTestProject1
{
	public class Utils
	{
		private Utils()
		{
		}

		public static object CreateInstance(string targetType)
		{
			return CreateInstance(Type.GetType(targetType, true, true));
		}

		public static object CreateInstance(Type targetType)
		{
			return targetType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null).Invoke(null);
		}

		public static object InvokeMethod(object target, string name, params object[] args)
		{
            return target.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, GetTypesArray(args), null).Invoke(target, args);
        }

        public static object InvokeMethod(Type target, string name, params object[] args)
		{
            return target.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, GetTypesArray(args), null).Invoke(null, args);
        }

        public static object GetField(object target, string name)
		{
			return target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).GetValue(target);
		}

		public static void SetField(object target, string name, object val)
		{
			target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).SetValue(target, val);
		}

		public static object GetProperty(object target, string name)
		{
			return target.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).GetValue(target, null);
		}

		public static void SetProperty(object target, string name, object val)
		{
			target.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).SetValue(target, val, null);
		}

		public static object GetProperty(object target, string name, params object[] index)
		{
			return target.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).GetValue(target, index);
		}

		public static void SetProperty(object target, string name, object val, params object[] index)
		{
			target.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).SetValue(target, val, index);
		}

		private static Type[] GetTypesArray(object[] args)
		{
			Type[] types = new Type[args.Length];

			for(int i = 0; i < args.Length; i++)
			{
				if (args[i] == null)
				{
					types[i] = typeof(object);
				}
				else
				{
					types[i] = args[i].GetType();
				}
			}

			return types;
		}

        internal const string PROPERTY_KEY = "prop1";

        internal static void RemovePropertyFromAllContexts() {
            GlobalContext.Properties.Remove(PROPERTY_KEY);
            ThreadContext.Properties.Remove(PROPERTY_KEY);
            LogicalThreadContext.Properties.Remove(PROPERTY_KEY);
        }

        // Wrappers because repository/logger retrieval APIs require an Assembly argument on NETSTANDARD1_3
        internal static ILog GetLogger(string name)
        {
            return LogManager.GetLogger(name);
        }

        internal static ILoggerRepository GetRepository()
        {
            return LogManager.GetRepository();
        }
    }
}