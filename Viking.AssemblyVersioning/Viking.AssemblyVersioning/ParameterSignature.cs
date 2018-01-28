using System;
using System.Reflection;

namespace Viking.AssemblyVersioning
{
    public class ParameterSignature : IEquatable<ParameterSignature>
    {
        public ParameterSignature(ParameterInfo param)
        {
            Parameter = param;
        }

        private ParameterInfo Parameter { get; }
        private Type Type => Parameter.ParameterType;

        public ParameterAttributes Attributes => Parameter.Attributes;
        public bool IsGeneric => Type.IsGenericParameter;
        public int Position => Parameter.Position;
        public string Name => Parameter.Name;

        public bool Equals(ParameterSignature other)
        {
            if (Attributes != other.Attributes)
                return false;

            if (IsGeneric != other.IsGeneric)
                return false;

            if (IsGeneric)
                return GenericParameter.HaveSameSignatures(Type, other.Type);
            return TypeSignature.TypesHaveSameSignatures(Type, other.Type);
        }
    }
}
