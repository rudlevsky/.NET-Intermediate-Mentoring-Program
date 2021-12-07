/*
 * Create a class based on ExpressionVisitor, which makes expression tree transformation:
 * 1. converts expressions like <variable> + 1 to increment operations, <variable> - 1 - into decrement operations.
 * 2. changes parameter values in a lambda expression to constants, taking the following as transformation parameters:
 *    - source expression;
 *    - dictionary: <parameter name: value for replacement>
 * The results could be printed in console or checked via Debugger using any Visualizer.
 */
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionTrees.Task1.ExpressionsTransformer
{
	class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Expression Visitor for increment/decrement.");
            Console.WriteLine();

			Dictionary<string, int> paramDictionary = new Dictionary<string, int>
			{
				{ "a", 2 },
				{ "b", 3 }
			};

			Expression<Func<int, int, int>> expression = (a, b) => b + (a + 5) * (b - 1) * (b + 1) + (1 + a);
            var resIncDecExpr = new IncDecExpressionVisitor().VisitAndConvert(expression, string.Empty);
            var res = resIncDecExpr.Compile().Invoke(2, 3);

            var resChangeParamExpr = new ChangeParameterVisitor(paramDictionary).Visit(expression);

            Console.WriteLine("Task #1");
            Console.WriteLine(expression);
            Console.WriteLine(resIncDecExpr);
            Console.WriteLine(res);

            Console.WriteLine();
            Console.WriteLine("Task #2");
            Console.WriteLine(resChangeParamExpr);

            Console.ReadLine();
        }
    }
}
