using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.JobDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class JobService : IJobService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public JobService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<JobDtoResponse>> GetAllJobsAsync()
        {
            var jobs = await _unitOfWork.JobRepository.GetAllAsync();

            // Khởi tạo danh sách JobDtoResponse với FullName từ Account
            var jobDtoResponses = new List<JobDtoResponse>();

            foreach (var job in jobs)
            {
                // Lấy thông tin Account để lấy FullName
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(job.AccountId);

                var jobResponse = _mapper.Map<JobDtoResponse>(job);
                jobResponse.FullName = account?.FullName;  // Lấy tên người liên quan

                jobDtoResponses.Add(jobResponse);
            }

            return jobDtoResponses;
        }


        public async Task<JobDtoResponse> GetJobByIdAsync(int jobId)
        {
            var job = await _unitOfWork.JobRepository.GetByIDAsync(jobId);
            if (job == null)
            {
                return null;
            }

            // Lấy thông tin Account để lấy FullName
            var account = await _unitOfWork.AccountRepository.GetByIDAsync(job.AccountId);

            var jobResponse = _mapper.Map<JobDtoResponse>(job);
            jobResponse.FullName = account?.FullName;  // Lấy tên người liên quan

            return jobResponse;
        }


        public async Task<JobDtoResponse> CreateJobAsync(JobDtoRequest newJob)
        {
            // Kiểm tra AccountId có tồn tại hay không
            var account = await _unitOfWork.AccountRepository.GetByIDAsync(newJob.AccountId);
            if (account == null)
            {
                throw new KeyNotFoundException("AccountId does not exist.");
            }

            var jobEntity = _mapper.Map<StaffJob>(newJob);
            await _unitOfWork.JobRepository.AddAsync(jobEntity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<JobDtoResponse>(jobEntity);
        }

        public async Task<bool> UpdateJobAsync(int jobId, JobDtoRequest updatedJob)
        {
            // Kiểm tra JobId có tồn tại không
            var jobEntity = await _unitOfWork.JobRepository.GetByIDAsync(jobId);
            if (jobEntity == null)
            {
                return false;
            }

            // Kiểm tra AccountId có tồn tại không
            var account = await _unitOfWork.AccountRepository.GetByIDAsync(updatedJob.AccountId);
            if (account == null)
            {
                throw new KeyNotFoundException("AccountId does not exist.");
            }

            _mapper.Map(updatedJob, jobEntity);
            await _unitOfWork.JobRepository.UpdateAsync(jobEntity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteJobAsync(int jobId)
        {
            var jobEntity = await _unitOfWork.JobRepository.GetByIDAsync(jobId);
            if (jobEntity == null)
            {
                return false;
            }

            await _unitOfWork.JobRepository.DeleteAsync(jobEntity);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
