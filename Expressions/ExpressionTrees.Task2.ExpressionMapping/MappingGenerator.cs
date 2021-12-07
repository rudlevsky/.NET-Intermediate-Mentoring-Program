using System;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionTrees.Task2.ExpressionMapping
{
    public class MappingGenerator
    {
        public Mapper<TSource, TDestination> Generate<TSource, TDestination>()
        {
            var sourceParam = Expression.Parameter(typeof(TSource));
            var destProperties = typeof(TDestination).GetProperties();

            var sourceProps = typeof(TSource).GetProperties().Where(p =>
                destProperties.FirstOrDefault(x => x.Name == p.Name && x.PropertyType == p.PropertyType) != null
            );

			var bindings = sourceProps.Select(p =>
			    Expression.Bind(typeof(TDestination).GetProperty(p.Name, p.PropertyType), Expression.Property(sourceParam, p))
			);

			var body = Expression.MemberInit(Expression.New(typeof(TDestination)), bindings);

			var mapFunction = Expression.Lambda<Func<TSource, TDestination>>(body, sourceParam);

            return new Mapper<TSource, TDestination>(mapFunction.Compile());
        }
    }
}
