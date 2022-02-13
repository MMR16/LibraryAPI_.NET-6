using AutoMapper;
using LibraryAPI.Entites;
using LibraryAPI.Models;
using LibraryAPI.Servicces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private ICourseRepository _courseRepository;
        private readonly IAuthorRepository _authorRepository;
        public CoursesController(IMapper mapper, ICourseRepository courseRepository, IAuthorRepository authorRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
        }



        [HttpGet]
        //[HttpGet("{authorId}")]
        public ActionResult<IEnumerable<CoursesDto>> GetCoursesForAuthor(Guid authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var coursesForAuthorFromRepo = _courseRepository.GetCourses(authorId);
            return Ok(_mapper.Map<IEnumerable<CoursesDto>>(coursesForAuthorFromRepo));

        }

        [HttpGet("{courseId}", Name = "GetAuthorCourse")]
        public ActionResult<CoursesDto> GetAuthorCourse(Guid authorId, Guid courseId)
        {
            if (!_authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var courseForAuthorFromRepo = _courseRepository.GetCourse(authorId, courseId);
            if (courseForAuthorFromRepo is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CoursesDto>(courseForAuthorFromRepo));
        }

        [HttpPost]
        public ActionResult<CoursesDto> CreateCoursesForAuthor(Guid authorId, CoursesForCreationDto course)
        {
            if (!_authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var courseEntity = _mapper.Map<Course>(course);
            _courseRepository.AddCourse(authorId, courseEntity);
            _courseRepository.save();
            var coursesToReturn = _mapper.Map<CoursesDto>(courseEntity);
            return CreatedAtRoute("GetAuthorCourse", new { authorId = authorId, courseId = coursesToReturn.Id }, coursesToReturn);
        }

        [HttpPut("{courseId:guid}")]
        public IActionResult UpdateCourseForAuthor(Guid authorId, Guid courseId, CourseForUpdateDto course)
        {
            if (!_authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var courseForAuthorFromRepo = _courseRepository.GetCourse(authorId, courseId);

            // upserting
            // using put to update exisit resource & if it doesn't exisit it will create it 
            if (courseForAuthorFromRepo is null)
            {
                var courseToAdd = _mapper.Map<Course>(course);
                courseToAdd.Id = courseId;
                _courseRepository.AddCourse(authorId, courseToAdd);
                _courseRepository.save();
                var courseToReturn = _mapper.Map<CoursesDto>(courseToAdd);
                return CreatedAtRoute("GetAuthorCourse", new { authorId = authorId, courseId = courseToAdd }, courseToReturn);

                //return NotFound();
            }

            // map the entity to CourseForUpdateDto
            // apply the updated fireld values to that dto
            // map the CourseForUpdateDto back to an entity 
            _mapper.Map(course, courseForAuthorFromRepo);

            _courseRepository.UpdateCourse(courseForAuthorFromRepo);
            _courseRepository.save();
            return NoContent();
        }

        [HttpPatch("{courseId:guid}")]
        public ActionResult PartiallyUpdateCourseForAuthor(Guid authorId, Guid courseId, JsonPatchDocument<CourseForUpdateDto> PatchDocument)
        {
            if (!_authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var coureseForAuthorFromRepo = _courseRepository.GetCourse(authorId, courseId);

            //using upserting to add using put if resourse not found
            if (coureseForAuthorFromRepo is null)
            {
                var courseDto = new CourseForUpdateDto();
                PatchDocument.ApplyTo(courseDto,ModelState);
                if (!TryValidateModel(courseDto))
                {
                    return ValidationProblem(ModelState);
                }
                var courseToAdd = _mapper.Map<Course>(courseDto);
                courseToAdd.Id = courseId;
                _courseRepository.AddCourse(authorId, courseToAdd);
                _courseRepository.save();

                var courseToReturn = _mapper.Map<CoursesDto>(courseToAdd);
                return CreatedAtRoute("GetAuthorCourse",new {authorId, courseId=courseToReturn.Id },courseToReturn);
            }
            var courseToPatch = _mapper.Map<CourseForUpdateDto>(coureseForAuthorFromRepo);
            //for validation we use model state
            PatchDocument.ApplyTo(courseToPatch,ModelState);
            if (!TryValidateModel(courseToPatch))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(courseToPatch, coureseForAuthorFromRepo);
            _courseRepository.UpdateCourse(coureseForAuthorFromRepo);
            _courseRepository.save();
            return NoContent();

        }

        [HttpDelete("{courseId:guid}")]
        public ActionResult DeleteCourseForAuthor(Guid authorId,Guid courseId)
        {
            if (!_authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var courseForAuthorFromRepo= _courseRepository.GetCourse(authorId,courseId);
            if (courseForAuthorFromRepo is null)
            {
                return NotFound();
            }
            _courseRepository.DeleteCourse(courseForAuthorFromRepo);
            _courseRepository.save();
            return NoContent();

        }

        //[HttpOptions,Route("api/authors/courses")]

        //public IActionResult GetAuthorsOption()
        //{
        //    Response.Headers.Add("Allow", ("GET,POST,OPTIONS,DELETE,PATCH"));
        //    return Ok();
        //}

        // overridel model validation return for Patch Method
        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return options.Value.InvalidModelStateResponseFactory(ControllerContext) as ActionResult;
        }

    }
}
