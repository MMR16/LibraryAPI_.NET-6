using LibraryAPI.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models
{
    [CourseTitleMustbeDiffrentFromDescriptionAttreibute]
    public abstract class CourseForManipulationDto
    {
        [Required(ErrorMessage = "you should fill out a title")]
        [MaxLength(100, ErrorMessage = "the title shouldn't have more than 100 characters")]
        public string Title { get; set; }


        [MaxLength(1500, ErrorMessage = "the Description shouldn't have more than 1500 characters")]
       public virtual string Description { get; set; }

    }
}
