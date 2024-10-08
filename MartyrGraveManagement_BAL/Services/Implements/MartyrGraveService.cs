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
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveInformationDTOs;
using MartyrGraveManagement_BAL.ModelViews.EmailDTOs;
using MartyrGraveManagement_BAL.ModelViews.CustomerDTOs;
using System.Linq.Expressions;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class MartyrGraveService : IMartyrGraveService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISendEmailService _sendEmailService;

        public MartyrGraveService(IUnitOfWork unitOfWork, IMapper mapper, ISendEmailService sendEmailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sendEmailService = sendEmailService;
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


        public async Task<List<MartyrGraveDtoResponse>> GetAllMartyrGravesAsync()
        {
            //var graves = await _unitOfWork.MartyrGraveRepository.GetAllAsync();
            //return _mapper.Map<IEnumerable<MartyrGraveDtoResponse>>(graves);
            try
            {
                List<MartyrGraveDtoResponse> graveList = new List<MartyrGraveDtoResponse>();
                var graves = await _unitOfWork.MartyrGraveRepository.GetAllAsync();
                if (graves.Any())
                {
                    foreach (var grave in graves) {
                        var graveView = _mapper.Map<MartyrGraveDtoResponse>(grave);
                        var graveInformations = await _unitOfWork.MartyrGraveInformationRepository.GetAsync(g => g.MartyrId == grave.MartyrId);
                        if (graveInformations.Any())
                        {
                            foreach (var information in graveInformations)
                            {
                                var informationView = new MartyrGraveInformationDtoResponse
                                {
                                    InformationId = information.InformationId,
                                    MartyrId = information.MartyrId,
                                    Name = information.Name,
                                    NickName = information.NickName,
                                    Position = information.Position,
                                    Medal = information.Medal,
                                    HomeTown = information.HomeTown,
                                    DateOfBirth = information.DateOfBirth,
                                    DateOfSacrifice = information.DateOfSacrifice
                                };
                                graveView.MatyrGraveInformations.Add(informationView);
                            }
                        }

                        var graveImages = await _unitOfWork.GraveImageRepository.GetAsync(g => g.MartyrId == grave.MartyrId);
                        if (graveImages.Any())
                        {
                            foreach (var image in graveImages)
                            {
                                var imageView = new GraveImageDtoRequest
                                {
                                    UrlPath = image.UrlPath
                                };
                                graveView.Images.Add(imageView);
                            }
                        }
                        graveList.Add(graveView);
                    }
                }
                
                return graveList;
            }
            catch (Exception ex) { 
                throw new Exception(ex.Message);
            }
        }

        public async Task<MartyrGraveDtoResponse> GetMartyrGraveByIdAsync(int id)
        {
            //var grave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(id);
            //return _mapper.Map<MartyrGraveDtoResponse>(grave);
            try
            {
                var grave = (await _unitOfWork.MartyrGraveRepository.GetAsync(g => g.MartyrId == id)).FirstOrDefault();
                if (grave != null)
                {
                    var graveView = _mapper.Map<MartyrGraveDtoResponse>(grave);
                    var graveInformations = await _unitOfWork.MartyrGraveInformationRepository.GetAsync(g => g.MartyrId == grave.MartyrId);
                    if(graveInformations.Any())
                    {
                        foreach (var information in graveInformations)
                        {
                            var informationView = new MartyrGraveInformationDtoResponse
                            {
                                InformationId = information.InformationId,
                                MartyrId = information.MartyrId,
                                Name = information.Name,
                                NickName = information.NickName,
                                Position = information.Position,
                                Medal = information.Medal,
                                HomeTown = information.HomeTown,
                                DateOfBirth = information.DateOfBirth,
                                DateOfSacrifice = information.DateOfSacrifice
                            };
                            graveView.MatyrGraveInformations.Add(informationView);
                        }
                    }

                    var graveImages = await _unitOfWork.GraveImageRepository.GetAsync(g => g.MartyrId == grave.MartyrId);
                    if(graveImages.Any())
                    {
                        foreach(var image in graveImages)
                        {
                            var imageView = new GraveImageDtoRequest
                            {
                                UrlPath = image.UrlPath
                            };
                            graveView.Images.Add(imageView);    
                        }
                    }

                    return graveView;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateStatusMartyrGraveAsync(int id)
        {
            try
            {
                var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(id);
                if (martyrGrave == null)
                {
                    return false;
                }
                if (martyrGrave.Status == true)
                {
                    martyrGrave.Status = false;
                    await _unitOfWork.MartyrGraveRepository.UpdateAsync(martyrGrave);
                    await _unitOfWork.SaveAsync();
                    return true;
                }
                else
                {
                    martyrGrave.Status = true;
                    await _unitOfWork.MartyrGraveRepository.UpdateAsync(martyrGrave);
                    await _unitOfWork.SaveAsync();
                    return true;
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }


        public async Task<(List<MartyrGraveGetAllDtoResponse> response, int totalPage)> GetAllMartyrGravesForManagerAsync(int page, int pageSize)
        {
            try
            {
                // Tính tổng số mộ liệt sĩ
                var totalMartyrGraves = await _unitOfWork.MartyrGraveRepository.CountAsync();

                // Tính toán tổng số trang
                var totalPage = (int)Math.Ceiling(totalMartyrGraves / (double)pageSize);

                // Lấy dữ liệu mộ liệt sĩ với phân trang
                var martyrGraves = await _unitOfWork.MartyrGraveRepository.GetAsync(
                    includeProperties: "MartyrGraveInformations",
                    pageIndex: page,
                    pageSize: pageSize
                );

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

                // Trả về danh sách và tổng số trang
                return (martyrGraveList, totalPage);
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

                if (martyrGraveDto.Customer.UserName != null && martyrGraveDto.Customer.Phone != null)
                {
                    var customerCode = GenerateCustomerCode(martyrGraveDto.Customer.UserName, martyrGraveDto.Customer.Phone);
                    var existedCustomerCode = (await _unitOfWork.AccountRepository.FindAsync(c => c.CustomerCode == customerCode)).FirstOrDefault();

                    if (existedCustomerCode != null)
                    {
                        // Tạo thực thể từ DTO
                        var martyrGrave = _mapper.Map<MartyrGrave>(martyrGraveDto);

                        string martyrCode = GenerateMartyrCode(martyrGrave.AreaNumber, martyrGrave.RowNumber, martyrGrave.MartyrNumber);
                        var existedMartyrGrave = (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrCode == martyrCode)).FirstOrDefault();

                        if (existedMartyrGrave != null)
                        {
                            return (false, "MartyrCode đã tồn tại hãy kiểm tra lại", null, null);
                        }

                        // Gọi hàm GenerateMartyrCode để tạo mã MartyrCode
                        martyrGrave.MartyrCode = martyrCode;
                        martyrGrave.CustomerCode = customerCode;
                        martyrGrave.Status = true;

                        // Thêm MartyrGrave vào cơ sở dữ liệu
                        await _unitOfWork.MartyrGraveRepository.AddAsync(martyrGrave);
                        await _unitOfWork.SaveAsync();

                        var insertedGrave = (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrCode == martyrGrave.MartyrCode)).FirstOrDefault();

                        if (insertedGrave != null)
                        {
                            // Thêm thông tin MartyrGraveInformations
                            if (martyrGraveDto.Informations.Any())
                            {
                                foreach (var martyrGraveInformation in martyrGraveDto.Informations)
                                {
                                    var information = new MartyrGraveInformation
                                    {
                                        MartyrId = insertedGrave.MartyrId,
                                        Name = martyrGraveInformation.Name,
                                        NickName = martyrGraveInformation.NickName,
                                        Position = martyrGraveInformation.Position,
                                        Medal = martyrGraveInformation.Medal,
                                        HomeTown = martyrGraveInformation.HomeTown,
                                        DateOfBirth = martyrGraveInformation.DateOfBirth,
                                        DateOfSacrifice = martyrGraveInformation.DateOfSacrifice
                                    };
                                    await _unitOfWork.MartyrGraveInformationRepository.AddAsync(information);
                                }
                                await _unitOfWork.SaveAsync();
                            }

                            // Thêm hình ảnh GraveImages
                            if (martyrGraveDto.Image.Any())
                            {
                                foreach (var imageDto in martyrGraveDto.Image)
                                {
                                    var graveImage = new GraveImage
                                    {
                                        MartyrId = insertedGrave.MartyrId,
                                        UrlPath = imageDto.UrlPath
                                    };
                                    await _unitOfWork.GraveImageRepository.AddAsync(graveImage);
                                }
                                await _unitOfWork.SaveAsync();
                            }
                        }
                        else
                        {
                            return (false, "Không tìm thấy MartyrGrave đã tạo", null, null);
                        }

                        // Trả về DTO response
                        return (true, "Mộ đã được tạo thành công", null, null);
                    }

                    // Tạo Account mới cho khách hàng nếu không tồn tại
                    var accountMapping = new Account
                    {
                        FullName = martyrGraveDto.Customer.UserName,
                        PhoneNumber = martyrGraveDto.Customer.Phone,
                        Address = martyrGraveDto.Customer.Address,
                        EmailAddress = martyrGraveDto.Customer.EmailAddress,
                        DateOfBirth = martyrGraveDto.Customer.Dob,
                        RoleId = 4,
                        Status = true,
                        CustomerCode = customerCode,
                        AccountName = $"{getLastName(martyrGraveDto.Customer.UserName)}{martyrGraveDto.Customer.Dob.Year}-{martyrGraveDto.Customer.Phone}"
                    };

                    string randomPassword = CreateRandomPassword(8);
                    accountMapping.HashedPassword = await HashPassword(randomPassword);

                    await _unitOfWork.AccountRepository.AddAsync(accountMapping);
                    await _unitOfWork.SaveAsync();

                    var insertedAccount = (await _unitOfWork.AccountRepository.FindAsync(a => a.CustomerCode == customerCode)).FirstOrDefault();
                    if (insertedAccount != null)
                    {
                        // Tạo MartyrGrave
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
                        martyrGrave.CustomerCode = insertedAccount.CustomerCode;
                        martyrGrave.Status = true;

                        // Thêm MartyrGrave vào cơ sở dữ liệu
                        await _unitOfWork.MartyrGraveRepository.AddAsync(martyrGrave);
                        await _unitOfWork.SaveAsync();

                        var insertedGrave = (await _unitOfWork.MartyrGraveRepository.GetAsync(m => m.MartyrCode == martyrGrave.MartyrCode, includeProperties: "MartyrGraveInformations")).FirstOrDefault();

                        if (insertedGrave != null)
                        {
                            // Thêm thông tin MartyrGraveInformations
                            if (martyrGraveDto.Informations.Any())
                            {
                                foreach (var martyrGraveInformation in martyrGraveDto.Informations)
                                {
                                    var information = new MartyrGraveInformation
                                    {
                                        MartyrId = insertedGrave.MartyrId,
                                        Name = martyrGraveInformation.Name,
                                        NickName = martyrGraveInformation.NickName,
                                        Position = martyrGraveInformation.Position,
                                        Medal = martyrGraveInformation.Medal,
                                        HomeTown = martyrGraveInformation.HomeTown,
                                        DateOfBirth = martyrGraveInformation.DateOfBirth,
                                        DateOfSacrifice = martyrGraveInformation.DateOfSacrifice
                                    };
                                    await _unitOfWork.MartyrGraveInformationRepository.AddAsync(information);
                                }
                                await _unitOfWork.SaveAsync();
                            }

                            // Thêm hình ảnh GraveImages
                            if (martyrGraveDto.Image.Any())
                            {
                                foreach (var imageDto in martyrGraveDto.Image)
                                {
                                    var graveImage = new GraveImage
                                    {
                                        MartyrId = insertedGrave.MartyrId,
                                        UrlPath = imageDto.UrlPath
                                    };
                                    await _unitOfWork.GraveImageRepository.AddAsync(graveImage);
                                }
                                await _unitOfWork.SaveAsync();
                            }
                            if (accountMapping.EmailAddress != null)
                            {
                                EmailDTO email = new EmailDTO
                                {
                                    To = accountMapping.EmailAddress,
                                    Subject = "Tài khoản đăng nhập vào phần mềm An Nhiên",
                                    Body = _sendEmailService.emailBodyForMartyrGraveAccount(accountMapping, insertedGrave, randomPassword)
                                };
                                await _sendEmailService.SendEmailMartyrGraveAccount(email);
                            }
                        }
                        else
                        {
                            return (false, "Không tìm thấy MartyrGrave đã tạo", null, null);
                        }

                        // Trả về DTO response với thông tin tài khoản
                        return (true, "Mộ đã được tạo thành công, trả về tài khoản đăng nhập customer", accountMapping.AccountName, randomPassword);
                    }

                    return (false, "Không tìm thấy account đã tạo", null, null);
                }
                else
                {
                    // Tạo thực thể từ DTO
                    var martyrGrave = _mapper.Map<MartyrGrave>(martyrGraveDto);

                    string martyrCode = GenerateMartyrCode(martyrGrave.AreaNumber, martyrGrave.RowNumber, martyrGrave.MartyrNumber);
                    var existedMartyrGrave = (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrCode == martyrCode)).FirstOrDefault();

                    if (existedMartyrGrave != null)
                    {
                        return (false, "MartyrCode đã tồn tại hãy kiểm tra lại", null, null);
                    }

                    // Gọi hàm GenerateMartyrCode để tạo mã MartyrCode
                    martyrGrave.MartyrCode = martyrCode;
                    martyrGrave.Status = true;

                    // Thêm MartyrGrave vào cơ sở dữ liệu
                    await _unitOfWork.MartyrGraveRepository.AddAsync(martyrGrave);
                    await _unitOfWork.SaveAsync();

                    var insertedGrave = (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrCode == martyrGrave.MartyrCode)).FirstOrDefault();

                    if (insertedGrave != null)
                    {
                        // Thêm thông tin MartyrGraveInformations
                        if (martyrGraveDto.Informations.Any())
                        {
                            foreach (var martyrGraveInformation in martyrGraveDto.Informations)
                            {
                                var information = new MartyrGraveInformation
                                {
                                    MartyrId = insertedGrave.MartyrId,
                                    Name = martyrGraveInformation.Name,
                                    NickName = martyrGraveInformation.NickName,
                                    Position = martyrGraveInformation.Position,
                                    Medal = martyrGraveInformation.Medal,
                                    HomeTown = martyrGraveInformation.HomeTown,
                                    DateOfBirth = martyrGraveInformation.DateOfBirth,
                                    DateOfSacrifice = martyrGraveInformation.DateOfSacrifice
                                };
                                await _unitOfWork.MartyrGraveInformationRepository.AddAsync(information);
                            }
                            await _unitOfWork.SaveAsync();
                        }

                        // Thêm hình ảnh GraveImages
                        if (martyrGraveDto.Image.Any())
                        {
                            foreach (var imageDto in martyrGraveDto.Image)
                            {
                                var graveImage = new GraveImage
                                {
                                    MartyrId = insertedGrave.MartyrId,
                                    UrlPath = imageDto.UrlPath
                                };
                                await _unitOfWork.GraveImageRepository.AddAsync(graveImage);
                            }
                            await _unitOfWork.SaveAsync();
                        }
                    }
                    else
                    {
                        return (false, "Không tìm thấy MartyrGrave đã tạo", null, null);
                    }

                    // Trả về DTO response
                    return (true, "Mộ đã được tạo thành công", null, null);
                }
            }
            catch (Exception ex)
            {
                // Rollback các thay đổi khi xảy ra lỗi
                var customerCode = GenerateCustomerCode(martyrGraveDto.Customer.UserName, martyrGraveDto.Customer.Phone);
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

        public async Task<(bool status, string result)> UpdateMartyrGraveAsyncV2(int id, MartyrGraveUpdateDtoRequest martyrGraveDto)
        {
            try
            {
                // Kiểm tra AreaId có tồn tại không
                var area = await _unitOfWork.AreaRepository.GetByIDAsync(martyrGraveDto.AreaId);
                if (area == null)
                {
                    return (false, "Không tìm thấy khu vực");
                }

                // Kiểm tra MartyrGrave có tồn tại không
                var existingGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(id);
                if (existingGrave == null)
                {
                    return (false, "Không tìm thấy MartyrGrave");
                }

                // Cập nhật thông tin MartyrGrave từ DTO
                _mapper.Map(martyrGraveDto, existingGrave);

                // Tạo lại mã MartyrCode
                string martyrCode = GenerateMartyrCode(existingGrave.AreaNumber, existingGrave.RowNumber, existingGrave.MartyrNumber);
                var existedMartyrGrave = (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrCode == martyrCode && m.MartyrId != id)).FirstOrDefault();

                if (existedMartyrGrave != null)
                {
                    return (false, "MartyrCode đã tồn tại, hãy kiểm tra lại");
                }

                existingGrave.MartyrCode = martyrCode;

                // Cập nhật MartyrGrave vào cơ sở dữ liệu
                await _unitOfWork.MartyrGraveRepository.UpdateAsync(existingGrave);
                await _unitOfWork.SaveAsync();

                // Cập nhật các thông tin MartyrGraveInformations
                var existingInformations = await _unitOfWork.MartyrGraveInformationRepository.GetAsync(m => m.MartyrId == existingGrave.MartyrId);

                // Xóa các thông tin cũ
                foreach (var existingInformation in existingInformations)
                {
                    await _unitOfWork.MartyrGraveInformationRepository.DeleteAsync(existingInformation);
                }
                await _unitOfWork.SaveAsync();

                // Thêm lại các thông tin mới từ DTO
                if (martyrGraveDto.Informations.Any())
                {
                    foreach (var martyrGraveInformation in martyrGraveDto.Informations)
                    {
                        var information = new MartyrGraveInformation
                        {
                            MartyrId = existingGrave.MartyrId,
                            Name = martyrGraveInformation.Name,
                            NickName = martyrGraveInformation.NickName,
                            Position = martyrGraveInformation.Position,
                            Medal = martyrGraveInformation.Medal,
                            HomeTown = martyrGraveInformation.HomeTown,
                            DateOfBirth = martyrGraveInformation.DateOfBirth,
                            DateOfSacrifice = martyrGraveInformation.DateOfSacrifice
                        };
                        await _unitOfWork.MartyrGraveInformationRepository.AddAsync(information);
                    }
                    await _unitOfWork.SaveAsync();
                }

                // Cập nhật hình ảnh GraveImages
                var existingImages = await _unitOfWork.GraveImageRepository.GetAsync(i => i.MartyrId == existingGrave.MartyrId);

                // Xóa các ảnh cũ
                foreach (var existingImage in existingImages)
                {
                    await _unitOfWork.GraveImageRepository.DeleteAsync(existingImage);
                }
                await _unitOfWork.SaveAsync();

                // Thêm các ảnh mới
                if (martyrGraveDto.Image.Any())
                {
                    foreach (var imageDto in martyrGraveDto.Image)
                    {
                        var graveImage = new GraveImage
                        {
                            MartyrId = existingGrave.MartyrId,
                            UrlPath = imageDto.UrlPath
                        };
                        await _unitOfWork.GraveImageRepository.AddAsync(graveImage);
                    }
                    await _unitOfWork.SaveAsync();
                }

                return (true, "Mộ đã được cập nhật thành công");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while updating MartyrGrave: {ex.Message}", ex);
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

        public async Task<(bool status, string result, string? accountName, string? password)> CreateRelativeGraveAsync(int martyrGraveId, CustomerDtoRequest customer)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var existedGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(martyrGraveId);
                    if (existedGrave != null)
                    {
                        if (existedGrave.CustomerCode == null)
                        {
                            var customerCode = GenerateCustomerCode(customer.UserName, customer.Phone);
                            var existedCustomerCode = (await _unitOfWork.AccountRepository.FindAsync(c => c.CustomerCode == customerCode)).FirstOrDefault();
                            if (existedCustomerCode == null)
                            {
                                var accountMapping = new Account
                                {
                                    FullName = customer.UserName,
                                    PhoneNumber = customer.Phone,
                                    Address = customer.Address,
                                    EmailAddress = customer.EmailAddress,
                                    DateOfBirth = customer.Dob,
                                    RoleId = 4,
                                    Status = true,
                                    CustomerCode = customerCode,
                                    AccountName = $"{getLastName(customer.UserName)}{customer.Dob.Year}-{customer.Phone}"
                                };

                                string randomPassword = CreateRandomPassword(8);
                                accountMapping.HashedPassword = await HashPassword(randomPassword);

                                await _unitOfWork.AccountRepository.AddAsync(accountMapping);
                                await _unitOfWork.SaveAsync();

                                var insertedAccount = (await _unitOfWork.AccountRepository.FindAsync(a => a.CustomerCode == customerCode)).FirstOrDefault();
                                if (insertedAccount != null)
                                {
                                    existedGrave.CustomerCode = customerCode;
                                    await _unitOfWork.MartyrGraveRepository.UpdateAsync(existedGrave);
                                    await _unitOfWork.SaveAsync();
                                    if (accountMapping.EmailAddress != null)
                                    {
                                        EmailDTO email = new EmailDTO
                                        {
                                            To = accountMapping.EmailAddress,
                                            Subject = "Tài khoản đăng nhập vào phần mềm An Nhiên",
                                            Body = _sendEmailService.emailBodyForUpdateMartyrGraveAccount(accountMapping, existedGrave, randomPassword)
                                        };
                                        await _sendEmailService.SendEmailMartyrGraveAccount(email);
                                    }
                                    await transaction.CommitAsync();
                                    return (true, "Đã cập nhật thân nhân mộ thành công", accountMapping.AccountName, randomPassword);
                                }
                                else
                                {
                                    return (false, "Không tìm thấy customer đã tạo", null, null);
                                }
                            }
                            else
                            {
                                existedGrave.CustomerCode = customerCode;
                                await _unitOfWork.MartyrGraveRepository.UpdateAsync(existedGrave);
                                await _unitOfWork.SaveAsync();
                                await transaction.CommitAsync();
                                return (false, "Đã cập nhật thân nhân mộ thành công", null, null);
                            }
                        }
                        else
                        {
                            return (false, "Mộ này đã có người thân", null, null);
                        }
                    }
                    else
                    {
                        return (false, "Không tìm thấy mộ", null, null);
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<List<MartyrGraveDtoResponse>> GetMartyrGraveByCustomerCode(string customerCode)
        {
            try
            {
                List<MartyrGraveDtoResponse> graveList = new List<MartyrGraveDtoResponse>();
                var graves = await _unitOfWork.MartyrGraveRepository.GetAsync(g => g.CustomerCode == customerCode);
                if (graves.Any())
                {
                    foreach (var grave in graves)
                    {
                        var graveView = _mapper.Map<MartyrGraveDtoResponse>(grave);
                        var graveInformations = await _unitOfWork.MartyrGraveInformationRepository.GetAsync(g => g.MartyrId == grave.MartyrId);
                        if (graveInformations.Any())
                        {
                            foreach (var information in graveInformations)
                            {
                                var informationView = new MartyrGraveInformationDtoResponse
                                {
                                    InformationId = information.InformationId,
                                    MartyrId = information.MartyrId,
                                    Name = information.Name,
                                    NickName = information.NickName,
                                    Position = information.Position,
                                    Medal = information.Medal,
                                    HomeTown = information.HomeTown,
                                    DateOfBirth = information.DateOfBirth,
                                    DateOfSacrifice = information.DateOfSacrifice
                                };
                                graveView.MatyrGraveInformations.Add(informationView);
                            }
                        }

                        var graveImages = await _unitOfWork.GraveImageRepository.GetAsync(g => g.MartyrId == grave.MartyrId);
                        if (graveImages.Any())
                        {
                            foreach (var image in graveImages)
                            {
                                var imageView = new GraveImageDtoRequest
                                {
                                    UrlPath = image.UrlPath
                                };
                                graveView.Images.Add(imageView);
                            }
                        }
                        graveList.Add(graveView);
                    }
                }

                return graveList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<List<MartyrGraveSearchDtoResponse>> SearchMartyrGravesAsync(MartyrGraveSearchDtoRequest searchCriteria)
        {
            // Tạo điều kiện tìm kiếm linh hoạt
            Expression<Func<MartyrGraveInformation, bool>> filter = m =>
                (string.IsNullOrEmpty(searchCriteria.Name) || m.Name.Contains(searchCriteria.Name)) &&
                (!searchCriteria.YearOfBirth.HasValue || m.DateOfBirth.HasValue && m.DateOfBirth.Value.Year == searchCriteria.YearOfBirth.Value) &&
                (!searchCriteria.YearOfSacrifice.HasValue || m.DateOfSacrifice.Year == searchCriteria.YearOfSacrifice.Value) &&
                (string.IsNullOrEmpty(searchCriteria.HomeTown) || m.HomeTown.Contains(searchCriteria.HomeTown));

            // Sử dụng GenericRepository để lấy danh sách MartyrGraveInformation
            var martyrGraves = await _unitOfWork.MartyrGraveInformationRepository.GetAsync(filter, includeProperties: "MartyrGrave");

            // Ánh xạ từ entity sang DTO
            var result = martyrGraves.Select(m => new MartyrGraveSearchDtoResponse
            {
                MartyrId = m.MartyrId,
                Name = m.Name,
                NickName = m.NickName,
                HomeTown = m.HomeTown,
                DateOfBirth = m.DateOfBirth,
                DateOfSacrifice = m.DateOfSacrifice,
                MartyrCode = m.MartyrGrave?.MartyrCode
            }).ToList();

            return result;
        }


    }
}
