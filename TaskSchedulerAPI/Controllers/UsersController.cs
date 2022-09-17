using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using TaskSchedulerAPI.Data;
using TaskSchedulerAPI.Models;

namespace TaskSchedulerAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        #region *** CRUD ***
        [HttpGet]
        [Route("GetUser")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetUser([FromHeader]string userPublicKey) {
            if (userPublicKey == null || userPublicKey == "") return BadRequest(new { error = "Відсутній ключ клієнта" });
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            int userId = Int32.Parse(claimsIdentity.FindFirst("id")?.Value);
            User user = DBFunction.GetUserByUserId(userId);
            if(user == null) return BadRequest(new { error = "Такого користувача не існує" });
            user.AES_Decrypt_IUser();
            user.RSA_Encrypt_IUser(userPublicKey);
            return new ObjectResult(user);
        }
        [HttpPost]
        [Route("CreateUser")]
        public IActionResult CreateUser(RegisterModel model) {
            model.RSA_Decrypt_IUser();
            if (DBFunction.Availability_Email_Name(model.UserName, model.UserEmail, 0)) return BadRequest(new { error = "Користувач з таким логіном і/або поштою вже існує" });
            model.AES_Encrypt_IUser();
            int numberExecuteRequest = DBFunction.CreateUser(model);
            if (numberExecuteRequest != 1) return BadRequest(new { error = "Не вдалося створити користувача, повторіть спробу" });
            return Ok();
        }
        [HttpPut]
        [Route("UpdateUser")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult UpdateUser(User user) {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            int userId = Int32.Parse(claimsIdentity.FindFirst("id")?.Value);
            if (userId != user.UserId) return BadRequest(new { error = "Помилка безпеки, запит відхилено" });
            if (DBFunction.Availability_Email_Name(user.UserName, user.UserEmail, user.UserId)) return BadRequest(new { error = "Користувач з таким логіном і/або поштою вже існує" });
            user.RSA_Decrypt_IUser();
            user.AES_Encrypt_IUser();
            int numberExecuteRequest = DBFunction.UpdateUser(user);
            if (numberExecuteRequest != 1) return BadRequest(new { error = "Не вдалося оновити дані про користувача, повторіть спробу" });
            return Ok();
        }
        [HttpDelete]
        [Route("DeleteUser")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult DeleteUser() {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            int userId = Int32.Parse(claimsIdentity.FindFirst("id")?.Value);
            DBFunction.DeleteAllAssignment(userId);
            int numberExecuteRequest = DBFunction.DeleteUser(userId);
            if (numberExecuteRequest != 1) return BadRequest(new { error = "Не вдалося видалити користувача, повторіть спробу" });
            return Ok();
        }
        #endregion

        [HttpGet]
        [Route("GetPublicKey")]
        public IActionResult GetPublicKey() {
            return Ok(СryptographyData._publicKey);
        }
    }
}
