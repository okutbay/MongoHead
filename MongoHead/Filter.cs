using System;
using System.Collections.Generic;
using System.Text;

using System.Linq.Expressions;
using System.Reflection;

namespace MongoHead
{
    //List<Filter> filter = new List<Filter>()
    // {
    //     new Filter { PropertyName = "City" , 
    //        Operation = Op .Equals, Value = "Mitrovice"  },
    //     new Filter { PropertyName = "Name" , 
    //        Operation = Op .StartsWith, Value = "L"  },
    //     new Filter { PropertyName = "Salary" , 
    //        Operation = Op .GreaterThan, Value = 9000.0 }
    // };

    // var deleg = ExpressionBuilder.GetExpression<Person>(filter).Compile();
    // var filteredCollection = persons.Where(deleg).ToList();

    public class Filter
    {
        public string PropertyName { get; set; }
        public Op Operation { get; set; }
        public object Value { get; set; }
    }

    public enum Op
    {
        Equals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith,
        NotEqual
    }

    public static class ExpressionBuilder
    {
        private static readonly MethodInfo containsMethod = typeof(string).GetMethod("Contains");
        private static readonly MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        private static readonly MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

        public static Expression<Func<T, bool>> GetExpression<T>(IList<Filter> filters, bool UseAndLogic = true)
        {
            if (filters.Count == 0)
            {
                return null;
            }

            ParameterExpression param = Expression.Parameter(typeof(T), "t");
            Expression exp = null;

            if (filters.Count == 1)
            {
                exp = GetExpression<T>(param, filters[0]);
            }
            else if (filters.Count == 2)
            {
                exp = GetExpression<T>(param, filters[0], filters[1]);
            }
            else
            {
                while (filters.Count > 0)
                {
                    var f1 = filters[0];
                    var f2 = filters[1];

                    if (exp == null)
                    {
                        if (UseAndLogic)
                        {
                            exp = GetExpression<T>(param, filters[0], filters[1]);
                        }
                        else
                        {
                            exp = GetExpression<T>(param, filters[0], filters[1], false);
                        }
                    }
                    else
                    {
                        if (UseAndLogic)
                        {
                            exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0], filters[1]));
                        }
                        else
                        {
                            exp = Expression.OrElse(exp, GetExpression<T>(param, filters[0], filters[1], false));
                        }
                    }

                    filters.Remove(f1);
                    filters.Remove(f2);

                    if (filters.Count == 1)
                    {
                        if (UseAndLogic)
                        {
                            exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0]));
                        }
                        else
                        {
                            exp = Expression.OrElse(exp, GetExpression<T>(param, filters[0]));
                        }

                        filters.RemoveAt(0);
                    }
                }
            }

            return Expression.Lambda<Func<T, bool>>(exp, param);
        }

        private static Expression GetExpression<T>(ParameterExpression param, Filter filter)
        {
            MemberExpression member = Expression.Property(param, filter.PropertyName);
            ConstantExpression constant = Expression.Constant(filter.Value);

            switch (filter.Operation)
            {
                case Op.Equals:
                    return Expression.Equal(member, constant);

                case Op.GreaterThan:
                    return Expression.GreaterThan(member, constant);

                case Op.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(member, constant);

                case Op.LessThan:
                    return Expression.LessThan(member, constant);

                case Op.LessThanOrEqual:
                    return Expression.LessThanOrEqual(member, constant);

                case Op.Contains:
                    var toLower = Expression.Call(member, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                    ConstantExpression constant1 = Expression.Constant(filter.Value.ToString().ToLower());

                    return Expression.Call(toLower,
                            typeof(string).GetMethod("Contains"),
                            constant1);

                //return Expression.Call(member, containsMethod, constant);

                case Op.StartsWith:
                    return Expression.Call(member, startsWithMethod, constant);

                case Op.EndsWith:
                    return Expression.Call(member, endsWithMethod, constant);

                case Op.NotEqual:
                    return Expression.NotEqual(member, constant);
            }

            return null;
        }

        private static BinaryExpression GetExpression<T>(ParameterExpression param, Filter filter1, Filter filter2, bool UseAndLogic = true)
        {
            Expression bin1 = GetExpression<T>(param, filter1);
            Expression bin2 = GetExpression<T>(param, filter2);

            if (UseAndLogic)
            {
                return Expression.AndAlso(bin1, bin2);
            }
            else
            {
                return Expression.OrElse(bin1, bin2);
            }
        }
    }


}
