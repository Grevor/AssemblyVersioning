using System;

namespace Viking.AssemblyVersioning
{
    public class GenericSignature : IEquatable<GenericSignature>
    {
        public GenericSignature(Type[] signature) { Signature = signature; }

        private Type[] Signature { get; }

        public int SingatureLength => Signature.Length;
        public bool IsParameterGeneric(int parameter) => Signature[parameter].IsGenericParameter;
        public bool IsParameterSpecified(int parameter) => !IsParameterGeneric(parameter);

        public Type GetSpecifiedParameter(int parameter)
        {
            if (!IsParameterSpecified(parameter))
                throw new ArgumentException($"The parameter at index {parameter} is not specified: it is a generic parameter.");
            return Signature[parameter];
        }
        public GenericParameter GetGenericParameter(int parameter) => new GenericParameter(Signature[parameter]);

        //public override bool Equals(object obj) => obj is GenericSignature ? Equals(obj as GenericSignature) : false;
        public bool Equals(GenericSignature other)
        {
            for (int i = 0; i < SingatureLength; ++i)
            {
                var generic = IsParameterGeneric(i);
                if (other.IsParameterGeneric(i) != generic)
                    return false;
                if (generic)
                {
                    if (!GetGenericParameter(i).HasSameSignature(other.GetGenericParameter(i)))
                        return false;
                }
                else // Specified
                {
                    if (!TypeSignature.TypesHaveSameSignatures(GetSpecifiedParameter(i), other.GetSpecifiedParameter(i)))
                        return false;
                }
            }
            return true;
        }

        //public bool GenericSignatureCanBeRelaxedTo(GenericSignature other) => Equals(other);
    }
}
