using Decoratid.Core;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Idioms.HasBitsing;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Storidioms.Indexing
{


    /* What is Indexing?
     * Indexing provides additional high-performance bitwise query methods on a store's items. 
     * It is implemented as store of HasBits keyed by item Id.  Queries on this store will
     * point to an SOID that the store contains.  Updates to an item will regenerate the item's 
     * indexed entry.
     * 
     * 
     */






}
