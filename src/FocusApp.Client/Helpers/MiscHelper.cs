using System.Linq.Expressions;

namespace FocusApp.Client.Helpers
{
    internal static class MiscHelper
    {
        /// <summary>
        /// For the given property, returns the name of the property, including the path to the property, as a string.
        /// </summary>
        /// <remarks>
        /// From https://github.com/Nioux/SRD5Helper/blob/7f936733929f1ad6d53baf99674e941a75d451c7/SRD5Helper/Tools/Helpers.cs
        /// </remarks>
        /// <returns></returns>
        public static string NameOf<T>(Expression<Func<T>> pathExpr)
        {
            var members = new Stack<string>();
            for (var memberExpr = pathExpr.Body as MemberExpression; memberExpr != null; memberExpr = memberExpr.Expression as MemberExpression)
            {
                members.Push(memberExpr.Member.Name);
            }
            return string.Join(".", members);
        }
    }
}
