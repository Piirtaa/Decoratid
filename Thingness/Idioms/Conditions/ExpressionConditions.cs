using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;

namespace Sandbox.Conditions
{
    public class ExpressionCondition : FuncCondition
    {
        #region Declarations
        private readonly object _stateLock = new object();

        private Expression<Func<bool>> _expression = null;
        #endregion

        #region Ctor
        public ExpressionCondition(Expression<Func<bool>> expr)
            : base()
        {
            Condition.Requires(expr).IsNotNull();
            this.Expression = expr;

        }
        #endregion

        #region Properties
        public Expression<Func<bool>> Expression
        {
            get { return this._expression; }
            set
            {
                lock (this._stateLock)
                {
                    this._expression = value;
                    //also change the condition
                    this.ConditionStrategy = value.Compile();
                }
            }
        }
        #endregion
    }

    public class ExpressionConditionOf<T> : FuncConditionOf<T>
    {
        #region Declarations
        private readonly object _stateLock = new object();

        private Expression<Func<T, bool>> _expression = null;
        #endregion

        #region Ctor
        public ExpressionConditionOf(Expression<Func<T,bool>> expr)
            : base()
        {
            Condition.Requires(expr).IsNotNull();
            this.Expression = expr;

        }
        #endregion

        #region Properties
        public Expression<Func<T, bool>> Expression
        {
            get { return this._expression; }
            set
            {
                lock (this._stateLock)
                {
                    this._expression = value;
                    //also change the condition
                    this.ConditionStrategy = value.Compile();
                }
            }
        }
        #endregion
    }

    public class MutableExpressionCondition : MutableFuncCondition
    {
        #region Declarations
        private readonly object _stateLock = new object();

        private Expression<Func<bool>> _conditionExpression = null;
        private Expression<Action> _mutateExpression = null;
        #endregion

        #region Ctor
        public MutableExpressionCondition(Expression<Func<bool>> conditionExpression, Expression<Action> mutateExpression)
            : base()
        {
            Condition.Requires(conditionExpression).IsNotNull();
            Condition.Requires(mutateExpression).IsNotNull();

            this.ConditionExpression = conditionExpression;
            this.MutateExpression = mutateExpression;
        }
        #endregion

        #region Properties
        public Expression<Func<bool>> ConditionExpression
        {
            get { return this._conditionExpression; }
            set
            {
                lock (this._stateLock)
                {
                    this._conditionExpression = value;
                    //also change the condition
                    this.ConditionStrategy = value.Compile();
                }
            }
        }
        public Expression<Action> MutateExpression
        {
            get { return this._mutateExpression; }
            set
            {
                lock (this._stateLock)
                {
                    this._mutateExpression = value;
                    //also change the condition
                    this.MutateStrategy = value.Compile();
                }
            }
        }
        #endregion
    }

    public class MutableExpressionConditionOf<T> : MutableFuncConditionOf<T>
    {
        #region Declarations
        private readonly object _stateLock = new object();

        private Expression<Func<T, bool>> _conditionExpression = null;
        private Expression<Action<T>> _mutateExpression = null;
        #endregion

        #region Ctor
        public MutableExpressionConditionOf(Expression<Func<T,bool>> conditionExpression, Expression<Action<T>> mutateExpression)
            : base()
        {
            Condition.Requires(conditionExpression).IsNotNull();
            Condition.Requires(mutateExpression).IsNotNull();

            this.ConditionExpression = conditionExpression;
            this.MutateExpression = mutateExpression;
        }
        #endregion

        #region Properties
        public Expression<Func<T,bool>> ConditionExpression
        {
            get { return this._conditionExpression; }
            set
            {
                lock (this._stateLock)
                {
                    this._conditionExpression = value;
                    //also change the condition
                    this.ConditionStrategy = value.Compile();
                }
            }
        }
        public Expression<Action<T>> MutateExpression
        {
            get { return this._mutateExpression; }
            set
            {
                lock (this._stateLock)
                {
                    this._mutateExpression = value;
                    //also change the condition
                    this.MutateStrategy = value.Compile();
                }
            }
        }
        #endregion
    }
}
