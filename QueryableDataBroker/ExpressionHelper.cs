using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace QueryableDataBroker
{
    public class ExpressionHelper
    {
        public static MethodCallExpression CallMethod(Expression instance, string methodName, params object[] parameters)
        {
            Type instanceType = null;

            if (instance is MemberExpression)
            {
                var memberExp = (MemberExpression)instance;
                var propInfo = (PropertyInfo)memberExp.Member;
                instanceType = propInfo.PropertyType;
            }
            else if (instance is ConstantExpression)
            {
                var constantExp = (ConstantExpression)instance;
                instanceType = constantExp.Value.GetType();
            }
            else
            {
                throw new Exception("Type of instance could not be determinated.");
            }

            var paramsAndTypes = new Dictionary<ConstantExpression, Type>();
            foreach (var p in parameters)
            {
                var type = p.GetType();
                paramsAndTypes.Add(Expression.Constant(p, type), type);
            }

            var method = instanceType.GetMethod(methodName, paramsAndTypes.Values.ToArray());

            return Expression.Call(instance, method, paramsAndTypes.Keys);
        }

        public static Expression<Func<I, O>> Lambda<I, O>(Expression body, ParameterExpression param)
        {
            var lambda = Expression.Lambda<Func<I, O>>(body, new[] { param });

            return lambda;
        }

        public static Expression<Func<T, bool>> Compare<T>(PropertyInfo property, object toValue, ComparisonMode compare)
        {
            var itemX = Expression.Parameter(typeof(T), "x");
            var prop = Expression.Property(itemX, property);

            var constVal = Expression.Constant(toValue, toValue.GetType());

            Expression exp = null;

            switch(compare)
            {
                case ComparisonMode.LessThan :
                    exp = Expression.LessThan(prop, constVal);
                    break;
                case ComparisonMode.LessThanEqual:
                    exp = Expression.LessThanOrEqual(prop, constVal);
                    break;
                case ComparisonMode.GreaterThan:
                    exp = Expression.GreaterThan(prop, constVal);
                    break;
                case ComparisonMode.GreaterThanEqual:
                    exp = Expression.GreaterThanOrEqual(prop, constVal);
                    break;
            }

            return ExpressionHelper.Lambda<T, bool>(exp, itemX);
        }

        public static Expression<Func<T, bool>> IsEqualToAny<T>(PropertyInfo property, Type convertTo, params object[] values)
        {
            var itemX = Expression.Parameter(typeof(T), "x");
            var prop = Expression.Property(itemX, property);

            // convert values to same type as property
            if (property.PropertyType != values[0].GetType())
            {
                values = values.Select(v => Convert.ChangeType(v, property.PropertyType)).ToArray();
            }

            var constValues = values.Select(v => Expression.Constant(v, property.PropertyType)).ToArray();

            var exp = Expression.Equal(prop, constValues[0]);
            for (int i = 1; i < constValues.Length; i++)
            {
                exp = Expression.OrElse(exp, Expression.Equal(prop, constValues[i]));
            }

            var lambda = ExpressionHelper.Lambda<T, bool>(exp, itemX);

            return lambda;
        }

        public static PropertyInfo GetPropertyFromExpression<T, P>(Expression<Func<T, P>> property)
        {
            var member = ((MemberExpression)property.Body).Member;

            var type = typeof(T);
            var prop = type.GetRuntimeProperty(member.Name);
            if (prop == null)
            {
                throw new MemberAccessException(String.Format("Property {0} was not found in type {1}.", member.Name, type.Name));
            }

            return prop;
        }
    }

    public enum ComparisonMode
    {
        LessThan,
        LessThanEqual,
        GreaterThan,
        GreaterThanEqual
    }
}
