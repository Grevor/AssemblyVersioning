using System;
using System.Collections.Generic;

namespace Viking.AssemblyVersioning
{
    public static class TypeComparer
    {
        private class TypeCmp : IComparer<Type>
        {
            public int Compare(Type x, Type y) => string.Compare(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
        }

        public static IComparer<Type> FullNameComparer { get; } = new TypeCmp();

        public static bool TypeSignaturesMatch(Type a, Type b)
        {

            // Check for certain equalities.
            if (a.IsGenericParameter != b.IsGenericParameter)
                return false;
            if (a.IsGenericTypeDefinition != b.IsGenericTypeDefinition)
                return false;
            if (a.IsGenericType != b.IsGenericType)
                return false;

            var isGenericParam = a.IsGenericParameter;
            var isGeneric = a.IsGenericTypeDefinition;

            if (isGenericParam) return GenericParameterSignaturesMatch(a, b);
            if (isGeneric) return GenericTypeSignaturesMatch(a, b);

            // base-case, the types are fully defined generic types or are not generic. Simply check equality of full names is enough due to the exact typing.
            return SimpleTypesSignatureMatch(a, b);
        }

        public static bool GenericTypeSignaturesMatch(Type a, Type b)
        {
            if (!a.IsGenericTypeDefinition || !b.IsGenericTypeDefinition)
                throw new ArgumentException("Both a and b must be generic types.");

            if (!GenericArgumentsSignaturesMatch(a.GetGenericArguments(), b.GetGenericArguments()))
                return false;

            return SimpleTypesSignatureMatch(a, b);
        }

        public static bool GenericArgumentsSignaturesMatch(Type[] baseline, Type[] candidate)
        {
            if (baseline.Length != candidate.Length)
                return false;

            for (int i = 0; i < baseline.Length; ++i)
            {
                var bp = baseline[i];
                var cp = candidate[i];

                if (!GenericParameterSignaturesMatch(bp, cp))
                    return false;
            }
            return true;
        }

        public static bool GenericParameterSignaturesMatch(Type a, Type b)
        {
            if (!a.IsGenericParameter || !b.IsGenericParameter)
                throw new ArgumentException("Both a and b must be generic parameters.");
            if (a.GenericParameterAttributes != b.GenericParameterAttributes)
                return false;

            var ac = a.GetGenericParameterConstraints();
            var bc = a.GetGenericParameterConstraints();
            Array.Sort(ac, FullNameComparer);
            Array.Sort(bc, FullNameComparer);

            if (ac.Length != bc.Length)
                return false;
            for(int i = 0; i < ac.Length; ++i)
            {
                var act = ac[i];
                var bct = bc[i];
                if (!TypeSignaturesMatch(act, bct))
                    return false;
            }
            //TODO: Might include parameter names here if needed.
            return true;
        }

        public static bool SimpleTypesSignatureMatch(Type a, Type b) => a.FullName.Equals(b.FullName, StringComparison.OrdinalIgnoreCase);
    }
}
