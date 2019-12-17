using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CourseLibrary.API.ValidationAttributes;

namespace CourseLibrary.API.Models
{
    [CourseTitleMustBeDifferentFromDescription(
        ErrorMessage = "Title must be different from description."
    )]
    public class CourseForCreationDto : CourseForManipulationDto
    {
  }
}