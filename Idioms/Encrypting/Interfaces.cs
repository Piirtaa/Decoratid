﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Encrypting
{
    public interface IHasSymmetricCipherPair
    {
        SymmetricCipherPair CipherPair { get; }
    }
    
}