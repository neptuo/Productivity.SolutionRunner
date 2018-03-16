using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services
{
    public class FileLockProvider
    {
        private static readonly ConcurrentDictionary<string, object> storage = new ConcurrentDictionary<string, object>();

        public static object Get(string fileName)
        {
            object target = storage.GetOrAdd(fileName, CreateLockObject);
            return target;
        }

        private static object CreateLockObject(string fileName) => new object();
    }
}
