using System.Linq.Expressions;

namespace ExpressionTrees.Task1.ExpressionsTransformer
{
	public class IncDecExpressionVisitor : ExpressionVisitor
    {
        protected override Expression VisitBinary(BinaryExpression node)
        {
            Expression param = null;
            ConstantExpression constant = null;

            if (node.Left.NodeType == ExpressionType.Parameter && node.Right.NodeType == ExpressionType.Constant)
            {
                param = node.Left;
                constant = node.Right as ConstantExpression;
            }

            if (node.Left.NodeType == ExpressionType.Constant && node.Right.NodeType == ExpressionType.Parameter)
            {
                param = node.Right;
                constant = node.Left as ConstantExpression;
            }

            if (param != null && constant?.Type == typeof(int) && (int)constant.Value == 1)
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Add:
                        return Expression.Increment(param);
                    case ExpressionType.Subtract:
                        return Expression.Decrement(param);
                }
            }

			return base.VisitBinary(node);
        }
    }
}
