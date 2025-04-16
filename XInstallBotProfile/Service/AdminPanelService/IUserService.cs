using Microsoft.AspNetCore.Mvc;
using XInstallBotProfile.Models;
using XInstallBotProfile.Service.AdminPanelService.Models.Request;
using XInstallBotProfile.Service.AdminPanelService.Models.Response;

namespace XInstallBotProfile.Service.AdminPanelService
{
    public interface IUserService
    {
        public Task<GetAllUsersResponse> GetAllUsers();
        public Task<GetUserByIdResponse> GetUserById(int id);
        public Task<CreateUserResponse> CreateUser(CreateUserRequest request);
        public Task<bool> CreateUserRecord(int UserId, CreateUserRecordRequest request);
        public Task<bool> CreateUserRecordXInstallApp(int UserId, CreateUserRecordXInstallAppRequest request);
        public Task<UpdateUsernameResponse> UpdateUsername(int id, UpdateUsernameRequest request);
        public Task<UpdateFlagsResponse> UpdateUserFlags(int id, UpdateFlagsRequest request);
        public Task SaveUserChanges(SaveUserRequest request);
        public void SaveUserAsync(CreateUserRequest request);
        public Task DeleteUser(List<int> userIds);
        public Task<bool> DeleteUserRecords(List<long> ids);
        public Task<GetStatisticResponse> GetStatistic(GetStatisticRequest request);
        public Task<GetStatisticXInstallAppResponse> GetStatisticXInstallApp(GetStatisticXInstallAppRequest request);

        public Task<bool> UpdateStatistic(UpdateStatisticRequest request);
        public Task<bool> UpdateStatisticXInstallApp(UpdateStatisticXInstallAppRequest request);

        public Task<GenerateUserResponse> GenerateUser();

        public Task<FileContentResult> ExportStatisticInExcel(GetStatisticRequest request);

        public Task<FileContentResult> ExportStatisticInPdf(GetStatisticRequest request);
    }
}
