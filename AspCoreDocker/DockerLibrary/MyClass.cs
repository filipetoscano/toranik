using System;

namespace DockerLibrary
{
    /// <summary>
    /// Description of my class.
    /// </summary>
    public class MyClass
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        public MyClass()
        {
        }

        /// <summary>
        /// Runs my class.
        /// </summary>
        /// <returns>Value from private NuGet.</returns>
        public string Run()
        {
            return DateTime.UtcNow.ToString();
        }
    }
}
