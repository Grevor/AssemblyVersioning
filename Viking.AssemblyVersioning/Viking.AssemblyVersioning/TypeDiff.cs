using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Viking.AssemblyVersioning
{
    public struct FunctionDefinition
    {
        public string Name { get; }
    }
    public class TypeDiff
    {
        public TypeDiff(Type baseline, Type candidate)
        {
            var attribs = baseline.Attributes;
            var cand = baseline.Attributes;
        }
    }
}
