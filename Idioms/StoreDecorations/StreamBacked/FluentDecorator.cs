using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Crypto;
using Decoratid.Core.Storing.Decorations.StreamBacked;

using Decoratid.Core.Storing.Decorations;

namespace Decoratid.Core.Storing
{
    /// <summary>
    /// Fluent decorator.  By convention put all FluentDecorators in Decoratid.Core.Storing namespace.
    /// </summary>
    public static partial class FluentDecorator
    {
        /// <summary>
        /// gets the first file backed store in the decoration chain
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static BackingFileDecoration GetFileBackedDecoration(this IStore decorated)
        {
            return decorated.FindDecoratorOf<BackingFileDecoration>(true);
        }
        public static BackingLockedFileDecoration GetLockedFileBackedDecoration(this IStore decorated)
        {
            return decorated.FindDecoratorOf<BackingLockedFileDecoration>(true);
        }
        /// <summary>
        /// adds a backing stream.  Writes on backing condition (if supplied), else Writes on every commit
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="stream"></param>
        /// <param name="backingCondition"></param>
        /// <returns></returns>
        public static BackingStreamDecoration DecorateWithBackingStream(this IStore decorated, Stream stream, ICondition backingCondition = null)
        {
            Condition.Requires(decorated).IsNotNull();
            if (backingCondition != null)
                return new BackingStreamDecoration(decorated, stream, backingCondition);

            return new BackingStreamDecoration(decorated, stream);
        }
        /// <summary>
        /// adds a backing file.  Writes on backing condition (if supplied), else Writes on every commit
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="filePath"></param>
        /// <param name="backingCondition"></param>
        /// <returns></returns>
        public static BackingFileDecoration DecorateWithBackingFile(this IStore decorated, string filePath, ICondition backingCondition = null)
        {
            Condition.Requires(decorated).IsNotNull();
            if (backingCondition != null)
                return new BackingFileDecoration(decorated, filePath, backingCondition);

            return new BackingFileDecoration(decorated, filePath);
        }
        /// <summary>
        /// adds a backing locked file.  Writes on backing condition (if supplied), else Writes on every commit
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="filePath"></param>
        /// <param name="backingCondition"></param>
        /// <returns></returns>
        public static BackingLockedFileDecoration DecorateWithBackingLockedFile(this IStore decorated, string filePath, ICondition backingCondition = null)
        {
            Condition.Requires(decorated).IsNotNull();
            if (backingCondition != null)
                return new BackingLockedFileDecoration(decorated, filePath, backingCondition);

            return new BackingLockedFileDecoration(decorated, filePath);
        }
        /// <summary>
        /// adds encryption to the backing stream
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="keyAndEntropy"></param>
        /// <returns></returns>
        public static BackingEncryptedStreamDecoration DecorateWithBackingEncryption(this BackingStreamDecoration decorated, SymmetricCipherPair keyAndEntropy)
        {
            Condition.Requires(decorated).IsNotNull();

            return new BackingEncryptedStreamDecoration(decorated, keyAndEntropy);
        }
    }
}
