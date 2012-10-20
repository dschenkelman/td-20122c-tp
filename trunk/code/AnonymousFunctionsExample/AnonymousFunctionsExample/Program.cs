using System;
using System.Collections.Generic;

namespace AnonymousFunctionsExample
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<string> list = new List<string>()
                           {
                               "Seba", 
                               "Damian", 
                               "Matias"
                           };

            var list2 = new List<int>()
                           {
                               6, 
                               14, 
                               5, 
                           };

            Func<string, bool> f1 = palabra => palabra.Length > 4;
            
            list.Filter(f1).ForEach(v =>
                                        {
                                            Console.WriteLine(v);
                                        });

            Console.ReadLine();
        }
    }
}
