using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Viking.AssemblyVersioning
{
    public class AssemblyExportedTypesComparer
    {
        public IEnumerable<string> TypesExclusiveToBaseline { get; }
        public IEnumerable<string> TypesExclusiveToCandidate { get; }
        public IEnumerable<string> TypesContainedInBoth { get; }

        public Type[] BaselineTypes { get; }
        public Type[] CandidateTypes { get; }

        public AssemblyExportedTypesComparer(Assembly baseline, Assembly candidate)
        {
            var baselineTypes = new HashSet<string>(baseline.GetExportedTypes().Select(a => a.FullName));
            var candidateTypes = new HashSet<string>(candidate.GetExportedTypes().Select(a => a.FullName));

            var exb = new List<string>();
            var exc = new List<string>();
            var union = new List<string>();

            foreach (var baseType in baselineTypes)
                if (candidateTypes.Contains(baseType))
                    union.Add(baseType);
                else
                    exb.Add(baseType);

            foreach (var candType in candidateTypes)
                if (!baselineTypes.Contains(candType))
                    exc.Add(candType);

            union.Sort(StringComparer.OrdinalIgnoreCase);
            exb.Sort(StringComparer.OrdinalIgnoreCase);
            exc.Sort(StringComparer.OrdinalIgnoreCase);

            TypesExclusiveToBaseline = exb;
            TypesExclusiveToCandidate = exc;
            TypesContainedInBoth = union;
            BaselineTypes = baseline.GetExportedTypes();
            CandidateTypes = baseline.GetExportedTypes();
            Array.Sort(BaselineTypes, TypeComparer.FullNameComparer);
            Array.Sort(CandidateTypes, TypeComparer.FullNameComparer);
        }

        
    }
}
