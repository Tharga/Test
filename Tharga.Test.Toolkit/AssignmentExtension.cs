using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Tharga.Test.Toolkit
{
    public static class AssignmentExtension
    {
        private const string SystemNamespace = "System";
        private const BindingFlags InstanceMembers = BindingFlags.Instance | BindingFlags.Public;
        private static readonly Type[] NumericScalars = { typeof(BigInteger), typeof(Complex) };
        private static readonly PropertyInfo IsByRefLikeProperty = typeof(Type).GetProperty("IsByRefLike");

        public static IEnumerable<(string method, object data)> TestAssignments(this Type type, Func<Type, object> createSpecimen, string[] ignoreFunctions = null, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public)
        {
            var methods = type.GetMethods(bindingAttr).Where(x => ignoreFunctions == null || ignoreFunctions.All(y => x.Name != y));
            foreach (var valueTuple in TestAssignments(methods, null, createSpecimen))
            {
                yield return valueTuple;
            }
        }

        public static IEnumerable<(string method, object data)> TestAssignments<T>(this T converter, Func<Type, object> createSpecimen, string[] ignoreFunctions = null, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public)
        {
            var standardTypes = new[] { "GetType", "Equals" };
            var tps = ignoreFunctions?.Union(standardTypes).ToArray() ?? standardTypes;
            var methods = converter.GetType().GetMethods(bindingAttr).Where(x => tps.All(y => x.Name != y));
            foreach (var valueTuple in TestAssignments(methods, converter, createSpecimen))
            {
                yield return valueTuple;
            }
        }

        // Callers filter `methods` by ignoreFunctions before calling in, so the
        // filter list itself is not needed here.
        private static IEnumerable<(string methods, object data)> TestAssignments(IEnumerable<MethodInfo> methods, object converter, Func<Type, object> createSpecimen)
        {
            foreach (var method in methods)
            {
                var pms = method.GetParameters();
                var pams = pms.Select(x => createSpecimen(x.ParameterType)).ToArray();
                var response = method.Invoke(converter, pams);
                yield return (method.Name, response);
            }
        }

        /// <summary>
        /// Returns <c>true</c> when no public instance field or property in the object graph of <paramref name="s1"/> holds its default value.
        /// </summary>
        /// <param name="s1">The object to check.</param>
        /// <param name="ignoreProperties">Member paths that are not reported, for example <c>"Customer.Name"</c> or <c>"Order.Customer.Name"</c>.</param>
        /// <remarks>
        /// <para>
        /// Only public instance fields and properties are walked. Static members and non-public members are not checked.
        /// Indexers and properties of a by-ref-like type, such as <c>Span&lt;T&gt;</c>, are not read.
        /// </para>
        /// <para>
        /// Each object is checked on its own, even when it is equal to another object elsewhere in the graph. Two rules stop
        /// cycles: an object instance that is reached along a second path is not checked again, and a value that is equal to
        /// one of the objects it was reached through is not walked, such as a struct or record whose member returns an equal
        /// copy of itself.
        /// </para>
        /// <para>
        /// A string counts as not assigned only when it is <c>null</c>. An empty string is assigned.
        /// </para>
        /// <para>
        /// A scalar value type is compared with its default as a whole. Scalars are enums, every non-generic struct declared
        /// in the <c>System</c> namespace (the primitives such as <c>int</c>, <c>long</c>, <c>bool</c> and <c>char</c>, and
        /// <c>decimal</c>, <c>Guid</c>, <c>DateTime</c>, <c>DateTimeOffset</c>, <c>TimeSpan</c>, <c>DateOnly</c>, <c>TimeOnly</c>
        /// and the like), <c>BigInteger</c> and <c>Complex</c>. Any other struct, including generic ones such as
        /// <c>ValueTuple</c> and <c>KeyValuePair</c>, is checked member by member. A <c>Nullable&lt;T&gt;</c> is reported when it
        /// is <c>null</c> or holds the default of <c>T</c>.
        /// </para>
        /// <para>
        /// A collection is checked by its elements only. Members declared on the collection type itself, and properties
        /// such as <c>Count</c> or <c>IsReadOnly</c>, are not checked, so an empty collection counts as assigned.
        /// </para>
        /// </remarks>
        public static bool IsAssigned(this object s1, string[] ignoreProperties = null)
        {
            return !AssignmentIssues(s1, ignoreProperties).Any();
        }

        /// <summary>
        /// Returns one issue for every public instance field or property in the object graph of <paramref name="s1"/> that holds its default value.
        /// </summary>
        /// <param name="s1">The object to check.</param>
        /// <param name="ignoreProperties">Member paths that are not reported, for example <c>"Customer.Name"</c> or <c>"Order.Customer.Name"</c>.</param>
        /// <returns>The unassigned members. An issue found inside a collection element carries the element's position in <see cref="IAssignmentIssue.Index"/>.</returns>
        /// <remarks>
        /// <para>
        /// Only public instance fields and properties are walked. Static members and non-public members are not checked.
        /// Indexers and properties of a by-ref-like type, such as <c>Span&lt;T&gt;</c>, are not read.
        /// </para>
        /// <para>
        /// Each object is checked on its own, even when it is equal to another object elsewhere in the graph. Two rules stop
        /// cycles: an object instance that is reached along a second path is not checked again, and a value that is equal to
        /// one of the objects it was reached through is not walked, such as a struct or record whose member returns an equal
        /// copy of itself.
        /// </para>
        /// <para>
        /// A string counts as not assigned only when it is <c>null</c>. An empty string is assigned.
        /// </para>
        /// <para>
        /// A scalar value type is compared with its default as a whole. Scalars are enums, every non-generic struct declared
        /// in the <c>System</c> namespace (the primitives such as <c>int</c>, <c>long</c>, <c>bool</c> and <c>char</c>, and
        /// <c>decimal</c>, <c>Guid</c>, <c>DateTime</c>, <c>DateTimeOffset</c>, <c>TimeSpan</c>, <c>DateOnly</c>, <c>TimeOnly</c>
        /// and the like), <c>BigInteger</c> and <c>Complex</c>. Any other struct, including generic ones such as
        /// <c>ValueTuple</c> and <c>KeyValuePair</c>, is checked member by member. A <c>Nullable&lt;T&gt;</c> is reported when it
        /// is <c>null</c> or holds the default of <c>T</c>.
        /// </para>
        /// <para>
        /// A collection is checked by its elements only. Members declared on the collection type itself, and properties
        /// such as <c>Count</c> or <c>IsReadOnly</c>, are not checked, so an empty collection counts as assigned.
        /// </para>
        /// </remarks>
        public static IEnumerable<IAssignmentIssue> AssignmentIssues(this object s1, string[] ignoreProperties = null)
        {
            foreach (var issue in DoAssignmentIssues(null, null, s1, new HashSet<object>(ReferenceIdentityComparer.Instance), AncestorPath.Empty, ignoreProperties))
            {
                yield return issue;
            }
        }

        private static IEnumerable<IAssignmentIssue> DoAssignmentIssues(string parentObject1Name, string field1Name, object s1, HashSet<object> visited, AncestorPath ancestors, string[] ignoreProperties)
        {
            if (s1 == null)
            {
                if (ignoreProperties == null || ignoreProperties.All(x => x != $"{parentObject1Name}.{field1Name}"))
                {
                    yield return new AssignmentIssue(parentObject1Name, $"The field '{parentObject1Name}.{field1Name}' is not assigned.", null);
                }

                yield break;
            }

            if (s1 is string || s1 is Uri)
            {
                yield break;
            }

            var tp1 = s1.GetType();

            var pp1 = field1Name ?? tp1.Name;
            var item1Name = parentObject1Name != null ? $"{parentObject1Name}.{pp1}" : tp1.Name;

            if (IsScalar(tp1))
            {
                foreach (var diff2 in CheckValue(s1, item1Name, ignoreProperties))
                {
                    yield return diff2;
                }
            }
            else if (tp1.IsValueType)
            {
                if (ancestors.HasEqual(s1))
                {
                    yield break;
                }

                foreach (var diff in CheckMembers(item1Name, s1, visited, ancestors, ignoreProperties))
                {
                    yield return diff;
                }
            }
            else
            {
                foreach (var diff1 in CheckReferenceTypes(item1Name, s1, visited, ancestors, ignoreProperties))
                {
                    yield return diff1;
                }
            }
        }

        private static IEnumerable<IAssignmentIssue> CheckReferenceTypes(string item1Name, object s1, HashSet<object> visited, AncestorPath ancestors, string[] ignoreProperties)
        {
            if (s1 is IEnumerable enumerable)
            {
                var enumr1 = enumerable.GetEnumerator();
                try
                {
                    var index = 0;
                    while (true)
                    {
                        var ptr1 = enumr1.MoveNext();

                        if (!ptr1)
                        {
                            yield break;
                        }

                        var data1 = enumr1.Current;

                        var diffs = DoAssignmentIssues(item1Name, null, data1, visited, ancestors, ignoreProperties);
                        foreach (var diff in diffs)
                        {
                            yield return new AssignmentIssue(diff.ObjectName, diff.Message, index);
                        }

                        index++;
                    }
                }
                finally
                {
                    // The non-generic IEnumerator has no Dispose, but most
                    // implementations are disposable. Not disposing leaks
                    // whatever the source enumerator holds open.
                    (enumr1 as IDisposable)?.Dispose();
                }
            }
            else if (visited.Add(s1) && !ancestors.HasEqual(s1))
            {
                foreach (var diff2 in CheckMembers(item1Name, s1, visited, ancestors, ignoreProperties))
                {
                    yield return diff2;
                }
            }
        }

        private static IEnumerable<IAssignmentIssue> CheckValue(object s1, string item1Name, string[] ignoreProperties)
        {
            var type = s1.GetType();
            if (ignoreProperties == null || ignoreProperties.All(x => x != item1Name))
            {
                if (s1.Equals(GetDefault(type)))
                {
                    yield return new AssignmentIssue(item1Name, $"Value of type {type.Name} in '{item1Name}' is not assigned.", null);
                }
            }
        }

        private static IEnumerable<IAssignmentIssue> CheckMembers(string item1Name, object s1, HashSet<object> visited, AncestorPath ancestors, string[] ignoreProperties)
        {
            var tp = s1.GetType();
            var memberAncestors = ancestors.With(s1);

            var fields = tp.GetFields(InstanceMembers);
            foreach (var field in fields)
            {
                var f1 = field.GetValue(s1);
                var diffs = DoAssignmentIssues(item1Name, field.Name, f1, visited, memberAncestors, ignoreProperties);

                foreach (var diff in diffs)
                {
                    yield return diff;
                }
            }

            var props = tp.GetProperties(InstanceMembers).Where(IsReadable);
            foreach (var prop in props)
            {
                var v1 = prop.GetValue(s1, null);
                var diffs = DoAssignmentIssues(item1Name, prop.Name, v1, visited, memberAncestors, ignoreProperties);

                foreach (var diff in diffs)
                {
                    yield return diff;
                }
            }
        }

        private static bool IsReadable(PropertyInfo prop)
        {
            return prop.GetIndexParameters().Length == 0 && !IsByRefLike(prop.PropertyType);
        }

        private static bool IsByRefLike(Type type)
        {
            return IsByRefLikeProperty != null && (bool)IsByRefLikeProperty.GetValue(type, null);
        }

        private static bool IsScalar(Type type)
        {
            return type.IsEnum
                   || (type.IsValueType && !type.IsGenericType && type.Namespace == SystemNamespace)
                   || NumericScalars.Contains(type);
        }

        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}