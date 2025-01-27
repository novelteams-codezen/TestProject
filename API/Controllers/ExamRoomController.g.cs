using Microsoft.AspNetCore.Mvc;
using TestProject.Models;
using TestProject.Services;
using TestProject.Entities;
using TestProject.Filter;
using TestProject.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;

namespace TestProject.Controllers
{
    /// <summary>
    /// Controller responsible for managing examroom related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting examroom information.
    /// </remarks>
    [Route("api/examroom")]
    [Authorize]
    public class ExamRoomController : ControllerBase
    {
        private readonly IExamRoomService _examRoomService;

        /// <summary>
        /// Initializes a new instance of the ExamRoomController class with the specified context.
        /// </summary>
        /// <param name="iexamroomservice">The iexamroomservice to be used by the controller.</param>
        public ExamRoomController(IExamRoomService iexamroomservice)
        {
            _examRoomService = iexamroomservice;
        }

        /// <summary>Adds a new examroom</summary>
        /// <param name="model">The examroom data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("ExamRoom",Entitlements.Create)]
        public IActionResult Post([FromBody] ExamRoom model)
        {
            var id = _examRoomService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of examrooms based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examrooms</returns>
        [HttpGet]
        [UserAuthorize("ExamRoom",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult Get([FromQuery] string filters, string searchTerm, int pageNumber = 1, int pageSize = 10, string sortField = null, string sortOrder = "asc")
        {
            List<FilterCriteria> filterCriteria = null;
            if (pageSize < 1)
            {
                return BadRequest("Page size invalid.");
            }

            if (pageNumber < 1)
            {
                return BadRequest("Page mumber invalid.");
            }

            if (!string.IsNullOrEmpty(filters))
            {
                filterCriteria = JsonHelper.Deserialize<List<FilterCriteria>>(filters);
            }

            var result = _examRoomService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific examroom by its primary key</summary>
        /// <param name="id">The primary key of the examroom</param>
        /// <returns>The examroom data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("ExamRoom",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _examRoomService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific examroom by its primary key</summary>
        /// <param name="id">The primary key of the examroom</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("ExamRoom",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _examRoomService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific examroom by its primary key</summary>
        /// <param name="id">The primary key of the examroom</param>
        /// <param name="updatedEntity">The examroom data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("ExamRoom",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] ExamRoom updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _examRoomService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific examroom by its primary key</summary>
        /// <param name="id">The primary key of the examroom</param>
        /// <param name="updatedEntity">The examroom data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("ExamRoom",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<ExamRoom> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _examRoomService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}