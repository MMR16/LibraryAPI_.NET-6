using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;

namespace LibraryAPI.Helper
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //check if parameter is type of IEnumerable
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();

            //if the value is null or whitespaces return null
            if (string.IsNullOrWhiteSpace(value))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            //if value isn't null or white spaces & Enumerable
            //get Enumerable type & converter
            var elementType = bindingContext.ModelType.GenericTypeArguments[0];
            var converter = TypeDescriptor.GetConverter(elementType);

            //convert each item in value list to enumerable type
            var values = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).
                Select(e => converter.ConvertFromString(e.Trim())).ToArray();


            //create array of type & set it as model value
            var typedValues = Array.CreateInstance(elementType, values.Length);
            values.CopyTo(typedValues, 0);
            bindingContext.Model = typedValues;

            //return successful result & passing it to model
            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;

        }
    }
}
