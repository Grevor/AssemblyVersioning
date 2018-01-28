using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Viking.AssemblyVersioning
{
    public class MethodSignature : IEquatable<MethodSignature>
    {
        private static MethodAttributes RelaxableAttributes { get; } =
            MethodAttributes.Final
            | MethodAttributes.Family
            | MethodAttributes.Private;

        private static MethodAttributes SettableAttributes { get; } =
            MethodAttributes.Public;

        public MethodSignature(MethodInfo method)
        {
            Method = method;
            if (method.IsGenericMethodDefinition)
                GenericSignature = new GenericSignature(method.GetGenericArguments());
            ParameterSignatures = method.GetParameters().Select(a => new ParameterSignature(a)).ToArray();
        }

        private MethodInfo Method { get; }
        private GenericSignature GenericSignature { get; }
        private ParameterSignature[] ParameterSignatures { get; }

        public MethodAttributes Attributes => Method.Attributes;
        public bool IsGeneric => GenericSignature != null;
        public string Name => Method.Name;
        public ParameterSignature ReturnType => new ParameterSignature(Method.ReturnParameter);

        public bool IsAbstract => Method.IsAbstract;
        public bool IsVirtual => Method.IsVirtual;
        public bool IsProtected => Method.IsFamily;

        public ParameterSignature GetParameter(int i) => ParameterSignatures[i];

        public IEnumerable<ParameterSignature> GetRenamedArguments(MethodSignature other) => ParameterSignatures.Where(a => !other.ParameterSignatures.Select(b => b.Name).Contains(a.Name));

        public bool Equals(MethodSignature other)
        {
            if (!Name.Equals(other.Name, StringComparison.Ordinal))
                return false;

            if (Attributes != other.Attributes)
                return false;

            if (IsGeneric != other.IsGeneric)
                return false;

            if (IsGeneric && GenericSignature.Equals(other.GenericSignature))
                return false;

            if (!ReturnType.Equals(other.ReturnType))
                return false;

            if (ParameterSignatures.Length != other.ParameterSignatures.Length)
                return false;
            for (int i = 0; i < ParameterSignatures.Length; ++i)
                if (!ParameterSignatures[i].Equals(other.ParameterSignatures[i]))
                    return false;

            return true;
        }

        public bool CanBeRelaxedTo(MethodSignature other)
        {
            if (!Name.Equals(other.Name, StringComparison.Ordinal))
                return false;

            if (!AttributesCanBeRelaxedTo(other.Attributes))
                return false;

            if (IsGeneric != other.IsGeneric)
                return false;

            if (IsGeneric && GenericSignature.Equals(other.GenericSignature))
                return false;

            if (!ReturnType.Equals(other.ReturnType))
                return false;

            if (ParameterSignatures.Length != other.ParameterSignatures.Length)
                return false;
            for (int i = 0; i < ParameterSignatures.Length; ++i)
                if (!ParameterSignatures[i].Equals(other.ParameterSignatures[i]))
                    return false;

            return true;
        }

        public bool AttributesCanBeRelaxedTo(MethodAttributes attr) =>
            (Attributes & RelaxableAttributes).HasFlag(attr & RelaxableAttributes)
            && ((attr & SettableAttributes).HasFlag(Attributes & SettableAttributes) || (attr & SettableAttributes) == (Attributes & SettableAttributes))
            && (Attributes | RelaxableAttributes | SettableAttributes) == (attr | RelaxableAttributes | SettableAttributes);

        public override string ToString() => Method.ToString();
    }
}
