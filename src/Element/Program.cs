using Element.Data;
using System;
using System.Threading;

namespace Element
{
    class Program
    {
        static void Main()
        {
            for (var i = 0; i < 5; i++)
            {
                new Thread(() =>
                {
                    using (var ctx = ElementContextBuilder.CreateContext())
                    {
                        Console.WriteLine($"Context Created from Thread#{Thread.CurrentThread.ManagedThreadId}");
                        Thread.Sleep(new Random().Next(1000, 10000));
                        Console.WriteLine($"Disposing Context in Thread#{Thread.CurrentThread.ManagedThreadId}");
                    }
                }).Start();
            }

            Console.ReadLine();
        }
    }
}
