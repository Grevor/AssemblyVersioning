using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Viking.AssemblyVersioning
{
    public class GenericParameter
    {
        private Type Parameter { get; }

        public GenericParameterAttributes Attributes => Parameter.GenericParameterAttributes;
        public IEnumerable<Type> InterfaceConstraints { get; }
        public IEnumerable<Type> GenericConstraints { get; }
        public Type BaseClassRestriction { get; }
        public bool IsContravariant => Attributes.HasFlag(GenericParameterAttributes.Contravariant);
        public bool IsCovariant => Attributes.HasFlag(GenericParameterAttributes.Covariant);

        public string GivenName => Parameter.Name;

        public GenericParameter(Type t)
        {
            if (!t.IsGenericParameter)
                throw new ArgumentException("The given type must be a generic parameter type");
            Parameter = t;
            var constraints = t.GetGenericParameterConstraints();
            // TODO: ensure that Object is found as base-class if no restriction is given.
            BaseClassRestriction = constraints.FirstOrDefault(a => !a.IsInterface) ?? t.BaseType;
            InterfaceConstraints = constraints.Where(a => a.IsInterface);
            GenericConstraints = constraints.Where(a => a.IsGenericParameter);
        }

        public bool HasSameSignature(GenericParameter other)
        {
            if (Attributes != other.Attributes)
                return false;

            if (!TypeSignature.TypesHaveSameSignatures(BaseClassRestriction, other.BaseClassRestriction))
                return false;
            if (!EqualityComparer.IEnumerableEquality(InterfaceConstraints, other.InterfaceConstraints, GenericParameterConstraint.ConstraintsAreEqual))
                return false;
            if (!EqualityComparer.IEnumerableEquality(GenericConstraints, other.GenericConstraints, GenericParameterConstraint.ConstraintsAreEqual))
                return false;
            return true;
        }

        public static bool HaveSameSignatures(Type a, Type b) => new GenericParameter(a).HasSameSignature(new GenericParameter(b));
    }
}
