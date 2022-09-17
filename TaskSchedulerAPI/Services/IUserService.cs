using TaskSchedulerAPI.Models;

namespace TaskSchedulerAPI.Services {
    public interface IUserService {
        bool IsValidUserInformation(LoginModel model);
        object GetUserDetails();
    }
}
