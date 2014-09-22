using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.Storing;
using Decoratid.Extensions;
using CuttingEdge.Conditions;

namespace Decoratid.Idioms.Intercepting
{
    /// <summary>
    /// marker interface to faciliate fast InterceptionException classification
    /// </summary>
    public interface IInterceptionException
    {
    }
    /// <summary>
    /// base class for an interception exception.  provide the unit of work state
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    [Serializable]
    public class InterceptionException<TArg, TResult> : ApplicationException, IInterceptionException
    {
        #region Declarations
        private InterceptUnitOfWork<TArg, TResult> _uow = null;
        #endregion

        #region Ctor
        public InterceptionException(InterceptUnitOfWork<TArg,TResult> uow, string message)
            : base(message)
        {
            Condition.Requires(uow).IsNotNull();
            this._uow = uow;
        }
        public InterceptionException(InterceptUnitOfWork<TArg, TResult> uow, string message, Exception innerException) 
            : base(message, innerException) 
        {
            Condition.Requires(uow).IsNotNull();
            this._uow = uow;
        }
        protected internal InterceptionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            info.AddValue("_uow", this._uow);
        }
        #endregion

        #region Overrides
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // call base
            base.GetObjectData(info, context);

            //pull out added info
            this._uow = info.GetValue("_uow", typeof(InterceptUnitOfWork<TArg, TResult>)).ConvertTo<InterceptUnitOfWork<TArg, TResult>>();

        }
        #endregion

        #region Properties
        public InterceptUnitOfWork<TArg, TResult> UoW { get { return _uow; } }
        #endregion
    }

    /// <summary>
    /// an interception specifically associated with a layer
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    [Serializable]
    public class LayerInterceptionException<TArg, TResult> : InterceptionException<TArg, TResult>
    {
        #region Declarations
        private string _layerId = null;
        #endregion
        
        #region Ctor
        public LayerInterceptionException(InterceptLayer<TArg, TResult> layer, InterceptUnitOfWork<TArg,TResult> uow, string message)
            : base(uow, message)
        {
            Condition.Requires(layer).IsNotNull();
            this._layerId = layer.Id;
        }
        public LayerInterceptionException(InterceptLayer<TArg, TResult> layer, InterceptUnitOfWork<TArg, TResult> uow, string message, Exception innerException) 
            : base(uow, message, innerException) 
        {
            Condition.Requires(layer).IsNotNull();
            this._layerId = layer.Id;
        }
        protected internal LayerInterceptionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            info.AddValue("_layerId", this._layerId);
        }
        #endregion

        #region Overrides
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // call base
            base.GetObjectData(info, context);

            //pull out added info
            this._layerId = info.GetString("_layerId");
        }
        #endregion

        #region Properties
        public string LayerId { get { return _layerId; } }
        #endregion
    }

    /// <summary>
    /// an interception exception associated with arg decoration
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    [Serializable]
    public class ArgDecorationInterceptionException<TArg, TResult> : LayerInterceptionException<TArg, TResult>
    {
        #region Ctor
        public ArgDecorationInterceptionException(InterceptLayer<TArg, TResult> layer, InterceptUnitOfWork<TArg, TResult> uow, string message)
            : base(layer, uow, message)
        {
        }
        public ArgDecorationInterceptionException(InterceptLayer<TArg, TResult> layer, InterceptUnitOfWork<TArg, TResult> uow, string message, Exception innerException)
            : base(layer, uow, message, innerException)
        {
        }
        #endregion
    }
    /// <summary>
    /// an interception exception associated with arg validation
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    [Serializable]
    public class ArgValidationInterceptionException<TArg, TResult> : LayerInterceptionException<TArg, TResult>
    {
        #region Ctor
        public ArgValidationInterceptionException(InterceptLayer<TArg, TResult> layer, InterceptUnitOfWork<TArg, TResult> uow, string message)
            : base(layer, uow, message)
        {
        }
        public ArgValidationInterceptionException(InterceptLayer<TArg, TResult> layer, InterceptUnitOfWork<TArg, TResult> uow, string message, Exception innerException)
            : base(layer, uow, message, innerException)
        {
        }
        #endregion
    }
    /// <summary>
    /// an interception exception associated with result decoration
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    [Serializable]
    public class ResultDecorationInterceptionException<TArg, TResult> : LayerInterceptionException<TArg, TResult>
    {
        #region Ctor
        public ResultDecorationInterceptionException(InterceptLayer<TArg, TResult> layer, InterceptUnitOfWork<TArg, TResult> uow, string message)
            : base(layer, uow, message)
        {
        }
        public ResultDecorationInterceptionException(InterceptLayer<TArg, TResult> layer, InterceptUnitOfWork<TArg, TResult> uow, string message, Exception innerException)
            : base(layer, uow, message, innerException)
        {
        }
        #endregion
    }
    /// <summary>
    /// an interception exception associated with result validation
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    [Serializable]
    public class ResultValidationInterceptionException<TArg, TResult> : LayerInterceptionException<TArg, TResult>
    {
        #region Ctor
        public ResultValidationInterceptionException(InterceptLayer<TArg, TResult> layer, InterceptUnitOfWork<TArg, TResult> uow, string message)
            : base(layer, uow, message)
        {
        }
        public ResultValidationInterceptionException(InterceptLayer<TArg, TResult> layer, InterceptUnitOfWork<TArg, TResult> uow, string message, Exception innerException)
            : base(layer, uow, message, innerException)
        {
        }
        #endregion
    }
}
