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
    /// Controller responsible for managing minutes related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting minutes information.
    /// </remarks>
    [Route("api/minutes")]
    [Authorize]
    public class MinutesController : ControllerBase
    {
        private readonly IMinutesService _minutesService;

        /// <summary>
        /// Initializes a new instance of the MinutesController class with the specified context.
        /// </summary>
        /// <param name="iminutesservice">The iminutesservice to be used by the controller.</param>
        public MinutesController(IMinutesService iminutesservice)
        {
            _minutesService = iminutesservice;
        }

        /// <summary>Adds a new minutes</summary>
        /// <param name="model">The minutes data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("Minutes",Entitlements.Create)]
        public IActionResult Post([FromBody] Minutes model)
        {
            var id = _minutesService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of minutess based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of minutess</returns>
        [HttpGet]
        [UserAuthorize("Minutes",Entitlements.Read)]
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

            var result = _minutesService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific minutes by its primary key</summary>
        /// <param name="id">The primary key of the minutes</param>
        /// <returns>The minutes data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("Minutes",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _minutesService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific minutes by its primary key</summary>
        /// <param name="id">The primary key of the minutes</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("Minutes",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _minutesService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific minutes by its primary key</summary>
        /// <param name="id">The primary key of the minutes</param>
        /// <param name="updatedEntity">The minutes data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("Minutes",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] Minutes updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _minutesService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific minutes by its primary key</summary>
        /// <param name="id">The primary key of the minutes</param>
        /// <param name="updatedEntity">The minutes data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("Minutes",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<Minutes> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _minutesService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}