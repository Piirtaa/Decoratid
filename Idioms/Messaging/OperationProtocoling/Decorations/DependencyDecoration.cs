﻿using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Idioms.Depending;
using System.Collections.Generic;
using System.Linq;
using Decoratid.Extensions;
using Decoratid.Core.Decorating;
using System;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Tasking;
using Decoratid.Idioms.Tasking.Decorations;

namespace Decoratid.Idioms.Messaging.OperationProtocoling.Decorations
{
    /// <summary>
    /// indicates the Operation has a dependency on other Operations in the same store
    /// </summary>
    public interface IHasOperationDependency : IDecoratedOperation, IHasDependencyOf<string>
    {
    }

    /// <summary>
    /// decorate a Operation indicating it needs the provided Operations to complete before starting.
    /// Automatically decorates with ConditionalTriggers to accomplish this.
    /// </summary>
    public class DependencyDecoration : DecoratedOperationBase, IHasOperationDependency
    {
        #region Ctor
        public DependencyDecoration(IOperation decorated, List<string> prerequisiteOperationIds)
            : base(decorated)
        {
            //we can only have one dependency decoration per cake
            if (DecorationUtils.HasDecoration<DependencyDecoration>(decorated))
            {
                throw new InvalidOperationException("already decorated");
            }

            this.Dependency = new DependencyOf<string>(this.Id);
            prerequisiteOperationIds.WithEach(x => { this.Dependency.Prerequisites.Add(x); });
        }
        #endregion

        #region IDecoratedOperation
        public override ITask GetPerformTask(IStore requestStore, IStore responseStore)
        {
            var task = base.GetPerformTask(requestStore, responseStore);
            task = task.DependsOn(this.Dependency.Prerequisites.ToArray());
            return task;
        }
        public override IDecorationOf<IOperation> ApplyThisDecorationTo(IOperation Operation)
        {
            return new DependencyDecoration(Operation, this.Dependency.Prerequisites);
        }
        #endregion

        #region Properties
        public IDependencyOf<string> Dependency { get; protected set; }
        #endregion
       
    }

    public static class DependencyDecorationExtensions
    {
        public static IHasOperationDependency DependsOn(this IOperation Operation, params string[] prerequisiteOperationIds)
        {
            Condition.Requires(Operation).IsNotNull();

            //we can only have one dependency decoration per cake, so go grab that one and update it
            if (DecorationUtils.HasDecoration<DependencyDecoration>(Operation))
            {
                var dec = DecorationUtils.GetDecoration<DependencyDecoration>(Operation);
                dec.Dependency.Prerequisites.AddRange(prerequisiteOperationIds);
                return dec;
            }

            return new DependencyDecoration(Operation, prerequisiteOperationIds.ToList());
        }
        public static IOperation DoesNotDependOn(this IOperation Operation, params string[] prerequisiteOperationIds)
        {
            Condition.Requires(Operation).IsNotNull();
            if (DecorationUtils.HasDecoration<DependencyDecoration>(Operation))
            {
                var dec = DecorationUtils.GetDecoration<DependencyDecoration>(Operation);
                prerequisiteOperationIds.WithEach(pre =>
                {
                    dec.Dependency.Prerequisites.Remove(pre);
                });

                return dec;
            }

            return Operation;
        }

    }
}
