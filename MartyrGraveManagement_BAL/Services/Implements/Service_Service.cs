using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.MaterialDTOs;
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
    public class Service_Service : IService_Service
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public Service_Service(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(bool status, string result)> AddService(ServiceDtoRequest service)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var existedCategory = await _unitOfWork.ServiceCategoryRepository.GetByIDAsync(service.CategoryId);
                    if (existedCategory != null)
                    {
                        double totalPrice = 0;
                        var newService = new Service
                        {
                            CategoryId = service.CategoryId,
                            ServiceName = service.ServiceName,
                            Description = service.Description,
                            ImagePath = service.ImagePath,
                            Price = 0,
                            Status = true
                        };
                        await _unitOfWork.ServiceRepository.AddAsync(newService);
                        await _unitOfWork.SaveAsync();
                        if (service.Materials.Any())
                        {
                            foreach (var material in service.Materials)
                            {
                                var insertMaterial = new Material
                                {
                                    ServiceId = newService.ServiceId,
                                    MaterialName = material.MaterialName,
                                    Description = material.Description,
                                    Price = material.Price
                                };
                                await _unitOfWork.MaterialRepository.AddAsync(insertMaterial);
                                await _unitOfWork.SaveAsync();
                                totalPrice += material.Price;
                            }
                        }
                        newService.Price = totalPrice + (totalPrice * 0.05);
                        await _unitOfWork.ServiceRepository.UpdateAsync(newService);
                        await _unitOfWork.SaveAsync();
                        await transaction.CommitAsync();
                        return (true, "Add successfully");
                    }
                    else
                    {
                        return (false, "Category not found");
                    }


                }
                catch (Exception ex) 
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
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

        public async Task<List<ServiceDtoResponse>> GetAllServices(int? categoryId)
        {
            try
            {
                List<ServiceDtoResponse> serviceList = new List<ServiceDtoResponse>();
                List<Service> services = new List<Service>();
                if (categoryId == null) {
                   services = (await _unitOfWork.ServiceRepository.GetAsync(s => s.Status == true)).ToList();
                }
                else 
                {
                   services = (await _unitOfWork.ServiceRepository.GetAsync(s => s.CategoryId == categoryId && s.Status == true)).ToList();
                }
                
                if (services != null)
                {
                    foreach (var service in services)
                    {
                        var mapper = _mapper.Map<ServiceDtoResponse>(service);
                        var category = await _unitOfWork.ServiceCategoryRepository.GetByIDAsync(service.CategoryId);
                        mapper.CategoryName = category.CategoryName;
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

        public async Task<ServiceDetailDtoResponse> GetServiceById(int serviceId)
        {
            try
            {
                var service = await _unitOfWork.ServiceRepository.GetByIDAsync(serviceId);
                if (service != null) {
                    double wage = 0;
                    var serviceView = _mapper.Map<ServiceDetailDtoResponse>(service);
                    var materials = await _unitOfWork.MaterialRepository.GetAsync(m => m.ServiceId == service.ServiceId);
                    if(materials.Any())
                    {
                        foreach( var material in materials)
                        {
                            var materialView = new MaterialDtoResponse
                            {
                                MaterialId = material.MaterialId,
                                MaterialName = material.MaterialName,
                                Description = material.Description,
                                Price = material.Price
                            };
                            serviceView.Materials.Add(materialView);
                            wage += material.Price;
                        }
                    }
                    serviceView.wage = wage * 0.05;
                    return serviceView;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<(List<ServiceDtoResponse> serviceList, int totalPage)> GetServicesForAdmin(int? categoryId, int page, int pageSize)
        {
            try
            {
                var totalService = await _unitOfWork.ServiceRepository.CountAsync();
                var totalPage = (int)Math.Ceiling(totalService / (double)pageSize);
                List<ServiceDtoResponse> serviceList = new List<ServiceDtoResponse>();
                List<Service> services = new List<Service>();
                if (categoryId == null)
                {
                    services = (await _unitOfWork.ServiceRepository.GetAsync(pageIndex: page, pageSize: pageSize)).ToList();
                }
                else
                {
                    services = (await _unitOfWork.ServiceRepository.GetAsync(s => s.CategoryId == categoryId, pageIndex: page, pageSize: pageSize)).ToList();
                }

                if (services != null)
                {
                    foreach (var service in services)
                    {
                        var mapper = _mapper.Map<ServiceDtoResponse>(service);
                        serviceList.Add(mapper);
                    }

                }
                return (serviceList, totalPage);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool status, string result)> UpdateService(ServiceDtoRequest service, int serviceId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var existedCategory = await _unitOfWork.ServiceCategoryRepository.GetByIDAsync(service.CategoryId);
                    if (existedCategory == null)
                    {

                        return (false, "Category not found, check again");
                    }
                    var existedService = await _unitOfWork.ServiceRepository.GetByIDAsync(serviceId);
                    if (existedService != null)
                    {
                        double totalPrice = 0;
                        existedService.CategoryId = service.CategoryId;
                        existedService.ServiceName = service.ServiceName;
                        existedService.Description = service.Description;
                        existedService.ImagePath = service.ImagePath;
                        existedService.Price = 0;
                        existedService.Status = true;
                        await _unitOfWork.ServiceRepository.UpdateAsync(existedService);
                        await _unitOfWork.SaveAsync();

                        var currentMaterials = await _unitOfWork.MaterialRepository.GetAsync(p => p.ServiceId == existedService.ServiceId);
                        if (currentMaterials.Any())
                        {
                            foreach (var material in currentMaterials)
                            {
                                await _unitOfWork.MaterialRepository.DeleteAsync(material);
                                await _unitOfWork.SaveAsync();
                            }
                        }

                        if (service.Materials.Any())
                        {
                            foreach (var material in service.Materials)
                            {
                                var insertMaterial = new Material
                                {
                                    ServiceId = existedService.ServiceId,
                                    MaterialName = material.MaterialName,
                                    Description = material.Description,
                                    Price = material.Price
                                };
                                await _unitOfWork.MaterialRepository.AddAsync(insertMaterial);
                                await _unitOfWork.SaveAsync();
                                totalPrice += material.Price;
                            }
                        }
                        existedService.Price = totalPrice + (totalPrice * 0.05);
                        await _unitOfWork.ServiceRepository.UpdateAsync(existedService);
                        await _unitOfWork.SaveAsync();
                        await transaction.CommitAsync();
                        return (true, "Update successfully");
                    }
                    else
                    {
                        return (false, "Service not found, check again");
                    }



                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
