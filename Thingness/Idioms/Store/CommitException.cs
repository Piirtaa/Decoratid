﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness.Idioms.Store
{
    [Serializable]
    public class CommitException : ApplicationException
    {

        public CommitException(CommitBag bagOfBadItems)
            : base()
        {
            this.BagOfBadItems = bagOfBadItems;
        }


        public CommitBag BagOfBadItems { get; set; }
    }
}
