using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viking.AssemblyVersioning
{
    public class AssemblyComparisonResults : IAssemblyCheckResults
    {
        private List<AssemblyCheckRecord> Records { get; } = new List<AssemblyCheckRecord>();
        private List<AssemblyCheckRecord> Errors { get; } = new List<AssemblyCheckRecord>();
        private List<AssemblyCheckRecord> Warnings { get; } = new List<AssemblyCheckRecord>();
        private List<AssemblyCheckRecord> Infos { get; } = new List<AssemblyCheckRecord>();
        public IEnumerable<AssemblyCheckRecord> ChronologicalRecords => Records;

        public IEnumerable<AssemblyCheckRecord> GetMatchingFilter(Func<AssemblyCheckRecord, bool> filter) => ChronologicalRecords.Where(filter);

        public void Info(Check failedCheck, string source, string message) => AddToTwo(new AssemblyCheckRecord(Category.Info, failedCheck, source, message), Infos);
        public void Warning(Check failedCheck, string source, string message) => AddToTwo(new AssemblyCheckRecord(Category.Warning, failedCheck, source, message), Warnings);
        public void Error(Check failedCheck, string source, string message) => AddToTwo(new AssemblyCheckRecord(Category.Error, failedCheck, source, message), Errors);

        private void AddToTwo(AssemblyCheckRecord assemblyCheckRecord, List<AssemblyCheckRecord> errors)
        {
            Records.Add(assemblyCheckRecord);
            errors.Add(assemblyCheckRecord);
        }
    }
}
