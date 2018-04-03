using System.ComponentModel.DataAnnotations;
using System;
using System.Reflection;

namespace MosListAPI.Attributes{
    public class NonDefaultAttribute : ValidationAttribute{
        private Type _type;
        public NonDefaultAttribute(Type type){
            _type = type;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext){
            if (value == null || (_type.GetTypeInfo().IsValueType && Activator.CreateInstance(_type).Equals(value)))
                return new ValidationResult("Value either missing or invalid");
            return ValidationResult.Success;
        }
    }
}