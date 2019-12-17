using System;
using System.Collections.Generic;
using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public CoursesController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }
     
        [HttpGet]
        public ActionResult<IEnumerable<CourseDto>> GetCoursesForAuthor(Guid authorId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var coursesFromRepo = _courseLibraryRepository.GetCourses(authorId);
            return Ok(_mapper.Map<IEnumerable<CourseDto>>(coursesFromRepo));
        }

        [HttpGet("{courseId}", Name = "GetCourse")]
        public ActionResult<CourseDto> GetCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseFromRepo = _courseLibraryRepository.GetCourse(authorId, courseId);
            if (courseFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CourseDto>(courseFromRepo));
        }

        [HttpPost]
        public ActionResult<CourseDto> CreateCourseForAuthor(Guid authorId, CourseForCreationDto course)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseEntity = _mapper.Map<Entities.Course>(course);
            _courseLibraryRepository.AddCourse(authorId, courseEntity);
            _courseLibraryRepository.Save();

            var courseToReturn = _mapper.Map<CourseDto>(courseEntity);

            return CreatedAtRoute("GetCourse",
                new { authorId = courseToReturn.AuthorId, courseId = courseToReturn.Id },
                courseToReturn);
        }

        [HttpPut("{courseId}")]
        public IActionResult UpdateCourseForAuthor(
            Guid authorId,
            Guid courseId,
            CourseForUpdateDto course)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromRepo = _courseLibraryRepository.GetCourse(authorId, courseId);


            if (courseForAuthorFromRepo == null)
            {
                var courseToAdd = _mapper.Map<Entities.Course>(course);
                courseToAdd.Id = courseId;
                
                _courseLibraryRepository.AddCourse(authorId, courseToAdd);

                _courseLibraryRepository.Save();
                
                var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);
                
                return CreatedAtRoute("GetCourse",
                    new { authorId, courseId = courseToReturn.Id },
                    courseToReturn);
      }

            // map the entity to a CourseForUpdateDto
            // apply the updated field values to that dto
            // map the CourseForUpdateDto back to entity
            _mapper.Map(course, courseForAuthorFromRepo);

            _courseLibraryRepository.UpdateCourse(courseForAuthorFromRepo);

            _courseLibraryRepository.Save();

            return Ok(course);
        }
    }
}