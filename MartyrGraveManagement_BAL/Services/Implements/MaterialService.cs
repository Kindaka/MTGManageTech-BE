using MartyrGraveManagement_BAL.ModelViews.MaterialDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class MaterialService : IMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MaterialService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MaterialDtoResponse>> GetAllMaterials()
        {
            try
            {
                var materials = await _unitOfWork.MaterialRepository.GetAsync(m => m.Status == true);
                return materials.Select(m => new MaterialDtoResponse
                {
                    MaterialId = m.MaterialId,
                    MaterialName = m.MaterialName,
                    Description = m.Description,
                    Price = m.Price,
                    Status = m.Status
                });
            }
            catch (Exception ex) { 
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<MaterialDtoResponse>> GetMaterialsForAdmin()
        {
            try
            {
                var materials = await _unitOfWork.MaterialRepository.GetAllAsync();
                return materials.Select(m => new MaterialDtoResponse
                {
                    MaterialId = m.MaterialId,
                    MaterialName = m.MaterialName,
                    Description = m.Description,
                    Price = m.Price,
                    Status = m.Status
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<MaterialDtoResponse> GetMaterialById(int id)
        {
            try
            {
                var material = await _unitOfWork.MaterialRepository.GetByIDAsync(id);
                if (material == null) return null;

                return new MaterialDtoResponse
                {
                    MaterialId = material.MaterialId,
                    MaterialName = material.MaterialName,
                    Description = material.Description,
                    Price = material.Price,
                    Status = material.Status
                };
            }
            catch (Exception ex) { 
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool success, string message)> CreateMaterial(MaterialDtoRequest materialDto)
        {
            try
            {
                var material = new Material
                {
                    MaterialName = materialDto.MaterialName,
                    Description = materialDto.Description,
                    Price = materialDto.Price,
                    Status = true
                };

                await _unitOfWork.MaterialRepository.AddAsync(material);
                await _unitOfWork.SaveAsync();

                return (true, "Vật liệu đã được tạo thành công");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool success, string message)> UpdateMaterial(int id, MaterialDtoRequest materialDto)
        {
            try
            {
                var material = await _unitOfWork.MaterialRepository.GetByIDAsync(id);
                if (material == null)
                    return (false, "Material not found");

                material.MaterialName = materialDto.MaterialName;
                material.Description = materialDto.Description;
                material.Price = materialDto.Price;

                await _unitOfWork.MaterialRepository.UpdateAsync(material);
                await _unitOfWork.SaveAsync();

                return (true, "Vật liệu được cập nhật thành công");
            }
            catch (Exception ex) { 
                throw new Exception(ex.Message);    
            }
        }

        public async Task<(bool success, string message)> UpdateStatus(int id)
        {
            try
            {
                var material = await _unitOfWork.MaterialRepository.GetByIDAsync(id);
                if (material == null)
                    return (false, "Material not found");

                if (material.Status == true)
                {
                    material.Status = false;
                    await _unitOfWork.MaterialRepository.UpdateAsync(material);
                    await _unitOfWork.SaveAsync();
                }
                else
                {
                    material.Status = true;
                    await _unitOfWork.MaterialRepository.UpdateAsync(material);
                    await _unitOfWork.SaveAsync();
                }

                return (true, "Cập nhật trạng thái vật liệu thành công");
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
    }
}
