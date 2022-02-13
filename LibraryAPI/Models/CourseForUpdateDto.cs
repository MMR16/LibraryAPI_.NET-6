using LibraryAPI.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models
{
    [CourseTitleMustbeDiffrentFromDescriptionAttreibute]
    public class CourseForUpdateDto : CourseForManipulationDto
    {
        [Required(ErrorMessage = "you should fill out a Description")]
        public override string Description { get =>base.Description; set =>base.Description =value; }

    }
}