using System;
using System.Collections.Generic;

namespace Viking.AssemblyVersioning
{
    public enum Category
    {
        Error, Warning, Info
    }

    public struct AssemblyCheckRecord
    {
        public Category Category { get; }
        public Check FailedCheck { get; }
        public string Source { get; }
        public string Message { get; }

        public AssemblyCheckRecord(Category category, Check check, string type, string message)
        {
            FailedCheck = check;
            Source = type;
            Message = message;
            Category = category;
        }
    }

    public interface IAssemblyCheckResults
    {
        void Error(Check failedCheck, string source, string message);
        void Warning(Check failedCheck, string source, string message);
        void Info(Check failedCheck, string source, string message);

        IEnumerable<AssemblyCheckRecord> ChronologicalRecords { get; }
        IEnumerable<AssemblyCheckRecord> GetMatchingFilter(Func<AssemblyCheckRecord, bool> filter);
    }
}
