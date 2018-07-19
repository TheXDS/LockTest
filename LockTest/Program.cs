using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LockTest
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var c = new Crasher();
            await c.HangAsync();


        }


    }

    class Crasher
    {
        private int _counter= 0;
        private object _lock = new object();

        public Task HangAsync()
        {
            var t1 = Task.Run((Action)Hang);
            var t2 = Task.Run((Action)Hang);
            return Task.WhenAll(t1, t2);
        }



        public void Hang()
        {
            if (Monitor.TryEnter(_lock, 10000))
            {
                try
                {
                    Console.WriteLine($"Inicio de hilo {++_counter}... Hang!");

                    var task = Task.Run(() =>
                    {
                        while (true) ;
                    });

                    if (task.Wait(TimeSpan.FromSeconds(20)))
                        Console.WriteLine($"Fin de hilo {_counter--}");
                    else
                        Console.WriteLine($"Abortar hilo {_counter--}");
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }
            else
            {
                Console.WriteLine($"Hilo {_counter+1} no entró.");
            }
        }
    }
}
