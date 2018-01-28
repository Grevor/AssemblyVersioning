using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Viking.AssemblyVersioning
{
    public class AssemblyDatum
    {
        public AssemblyDatum(string path): this(Assembly.LoadFrom(path)) { }
        public AssemblyDatum(Assembly assembly)
        {
            Assembly = assembly;
        }

        public Assembly Assembly { get; }
        public AssemblyName Name => Assembly.GetName();
        public Version AssemblyVersion => Name.Version;
        public byte[] PublicKey => Name.KeyPair.PublicKey;
        public string DisplayName => Name.FullName;
        public string AssemblyName => Name.Name;
        public ProcessorArchitecture Architecture => Name.ProcessorArchitecture;
    }
}
