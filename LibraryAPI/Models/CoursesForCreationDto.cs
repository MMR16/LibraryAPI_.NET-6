using LibraryAPI.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models
{
    //[CourseTitleMustbeDiffrentFromDescriptionAttreibute(ErrorMessage ="write difrrent title because it is the same as description")]
    [CourseTitleMustbeDiffrentFromDescriptionAttreibute]
    public class CoursesForCreationDto : CourseForManipulationDto// : IValidatableObject
    {

        //using abstract calss instead
        #region Old Class

        //[Required(ErrorMessage ="you should fill out a title")]
        //[MaxLength(100,ErrorMessage ="the title shouldn't have more than 100 characters")]
        //public string Title { get; set; }
        //[MaxLength(1500, ErrorMessage = "the Description shouldn't have more than 1500 characters")]
        //public string Description { get; set; }

        //// property vaildation by implement IValidatableObject
        //// it is best to use custom validation over class than the attribute

        #endregion

        #region Property balidation
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Title == Description)
        //    {
        //        yield return new ValidationResult("The provided description should be diffrent from title.",
        //                                           new[] { "CoursesForCreationDto" });
        //    }
        //} 
        #endregion
    }
}
