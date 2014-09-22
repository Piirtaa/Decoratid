using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.Storing;
using Decoratid.Extensions;
using CuttingEdge.Conditions;

namespace Decoratid.Idioms.Intercepting.Decorating
{
    /// <summary>
    /// marker interface to faciliate fast classification
    /// </summary>
    public interface IDecoratingInterceptionException
    {
    }

    [Serializable]
    public class DecoratingInterceptionException<TArg, TResult> : ApplicationException, IDecoratingInterceptionException
    {
        #region Declarations
        private DecoratingInterceptUnitOfWork<TArg, TResult> _uow = null;
        #endregion

        #region Ctor
        public DecoratingInterceptionException(DecoratingInterceptUnitOfWork<TArg,TResult> uow, string message)
            : base(message)
        {
            Condition.Requires(uow).IsNotNull();
            this._uow = uow;
        }
        public DecoratingInterceptionException(DecoratingInterceptUnitOfWork<TArg, TResult> uow, string message, Exception innerException) 
            : base(message, innerException) 
        {
            Condition.Requires(uow).IsNotNull();
            this._uow = uow;
        }
        protected internal DecoratingInterceptionException(SerializationInfo info, StreamingContext context)
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
            this._uow = info.GetValue("_uow", typeof(DecoratingInterceptUnitOfWork<TArg, TResult>)).ConvertTo<DecoratingInterceptUnitOfWork<TArg, TResult>>();

        }
        #endregion

        #region Properties
        public DecoratingInterceptUnitOfWork<TArg, TResult> UoW { get { return _uow; } }
        #endregion
    }

    [Serializable]
    public class DecoratingInterceptionLayerException<TArg, TResult> : DecoratingInterceptionException<TArg, TResult>
    {
        #region Declarations
        private string _layerId = null;
        #endregion
        
        #region Ctor
        public DecoratingInterceptionLayerException(DecoratingInterceptLayer<TArg, TResult> layer, DecoratingInterceptUnitOfWork<TArg, TResult> uow, string message)
            : base(uow, message)
        {
            Condition.Requires(layer).IsNotNull();
            this._layerId = layer.Id;
        }
        public DecoratingInterceptionLayerException(DecoratingInterceptLayer<TArg, TResult> layer, DecoratingInterceptUnitOfWork<TArg, TResult> uow, string message, Exception innerException) 
            : base(uow, message, innerException) 
        {
            Condition.Requires(layer).IsNotNull();
            this._layerId = layer.Id;
        }
        protected internal DecoratingInterceptionLayerException(SerializationInfo info, StreamingContext context)
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

    [Serializable]
    public class ArgDecorationDecoratingInterceptionException<TArg, TResult> : DecoratingInterceptionLayerException<TArg, TResult>
    {
        #region Ctor
        public ArgDecorationDecoratingInterceptionException(DecoratingInterceptLayer<TArg, TResult> layer, DecoratingInterceptUnitOfWork<TArg, TResult> uow, string message)
            : base(layer, uow, message)
        {
        }
        public ArgDecorationDecoratingInterceptionException(DecoratingInterceptLayer<TArg, TResult> layer, DecoratingInterceptUnitOfWork<TArg, TResult> uow, string message, Exception innerException)
            : base(layer, uow, message, innerException)
        {
        }
        #endregion
    }

    [Serializable]
    public class ArgValidationDecoratingInterceptionException<TArg, TResult> : DecoratingInterceptionLayerException<TArg, TResult>
    {
        #region Ctor
        public ArgValidationDecoratingInterceptionException(DecoratingInterceptLayer<TArg, TResult> layer, DecoratingInterceptUnitOfWork<TArg, TResult> uow, string message)
            : base(layer, uow, message)
        {
        }
        public ArgValidationDecoratingInterceptionException(DecoratingInterceptLayer<TArg, TResult> layer, DecoratingInterceptUnitOfWork<TArg, TResult> uow, string message, Exception innerException)
            : base(layer, uow, message, innerException)
        {
        }
        #endregion
    }

    [Serializable]
    public class ResultDecorationDecoratingInterceptionException<TArg, TResult> : DecoratingInterceptionLayerException<TArg, TResult>
    {
        #region Ctor
        public ResultDecorationDecoratingInterceptionException(DecoratingInterceptLayer<TArg, TResult> layer, DecoratingInterceptUnitOfWork<TArg, TResult> uow, string message)
            : base(layer, uow, message)
        {
        }
        public ResultDecorationDecoratingInterceptionException(DecoratingInterceptLayer<TArg, TResult> layer, DecoratingInterceptUnitOfWork<TArg, TResult> uow, string message, Exception innerException)
            : base(layer, uow, message, innerException)
        {
        }
        #endregion
    }

    [Serializable]
    public class ResultValidationDecoratingInterceptionException<TArg, TResult> : DecoratingInterceptionLayerException<TArg, TResult>
    {
        #region Ctor
        public ResultValidationDecoratingInterceptionException(DecoratingInterceptLayer<TArg, TResult> layer, DecoratingInterceptUnitOfWork<TArg, TResult> uow, string message)
            : base(layer, uow, message)
        {
        }
        public ResultValidationDecoratingInterceptionException(DecoratingInterceptLayer<TArg, TResult> layer, DecoratingInterceptUnitOfWork<TArg, TResult> uow, string message, Exception innerException)
            : base(layer, uow, message, innerException)
        {
        }
        #endregion
    }
}
