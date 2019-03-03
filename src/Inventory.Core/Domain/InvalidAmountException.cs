using System;

namespace Inventory.Domain
{
    [Serializable]
    public class InvalidAmountException : Exception
    {
        public InvalidAmountException() { }
    }
}