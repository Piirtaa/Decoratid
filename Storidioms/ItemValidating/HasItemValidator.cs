using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.Polyfacing;

namespace Decoratid.Storidioms.ItemValidating
{

    /// <summary>
    /// basically a blank validator where one supplies the validating condition.  Also polyfacing so we 
    /// can compose with it (this is typical pattern with polyfacing - use of "has" containers.  this
    /// gives us has property to decorate, and known container type with the polyface).
    /// </summary>
    [Serializable]
    public class HasItemValidator : IHasItemValidator, IPolyfacing
    {
        #region Ctor
        public HasItemValidator(IItemValidator validator)
        {
            Condition.Requires(validator).IsNotNull();
            this.Validator = validator;
        }
        #endregion

        #region IItemValidator
        public IConditionOf<IHasId> IsValidCondition { get { return this.Validator.IsValidCondition; } }
        #endregion

        #region IHasItemValidator
        public IItemValidator Validator { get; private set; }
        #endregion

        #region IPolyfacing
        Polyface IPolyfacing.RootFace { get; set; }
        #endregion

        #region Static Methods
        public static HasItemValidator New(IItemValidator validator)
        {
            return new HasItemValidator(validator);
        }
        #endregion
    }

    public static class HasItemValidatorExtensions
    {
        public static Polyface IsHasItemValidator(this Polyface root, IItemValidator validator)
        {
            Condition.Requires(root).IsNotNull();
            var obj = HasItemValidator.New(validator);
            root.Is(obj);
            return root;
        }
        public static IItemValidator AsHasItemValidator(this Polyface root)
        {
            Condition.Requires(root).IsNotNull();
            var rv = root.As<HasItemValidator>();
            return rv;
        }
    }
}
