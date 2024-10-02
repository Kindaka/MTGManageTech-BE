using MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class MartyrGraveService : IMartyrGraveService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MartyrGraveService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Hàm tạo MartyrCode bằng cách ghép AreaNumber, RowNumber, MartyrNumber
        private string GenerateMartyrCode(int areaNumber, int rowNumber, int martyrNumber)
        {
            return $"MTG-{areaNumber}-{rowNumber}-{martyrNumber}";
        }

        public async Task<MartyrGraveDtoResponse> CreateMartyrGraveAsync(MartyrGraveDtoRequest martyrGraveDto)
        {
            // Kiểm tra AreaId có tồn tại không
            var area = await _unitOfWork.AreaRepository.GetByIDAsync(martyrGraveDto.AreaId);
            if (area == null)
            {
                throw new KeyNotFoundException("AreaId does not exist.");
            }

            // Tạo thực thể từ DTO
            var martyrGrave = _mapper.Map<MartyrGrave>(martyrGraveDto);

            // Gọi hàm GenerateMartyrCode để tạo mã MartyrCode
            martyrGrave.MartyrCode = GenerateMartyrCode(martyrGrave.AreaNumber, martyrGrave.RowNumber, martyrGrave.MartyrNumber);

            // Thêm MartyrGrave vào cơ sở dữ liệu
            await _unitOfWork.MartyrGraveRepository.AddAsync(martyrGrave);
            await _unitOfWork.SaveAsync();

            // Trả về DTO response
            return _mapper.Map<MartyrGraveDtoResponse>(martyrGrave);
        }

        public async Task<MartyrGraveDtoResponse> UpdateMartyrGraveAsync(int id, MartyrGraveDtoRequest martyrGraveDto)
        {
            // Kiểm tra AreaId có tồn tại không
            var area = await _unitOfWork.AreaRepository.GetByIDAsync(martyrGraveDto.AreaId);
            if (area == null)
            {
                throw new KeyNotFoundException("AreaId does not exist.");
            }

            var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(id);
            if (martyrGrave == null)
            {
                return null;
            }

            // Cập nhật các thuộc tính từ DTO sang thực thể
            _mapper.Map(martyrGraveDto, martyrGrave);

            // Tạo lại MartyrCode dựa trên các thông tin mới
            martyrGrave.MartyrCode = GenerateMartyrCode(martyrGrave.AreaNumber, martyrGrave.RowNumber, martyrGrave.MartyrNumber);

            // Cập nhật thông tin vào cơ sở dữ liệu
            await _unitOfWork.MartyrGraveRepository.UpdateAsync(martyrGrave);
            await _unitOfWork.SaveAsync();

            // Trả về kết quả cập nhật
            return _mapper.Map<MartyrGraveDtoResponse>(martyrGrave);
        }


        public async Task<IEnumerable<MartyrGraveDtoResponse>> GetAllMartyrGravesAsync()
        {
            var graves = await _unitOfWork.MartyrGraveRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MartyrGraveDtoResponse>>(graves);
        }

        public async Task<MartyrGraveDtoResponse> GetMartyrGraveByIdAsync(int id)
        {
            var grave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(id);
            return _mapper.Map<MartyrGraveDtoResponse>(grave);
        }

        public async Task<bool> DeleteMartyrGraveAsync(int id)
        {
            var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(id);
            if (martyrGrave == null)
            {
                return false;
            }

            await _unitOfWork.MartyrGraveRepository.DeleteAsync(martyrGrave);
            await _unitOfWork.SaveAsync();
            return true;
        }


        public async Task<List<MartyrGraveGetAllDtoResponse>> GetAllMartyrGravesForManagerAsync()
        {
            try
            {
                // Lấy toàn bộ dữ liệu từ MartyrGrave bao gồm MartyrGraveInformations và Account
                var martyrGraves = await _unitOfWork.MartyrGraveRepository.GetAllAsync(includeProperties: "MartyrGraveInformations");

                // Khởi tạo danh sách kết quả
                List<MartyrGraveGetAllDtoResponse> martyrGraveList = new List<MartyrGraveGetAllDtoResponse>();

                // Duyệt qua tất cả các MartyrGrave đã lấy
                foreach (var m in martyrGraves)
                {
                    // Tìm Account dựa trên CustomerCode (Account có thể là người thân)
                    var customer = (await _unitOfWork.AccountRepository.FindAsync(a => a.CustomerCode == m.CustomerCode)).FirstOrDefault();

                    // Nếu tìm thấy Account thì thêm vào danh sách kết quả
                    if (customer != null)
                    {
                        var mapping = new MartyrGraveGetAllDtoResponse
                        {
                            Code = m.MartyrCode,
                            Name = m.MartyrGraveInformations.FirstOrDefault()?.Name, // Lấy tên từ MartyrGraveInformation
                            Location = $"{m.AreaNumber}-{m.RowNumber}-{m.MartyrNumber}", // Định dạng vị trí
                            RelativeName = customer.FullName, // Lấy tên người thân từ Account
                            RelativePhone = customer.PhoneNumber, // Lấy số điện thoại người thân từ Account
                            Status = m.Status // Lấy trạng thái của MartyrGrave
                        };
                        martyrGraveList.Add(mapping); // Thêm kết quả vào danh sách
                    }
                }

                return martyrGraveList; // Trả về danh sách các MartyrGrave đã được định dạng
            }
            catch (Exception ex)
            {
                // Ghi lại lỗi hoặc ném ngoại lệ lên phía trên để xử lý thêm
                throw new Exception($"Error in fetching martyr graves: {ex.Message}", ex);
            }
        }


    }
}
