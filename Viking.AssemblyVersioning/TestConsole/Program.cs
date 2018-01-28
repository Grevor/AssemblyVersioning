using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viking.AssemblyVersioning;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            var type = typeof(SS<int>);
            var method = type.GetMethod("Method");
            foreach (var p in method.GetGenericArguments())
                Console.WriteLine(p.FullName);
            Console.WriteLine(type.FullName);
            Console.WriteLine(typeof(SS<>).FullName);
            Console.WriteLine(type.GetMethod("Hej").ToString());

            Console.ReadLine();


            var results = new AssemblyComparisonResults();
            var checks = Check.RenamingOrDeletingType | Check.MethodSignatureChangeInClass;
            var baseline = new AssemblyDatum("CandidateAssemblyOld.dll");
            var candidate = new AssemblyDatum("CandidateAssembly.dll");
            AssemblyComparer.CheckEntireAssembly(baseline.Assembly, candidate.Assembly, results, checks);

            foreach (var record in results.ChronologicalRecords)
                Console.WriteLine($"{record.Category}:\t{record.Source}\t{Environment.NewLine}{record.FailedCheck} - {record.Message}");

            Console.ReadLine();
        }
    }

    interface II: IComparable, IEquatable<II> { }

    class SS<T>
    {
        public void Method<F, S>(int i, S s, F f, T t)
        where S : II
        { }
        public void Hej(int b) { }
    }

    class Ss<T>
    {

    }

    class TT
    {
        void a<T>(T a) where T : struct { }
    }
}
