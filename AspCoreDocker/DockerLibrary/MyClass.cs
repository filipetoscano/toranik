using System;

namespace DockerLibrary
{
    public class MyClass
    {
        public MyClass()
        {
        }

        public string Run()
        {
            return DateTime.UtcNow.ToString();
        }
    }
}
