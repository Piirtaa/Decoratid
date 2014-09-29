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
        public static MaskingDecoration DecorateWithOnlyTheseOperations(this IStore decorated,
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
        public static MaskingDecoration DecorateWithoutCommit(this IStore decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new MaskingDecoration(decorated, StoreOperation.Get | StoreOperation.GetAll | StoreOperation.Search);
        }
        /// <summary>
        /// disables get operations
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static MaskingDecoration DecorateWithoutGet(this IStore decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new MaskingDecoration(decorated, StoreOperation.Commit | StoreOperation.GetAll | StoreOperation.Search);
        }
        /// <summary>
        /// disables search operations
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static MaskingDecoration DecorateWithoutSearch(this IStore decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new MaskingDecoration(decorated, StoreOperation.Get | StoreOperation.GetAll | StoreOperation.Commit);
        }
        /// <summary>
        /// disables getall operations
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static MaskingDecoration DecorateWithoutGetAll(this IStore decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new MaskingDecoration(decorated, StoreOperation.Get | StoreOperation.Commit | StoreOperation.Search);
        }
    }
}
