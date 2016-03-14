using System;

namespace ConsoleApp
{
    public class Program
    {
        public static void Main( string[] args )
        {
            DockerLibrary.MyClass c = new DockerLibrary.MyClass();
            Console.WriteLine( c.Run() );

            Console.WriteLine( "Ran!" );
        }
    }
}
