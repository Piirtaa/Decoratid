using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Storidioms.Masking;


namespace Decoratid.Core.Storing
{
    /// <summary>
    /// Fluent decorator.  By convention put all FluentDecorators in Decoratid.Core.Storing namespace.
    /// </summary>
    public static partial class FluentDecorator
    {
        /// <summary>
        /// constrains the operations so that only the ones specified are enabled
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static MaskingDecoration AllowOperations(this IStore decorated,
            StoreOperation mask)
        {
            Condition.Requires(decorated).IsNotNull();
            return new MaskingDecoration(decorated, mask);
        }
        /// <summary>
        /// disables commit operations
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static MaskingDecoration NoCommit(this IStore decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new MaskingDecoration(decorated, StoreOperation.Get | StoreOperation.GetAll | StoreOperation.Search);
        }
        /// <summary>
        /// disables get operations
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static MaskingDecoration NoGet(this IStore decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new MaskingDecoration(decorated, StoreOperation.Commit | StoreOperation.GetAll | StoreOperation.Search);
        }
        /// <summary>
        /// disables search operations
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static MaskingDecoration NoSearch(this IStore decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new MaskingDecoration(decorated, StoreOperation.Get | StoreOperation.GetAll | StoreOperation.Commit);
        }
        /// <summary>
        /// disables getall operations
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static MaskingDecoration NoGetAll(this IStore decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new MaskingDecoration(decorated, StoreOperation.Get | StoreOperation.Commit | StoreOperation.Search);
        }
    }
}
