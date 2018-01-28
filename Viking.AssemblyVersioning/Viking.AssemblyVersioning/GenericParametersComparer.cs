using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viking.AssemblyVersioning
{
    public class GenericParametersComparer 
    {
        private Type[] Baseline { get; }
        private Type[] Candidate { get; }

        public bool ParametersMatch { get; }

        public GenericParametersComparer(Type[] baseline, Type[] candidate)
        {
            ParametersMatch = TypeComparer.GenericArgumentsSignaturesMatch(baseline, candidate);
        }
    }
}
