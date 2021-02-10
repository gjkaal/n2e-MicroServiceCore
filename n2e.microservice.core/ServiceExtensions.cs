using n2e.MicroService.Core.Abstractions;
using System;

namespace n2e.MicroService.Core
{
    public static class ServiceExtensions {

        public const string ActionPrefix = "action:";
        private readonly static char[]  ShortPrefix = new char[] {'/', '-', '\\' };



        public static StartupAction FindStartupAction(this string[] args, StartupAction defaultValue)
        {
            if (args == null) return defaultValue;
            foreach (var arg in args)
            {
                if (string.IsNullOrEmpty(arg)) continue;
                if (arg.StartsWith(ActionPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    if (Enum.TryParse(arg.Substring(ActionPrefix.Length), out StartupAction action))
                    {
                        return action;
                    }
                }
                if (arg.Length == 2 && arg.IndexOfAny(ShortPrefix)==0)
                {
                    var shortAction = char.ToLowerInvariant(arg[1]);
                    foreach(StartupAction action in Enum.GetValues(typeof(StartupAction)))
                    {
                        var refAction = char.ToLowerInvariant(action.ToString()[0]);
                        if (shortAction == refAction) return action;
                    }
                }
            }
            return defaultValue;
        }
    }

}
