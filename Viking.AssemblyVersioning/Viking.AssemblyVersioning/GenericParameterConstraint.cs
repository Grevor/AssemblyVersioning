using System;

namespace Viking.AssemblyVersioning
{
    public class GenericParameterConstraint : IEquatable<GenericParameterConstraint>
    {

        public GenericParameterConstraint(Type constraint)
        {
            Constraint = constraint;
        }

        private Type Constraint { get; }


        public bool IsGeneric => Constraint.IsGenericParameter;

        public bool Equals(GenericParameterConstraint other)
        {
            if (IsGeneric != other.IsGeneric)
                return false;

            if (IsGeneric)
                return GenericParameter.HaveSameSignatures(Constraint, other.Constraint);
            else
                return TypeSignature.TypesHaveSameSignatures(Constraint, other.Constraint);
        }

        public static bool ConstraintsAreEqual(Type a, Type b) => new GenericParameterConstraint(a).Equals(new GenericParameterConstraint(b));
    }
}
