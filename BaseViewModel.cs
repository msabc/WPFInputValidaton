using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WpfApp1
{
    abstract class BaseViewModel : IDataErrorInfo, INotifyPropertyChanged
    {
        /// <summary>
        /// Contains validation data for a property.
        /// <para>Example: 
        /// Key is property named Number,
        /// Value is a combination of validator (that checks if it is a positive number)
        /// and a string that represents an error message that is displayed if the validation fails.</para>
        /// </summary>
        private IDictionary<string, (Func<bool>, string)> errorData = new Dictionary<string,(Func<bool>,string)>();

        

        public BaseViewModel(params Func<bool>[] validators)
        {
            int count = 0;
            foreach (var item in GetType().GetProperties())
            {
                var attribute = item.GetCustomAttribute<ValidatedAttribute>();
                if (attribute != null)
                {
                    AddValidator(item.Name, validators[count++]);
                }
            }

            CheckValidators();
        }

        public BaseViewModel()
        {
            InitializeValidators();

            CheckValidators();
        }


        public string this[string columnName]
        {
            get
            {
                //ako ima taj kljuc onda postoji taj property u klasi koja implementira ovu klasu
                if (errorData.ContainsKey(columnName))
                {
                    
                    (Func<bool> validator, string errorMessage) = errorData[columnName];

                    if (validator?.Invoke() == true)
                    {
                        Error = "";
                        return null;
                    }
                    else
                    {
                        Error = errorMessage;
                        return errorMessage;
                    }
                }

                return null;
            }
        }

        private string error;

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Error { get => error; private set => SetValue(ref error, value); }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CheckValidators()
        {
            int numDecoratedProperties = GetType().GetProperties().SelectMany(prop => prop.GetCustomAttributes<ValidatedAttribute>()).Count();

            if (errorData.Count != numDecoratedProperties)
            {
                throw new ArgumentOutOfRangeException("Validators","Number of validators must match the " +
                    "number of properties decorated with " + nameof(ValidatedAttribute));
            }
        }


        /// <summary>
        /// Sets the value of a property and raises <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="property">Property that has its value changed.</param>
        /// <param name="value">New value for the defined property.</param>
        protected void SetValue<T>(ref T property, object newValue, [CallerMemberName] string propertyName = null)
        {

            if (newValue is T value && !Equals(property, value))
            {
                property = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            
        }

        /// <summary>
        /// Add a validator for a property
        /// </summary>
        /// <param name="propertyName">Name of the property (use nameof())</param>
        /// <param name="validator">Checks if the property is valid</param>
        /// <param name="errorMessage"></param>
        protected void AddValidator(string propertyName, Func<bool> validator)
        {
            if (propertyName == null ||validator == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var item in GetType().GetProperties())
            {
                if (item.Name == propertyName)
                {
                    var attribute = item.GetCustomAttribute<ValidatedAttribute>();
                    if (attribute!= null)
                    {
                        errorData.Add(propertyName, (validator, attribute.ErrorMessage));
                        return;
                    }
                }
            }

            throw new Exception($"Property with name {propertyName} was not found.");
            
        }

        protected virtual void InitializeValidators()
        {

        }
    }
}