using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tharga.Test.Toolkit
{
    public abstract class DependencyTestBase
    {
        private readonly string[] _assembliesToIgnore;
        private readonly Assembly _assemblyToTest;

        protected DependencyTestBase(Assembly assemblyToTest, string[] assembliesToIgnore = null)
        {
            _assemblyToTest = assemblyToTest;
            _assembliesToIgnore = assembliesToIgnore ?? GetStandardAssembliesToIgnore().ToArray();
        }

        protected DependencyTestBase(Assembly assemblyToTest, Assembly[] assembliesToIgnore)
        {
            _assemblyToTest = assemblyToTest;
            _assembliesToIgnore = assembliesToIgnore?.Select(x => x.GetName().Name).ToArray() ?? GetStandardAssembliesToIgnore().ToArray();
        }

        public static IEnumerable<string> GetStandardAssembliesToIgnore()
        {
            return new[] { "netstandard", "nCrunch.TestRuntime.DotNetCore" };
        }

        protected IEnumerable<AssemblyName> GetDependencies()
        {
            return _assemblyToTest.GetReferencedAssemblies().Where(x => _assembliesToIgnore.All(y => y != x.Name));
        }
    }
}