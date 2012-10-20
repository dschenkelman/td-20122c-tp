using System;
using System.Collections.Generic;


namespace AnonymousFunctionsExample
{

    public static class Extensions
    {
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> genericList, Func<T, bool> matchea)
        {
            Console.WriteLine("Starting to iterate...");

            foreach (var value in genericList)
            {
                if (matchea(value))
                {
                    Console.WriteLine("Filtering...");
                    yield return value;
                }
            }
        }

        public static void ForEach<T>(this IEnumerable<T> lista, Action<T> accion)
        {
            foreach (var value in lista)
            {
                accion(value);
            }
        }
    }
}
