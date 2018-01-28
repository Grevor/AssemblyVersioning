namespace BaselineAssembly
{
    public class PublicClass { }
    internal class InternalClass { }



    public class PublicContainerClass
    {
        public class PublicClass { }
        internal class InternalClass { }
        protected class ProtectedClass { }
        protected internal class ProtectedInternalClass { }
    }
    internal class InternalContainerClass
    {
        public class PublicClass { }
        internal class InternalClass { }
        protected class ProtectedClass { }
        protected internal class ProtectedInternalClass { }
    }


    public class PublicClass<T> { }
    public class PublicClass<T, F> { }


    public class PublicContainerClass<T>
    {
        public class PublicClass<P> { }
        internal class InternalClass<P> { }
        protected class ProtectedClass<P> { }
        protected internal class ProtectedInternalClass<P> { }
    }

    public class ConstrainedGenericClass<T>
        where T : struct
    { }

    //Recursive definition
    public class RecursivelyConstrainedGenericClass<T, A>
        where T : A
        where A : RecursivelyConstrainedGenericClass<T, A>
    { }
}
