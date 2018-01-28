using System;

namespace Viking.AssemblyVersioning
{
    [Flags]
    public enum Check
    {
        /// <summary>
        /// Changing the public key of the strong name is a special kind of binary-level break for strongly signed assemblies.
        /// Assemblies compiled against the older key will not bind against the new one. Note that the private key can be changed so long as the public key stays the same.
        /// </summary>
        AssemblyStrongName = 1 << 0,

        /// <summary>
        /// New overloads of existing methods is a source-level breaking change, as certain operations in referencing assemblies may become ambiguous.
        /// It is also a source-level quiet semantics change, as extension methods in referencing assemblies will be rerouted upon recompilation.
        /// 
        /// <para/>
        /// public class Foo
        ///{
        ///    public void Bar(IEnumerable x);
        ///}
        /// 
        /// <para/>
        /// public class Foo
        ///{
        ///    public void Bar(IEnumerable x);
        ///    public void Bar(ICloneable x);
        ///}
        /// <para/>Breaking code:
        /// <para/>new Foo().Bar(new int[0]);
        /// </summary>
        NewMethodOverloadsInClass = 1 << 1,

        /// <summary>
        /// Implicit conversion operators is a source-level breaking change, as certain operations in referencing assemblies may become ambiguous.
        /// It may also constitute a source-level quiet semantics change, as extension methods in referencing assemblies will be rerouted upon recompilation.
        /// <para/>
        /// public class Foo
        ///{
        ///    public static implicit operator int();
        ///}
        ///
        /// <para/>
        /// public class Foo
        ///{
        ///public static implicit operator int();
        ///public static implicit operator float();
        ///}
        ///
        /// <para/>Breaking code:
        ///void Bar(int x);
        ///void Bar(float x);
        ///Bar(new Foo());
        /// </summary>
        ImplicitOperatorOverloadsInClass = 1 << 2,

        /// <summary>
        /// Added methods to class is a source-level quiet semantics change, as extension methods in referencing assemblies will be rerouted upon recompilation.
        /// </summary>
        AddedInstanceMethodsInClass = 1 << 3,

        /// <summary>
        /// A method signature change (I.E a certain method signature is no longer supplied by the upgrade) is a binary-level and source-level breaking change.
        /// A method signature in this case also includes return type.
        /// A referencing assembly will not find the method to bind to, or will not be able to create delegates.
        /// </summary>
        MethodSignatureChangeInClass = 1 << 4,

        /// <summary>
        /// Adding a default parameter to an existing method without also providing an overload without it is a binary-level breaking change.
        /// Referencing assemblies must recompile such defaults to Foo(a, b: null).
        /// 
        /// public void Foo(int a) { }
        /// 
        /// public void Foo(int a, string b = null) { }
        /// 
        /// Foo(5);
        /// </summary>
        AddingMethodDefaultParameterInClass= 1 << 5,

        /// <summary>
        /// Changing the order or redefining enum values is a binary-level quiet breaking change. 
        /// Enum values are compiled as constants, meaning code may or may not break immediately.
        /// 
        /// public enum Foo { Bar, Baz }
        /// 
        /// public enum Foo { Baz, Bar }
        /// 
        /// Foo.Bar < Foo.Baz
        /// </summary>
        ReorderingOrRedefinitionOfEnumValues = 1 << 6,

        /// <summary>
        /// Converting an implicit interface implementation to an explicit one or vice versa causes both a binary-level and a source-level breaking change.
        /// In IL, interface references are fully qualified.
        /// </summary>
        ConvertingImplicitExplicitInterfaceImplementation = 1 << 7,

        /// <summary>
        /// In VB and C#, renaming of a variable is a source-level breaking change. 
        /// Referencing assemblies may fully qualify an argument destination: Foo(foo: "bar").
        /// </summary>
        ArgumentRenaming = 1 << 8,

        /// <summary>
        /// Renaming, moving or deleting a type is both a binary-level and source-level breaking change, as referencing assemblies depend on exact type-names.
        /// Note that "moving" a type is OK if the attribute <see cref="System.Runtime.CompilerServices.TypeForwardedToAttribute"/> is .
        /// </summary>
        RenamingOrDeletingType = 1 << 9,

        /// <summary>
        /// Interface changes of any kind is a binary-level and source-level breaking change. The CLR requires interfaces to be exactly defined in all aspects but custom attributes.
        /// This also includes moving members to a base interface, as explicit implementations generate IL with fully qualified signatures.
        /// </summary>
        InterfaceChanges = 1 << 10,

        /// <summary>
        /// Adding a type is a source-level breaking change, as 'using' statements in referencing assemblies may cause ambiguity.
        /// </summary>
        AddingType = 1 << 11,

        /// <summary>
        /// Changing the assembly name (I.E target architecture, AppDomain span, name, flags, etc.) is a binary-level breaking change, as well as a possible source-level breaking change.
        /// Referencing assemblies will no linger be able to bind to the assembly.
        /// </summary>
        AssemblyNameChange = 1 << 12,
    }
}
