using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.ServiceCategoryDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
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
    public class GraveService_Service : IGraveService_Service
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GraveService_Service(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(bool status, string result)> AddService(ServiceDtoRequest service)
        {
            try
            {
                var existedCategory = await _unitOfWork.ServiceCategoryRepository.GetByIDAsync(service.CategoryId);
                if (existedCategory != null)
                {
                    var newService = _mapper.Map<Service>(service);
                    await _unitOfWork.ServiceRepository.AddAsync(newService);
                    await _unitOfWork.SaveAsync();
                    return (true, "Add successfully");
                }
                else
                {
                    return (false, "Category not found");
                }
                

            }
            catch
            {
                return (false, "Add unsuccessfully");
            }
        }

        public async Task<(bool status, string result)> ChangeStatus(int serviceId)
        {
            try
            {
                var service = await _unitOfWork.ServiceRepository.GetByIDAsync(serviceId);
                if (service != null)
                {
                    if (service.Status == true)
                    {
                        service.Status = false;
                        await _unitOfWork.ServiceRepository.UpdateAsync(service);
                        await _unitOfWork.SaveAsync();
                        return (true, "Cập nhật thành công");
                    }
                    else
                    {
                        service.Status = true;
                        await _unitOfWork.ServiceRepository.UpdateAsync(service);
                        await _unitOfWork.SaveAsync();
                        return (true, "Cập nhật thành công");
                    }
                }
                else
                {
                    return (false, "Không tìm thấy dịch vụ");
                }
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ServiceDtoResponse>> GetAllServices()
        {
            try
            {
                List<ServiceDtoResponse> serviceList = new List<ServiceDtoResponse>();
                var services = await _unitOfWork.ServiceRepository.GetAllAsync();
                if (services != null)
                {
                    foreach (var service in services)
                    {
                        var mapper = _mapper.Map<ServiceDtoResponse>(service);
                        serviceList.Add(mapper);
                    }

                }
                return serviceList;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool status, string result)> UpdateService(ServiceDtoRequest service, int serviceId)
        {
            try
            {
                var existedCategory = await _unitOfWork.ServiceCategoryRepository.GetByIDAsync(service.CategoryId);
                if (existedCategory == null)
                {
                    
                    return (false, "Category not found, check again");
                }
                var existedService = await _unitOfWork.ServiceRepository.GetByIDAsync(serviceId);
                if(existedService != null)
                {
                    _mapper.Map(service, existedService);
                    await _unitOfWork.ServiceRepository.UpdateAsync(existedService);
                    await _unitOfWork.SaveAsync();
                    return (true, "Update successfully");
                }
                else
                {
                    return (false, "Service not found, check again");
                }
                


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
