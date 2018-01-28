using System;
using System.Linq;
using System.Reflection;

namespace Viking.AssemblyVersioning
{
    public static class AssemblyComparer
    {
        public static void CheckEntireAssembly(Assembly baseline, Assembly candidate, IAssemblyCheckResults results, Check checksToMake)
        {
            CheckAssemblyNames(baseline.GetName(), candidate.GetName(), results, checksToMake);
            CheckTypes(baseline, candidate, results, checksToMake);
        }

        public static void CheckTypes(Assembly baseline, Assembly candidate, IAssemblyCheckResults results, Check checksToMake)
        {
            var typeExportComparison = new AssemblyExportedTypesComparer(baseline, candidate);
            foreach(var type in typeExportComparison.TypesExclusiveToBaseline)
            {
                var category = $"Type@{type}";
                if (checksToMake.HasFlag(Check.RenamingOrDeletingType))
                    results.Error(Check.RenamingOrDeletingType, category, $"Public type '{type}' is missing in candidate. This is a breaking change on all levels.");
                else
                    results.Warning(Check.RenamingOrDeletingType, category, $"Public type '{type}' is missing in candidate. This is a breaking change on all levels.");
            }
            foreach(var type in typeExportComparison.TypesExclusiveToCandidate)
            {
                var category = $"Type@{type}";
                if (checksToMake.HasFlag(Check.AddingType))
                    results.Error(Check.AddingType, category, $"Public type '{type}' is added in candidate. This can be a source-level breaking change.");
                else
                    results.Info(Check.AddingType, category, $"Public type '{type}' is added in candidate. This can be a source-level breaking change.");
            }

            foreach(var type in typeExportComparison.TypesContainedInBoth)
            {
                CheckType(baseline.GetType(type), candidate.GetType(type), results, checksToMake);
            }
        }

        private static void CheckType(Type baseline, Type candidate, IAssemblyCheckResults results, Check checksToMake)
        {
            var category = $"Type@{baseline.FullName}";

            if (!TypeSignature.FirstSignatureCanBeRelaxedToSecondSignature(baseline, candidate))
            {
                if (checksToMake.HasFlag(Check.RenamingOrDeletingType) | checksToMake.HasFlag(Check.InterfaceChanges))
                    results.Error(Check.RenamingOrDeletingType, category, "Type signature missmatch. This is a binary- and source-level breaking change.");
                else
                    results.Warning(Check.RenamingOrDeletingType, category, "Type signature missmatch. This is a binary- and source-level breaking change.");
                return;
            }

            // In case of enum, additional checks must be made.
            if (baseline.IsEnum && candidate.IsEnum)
                CompareEnumType(baseline, candidate, results, checksToMake, category);
            else
                CompareMethods(baseline, candidate, results, checksToMake);
        }

        private static void CompareMethods(Type baseline, Type candidate, IAssemblyCheckResults results, Check checksToMake)
        {
            var source = $"Type@{baseline.FullName}";
            var bm = baseline.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).Select(a => new MethodSignature(a));
            var cm = candidate.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).Select(a => new MethodSignature(a));

            foreach(var bms in bm)
            {
                MethodSignature cms = cm.FirstOrDefault(a => bms.CanBeRelaxedTo(a));
                var methodSource = $"{source}@{bms.Name}";
                if (cms == null)
                {
                    var message = $"Could not find the overload '{bms}' for method {bms.Name} in candidate. This is a source- and binary-level breaking change.";

                    if (checksToMake.HasFlag(Check.MethodSignatureChangeInClass) | checksToMake.HasFlag(Check.InterfaceChanges))
                        results.Error(Check.MethodSignatureChangeInClass, source, message);
                    else
                        results.Warning(Check.MethodSignatureChangeInClass, source, message);

                    continue;
                }


                if (checksToMake.HasFlag(Check.ArgumentRenaming))
                {
                    foreach (var arg in bms.GetRenamedArguments(cms))
                        results.Error(Check.ArgumentRenaming, methodSource, $"Argument {arg.Position} '{arg.Name}' was renamed, possibly from '{cms.GetParameter(arg.Position).Name}'. This is a source-level break or silent semantics change.");
                }
            }

        }

        private static void CompareEnumType(Type baseline, Type candidate, IAssemblyCheckResults results, Check checksToMake, string category)
        {
            var bv = Enum.GetValues(baseline);
            var cv = Enum.GetValues(candidate);
            if (!EqualityComparer.IEnumerableEquality(bv.Cast<object>(), cv.Cast<object>(), (a, b) => CmpEnum(baseline, candidate, a, b)))
            {
                var message = "Redefinition of enum values. This may cause breakage in code.";
                if (checksToMake.HasFlag(Check.ReorderingOrRedefinitionOfEnumValues))
                    results.Error(Check.ReorderingOrRedefinitionOfEnumValues, category, message);
                else
                    results.Warning(Check.ReorderingOrRedefinitionOfEnumValues, category, message);
            }
        }
        private static bool CmpEnum(Type baseline, Type candidate, object a, object b)
        {
            if (!Enum.GetName(baseline, a).Equals(Enum.GetName(candidate, b), StringComparison.Ordinal))
                return false;
            try
            {
                var c = Convert.ChangeType(a, candidate);
                if (!Enum.GetName(baseline, a).Equals(Enum.GetName(candidate, c), StringComparison.Ordinal))
                    return false;
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks for breaking changes in the assembly name of a component.
        /// Note that some of the checks in here are not optional.
        /// </summary>
        /// <param name="baseline">Baseline assembly name.</param>
        /// <param name="candidate">Candidate assembly name.</param>
        /// <param name="results">The output results.</param>
        /// <param name="checksToMake">The type of checks to make.</param>
        public static void CheckAssemblyNames(AssemblyName baseline, AssemblyName candidate, IAssemblyCheckResults results, Check checksToMake)
        {
            const string Category = "Assembly";
            var comparison = new AssemblyNameComparer(baseline, candidate);
            if (!comparison.AssemblyFlagsMatch)
                results.Error(Check.AssemblyNameChange, Category, "Assembly flags do not match. This can lead to problems with JITing and/or strong name authentication.");
            if (!comparison.AssemblyNamesMatch)
                results.Error(Check.AssemblyNameChange, Category, "Assembly names do not match. This will cause all assemblies compiled against an older version to break.");
            if (checksToMake.HasFlag(Check.AssemblyStrongName) && (!comparison.PublicKeysMatch || !comparison.HashAlgorithmsMatch))
                results.Error(Check.AssemblyStrongName, Category, "The public key has changed. This will cause all strong named assemblies compiled against an older version to break.");
            if (!comparison.ProcessorArchitecturesMatch)
                results.Error(Check.AssemblyNameChange, Category, "The processor architecture does not match. This will cause all assemblies compiled against an older version to break.");
            if (!comparison.CulturesMatch)
                results.Warning(Check.AssemblyNameChange, Category, "The culture has changed. Certain culture-specific behavior such as string comparison may exhibit new behavior.");
            if (!comparison.VersionCompabilityMatch)
                results.Error(Check.AssemblyNameChange, Category, "The assembly version scope has changed. Assemblies compiled against an older version may not properly recognize the new version.");
        }
    }
}
