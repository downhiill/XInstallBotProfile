using XInstallBotProfile.Models;
using XInstallBotProfile.Service.AdminPanelService.Models.Request;
using XInstallBotProfile.Service.AdminPanelService.Models.Response;

namespace XInstallBotProfile.Service.AdminPanelService
{
    public interface IUserService
    {
        public Task<GetAllUsersResponse> GetAllUsers();
        public Task<GetUserByIdResponse> GetUserById(GetUserByIdRequest request);
        public Task<CreateUserResponse> CreateUser(CreateUserRequest request);
        public Task<bool> CreateUserRecord(int UserId, CreateUserRecordRequest request);
        public Task<UpdateUsernameResponse> UpdateUsername(int id, UpdateUsernameRequest request);
        public Task<UpdateFlagsResponse> UpdateUserFlags(int id, UpdateFlagsRequest request);
        public Task SaveUserChanges(SaveUserRequest request);
        public void SaveUserAsync(CreateUserRequest request);
        public Task DeleteUser(List<int> userIds);
        public Task<bool> DeleteUserRecord(long id);
        public Task<GetStatisticResponse> GetStatistic(GetStatisticRequest request);

        public Task<bool> UpdateStatistic(UpdateStatisticRequest request);

        public Task<GenerateUserResponse> GenerateUser();
    }
}
