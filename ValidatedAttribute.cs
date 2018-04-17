using System;

namespace WpfApp1
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false)]
    class ValidatedAttribute : Attribute
    {
        public ValidatedAttribute(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Message used to display when the validator returns false or null.
        /// </summary>
        public string ErrorMessage { get; }
    }
}