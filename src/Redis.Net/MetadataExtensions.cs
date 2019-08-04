using System;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Redis.Net {
    internal static class MetadataExtensions {
        /// <summary>
        ///     根据表达式获取 成员名称
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string MemberName<TModel, TValue>(this Expression<Func<TModel, TValue>> expression) {
            if (expression == null) {
                throw new ArgumentNullException(nameof(expression));
            }
            return expression.GetMemberInfo().Name;
        }

        #region GetMemberInfo 获取 表达式的 成员信息

        /// <summary>
        /// 获取 表达式中的成员信息
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="model"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MemberInfo GetMemberInfo<TModel, TValue>(this TModel model,
            Expression<Func<TModel, TValue>> expression) {
            if (expression == null) {
                throw new ArgumentNullException(nameof(expression));
            }
            return expression.GetMemberInfo();
        }

        /// <summary>
        ///     获取 表达式的 成员信息
        ///     如 r=> r.Name
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static MemberInfo GetMemberInfo(this Expression expression) {
            switch (expression.NodeType) {
                case ExpressionType.Lambda:
                    return GetMemberInfo(expression as LambdaExpression);
                case ExpressionType.MemberAccess:
                    return GetMemberInfo(expression as MemberExpression);
                case ExpressionType.Convert:
                    return GetMemberInfo(expression as UnaryExpression);
                case ExpressionType.Call:
                    return GetMemberInfo(expression as MethodCallExpression);
                default:
                    throw new InvalidExpressionException($"不支持的表达式类型:{expression.NodeType}\n表达式:{expression}");
            }
        }

        /// <summary>
        /// 获取 <see cref="LambdaExpression"/>表达式的 成员信息
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns></returns>
        private static MemberInfo GetMemberInfo(this LambdaExpression lambda) {
            return GetMemberInfo(lambda.Body);
        }

        /// <summary>
        /// 获取 <see cref="UnaryExpression"/>表达式的 成员信息
        /// </summary>
        /// <param name="unary"></param>
        /// <returns></returns>
        private static MemberInfo GetMemberInfo(this UnaryExpression unary) {
            return GetMemberInfo(unary.Operand);
        }

        /// <summary>
        /// 获取 <see cref="MemberExpression"/>表达式的 成员信息
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private static MemberInfo GetMemberInfo(this MemberExpression member) {
            return member.Member;
        }

        /// <summary>
        /// 获取 <see cref="MethodCallExpression"/>表达式的 成员信息
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns></returns>
        private static MemberInfo GetMemberInfo(this MethodCallExpression lambda) {
            return lambda.Method;
        }

        #endregion GetMemberInfo
    }
}