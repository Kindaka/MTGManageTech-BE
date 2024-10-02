using MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Security.Principal;

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
        private string getLastName(string fullName)
        {
            string[] parts = fullName.Trim().Split(' ');
            return parts[parts.Length - 1];
        }

        private string GenerateCustomerCode(string fullName, string phone)
        {
            string lastName = getLastName(fullName);
            return $"Customer-{lastName}-{phone}";
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

        public async Task<(bool status, string result, string? accountName, string? password)> CreateMartyrGraveAsyncV2(MartyrGraveDtoRequest martyrGraveDto)
        {
            try
            {
                // Kiểm tra AreaId có tồn tại không
                var area = await _unitOfWork.AreaRepository.GetByIDAsync(martyrGraveDto.AreaId);
                if (area == null)
                {
                    return (false, "Không tìm thấy khu vực", null, null);
                }

                var accountMapping = new Account
                {
                    FullName = martyrGraveDto.UserName,
                    PhoneNumber = martyrGraveDto.Phone,
                    Address = martyrGraveDto.Address,
                    DateOfBirth = martyrGraveDto.Dob,
                    RoleId = 4,
                    Status = true
                };

                accountMapping.CustomerCode = GenerateCustomerCode(martyrGraveDto.UserName, martyrGraveDto.Phone);
                accountMapping.AccountName = $"{getLastName(martyrGraveDto.UserName)}{martyrGraveDto.Dob.Year}-{martyrGraveDto.Phone}";
                string randomPassword = CreateRandomPassword(8);
                accountMapping.HashedPassword = await HashPassword(randomPassword);

                await _unitOfWork.AccountRepository.AddAsync(accountMapping);
                await _unitOfWork.SaveAsync();

                var insertedAccount = (await _unitOfWork.AccountRepository.FindAsync(a => a.CustomerCode == accountMapping.CustomerCode)).FirstOrDefault();
                if (insertedAccount != null)
                {
                    // Tạo thực thể từ DTO
                    var martyrGrave = _mapper.Map<MartyrGrave>(martyrGraveDto);

                    string martyrCode = GenerateMartyrCode(martyrGrave.AreaNumber, martyrGrave.RowNumber, martyrGrave.MartyrNumber);
                    var existedMartyrGrave = (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrCode == martyrCode)).FirstOrDefault();
                    if (existedMartyrGrave != null)
                    {
                          await _unitOfWork.AccountRepository.DeleteAsync(insertedAccount);
                          await _unitOfWork.SaveAsync();
                          return (false, "MartyrCode đã tồn tại hãy kiểm tra lại", null, null);
                    }

                    // Gọi hàm GenerateMartyrCode để tạo mã MartyrCode
                    martyrGrave.MartyrCode = martyrCode;
                    martyrGrave.CustomerCode = accountMapping.CustomerCode;
                    martyrGrave.Status = true;

                    // Thêm MartyrGrave vào cơ sở dữ liệu
                    await _unitOfWork.MartyrGraveRepository.AddAsync(martyrGrave);
                    await _unitOfWork.SaveAsync();
                    var insertedGrave = (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrCode == martyrGrave.MartyrCode)).FirstOrDefault();
                    if (insertedGrave != null)
                    {
                        if (martyrGraveDto.Informations.Any())
                        {
                            foreach (var martyrGraveInformation in martyrGraveDto.Informations)
                            {
                                var checkExistedGrave = (await _unitOfWork.MartyrGraveRepository.GetByIDAsync(insertedGrave.MartyrId));
                                if (checkExistedGrave != null)
                                {
                                    var information = new MartyrGraveInformation
                                    {
                                        MartyrId = checkExistedGrave.MartyrId,
                                        Name = martyrGraveInformation.Name,
                                        Medal = martyrGraveInformation.Medal,
                                        DateOfSacrifice = martyrGraveInformation.DateOfSacrifice
                                    };
                                    await _unitOfWork.MartyrGraveInformationRepository.AddAsync(information);
                                    await _unitOfWork.SaveAsync();
                                }
                                else
                                {
                                    await _unitOfWork.AccountRepository.DeleteAsync(insertedAccount);
                                    await _unitOfWork.MartyrGraveRepository.DeleteAsync(insertedGrave);
                                    await _unitOfWork.SaveAsync();
                                    return (false, "MartyrId không đúng, hãy kiểm tra lại", null, null);
                                }
                            }
                        }
                    }
                    else
                    {
                        return (false, "Không tìm thấy MartyrGrave đã tạo", null, null);
                    }
                    

                    // Trả về DTO response
                    return (true, "Mộ đã được tạo thành công, trả về tài khoản đăng nhập customer", accountMapping.AccountName, randomPassword);
                }
                return (false, "Không tìm thấy account đã tạo", null, null);
                
            }
            catch (Exception ex)
            {
                var customerCode = GenerateCustomerCode(martyrGraveDto.UserName, martyrGraveDto.Phone);
                var martyrCode = GenerateMartyrCode(martyrGraveDto.AreaNumber, martyrGraveDto.RowNumber, martyrGraveDto.MartyrNumber);
                var insertedAccount = (await _unitOfWork.AccountRepository.FindAsync(a => a.CustomerCode == customerCode)).FirstOrDefault();
                var insertedGrave = (await _unitOfWork.MartyrGraveRepository.FindAsync(a => a.MartyrCode == martyrCode)).FirstOrDefault();
                if (insertedAccount != null)
                {
                    await _unitOfWork.AccountRepository.DeleteAsync(insertedAccount);
                    await _unitOfWork.SaveAsync();
                }
                if (insertedGrave != null)
                {
                    await _unitOfWork.MartyrGraveRepository.DeleteAsync(insertedGrave);
                    await _unitOfWork.SaveAsync();
                }
                throw new Exception(ex.Message);
            }
        }

        private async Task<string> HashPassword(string password)
        {
            try
            {
                using (SHA512 sha512 = SHA512.Create())
                {
                    byte[] hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));

                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        stringBuilder.Append(hashBytes[i].ToString("x2"));
                    }

                    return await Task.FromResult(stringBuilder.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error hashing password: {ex.Message}");
            }
        }

        private static string CreateRandomPassword(int length)
        {
            // Khởi tạo Random
            Random random = new Random();

            // Tạo một mảng ký tự để chứa các ký tự số
            char[] result = new char[length];

            // Duyệt qua từng vị trí và gán giá trị ngẫu nhiên từ 0 đến 9
            for (int i = 0; i < length; i++)
            {
                result[i] = (char)('0' + random.Next(0, 10));  // Sinh số ngẫu nhiên từ 0 đến 9
            }

            // Chuyển mảng ký tự thành chuỗi
            return new string(result);
        }
    }
}
