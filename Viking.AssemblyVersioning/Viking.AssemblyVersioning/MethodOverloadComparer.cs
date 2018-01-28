using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Viking.AssemblyVersioning
{
    public class MethodOverloadComparer
    {
        public MethodOverloadComparer(MethodInfo baseline, MethodInfo candidate)
        {
            Baseline = baseline;
            Candidate = candidate;
        }

        public MethodInfo Baseline { get; }
        public MethodInfo Candidate { get; }

        public bool ReturnTypeMatch => Baseline.ReturnType.FullName.Equals(Candidate.ReturnType.FullName);
        public bool AreBothGeneric => Baseline.ContainsGenericParameters == Candidate.ContainsGenericParameters;
        public bool GenericParametersMatch => EqualityComparer.IEnumerableEquality(Baseline.GetGenericArguments().Select(a => a.FullName), Candidate.GetGenericArguments().Select(a => a.FullName));
        public bool ParametersMatch => GenericParametersMatch && EqualityComparer.IEnumerableEquality(Baseline.GetParameters(), Candidate.GetParameters(), FullEquals);
        public bool ParameterCountMatch => Baseline.GetParameters().Count() == Candidate.GetParameters().Count();

        private bool MatchingAttributes(ParameterInfo x, ParameterInfo y) => x.Attributes == y.Attributes;
        private bool MatchingDefaultValues(ParameterInfo x, ParameterInfo y) => x.HasDefaultValue == y.HasDefaultValue;

        private bool FullEquals(ParameterInfo x, ParameterInfo y)
        {
            return
                x.Attributes == y.Attributes
                && x.HasDefaultValue == y.HasDefaultValue
                && x.Name.Equals(y.Name)
                && x.ParameterType.FullName.Equals(y.ParameterType.FullName)
                && x.Position == y.Position;
        }
    }
}
