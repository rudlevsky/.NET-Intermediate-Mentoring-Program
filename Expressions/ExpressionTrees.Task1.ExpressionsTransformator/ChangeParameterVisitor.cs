using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionTrees.Task1.ExpressionsTransformer
{
	public class ChangeParameterVisitor : ExpressionVisitor
    {
        private readonly Dictionary<string, int> dictionary;
        public ChangeParameterVisitor(Dictionary<string, int> dictionary)
        {
            this.dictionary = dictionary;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
			string key = node.Name;
			return dictionary.ContainsKey(key) ? Expression.Constant(dictionary[key]) : base.VisitParameter(node);
        }

		protected override Expression VisitLambda<T>(Expression<T> node)
		{
            return Expression.Lambda(Visit(node.Body));
        }
	}
}
