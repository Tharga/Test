using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tharga.Test.Toolkit
{
    public static class AssignmentExtension
    {
        public static IEnumerable<(string method, object data)> TestAssignments(this Type type, Func<Type, object> createSpecimen, string[] ignoreFunctions = null, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public)
        {
            var methods = type.GetMethods(bindingAttr).Where(x => ignoreFunctions == null || ignoreFunctions.All(y => x.Name != y));
            foreach (var valueTuple in TestAssignments(methods, null, createSpecimen, ignoreFunctions))
            {
                yield return valueTuple;
            }
        }

        public static IEnumerable<(string method, object data)> TestAssignments<T>(this T converter, Func<Type, object> createSpecimen, string[] ignoreFunctions = null, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public)
        {
            var standardTypes = new[] { "GetType", "Equals" };
            var tps = ignoreFunctions?.Union(standardTypes).ToArray() ?? standardTypes;
            var methods = converter.GetType().GetMethods(bindingAttr).Where(x => tps.All(y => x.Name != y));
            foreach (var valueTuple in TestAssignments(methods, converter, createSpecimen, tps))
            {
                yield return valueTuple;
            }
        }

        private static IEnumerable<(string methods, object data)> TestAssignments(IEnumerable<MethodInfo> methods, object converter, Func<Type, object> createSpecimen, string[] tps)
        {
            foreach (var method in methods)
            {
                var pms = method.GetParameters();
                var pams = pms.Select(x => createSpecimen(x.ParameterType)).ToArray();
                var response = method.Invoke(converter, pams);
                yield return (method.Name, response);
            }
        }

        public static bool IsAssigned(this object s1, string[] ignoreProperties = null)
        {
            return !AssignmentIssues(s1, ignoreProperties).Any();
        }

        public static IEnumerable<IAssignmentIssue> AssignmentIssues(this object s1, string[] ignoreProperties = null)
        {
            return DoAssignmentIssues(null, null, s1, new List<object>(), ignoreProperties);
        }

        private static IEnumerable<IAssignmentIssue> DoAssignmentIssues(string parentObject1Name, string field1Name, object s1, List<object> visited, string[] ignoreProperties)
        {
            if (s1 == null)
            {
                if (ignoreProperties == null || ignoreProperties.All(x => x != $"{parentObject1Name}.{field1Name}"))
                {
                    yield return new AssignmentIssue(parentObject1Name, $"The field '{parentObject1Name}.{field1Name}' is not assigned.", null);
                }
            }
            else
            {
                var tp1 = s1.GetType();

                var pp1 = field1Name ?? s1.GetType().Name;
                var item1Name = parentObject1Name != null ? $"{parentObject1Name}.{pp1}" : s1.GetType().Name;

                if (s1 is string)
                {
                    if (s1.ToString().Equals(GetDefault(tp1)))
                    {
                        if (ignoreProperties == null || ignoreProperties.All(x => x != $"{parentObject1Name}.{field1Name}"))
                        {
                            yield return new AssignmentIssue(parentObject1Name, $"The string value '{parentObject1Name}.{field1Name}' is not assigned.", null);
                        }
                    }
                }
                else if (s1 is bool)
                {
                    foreach (var diff2 in CheckValue(s1, item1Name, ignoreProperties))
                    {
                        yield return diff2;
                    }
                }
                else if (s1 is DateTime)
                {
                    foreach (var diff2 in CheckValue(s1, item1Name, ignoreProperties))
                    {
                        yield return diff2;
                    }
                }
                else if (s1 is int)
                {
                    foreach (var diff2 in CheckValue(s1, item1Name, ignoreProperties))
                    {
                        yield return diff2;
                    }
                }
                else if (s1 is decimal || s1 is double)
                {
                    foreach (var diff2 in CheckValue(s1, item1Name, ignoreProperties))
                    {
                        yield return diff2;
                    }
                }
                else if (s1 is Uri)
                {
                    foreach (var diff2 in CheckValue(s1, item1Name, ignoreProperties))
                    {
                        yield return diff2;
                    }
                }
                else if (s1 is TimeSpan)
                {
                    foreach (var diff2 in CheckValue(s1, item1Name, ignoreProperties))
                    {
                        yield return diff2;
                    }
                }
                else if (s1 is Enum)
                {
                    foreach (var diff2 in CheckValue(s1, item1Name, ignoreProperties))
                    {
                        yield return diff2;
                    }
                }
                else if (tp1.IsValueType)
                {
                    foreach (var diff in CheckValueTypes(item1Name, s1, visited, ignoreProperties))
                    {
                        yield return diff;
                    }
                }
                else
                {
                    foreach (var diff1 in CheckReferenceTypes(item1Name, s1, visited, ignoreProperties))
                    {
                        yield return diff1;
                    }
                }
            }
        }

        private static IEnumerable<IAssignmentIssue> CheckReferenceTypes(string item1Name, object s1, List<object> visited, string[] ignoreProperties)
        {
            if (s1 is IEnumerable enumerable)
            {
                var enumr1 = enumerable.GetEnumerator();
                var used = new List<object>();

                var index = 0;
                while (true)
                {
                    var ptr1 = enumr1.MoveNext();

                    if (!ptr1)
                    {
                        yield break;
                    }

                    var data1 = enumr1.Current;

                    var diffs = DoAssignmentIssues(item1Name, null, data1, visited, ignoreProperties);
                    foreach (var diff in diffs)
                    {
                        yield return new AssignmentIssue(diff.ObjectName, diff.Message, index);
                    }

                    index++;
                }
            }

            foreach (var diff2 in CheckMembers(item1Name, s1, visited, ignoreProperties))
            {
                yield return diff2;
            }
        }

        private static IEnumerable<IAssignmentIssue> CheckValueTypes(string item1Name, object s1, List<object> visited, string[] ignoreProperties)
        {
            foreach (var diff2 in CheckMembers(item1Name, s1, visited, ignoreProperties))
            {
                yield return diff2;
            }

            visited.Add(s1);
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

        private static IEnumerable<IAssignmentIssue> CheckMembers(string item1Name, object s1, List<object> visited, string[] ignoreProperties)
        {
            if (visited.Contains(s1))
            {
                yield break;
            }

            visited.Add(s1);

            var fields = s1.GetType().GetFields();
            foreach (var field in fields)
            {
                var f1 = field.GetValue(s1);

                //var f2 = GetValueFromPropertyOrField(item1Name, item2Name, s2, field.Name, out var diff1);
                //if (f2 == null && diff1 != null)
                //    yield return diff1;
                //else
                //{
                //    var diffs = DoCompare(item1Name, item2Name, f1, f2, compareMode, visited);
                var diffs = DoAssignmentIssues(item1Name, field.Name, f1, visited, ignoreProperties);
                //
                foreach (var diff in diffs)
                {
                    yield return diff;
                }
                //}
            }

            var tp = s1.GetType();
            var props = tp.GetProperties();
            foreach (var prop in props)
            {
                var v1 = prop.GetValue(s1, null);
                //var v2 = GetValueFromPropertyOrField(item1Name, item2Name, s2, prop.Name, out var diff1);
                //if (v2 == null && diff1 != null)
                //    yield return diff1;
                //else
                //{
                var diffs = DoAssignmentIssues(item1Name, prop.Name, v1, visited, ignoreProperties);

                foreach (var diff in diffs)
                {
                    yield return diff;
                }
                //}
            }
        }

        private static object GetValueFromPropertyOrField(string item1Name, string item2Name, object s2, string name, out IAssignmentIssue diff)
        {
            diff = null;

            var prop = s2.GetType().GetProperty(name);
            if (prop != null)
            {
                var val = prop.GetValue(s2, null);
                return val;
            }

            var field = s2.GetType().GetField(name);
            if (field != null)
            {
                var val = field.GetValue(s2);
                return val;
            }

            //diff = new Diff(item1Name, item2Name, $"Cannot find the property or field named {name} in object of type {s2.GetType()}.", null);
            //return null;
            throw new NotImplementedException();
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