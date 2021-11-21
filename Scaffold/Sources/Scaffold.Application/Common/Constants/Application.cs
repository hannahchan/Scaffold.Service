namespace Scaffold.Application.Common.Constants;

using System;

public static class Application
{
    public static class Assembly
    {
        public static readonly string Name = typeof(Application).Assembly.GetName().Name!;

        public static readonly Version Version = typeof(Application).Assembly.GetName().Version!;
    }

    public static class ActivitySource
    {
        public static readonly string Name = Assembly.Name;

        public static readonly string Version = Assembly.Version.ToString();
    }
}
