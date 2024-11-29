using AutoMapper;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public LocationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<(bool status, string message)> ImportLocationsFromExcelAsync(string excelFilePath)
        {
            try
            {
                // Set the license context
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets[2];
                    if (worksheet == null)
                        return (false, "Worksheet not found in Excel file.");

                    var locationsToAdd = new List<Location>();
                    int row = 2; // Start at row 2 to skip headers

                    // Step 1: Gather all locations from the Excel file
                    var excelLocations = new List<(int AreaNumber, int RowNumber, int MartyrNumber)>();
                    while (worksheet.Cells[row, 1].Value != null)
                    {
                        var rowNumber = Convert.ToInt32(worksheet.Cells[row, 1].Value);
                        var martyrNumber = Convert.ToInt32(worksheet.Cells[row, 2].Value);
                        var areaNumber = Convert.ToInt32(worksheet.Cells[row, 3].Value);

                        excelLocations.Add((AreaNumber: areaNumber, RowNumber: rowNumber, MartyrNumber: martyrNumber));
                        row++;
                    }

                    // Step 2: Load existing locations in bulk
                    var areaNumbers = excelLocations.Select(e => e.AreaNumber).Distinct();
                    var existingLocations = await _unitOfWork.LocationRepository.FindAsync(
                        l => areaNumbers.Contains(l.AreaNumber));

                    // Step 3: Use a HashSet for fast existence checking
                    var existingLocationKeys = new HashSet<(int AreaNumber, int RowNumber, int MartyrNumber)>(
                        existingLocations.Select(l => (l.AreaNumber, l.RowNumber, l.MartyrNumber)));

                    // Step 4: Add only new locations to the list
                    foreach (var (areaNumber, rowNumber, martyrNumber) in excelLocations)
                    {
                        if (!existingLocationKeys.Contains((areaNumber, rowNumber, martyrNumber)))
                        {
                            locationsToAdd.Add(new Location
                            {
                                AreaNumber = areaNumber,
                                RowNumber = rowNumber,
                                MartyrNumber = martyrNumber,
                                Status = true
                            });
                        }
                    }

                    // Step 5: Save new locations in bulk if any
                    if (locationsToAdd.Any())
                    {
                        await _unitOfWork.LocationRepository.AddRangeAsync(locationsToAdd);
                        await _unitOfWork.SaveAsync();
                    }

                    return (true, $"{locationsToAdd.Count} locations added successfully.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error importing locations: {ex.Message}");
            }
        }
    }
    
}
