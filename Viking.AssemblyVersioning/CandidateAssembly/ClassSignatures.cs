using System.Collections;

namespace BaselineAssembly
{
    public class PublicClass
    {
        public int Method(bool b) => 1;
        public string GetCoolString(int coolArgument) => coolArgument.ToString();

        public void Protecc() { }
        protected void GenericMethod<T>(T t) where T:IComparer { }
    }
    internal class InternalClass { }
}
