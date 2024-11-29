using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.GraveServiceDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
        public async Task<(bool check, string response)> CreateServiceForGrave(GraveServiceDtoRequest request)
        {//
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var grave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(request.MartyrId);
                    if (grave != null)
                    {
                        List<int> checkService = new List<int>();
                        foreach (var serviceItem in request.ServiceId)
                        {
                            var service = await _unitOfWork.ServiceRepository.GetByIDAsync(serviceItem);
                            if (service != null)
                            {
                                if (!checkService.Contains(serviceItem))
                                {
                                    checkService.Add(serviceItem);
                                }
                            }
                            else
                            {
                                return (false, "Không tìm thấy dịch vụ, vui lòng kiểm tra lại");
                            }
                        }

                        // create service grave
                        foreach (var serviceItem in checkService)
                        {
                            var checkGraveService = (await _unitOfWork.GraveServiceRepository.GetAsync(gs => gs.MartyrId == request.MartyrId && gs.ServiceId == serviceItem)).FirstOrDefault();
                            if (checkGraveService == null) 
                            { 
                                var graveService = new GraveService
                                {
                                    MartyrId = request.MartyrId,
                                    ServiceId = serviceItem,
                                    CreatedDate = DateTime.Now,
                                };
                            await _unitOfWork.GraveServiceRepository.AddAsync(graveService);
                            
                            }

                        }
                        await _unitOfWork.SaveAsync();
                        await transaction.CommitAsync();
                        return (true, "Đã tạo thành công");
                    }
                    else
                    {
                        return (false, "Không tìm thấy mộ");
                    }

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<(bool check, string response)> UpdateServiceForGrave(int martyrId, UpdateServiceForGraveDtoRequest request)
        {//
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var grave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(martyrId);
                    if (grave != null)
                    {
                        List<int> checkService = new List<int>();
                        foreach (var serviceItem in request.ServiceId)
                        {
                            var service = await _unitOfWork.ServiceRepository.GetByIDAsync(serviceItem);
                            if (service != null)
                            {
                                if (!checkService.Contains(serviceItem))
                                {
                                    checkService.Add(serviceItem);
                                }
                            }
                            else
                            {
                                return (false, "Không tìm thấy dịch vụ, vui lòng kiểm tra lại");
                            }
                        }

                        var oldGraveServices = await _unitOfWork.GraveServiceRepository.FindAsync(b => b.MartyrId == grave.MartyrId);
                        if (oldGraveServices != null)
                        {
                            foreach (var oldGraveService in oldGraveServices)
                            {
                                await _unitOfWork.GraveServiceRepository.DeleteAsync(oldGraveService);
                            }
                        }

                        // create service grave
                        foreach (var serviceItem in checkService)
                        {
                            var checkGraveService = (await _unitOfWork.GraveServiceRepository.GetAsync(gs => gs.MartyrId == grave.MartyrId && gs.ServiceId == serviceItem)).FirstOrDefault();
                            if (checkGraveService == null)
                            {
                                var graveService = new GraveService
                                {
                                    MartyrId = grave.MartyrId,
                                    ServiceId = serviceItem,
                                    CreatedDate = DateTime.Now,
                                };
                                await _unitOfWork.GraveServiceRepository.AddAsync(graveService);
                                
                            }

                        }
                        await _unitOfWork.SaveAsync();
                        await transaction.CommitAsync();
                        return (true, "Đã cập nhật thành công");
                    }
                    else
                    {
                        return (false, "Không tìm thấy mộ");
                    }

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<List<GraveServiceDtoResponse>> GetAllServicesForGrave(int martyrId, int? categoryId)
        {
            try
            {
                List<GraveServiceDtoResponse> serviceList = new List<GraveServiceDtoResponse>();
                List<GraveService> listGraveServices = new List<GraveService>();
                if (categoryId == 0)
                {
                    listGraveServices = (await _unitOfWork.GraveServiceRepository.GetAsync(gs => gs.MartyrId == martyrId, includeProperties: "Service.ServiceCategory,MartyrGrave")).ToList();
                }
                else
                {
                    var category = await _unitOfWork.ServiceCategoryRepository.GetByIDAsync(categoryId);
                    if (category != null)
                    {
                        listGraveServices = (await _unitOfWork.GraveServiceRepository.GetAsync(gs => gs.MartyrId == martyrId && gs.Service.ServiceCategory.CategoryId == categoryId, includeProperties: "Service.ServiceCategory,MartyrGrave")).ToList();
                    }
                    else
                    {
                        throw new Exception("Không tìm thấy loại dịch vụ");
                    }
                }
                if (listGraveServices != null)
                {
                    foreach(var graveService in listGraveServices)
                    {
                        var item = new GraveServiceDtoResponse
                        {
                            GraveServiceId = graveService.GraveServiceId,
                            MartyrId = graveService.MartyrId,
                            ServiceId = graveService.ServiceId,
                            CategoryId = graveService.Service.ServiceCategory.CategoryId,
                            ServiceName = graveService.Service.ServiceName,
                            CategoryName = graveService.Service.ServiceCategory.CategoryName,
                            Description = graveService.Service.Description,
                            Price = graveService.Service.Price,
                            ImagePath = graveService.Service.ImagePath,
                            Status = graveService.Service.Status,
                            isScheduleService = graveService.Service.isScheduleService,
                            RecurringType = graveService.Service.RecurringType

                        };
                        serviceList.Add(item);
                    }



                    //if (services != null)
                    //{
                    //    foreach (var service in services)
                    //    {
                    //        var mapper = _mapper.Map<GraveServiceDtoResponse>(service);
                    //        var category = await _unitOfWork.ServiceCategoryRepository.GetByIDAsync(service.CategoryId);
                    //        mapper.CategoryName = category.CategoryName;
                    //        serviceList.Add(mapper);
                    //    }

                    //}
                    return serviceList;
                }
                else
                {
                    return serviceList;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool check, string response)> DeleteServiceOfGrave(int graveServiceId)
        {
            try
            {
                var graveService = await _unitOfWork.GraveServiceRepository.GetByIDAsync(graveServiceId);
                if(graveService != null)
                {
                    await _unitOfWork.GraveServiceRepository.DeleteAsync(graveService);
                    await _unitOfWork.SaveAsync();
                    return (true, "Đã xóa thành công");
                }
                return (false, "Không tìm thấy dịch vụ của mộ");
            }
            catch (Exception ex) { 
                throw new Exception(ex.Message);
            }
        }
    }
}
