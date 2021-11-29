using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Expressions.Task3.E3SQueryProvider
{
	public class ExpressionToFtsRequestTranslator : ExpressionVisitor
    {
        readonly StringBuilder _resultStringBuilder;

        public ExpressionToFtsRequestTranslator()
        {
            _resultStringBuilder = new StringBuilder();
        }

        public string Translate(Expression exp)
        {
            _resultStringBuilder.Append("'statements':[");
            Visit(exp);

            return _resultStringBuilder.Append("]").ToString();
        }

        #region protected methods

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == "Where")
            {
                var predicate = node.Arguments[1];
                Visit(predicate);

                return node;
            }

            Expression right = node.Arguments[0];
            Expression left = node.Object;

            if (node.Method.Name == "Equals")
            {
                Visit(left);
                _resultStringBuilder.Append("(");
                Visit(right);
                _resultStringBuilder.Append(")").Append("'}");

                return node;
            }

            if (node.Method.Name == "StartsWith")
            {
                Visit(left);
                _resultStringBuilder.Append("(");
                Visit(right);
                _resultStringBuilder.Append("*)").Append("'}");

                return node;
            }

            if (node.Method.Name == "EndsWith")
            {
                Visit(left);
                _resultStringBuilder.Append("(*");
                Visit(right);
                _resultStringBuilder.Append(")").Append("'}");

                return node;
            }

            if (node.Method.Name == "Contains")
            {
                Visit(left);
                _resultStringBuilder.Append("(*");
                Visit(right);
                _resultStringBuilder.Append("*)").Append("'}");

                return node;
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    Expression leftNode = null;
                    Expression rightNode = null;

                    if (node.Left.NodeType == ExpressionType.MemberAccess && node.Right.NodeType == ExpressionType.Constant)
                    {
                        leftNode = node.Left;
                        rightNode = node.Right;
                    }
                    
                    if (node.Right.NodeType == ExpressionType.MemberAccess && node.Left.NodeType == ExpressionType.Constant)
                    {
                        leftNode = node.Right;
                        rightNode = node.Left;
                    }

                    Visit(leftNode);
                    _resultStringBuilder.Append("(");
                    Visit(rightNode);
                    _resultStringBuilder.Append(")'}");
                    break;
				case ExpressionType.AndAlso:
					Visit(node.Left);
                    _resultStringBuilder.Append(",");
                    Visit(node.Right);
					break;
				default:
                    throw new NotSupportedException($"Operation '{node.NodeType}' is not supported");
            };

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _resultStringBuilder.Append("{'query':").Append($"'{node.Member.Name}").Append(":");

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _resultStringBuilder.Append(node.Value);

            return node;
        }

        #endregion
    }
}
