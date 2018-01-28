using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Viking.AssemblyVersioning
{
    public class TypeSignature : IEquatable<TypeSignature>
    {
        /// <summary>
        /// All attributes of a type that are not changeable when doing signature comparing.
        /// </summary>
        private static TypeAttributes NonChangeableAttributesMask { get; } =
            TypeAttributes.LayoutMask
            | TypeAttributes.CustomFormatMask
            | TypeAttributes.BeforeFieldInit
            | TypeAttributes.Class
            | TypeAttributes.HasSecurity
            | TypeAttributes.RTSpecialName
            | TypeAttributes.Import
            | TypeAttributes.Interface
            | TypeAttributes.NestedPublic
            | TypeAttributes.Public
            | TypeAttributes.Serializable
            | TypeAttributes.SpecialName;

        /// <summary>
        /// All attributes of a type that are relaxable (meaning we can remove the properties but not add them) when doing signature comparing.
        /// </summary>
        private static TypeAttributes RelaxableAttributesMask { get; } =
            TypeAttributes.Abstract
            | TypeAttributes.Sealed
            | TypeAttributes.NotPublic
            | TypeAttributes.NestedAssembly
            | TypeAttributes.NestedFamily;



        public TypeSignature(Type t)
        {
            Type = t;
            if (t.IsGenericTypeDefinition)
                GenericSignature = new GenericSignature(t.GetGenericArguments());
        }

        private Type Type { get; }
        private GenericSignature GenericSignature { get; }

        public bool IsGeneric => GenericSignature != null;
        public TypeAttributes Attributes => Type.Attributes;
        public string GivenName => Type.Name;
        public string FullName => Type.FullName;

        public bool IsInterface => Type.IsInterface;
        public bool IsSealed => Type.IsSealed;
        public bool IsAbstract => Type.IsAbstract;

        public bool Equals(TypeSignature signature)
        {
            return 
                FullName.Equals(signature.FullName, StringComparison.Ordinal)
                && Attributes.Equals(signature.Attributes)
                && GenericSignature == signature.GenericSignature;
        }

        public bool CanBeRelaxedTo(TypeSignature signature)
        {
            return
                FullName.Equals(signature.FullName, StringComparison.Ordinal)
                && TypeAttributesAllowRelaxationTo(signature.Attributes)
                && GenericSignature == signature.GenericSignature;
        }

        public bool TypeAttributesAllowRelaxationTo(TypeAttributes attr) =>
            (Attributes & NonChangeableAttributesMask) == (attr & NonChangeableAttributesMask) &&
            (Attributes & RelaxableAttributesMask).HasFlag(attr & RelaxableAttributesMask);

        public static bool TypesHaveSameSignatures(Type a, Type b) => new TypeSignature(a).Equals(new TypeSignature(b));
        public static bool FirstSignatureCanBeRelaxedToSecondSignature(Type a, Type b) => new TypeSignature(a).CanBeRelaxedTo(new TypeSignature(b));
    }
}
