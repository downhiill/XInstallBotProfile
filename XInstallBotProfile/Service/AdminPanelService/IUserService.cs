using XInstallBotProfile.Models;
using XInstallBotProfile.Service.AdminPanelService.Models.Request;
using XInstallBotProfile.Service.AdminPanelService.Models.Response;

namespace XInstallBotProfile.Service.AdminPanelService
{
    public interface IUserService
    {
        public Task<GetAllUsersResponse> GetAllUsers();
        public Task<CreateUserResponse> CreateUser(CreateUserRequest request);
        public Task<UpdateUsernameResponse> UpdateUsername(int id, UpdateUsernameRequest request);
        public Task<UpdateFlagsResponse> UpdateUserFlags(UpdateFlagsRequest request);
        public Task SaveUserChanges(SaveUserRequest request);
        public Task<int> SaveUserAsync(User user);
        public Task DeleteUser(List<int> userIds);
    }
}
