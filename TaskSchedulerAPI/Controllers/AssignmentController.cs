using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using TaskSchedulerAPI.Data;
using TaskSchedulerAPI.Models;

namespace TaskSchedulerAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase {
        #region *** CRUD ***
        [HttpGet]
        [Route("GetAllAssignments")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetAllAssignments() {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            int userId = Int32.Parse(claimsIdentity.FindFirst("id")?.Value);
            return new ObjectResult(DBFunction.GetAssignments(userId));
        }
        [HttpPost]
        [Route("CreateAssignment")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult CreateAssignment(Assignment assignment) {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            int userId = Int32.Parse(claimsIdentity.FindFirst("id")?.Value);
            if (assignment.UserId == 0) assignment.UserId = userId;
            int assignmentId = DBFunction.AddAssignment(assignment);
            if(assignmentId < 1) return BadRequest(new { error = "Не вдалося створити завдання, повторіть спробу" });
            return Ok(assignmentId);
        }
        [HttpPut]
        [Route("UpdateAssignment")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult UpdateAssignment(Assignment assignment) {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            int userId = Int32.Parse(claimsIdentity.FindFirst("id")?.Value);
            if (userId != assignment.UserId) return BadRequest(new { error = "Помилка безпеки, запит відхилено" });
            int numberExecuteRequest = DBFunction.UpdateAssignment(assignment);
            if(numberExecuteRequest != 1) return BadRequest(new { error = "Не вдалося оновити дані завдання, повторіть спробу" });
            return Ok();
        }
        [HttpDelete]
        [Route("DeleteAssignment/{assignmentId}")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult DeleteAssignment(int assignmentId) {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            int userId = Int32.Parse(claimsIdentity.FindFirst("id")?.Value);
            int numberExecuteRequest = DBFunction.DeleteAssignment(assignmentId, userId);
            if (numberExecuteRequest != 1) return BadRequest(new { error = "Не вдалося видалити завдання, повторіть спробу" });
            return Ok();
        }
        #endregion
    }
}