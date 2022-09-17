using TaskSchedulerAPI.Data;
using TaskSchedulerAPI.Models;

namespace TaskSchedulerAPI.Services {
    public class UserService : IUserService {
        public object GetUserDetails() {
            return null; 
        }

        public bool IsValidUserInformation(LoginModel model) {
            return DBFunction.AuthorizationUser(model);
        }
    }
}
