using AutoMapper;
using LibraryAPI.Entites;
using LibraryAPI.Helper;
using LibraryAPI.Models;
using LibraryAPI.ResourceParameters;
using LibraryAPI.Servicces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("api/authors")]
    //[Route("api/[controller]")]
    //[Route("api/[controller]/[action]")]
    //[Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, IMapper mapper)
        {
            _authorRepository = authorRepository ?? throw new ArgumentException(nameof(authorRepository));
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
        }
        //[HttpGet("api/authors")]
        //httpHead same as get request but without body
        //[HttpGet]
        //[HttpHead]
        ////public IActionResult GetAuthors()
        //public ActionResult<IEnumerable<AuthorDto>> GetAuthors()
        //{
        //    ////throw new Exception();
        //    var authorsFromRepo = _authorRepository.GetAuthors();
        //    #region befor automapper
        //    //var authors = new List<AuthorDto>();
        //    //foreach (var author in authorsFromRepo)
        //    //{
        //    //    authors.Add(new AuthorDto
        //    //    {
        //    //        Id = author.Id,
        //    //        Name = $"{author.FirstName } {author.LastName }",
        //    //        MainCategory = author.MainCategory,
        //    //        Age = author.DateOfBirth.GetCurrentAge()


        //    //    }); ;
        //    //} 
        //    #endregion
        //    var authors=_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);
        //    return Ok(authors);
        //}

        //filter & search
        //[HttpGet("filterByCategory")]

        //[HttpGet]
        //[HttpHead]
        //public ActionResult<IEnumerable<AuthorDto>> GetAuthors(
        //    [FromQuery(Name = "mainCategory")] string? mainCategory,
        //    [FromQuery(Name = "Search")] string? searchQuery)
        //{
        //    var authorFromRepo = _authorRepository.GetAuthors(mainCategory, searchQuery);
        //    //var data = _mapper.Map<IEnumerable<Author>,IEnumerable<AuthorDto>>(authorFromRepo);
        //    var data = _mapper.Map<IEnumerable<AuthorDto>>(authorFromRepo);
        //    return Ok(data);
        //}

        // :Guid is aconstraint

        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery] AuthorResourceParameter Parameter)
        {
            var authorFromRepo = _authorRepository.GetAuthors(Parameter);
            var data = _mapper.Map<IEnumerable<AuthorDto>>(authorFromRepo);
            return Ok(data);
        }

        [HttpGet("{authorId:Guid}", Name = "GetAuthor")]
        //[HttpGet]
        //public IActionResult GetAuthor(Guid authorId)
        public ActionResult<Author> GetAuthor(Guid authorId)
        {
            //var data = _authorRepository.GetAuthor(authorId);
            var data = _mapper.Map<AuthorDto>(_authorRepository.GetAuthor(authorId));
            #region notice
            ////if we don't want to use entity after check we use method  AuthorExists()
            //if (!_authorRepository.AuthorExists(authorId))
            //{
            //    return NotFound();
            //}

            ////if we want to use entity so we use null check
            //if (data is null)
            //{
            //    return NotFound();
            //}
            //return Ok(data);
            //using ternary operator 
            #endregion

            return data is null ? NotFound() : Ok(data);
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(AuthorForCreationDto author)
        {
            var authorEntity = _mapper.Map<Author>(author);
            _authorRepository.AddAuthor(authorEntity);
            _authorRepository.save();
            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
            //return Created("", authorToReturn);
            return CreatedAtRoute("GetAuthor", new { authorId = authorToReturn.Id }, authorToReturn);
        }

        [HttpDelete("{authorId:Guid}")]
        public ActionResult DeleteAuthorWithCources(Guid authorId)
        {
            var author = _authorRepository.GetAuthor(authorId);
            if (author is null)
            {
                return NotFound();
            }
            _authorRepository.DeleteAuthor(author);
            _authorRepository.save();
            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetAuthorsOption()
        {
            Response.Headers.Add("Allow", ("GET,POST,OPTIONS,DELETE"));
            return Ok();
        }
    }
}
