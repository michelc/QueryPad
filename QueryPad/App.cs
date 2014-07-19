using System;

namespace Altrr
{
    /// <summary>
    /// Application properties
    /// </summary>
    public static class App
    {
        /// <summary>
        /// Application's path
        /// </summary>
        public static string CurrentDirectory
        {
            get
            {
                if (_CurrentDirectory == null)
                {
                    _CurrentDirectory = AppDomain.CurrentDomain
                                                 .BaseDirectory
                                                 .Replace(@"\bin\Debug", "")
                                                 .Replace(@"\bin\Release", "");
                }

                return _CurrentDirectory;
            }
        }
        private static string _CurrentDirectory;

        /// <summary>
        /// Application's name
        /// </summary>
        public static string Name
        {
            get
            {
                if (_Name == null)
                {
                    _Name = AppDomain.CurrentDomain
                                     .FriendlyName
                                     .Replace(".vshost", "")
                                     .Replace(".exe", "");
                }

                return _Name;
            }
        }
        private static string _Name;

        /// <summary>
        /// Application's connections filename
        /// </summary>
        public static string ConnectionsFile
        {
            get
            {
                return System.IO.Path.Combine(App.CurrentDirectory, "App.Connections.secret");
            }
        }
    }
}
