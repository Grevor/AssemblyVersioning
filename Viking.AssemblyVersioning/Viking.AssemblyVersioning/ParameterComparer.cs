using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Viking.AssemblyVersioning
{
    public class ParameterComparer
    {
        public ParameterInfo Baseline { get; }
        public ParameterInfo Candidate { get; }

        public bool IsSameType => EqualityComparer.Equals(Baseline.ParameterType, Candidate.ParameterType);
        public bool HaveSameAttributes => Baseline.Attributes == Candidate.Attributes;
        public bool DefaultValueMatch => Baseline.HasDefaultValue == Candidate.HasDefaultValue;
        public bool HaveSameDefaultValue => (DefaultValueMatch && !Baseline.HasDefaultValue) || Equals(Baseline.DefaultValue, Candidate.DefaultValue);

        public bool AreEqual => IsSameType && HaveSameAttributes && HaveSameDefaultValue;

        public ParameterComparer(ParameterInfo baseline, ParameterInfo candidate)
        {
            Baseline = baseline;
            Candidate = candidate;
        }
    }
}
