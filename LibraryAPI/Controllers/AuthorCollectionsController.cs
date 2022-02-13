using AutoMapper;
using LibraryAPI.Entites;
using LibraryAPI.Helper;
using LibraryAPI.Models;
using LibraryAPI.Servicces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [ApiController, Route("api/authorcollections")]
    public class AuthorCollectionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAuthorRepository _authorRepository;

        public AuthorCollectionsController(
            IMapper Mapper,
            IAuthorRepository authorRepository)
        {
            _mapper = Mapper ?? throw new ArgumentNullException(nameof(Mapper));
            _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
        }


        // array of composite key like key value pair
        //key1=valyue1, key2=value2
        [HttpGet("{ids}",Name = "GetAuthorCollection")]
        public IActionResult GetAuthorCollection(
        [FromRoute][ModelBinder(binderType: typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids is null)
            {
                return BadRequest();
            }
            var authorEntities = _authorRepository.GetAuthors(ids);
            if (ids.Count() != authorEntities.Count())
            {
                return NotFound();
            }

            var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            return Ok(authorsToReturn);
        }


        [HttpPost]
        public ActionResult<IEnumerable<AuthorDto>> CreateAuthorCollection(IEnumerable<AuthorForCreationDto> authorCollection)
        {

            var authorEntities = _mapper.Map<IEnumerable<Author>>(authorCollection);

            foreach (var author in authorEntities)
            {
                _authorRepository.AddAuthor(author);
            }

            _authorRepository.save();
            var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            var idsString=string.Join(",",authorsToReturn.Select(author => author.Id));
            return CreatedAtRoute("GetAuthorCollection",new {ids= idsString},authorsToReturn);
        }


        //to know what methods are allowed in the resource
        [HttpOptions]
        public IActionResult GetAuthorsOption()
        {
            Response.Headers.Add("Allow", ("GET,POST,OPTIONS"));
            return Ok();
        }
    }
}
