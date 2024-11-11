using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.AreaDTos;
using MartyrGraveManagement_BAL.ModelViews.AreaDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class AreaService : IAreaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AreaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateNewArea(AreaDtoRequest newArea)
        {
            try
            {
                var area = new Area()
                {
                    AreaName = newArea.AreaName,
                    Description = newArea.Description,
                    Status = true
                };
                await _unitOfWork.AreaRepository.AddAsync(area);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AreaDTOResponse>> GetAreas()
        {
            try
            {
                // Lấy tất cả khu vực có trạng thái true
                var areas = await _unitOfWork.AreaRepository.GetAsync(area => area.Status == true);
                return _mapper.Map<List<AreaDTOResponse>>(areas);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AreaDTOResponse>> GetAllAreasForStaffOrManager()
        {
            try
            {
                // Lấy tất cả khu vực bất kể trạng thái
                var areas = await _unitOfWork.AreaRepository.GetAllAsync();
                return _mapper.Map<List<AreaDTOResponse>>(areas);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AreaDTOResponse> GetAreaById(int id)
        {
            try
            {
                var area = await _unitOfWork.AreaRepository.GetByIDAsync(id);
                return area == null ? null : _mapper.Map<AreaDTOResponse>(area);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateArea(int id, AreaDtoRequest updateArea)
        {
            try
            {
                var area = await _unitOfWork.AreaRepository.GetByIDAsync(id);
                if (area == null)
                {
                    return false;
                }

                area.AreaName = updateArea.AreaName;
                area.Description = updateArea.Description;
                await _unitOfWork.AreaRepository.UpdateAsync(area);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ToggleStatusArea(int id)
        {
            try
            {
                // Lấy thông tin Area theo ID
                var area = await _unitOfWork.AreaRepository.GetByIDAsync(id);
                if (area == null)
                {
                    return false;  // Trả về false nếu không tìm thấy Area
                }

                // Đảo ngược trạng thái hiện tại: nếu là true thì chuyển thành false và ngược lại
                area.Status = !area.Status;

                // Cập nhật lại vào database
                await _unitOfWork.AreaRepository.UpdateAsync(area);
                await _unitOfWork.SaveAsync();

                return true;  // Cập nhật thành công
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<bool> DeleteArea(int id)
        {
            try
            {
                var area = await _unitOfWork.AreaRepository.GetByIDAsync(id);
                if (area == null)
                {
                    return false;
                }

                await _unitOfWork.AreaRepository.DeleteAsync(area);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool status, string message)> ImportAreasFromExcelAsync(string excelFilePath)
        {
            try
            {
                // Set the license context
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets[1];
                    if (worksheet == null)
                        return (false, "Worksheet not found in Excel file.");

                    var areasToAdd = new List<Area>();
                    int row = 2; // Start at row 2 to skip headers

                    // Step 1: Gather all areas from the Excel file
                    var excelAreas = new List<(string AreaName, int AreaNumber, string? Description)>();
                    while (worksheet.Cells[row, 1].Value != null)
                    {
                        var areaName = (worksheet.Cells[row, 1].Value).ToString();
                        var areaNumber = Convert.ToInt32(worksheet.Cells[row, 2].Value);
                        var description = (worksheet.Cells[row, 3].Value).ToString();


                        excelAreas.Add((AreaName: areaName, AreaNumber: areaNumber, Description: description));
                        row++;
                    }

                    // Step 2: Load existing areas in bulk
                    var areaNumbers = excelAreas.Select(e => e.AreaNumber).Distinct();
                    var existingAreas = await _unitOfWork.AreaRepository.FindAsync(
                        l => areaNumbers.Contains(l.AreaNumber));

                    // Step 3: Use a HashSet for fast existence checking
                    var existingAreaKeys = new HashSet<(string AreaName, int AreaNumber, string? Description)>(
                        existingAreas.Select(l => (l.AreaName, l.AreaNumber, l.Description)));

                    // Step 4: Add only new locations to the list
                    foreach (var (areaName, areaNumber, description) in excelAreas)
                    {
                        if (!existingAreaKeys.Contains((areaName, areaNumber, description)))
                        {
                            areasToAdd.Add(new Area
                            {
                                AreaName = areaName,
                                Description = description,
                                AreaNumber = areaNumber,
                                Status = true
                            });
                        }
                    }

                    // Step 5: Save new locations in bulk if any
                    if (areasToAdd.Any())
                    {
                        await _unitOfWork.AreaRepository.AddRangeAsync(areasToAdd);
                        await _unitOfWork.SaveAsync();
                    }

                    return (true, $"{areasToAdd.Count} khu vực đã được thêm thành công.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error importing locations: {ex.Message}");
            }
        }
    }
}
