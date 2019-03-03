using System;
using System.Runtime.Serialization;

namespace Inventory.Domain
{
    [Serializable]
    public class InvalidAmountException : Exception
    {
        public InvalidAmountException() { }
    }
}