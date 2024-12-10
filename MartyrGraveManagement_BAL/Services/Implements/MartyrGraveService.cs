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
using OfficeOpenXml;
using Microsoft.VisualBasic;

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

        //public async Task<MartyrGraveDtoResponse> CreateMartyrGraveAsync(MartyrGraveDtoRequest martyrGraveDto)
        //{//
        //    // Kiểm tra AreaId có tồn tại không
        //    var area = await _unitOfWork.AreaRepository.GetByIDAsync(martyrGraveDto.AreaId);
        //    if (area == null)
        //    {
        //        throw new KeyNotFoundException("AreaId does not exist.");
        //    }

        //    // Tạo thực thể từ DTO
        //    var martyrGrave = _mapper.Map<MartyrGrave>(martyrGraveDto);

        //    // Gọi hàm GenerateMartyrCode để tạo mã MartyrCode
        //    //martyrGrave.MartyrCode = GenerateMartyrCode(martyrGrave.AreaNumber, martyrGrave.RowNumber, martyrGrave.MartyrNumber);

        //    // Thêm MartyrGrave vào cơ sở dữ liệu
        //    await _unitOfWork.MartyrGraveRepository.AddAsync(martyrGrave);
        //    await _unitOfWork.SaveAsync();

        //    // Trả về DTO response
        //    return _mapper.Map<MartyrGraveDtoResponse>(martyrGrave);
        //}

        //public async Task<MartyrGraveDtoResponse> UpdateMartyrGraveAsync(int id, MartyrGraveDtoRequest martyrGraveDto)
        //{//
        //    // Kiểm tra AreaId có tồn tại không
        //    var area = await _unitOfWork.AreaRepository.GetByIDAsync(martyrGraveDto.AreaId);
        //    if (area == null)
        //    {
        //        throw new KeyNotFoundException("AreaId does not exist.");
        //    }

        //    var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(id);
        //    if (martyrGrave == null)
        //    {
        //        return null;
        //    }

        //    // Cập nhật các thuộc tính từ DTO sang thực thể
        //    _mapper.Map(martyrGraveDto, martyrGrave);

        //    // Tạo lại MartyrCode dựa trên các thông tin mới
        //    //martyrGrave.MartyrCode = GenerateMartyrCode(martyrGrave.AreaNumber, martyrGrave.RowNumber, martyrGrave.MartyrNumber);

        //    // Cập nhật thông tin vào cơ sở dữ liệu
        //    await _unitOfWork.MartyrGraveRepository.UpdateAsync(martyrGrave);
        //    await _unitOfWork.SaveAsync();

        //    // Trả về kết quả cập nhật
        //    return _mapper.Map<MartyrGraveDtoResponse>(martyrGrave);
        //}

        public async Task<List<MartyrGraveSearchDtoResponse>> SearchMartyrGravesAsync(
    MartyrGraveSearchDtoRequest searchCriteria, int pageIndex, int pageSize)
        {
            // Server-side filter for basic conditions
            Expression<Func<MartyrGraveInformation, bool>> filter = m =>
                (searchCriteria.YearOfBirth == null || m.DateOfBirth != null && m.DateOfBirth.Contains(searchCriteria.YearOfBirth)) &&
                (searchCriteria.YearOfSacrifice == null || m.DateOfSacrifice != null && m.DateOfSacrifice.Contains(searchCriteria.YearOfSacrifice)) &&
                (string.IsNullOrEmpty(searchCriteria.HomeTown) || m.HomeTown != null);

            // Fetch filtered and paginated data from the database
            var martyrGraves = await _unitOfWork.MartyrGraveInformationRepository
                .GetAllAsync(filter: filter, includeProperties: "MartyrGrave", pageIndex: pageIndex, pageSize: pageSize);

            // Apply client-side filtering for unaccented names and hometowns
            string unaccentedSearchName = string.IsNullOrEmpty(searchCriteria.Name)
                ? string.Empty
                : ConvertToUnaccentedLowercaseString(searchCriteria.Name);

            string unaccentedHomeTown = string.IsNullOrEmpty(searchCriteria.HomeTown)
                ? string.Empty
                : ConvertToUnaccentedLowercaseString(searchCriteria.HomeTown);

            var filteredMartyrGraves = martyrGraves
                .AsEnumerable() // Switch to client-side evaluation
                .Where(m =>
                    (string.IsNullOrEmpty(searchCriteria.Name) || ConvertToUnaccentedLowercaseString(m.Name).Contains(unaccentedSearchName)) &&
                    (string.IsNullOrEmpty(searchCriteria.HomeTown) || ConvertToUnaccentedLowercaseString(m.HomeTown).Contains(unaccentedHomeTown)));

            // Map results to DTOs
            var result = new List<MartyrGraveSearchDtoResponse>();
            foreach (var m in filteredMartyrGraves)
            {
                var graveDto = new MartyrGraveSearchDtoResponse
                {
                    MartyrId = m.MartyrId,
                    Name = m.Name,
                    NickName = m.NickName,
                    HomeTown = m.HomeTown,
                    DateOfBirth = m.DateOfBirth,
                    DateOfSacrifice = m.DateOfSacrifice,
                    MartyrCode = m.MartyrGrave?.MartyrCode,
                    ImageUrls = new List<GraveImageDtoResponse>() // Initialize image URLs list
                };

                // Fetch associated images
                var graveImages = await _unitOfWork.GraveImageRepository
                    .GetAllAsync(g => g.MartyrId == m.MartyrId);

                foreach (var image in graveImages)
                {
                    if (!string.IsNullOrEmpty(image.UrlPath))
                    {
                        graveDto.ImageUrls.Add(new GraveImageDtoResponse
                        {
                            Image = image.UrlPath
                        });
                    }
                }

                result.Add(graveDto);
            }

            return result;
        }



        public async Task<(List<MartyrGraveGetAllDtoResponse> matyrGraveList, int totalPage)> GetAllMartyrGravesAsync(int page, int pageSize)
        {
            try
            {
                var totalMatyrGrave = await _unitOfWork.MartyrGraveRepository.CountAsync();
                var totalPage = (int)Math.Ceiling(totalMatyrGrave / (double)pageSize);
                List<MartyrGraveGetAllDtoResponse> graveList = new List<MartyrGraveGetAllDtoResponse>();
                var graves = await _unitOfWork.MartyrGraveRepository.GetAllAsync(includeProperties: "MartyrGraveInformations", pageIndex: page, pageSize: pageSize);
                if (graves.Any())
                {
                    foreach (var grave in graves)
                    {
                        var graveView = _mapper.Map<MartyrGraveGetAllDtoResponse>(grave);
                        var graveInformations = await _unitOfWork.MartyrGraveInformationRepository.GetAsync(g => g.MartyrId == grave.MartyrId);
                        if (graveInformations.Any())
                        {
                            foreach (var information in graveInformations)
                            {
                                string martyrName = information.Name;
                                graveView.Name.Add(martyrName);
                            }
                        }

                        var graveImage = (await _unitOfWork.GraveImageRepository.GetAsync(g => g.MartyrId == grave.MartyrId)).FirstOrDefault();
                        if (graveImage != null)
                        {
                            graveView.image = graveImage.UrlPath;
                        }
                        graveList.Add(graveView);
                    }
                }

                return (graveList, totalPage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<MartyrGraveDtoResponse> GetMartyrGraveByIdAsync(int id)
        {
            //var grave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(id);
            //return _mapper.Map<MartyrGraveDtoResponse>(grave);
            try
            {
                var grave = (await _unitOfWork.MartyrGraveRepository.GetAsync(g => g.MartyrId == id, includeProperties: "MartyrGraveInformations,Location,Account,Area")).FirstOrDefault();
                if (grave != null)
                {
                    var graveView = _mapper.Map<MartyrGraveDtoResponse>(grave);
                    graveView.AreaName = grave.Area.AreaName;
                    graveView.AreaNumber = grave.Location.AreaNumber;
                    graveView.RowNumber = grave.Location.RowNumber;
                    graveView.MartyrNumber = grave.Location.MartyrNumber;
                    graveView.CustomerName = grave.Account.FullName ?? "Unknown";
                    graveView.CustomerEmail = grave.Account.EmailAddress ?? "Unknown";
                    graveView.CustomerPhone = grave.Account.PhoneNumber ?? "Unknown";
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

                    return graveView;
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

        public async Task<bool> UpdateStatusMartyrGraveAsync(int id, int status)
        {
            try
            {
                var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(id);
                if (martyrGrave == null)
                {
                    return false;
                }
                if (status < 1 || status > 3)
                {
                    return false;
                }
                martyrGrave.Status = status;
                await _unitOfWork.MartyrGraveRepository.UpdateAsync(martyrGrave);
                await _unitOfWork.SaveAsync();
                return true;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        public async Task<(List<MartyrGraveGetAllForAdminDtoResponse> response, int totalPage)> GetAllMartyrGravesForManagerAsync(int page, int pageSize, int managerId)
        {
            try
            {
                var manager = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
                // Tính tổng số mộ liệt sĩ
                var totalMartyrGraves = (await _unitOfWork.MartyrGraveRepository.GetAsync(t => t.AreaId == manager.AreaId)).Count();

                // Tính toán tổng số trang
                var totalPage = (int)Math.Ceiling(totalMartyrGraves / (double)pageSize);



                // Lấy dữ liệu mộ liệt sĩ với phân trang
                var martyrGraves = await _unitOfWork.MartyrGraveRepository.GetAsync(g => g.AreaId == manager.AreaId,
                    includeProperties: "MartyrGraveInformations,Account,Location,Area,GraveImages",
                    pageIndex: page,
                    pageSize: pageSize
                );

                // Khởi tạo danh sách kết quả
                List<MartyrGraveGetAllForAdminDtoResponse> martyrGraveList = new List<MartyrGraveGetAllForAdminDtoResponse>();

                // Duyệt qua tất cả các MartyrGrave đã lấy
                foreach (var m in martyrGraves)
                {
                    // Tìm Account dựa trên CustomerCode (Account có thể là người thân)
                    //var customer = (await _unitOfWork.AccountRepository.FindAsync(a => a.AccountId == m.AccountId)).FirstOrDefault();

                    // Nếu tìm thấy Account thì thêm vào danh sách kết quả

                    var mapping = new MartyrGraveGetAllForAdminDtoResponse
                    {
                         martyrId = m.MartyrId,
                         Code = m.MartyrCode,
                         Name = m.MartyrGraveInformations.FirstOrDefault()?.Name, // Lấy tên từ MartyrGraveInformation
                         martyrCode = m.MartyrCode,
                         AreaDescription = m.Area.Description,
                         GraveImage = m.GraveImages.FirstOrDefault()?.UrlPath ?? "Unknown",
                         Location = $"{m.Location.AreaNumber}-{m.Location.RowNumber}-{m.Location.MartyrNumber}", // Định dạng vị trí
                         RelativeName = m.Account?.FullName, // Lấy tên người thân từ Account
                         RelativePhone = m.Account?.PhoneNumber, // Lấy số điện thoại người thân từ Account
                         Status = m.Status // Lấy trạng thái của MartyrGrave
                    };
                    martyrGraveList.Add(mapping); // Thêm kết quả vào danh sách

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


        public async Task<(bool status, string result, string? phone, string? password)> CreateMartyrGraveAsyncV2(MartyrGraveDtoRequest martyrGraveDto)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Kiểm tra AreaId có tồn tại không
                    var area = (await _unitOfWork.AreaRepository.GetAsync(a => a.AreaNumber == martyrGraveDto.AreaNumber)).FirstOrDefault();

                    var location = (await _unitOfWork.LocationRepository.GetAsync(l => l.AreaNumber == martyrGraveDto.AreaNumber && l.MartyrNumber == martyrGraveDto.MartyrNumber && l.RowNumber == martyrGraveDto.RowNumber)).FirstOrDefault();
                    if (area == null || location == null)
                    {
                        return (false, "Khu vực hoặc vị trí không tìm thấy", null, null);
                    }

                    if (martyrGraveDto.Customer.UserName != null && martyrGraveDto.Customer.Phone != null)
                    {
                        var customerCode = GenerateCustomerCode(martyrGraveDto.Customer.UserName, martyrGraveDto.Customer.Phone);
                        var existedCustomer = (await _unitOfWork.AccountRepository.FindAsync(c => c.PhoneNumber == martyrGraveDto.Customer.Phone)).FirstOrDefault();

                        if (existedCustomer != null)
                        {
                            existedCustomer.CustomerCode = customerCode;
                            await _unitOfWork.AccountRepository.UpdateAsync(existedCustomer);
                            await _unitOfWork.SaveAsync();
                            // Tạo thực thể từ DTO
                            var martyrGrave = new MartyrGrave();

                            string martyrCode = GenerateMartyrCode(location.AreaNumber, location.RowNumber, location.MartyrNumber);
                            var existedMartyrGrave = (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrCode == martyrCode)).FirstOrDefault();

                            if (existedMartyrGrave != null)
                            {
                                return (false, "MartyrCode đã tồn tại hãy kiểm tra lại", null, null);
                            }

                            // Gọi hàm GenerateMartyrCode để tạo mã MartyrCode
                            martyrGrave.AreaId = area.AreaId;
                            martyrGrave.LocationId = location.LocationId;
                            martyrGrave.MartyrCode = martyrCode;
                            martyrGrave.Status = 1; //Trạng thái 1 là mộ trạng thái đang tốt, 2 là khá, 3 là xuống cấp
                            martyrGrave.AccountId = existedCustomer.AccountId;

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
                            await transaction.CommitAsync();
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
                            CreateAt = DateTime.Now
                        };

                        string randomPassword = CreateRandomPassword(8);
                        accountMapping.HashedPassword = await HashPassword(randomPassword);

                        await _unitOfWork.AccountRepository.AddAsync(accountMapping);
                        await _unitOfWork.SaveAsync();

                        var insertedAccount = (await _unitOfWork.AccountRepository.FindAsync(a => a.CustomerCode == customerCode)).FirstOrDefault();
                        if (insertedAccount != null)
                        {
                            // Tạo MartyrGrave
                            var martyrGrave = new MartyrGrave();
                            string martyrCode = GenerateMartyrCode(location.AreaNumber, location.RowNumber, location.MartyrNumber);

                            var existedMartyrGrave = (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrCode == martyrCode)).FirstOrDefault();
                            if (existedMartyrGrave != null)
                            {
                                return (false, "MartyrCode đã tồn tại hãy kiểm tra lại", null, null);
                            }

                            // Gọi hàm GenerateMartyrCode để tạo mã MartyrCode
                            martyrGrave.AreaId = area.AreaId;
                            martyrGrave.LocationId = location.LocationId;
                            martyrGrave.MartyrCode = martyrCode;
                            martyrGrave.Status = 1;
                            martyrGrave.AccountId = insertedAccount.AccountId;

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
                            await transaction.CommitAsync();
                            // Trả về DTO response với thông tin tài khoản
                            return (true, "Mộ đã được tạo thành công, trả về tài khoản đăng nhập customer", accountMapping.PhoneNumber, randomPassword);
                        }

                        return (false, "Không tìm thấy account đã tạo", null, null);
                    }
                    else
                    {
                        return (false, "Cần phải nhập data của Customer, bắt buộc có SĐT và UserName", null, null);
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<(bool status, string result)> UpdateMartyrGraveAsyncV2(int id, MartyrGraveUpdateDtoRequest martyrGraveDto)
        {//
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {

                    // Kiểm tra MartyrGrave có tồn tại không
                    var existingGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(id);
                    if (existingGrave == null)
                    {
                        return (false, "Không tìm thấy MartyrGrave");
                    }



                    // Cập nhật các thông tin MartyrGraveInformations
                    var existingInformations = await _unitOfWork.MartyrGraveInformationRepository.GetAsync(m => m.MartyrId == existingGrave.MartyrId);

                    // Xóa các thông tin cũ
                    foreach (var existingInformation in existingInformations)
                    {
                        if (martyrGraveDto.Informations.Any())
                        {
                            foreach (var martyrGraveInformation in martyrGraveDto.Informations)
                            {
                                existingInformation.Name = martyrGraveInformation.Name;
                                existingInformation.NickName = martyrGraveInformation.NickName;
                                existingInformation.Position = martyrGraveInformation.Position;
                                existingInformation.Medal = martyrGraveInformation.Medal;
                                existingInformation.HomeTown = martyrGraveInformation.HomeTown;
                                existingInformation.DateOfBirth = martyrGraveInformation.DateOfBirth;
                                existingInformation.DateOfSacrifice = martyrGraveInformation.DateOfSacrifice;

                                await _unitOfWork.MartyrGraveInformationRepository.UpdateAsync(existingInformation);
                                break;
                            }
                        }
                        //await _unitOfWork.MartyrGraveInformationRepository.DeleteAsync(existingInformation);
                    }
                    await _unitOfWork.SaveAsync();

                    // Thêm lại các thông tin mới từ DTO
                    

                    

                    // Thêm các ảnh mới
                    if (martyrGraveDto.Image.Any())
                    {
                        // Cập nhật hình ảnh GraveImages
                        var existingImages = await _unitOfWork.GraveImageRepository.GetAsync(i => i.MartyrId == existingGrave.MartyrId);

                        // Xóa các ảnh cũ
                        foreach (var existingImage in existingImages)
                        {
                            await _unitOfWork.GraveImageRepository.DeleteAsync(existingImage);
                        }
                        await _unitOfWork.SaveAsync();
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
                    await transaction.CommitAsync();
                    return (true, "Mộ đã được cập nhật thành công");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Error while updating MartyrGrave: {ex.Message}", ex);
                }
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

        

        public async Task<List<MartyrGraveDtoResponse>> GetMartyrGraveByCustomerId(int customerId)
        {
            try
            {
                List<MartyrGraveDtoResponse> graveList = new List<MartyrGraveDtoResponse>();
                var graves = await _unitOfWork.MartyrGraveRepository.GetAsync(g => g.AccountId == customerId);
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



        

        private string ConvertToUnaccentedLowercaseString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var normalizedString = input.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }

        private async Task<(bool status, string result, List<(string? phone, string? password)> outputList)> CreateMartyrGraveListAsyncV3(List<MartyrGraveDtoRequest> martyrGraveList)
        {
            var outputList = new List<(string? phone, string? password)>();

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    foreach (var martyrGraveDto in martyrGraveList)
                    {
                        // Perform validation and retrieve Area and Location
                        var area = (await _unitOfWork.AreaRepository.GetAsync(a => a.AreaNumber == martyrGraveDto.AreaNumber)).FirstOrDefault();
                        var location = (await _unitOfWork.LocationRepository.GetAsync(l =>
                            l.AreaNumber == martyrGraveDto.AreaNumber &&
                            l.RowNumber == martyrGraveDto.RowNumber &&
                            l.MartyrNumber == martyrGraveDto.MartyrNumber)).FirstOrDefault();

                        if (area == null || location == null)
                        {
                            // Log issue or add error handling for missing Area/Location
                            continue;
                        }

                        var customerPhone = martyrGraveDto.Customer.Phone;
                        var customerUserName = martyrGraveDto.Customer.UserName;
                        if (string.IsNullOrEmpty(customerPhone) || string.IsNullOrEmpty(customerUserName))
                        {
                            // Log issue or skip record if mandatory customer data is missing
                            continue;
                        }

                        var customerCode = GenerateCustomerCode(customerUserName, customerPhone);
                        var existingCustomer = (await _unitOfWork.AccountRepository.FindAsync(c => c.PhoneNumber == customerPhone)).FirstOrDefault();
                        string? password = null;

                        // If customer does not exist, create new account
                        if (existingCustomer == null)
                        {
                            // Generate a random password
                            password = CreateRandomPassword(8);

                            var account = new Account
                            {
                                FullName = customerUserName,
                                PhoneNumber = customerPhone,
                                Address = martyrGraveDto.Customer.Address,
                                EmailAddress = martyrGraveDto.Customer.EmailAddress,
                                DateOfBirth = martyrGraveDto.Customer.Dob,
                                RoleId = 4,
                                Status = true,
                                CustomerCode = customerCode,
                                CreateAt = DateTime.Now,
                                HashedPassword = await HashPassword(password)
                            };

                            await _unitOfWork.AccountRepository.AddAsync(account);
                            await _unitOfWork.SaveAsync();
                            existingCustomer = account;
                        }

                        // Proceed to add MartyrGrave
                        var martyrCode = GenerateMartyrCode(location.AreaNumber, location.RowNumber, location.MartyrNumber);
                        if ((await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrCode == martyrCode)).Any())
                        {
                            // Log or skip if martyr code already exists
                            continue;
                        }

                        var martyrGrave = new MartyrGrave
                        {
                            AreaId = area.AreaId,
                            LocationId = location.LocationId,
                            MartyrCode = martyrCode,
                            Status = 1,
                            AccountId = existingCustomer.AccountId
                        };

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


                        // Save details for output and email notifications
                        outputList.Add((existingCustomer.PhoneNumber, password));

                        // Optionally send emails here or collect emails to send after batch processing
                    }

                    await transaction.CommitAsync();
                    return (true, "All records processed successfully", outputList);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Error during bulk insertion: " + ex.Message);
                }
            }
        }




        public async Task<(bool status, string message)> ImportMartyrGraves(string excelFilePath, string filePath)
        {
            var outputList = new List<(string PhoneNumber, string Password)>();
            var martyrGraveList = new List<MartyrGraveDtoRequest>();

            // Set ExcelPackage License
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first sheet

                    // Loop through the rows and create MartyrGraveDtoRequest objects
                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++) // Start at row 2 to skip headers
                    {
                        // Map data from the Excel sheet to the DTO
                        var martyrGraveDto = new MartyrGraveDtoRequest
                        {
                            AreaNumber = int.Parse(worksheet.Cells[row, 8].Text),
                            RowNumber = int.Parse(worksheet.Cells[row, 9].Text),
                            MartyrNumber = int.Parse(worksheet.Cells[row, 10].Text),
                            Customer = new CustomerDtoRequest
                            {
                                UserName = worksheet.Cells[row, 11].Text,
                                Phone = worksheet.Cells[row, 12].Text,
                                Address = worksheet.Cells[row, 13].Text,
                                Dob = worksheet.Cells[row, 14].Text
                            },
                            Informations = new List<MartyrGraveInformationDtoRequest>
                            {
                                new MartyrGraveInformationDtoRequest
                                {
                                    Name = worksheet.Cells[row, 1].Text,
                                    NickName = worksheet.Cells[row, 2].Text,
                                    Position = worksheet.Cells[row, 3].Text,
                                    Medal = worksheet.Cells[row, 4].Text,
                                    HomeTown = worksheet.Cells[row, 5].Text,
                                    DateOfBirth = worksheet.Cells[row, 6].Text,
                                    DateOfSacrifice = worksheet.Cells[row, 7].Text
                                }
                            },
                        };

                        martyrGraveList.Add(martyrGraveDto);
                    }

                    // Now pass the entire list to CreateMartyrGraveAsyncV3 in one batch
                    var (status, result, phonePasswordPairs) = await CreateMartyrGraveListAsyncV3(martyrGraveList);

                    if (status)
                    {
                        outputList.AddRange(phonePasswordPairs);
                        // Commit the transaction after processing all records
                        await _unitOfWork.SaveAsync();
                    }
                    else
                    {
                        // Handle failure if needed
                        return (false, "Failed to process all martyr graves: " + result);
                    }
                }

                // Export the phone and password data to an output Excel file
                var outputFilePath = ExportPhoneAndPasswordsToExcel(outputList, filePath);
                return (true, $"Data processed successfully. Exported to {outputFilePath}");
            }
            catch (Exception ex)
            {
                return (false, "Error reading or processing the Excel file: " + ex.Message);
            }
        }


        private string ExportPhoneAndPasswordsToExcel(List<(string PhoneNumber, string Password)> phonePasswordList, string filePath)
        {
            var outputFilePath = filePath; 

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Accounts");

                // Define headers for the export file
                worksheet.Cells[1, 1].Value = "PhoneNumber";
                worksheet.Cells[1, 2].Value = "Password";

                // Populate the worksheet with phone and password data
                for (int i = 0; i < phonePasswordList.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = phonePasswordList[i].PhoneNumber;
                    worksheet.Cells[i + 2, 2].Value = phonePasswordList[i].Password;
                }

                // Save the file to the specified output path
                package.SaveAs(new FileInfo(outputFilePath));
            }

            return outputFilePath;
        }

        public async Task<(List<MartyrGraveByAreaDtoResponse> martyrGraves, int totalPage)> GetMartyrGraveByAreaIdAsync(int areaId, int pageIndex, int pageSize)
        {
            try
            {
                var totalMartyrGraves = (await _unitOfWork.MartyrGraveRepository.GetAsync(g => g.AreaId == areaId)).Count();
                var totalPage = (int)Math.Ceiling(totalMartyrGraves / (double)pageSize);

                var martyrGraves = await _unitOfWork.MartyrGraveRepository.GetAsync(
                    filter: g => g.AreaId == areaId,
                    includeProperties: "MartyrGraveInformations,GraveImages,Location,Area",
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );

                var martyrGraveList = new List<MartyrGraveByAreaDtoResponse>();

                foreach (var grave in martyrGraves)
                {
                    var graveDto = new MartyrGraveByAreaDtoResponse
                    {
                        MartyrId = grave.MartyrId,
                        MartyrCode = grave.MartyrCode,
                        Status = grave.Status,
                        AreaName = grave.Area?.AreaName ?? "Unknown",
                        LocationDescription = $"{grave.Location?.AreaNumber}-{grave.Location?.RowNumber}-{grave.Location?.MartyrNumber}",
                        Images = grave.GraveImages?.Select(img => new GraveImageDtoResponse { Image = img.UrlPath }).ToList(),
                        MatyrGraveInformations = grave.MartyrGraveInformations?.Select(info => new MartyrGraveDTOsInformationDtoResponse
                        {
                            InformationId = info.InformationId,
                            Name = info.Name,
                            NickName = info.NickName,
                            Position = info.Position,
                            Medal = info.Medal,
                            HomeTown = info.HomeTown,
                            DateOfBirth = info.DateOfBirth,
                            DateOfSacrifice = info.DateOfSacrifice
                        }).ToList()
                    };

                    martyrGraveList.Add(graveDto);
                }

                return (martyrGraveList, totalPage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while fetching martyr graves by area: {ex.Message}", ex);
            }
        }



    }
}
