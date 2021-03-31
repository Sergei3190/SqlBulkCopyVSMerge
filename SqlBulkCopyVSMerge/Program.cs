using SqlBulkCopyVSMerge.Handlers;
using System;

namespace SqlBulkCopyVSMerge
{
    class Program
    {
        static void Main(string[] args)
        {
            Worker worker = new Worker();

            var result = worker.RunWorker();

            Console.WriteLine($"Программа отработала с результатом = {result}");

            Console.WriteLine($"Нажмите \"Enter\" для выхода из программы");
            Console.ReadLine();
        }
    }
}
