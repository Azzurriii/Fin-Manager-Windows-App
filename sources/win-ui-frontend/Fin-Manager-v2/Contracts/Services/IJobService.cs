using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.Contracts.Services;

public interface IJobService
{
    Task<List<JobModel>> GetJobsAsync();
    Task<bool> CreateJobAsync(CreateJobDto jobDto);
    Task<bool> UpdateJobAsync(UpdateJobDto job);
    Task<bool> DeleteJobAsync(int jobId);
    Task<List<JobModel>> GetJobsByUserIdAsync(int jobId);
}