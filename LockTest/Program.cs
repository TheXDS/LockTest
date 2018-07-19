// Program.cs
// 
// Author(s):
//      Cesar Morgan Recinos <xds_xps_ivx@hotmail.com>
// 
// Copyright (c) 2018 - 2018 Cesar Morgan Recinos
// 
// This program is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A
// PARTICULAR PURPOSE.  See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with
// this program. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LockTest
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var c = new Crasher();
            await c.HangAsync(5);
            Console.WriteLine("Fin de la app. Presione cualquier tecla para salir.");
            Console.ReadKey();
        }
    }

    internal class Crasher
    {
        private int _counter;
        private readonly object _lock = new object();
        public void Hang(int hangMilliseconds)
        {
            var thread = ++_counter;
            Console.WriteLine($"Hilo {thread} a la espera");

            lock (_lock)
            {
                Console.WriteLine($"Inicio de hilo {thread}... Hang {hangMilliseconds} ms!");

                var task = Task.Run(() =>
                {
                    // Simula una operación cualquiera en el objeto bloqueado.
                    var _ = _lock.ToString();

                    // Este bloque intencionalmente bloquea la ejecución.
                    Thread.Sleep(hangMilliseconds);
                });

                Console.WriteLine(task.Wait(TimeSpan.FromMilliseconds(10000))
                    ? $"Fin de hilo {thread}"
                    : $"Abortar hilo {thread}");
                
            }
        }

        public Task HangAsync(int threads)
        {
            var rnd = new Random();
            var tasks = new HashSet<Task>();

            for (var j = 1; j <= threads; j++)
            {
                tasks.Add(Task.Run(() => Hang(rnd.Next(3000, 12000))));
            }
            return Task.WhenAll(tasks);
        }
    }
}