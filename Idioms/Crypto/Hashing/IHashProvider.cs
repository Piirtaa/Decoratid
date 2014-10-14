using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Storing;

namespace Decoratid.Crypto
{
    /// <summary>
    /// interface defining a provider of one-way hashing functions.  Has an id to facilitate brokering via a store.
    /// </summary>
    public interface IHashProvider
    {
        string CreateHash(string password);
        bool ValidatePassword(string password, string correctHash);
    }
}
