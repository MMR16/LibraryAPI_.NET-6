using LibraryAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.ValidationAttributes
{
    public class CourseTitleMustbeDiffrentFromDescriptionAttreibute : ValidationAttribute
    {
        /// <summary>
        /// Custom Validation , custom attribute 
        /// excute before property vaildation so it is better
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            var course = validationContext.ObjectInstance as CourseForManipulationDto;
            if (course.Title == course.Description)
            {
                return new ValidationResult(
                    //ErrorMessage = "The provided description should be diffrent from title.", //static message
                    string.IsNullOrWhiteSpace(ErrorMessage) ?
                    ErrorMessage = "The provided description should be diffrent from title." : ErrorMessage, //generic message
                    new[] { nameof(CourseForManipulationDto) });
            }
            return ValidationResult.Success;
        }

    }
}
