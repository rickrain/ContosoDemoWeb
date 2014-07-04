using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureLoader.WebJob
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("{0} - Hello from PictureLoader.WebJob", DateTime.UtcNow);
                // Sleep 60 seconds.
                System.Threading.Thread.Sleep(60000);
            }
        }
    }
}
