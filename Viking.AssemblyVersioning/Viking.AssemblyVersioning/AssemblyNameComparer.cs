using System.Linq;
using System.Reflection;

namespace Viking.AssemblyVersioning
{
    public class AssemblyNameComparer
    {
        public AssemblyName Baseline { get; }
        public AssemblyName Candidate { get; }

        public bool AssemblyNamesMatch => Baseline.Name.Equals(Candidate.Name);
        public bool CulturesMatch => Baseline.CultureName.Equals(Candidate.CultureName);
        public bool AssemblyFlagsMatch => Baseline.Flags == Candidate.Flags;
        public bool ProcessorArchitecturesMatch => Baseline.ProcessorArchitecture == Candidate.ProcessorArchitecture;
        public bool VersionCompabilityMatch => Baseline.VersionCompatibility == Candidate.VersionCompatibility;
        public bool VersionNumberMatch => Baseline.Version == Candidate.Version;
        public bool CandidateVersionNumberIsGreaterThanOrEqual => Baseline.Version < Candidate.Version;
        public bool PublicKeysMatch { get; }
        public bool HashAlgorithmsMatch => Baseline.HashAlgorithm == Candidate.HashAlgorithm;

        public bool AllButVersionMatch => 
            AssemblyNamesMatch 
            && CulturesMatch
            && AssemblyFlagsMatch
            && ProcessorArchitecturesMatch
            && VersionCompabilityMatch
            && PublicKeysMatch
            && HashAlgorithmsMatch;

        public AssemblyNameComparer(AssemblyName baseline, AssemblyName candidate)
        {
            Baseline = baseline;
            Candidate = candidate;
            PublicKeysMatch = Baseline.GetPublicKey().SequenceEqual(Candidate.GetPublicKey());
        }
    }
}
