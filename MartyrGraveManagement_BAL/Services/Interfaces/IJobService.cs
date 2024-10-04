using MartyrGraveManagement_BAL.ModelViews.JobDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IJobService
    {
        Task<IEnumerable<JobDtoResponse>> GetAllJobsAsync();
        Task<JobDtoResponse> GetJobByIdAsync(int jobId);
        Task<JobDtoResponse> CreateJobAsync(JobDtoRequest newJob);
        Task<bool> UpdateJobAsync(int jobId, JobDtoRequest updatedJob);
        Task<bool> DeleteJobAsync(int jobId);
    }
}
