using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.ServiceCategoryDTOs;
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
    public class ServiceCategory_Service : IServiceCategory_Service
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ServiceCategory_Service(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<bool> AddServiceCategory(ServiceCategoryDto category)
        {
            try
            {
                var serviceCategory = _mapper.Map<ServiceCategory>(category);
                await _unitOfWork.ServiceCategoryRepository.AddAsync(serviceCategory);
                await _unitOfWork.SaveAsync();
                return true;

            }
            catch 
            {
                return false;
            }
        }

        public async Task<List<ServiceCategoryDtoResponse>> GetAllServiceCategories()
        {
            try
            {
                List<ServiceCategoryDtoResponse> categoryList = new List<ServiceCategoryDtoResponse>();
                var serviceCategorys = await _unitOfWork.ServiceCategoryRepository.GetAllAsync();
                if (serviceCategorys != null) { 
                    foreach(var serviceCategory in serviceCategorys)
                    {
                        var mapper = _mapper.Map<ServiceCategoryDtoResponse>(serviceCategory);
                        categoryList.Add(mapper);
                    } 
                    
                }
                return categoryList;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool status, string result)> UpdateServiceCategory(ServiceCategoryDto category, int categoryId)
        {
            try
            {
                var existedCategory = await _unitOfWork.ServiceCategoryRepository.GetByIDAsync(categoryId);
                if (existedCategory != null)
                {
                    _mapper.Map(category, existedCategory);
                    await _unitOfWork.ServiceCategoryRepository.UpdateAsync(existedCategory);
                    await _unitOfWork.SaveAsync();
                    return (true, "Update successfully");
                }
                else
                {
                    return (false, "Category not found");
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
