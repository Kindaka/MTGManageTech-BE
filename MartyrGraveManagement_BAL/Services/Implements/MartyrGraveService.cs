using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.CustomerDTOs;
using MartyrGraveManagement_BAL.ModelViews.EmailDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveInformationDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using OfficeOpenXml;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

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

        public async Task<(List<MartyrGraveSearchDtoResponse> martyrGraves, int totalPage)> SearchMartyrGravesAsync(
            MartyrGraveSearchDtoRequest searchCriteria, int page = 1, int pageSize = 15)
        {
            try
            {
                // Lấy tất cả dữ liệu cần thiết trước
                var query = await _unitOfWork.MartyrGraveInformationRepository
                    .GetAllAsync(
                        includeProperties: "MartyrGrave"
                    );

                // Thực hiện filter trong memory
                var filteredQuery = query.AsEnumerable();

                if (!string.IsNullOrEmpty(searchCriteria.Name))
                {
                    string searchName = RemoveDiacritics(searchCriteria.Name.ToLower());
                    filteredQuery = filteredQuery.Where(x =>
                        x.Name != null &&
                        RemoveDiacritics(x.Name.ToLower()).Contains(searchName));
                }

                if (!string.IsNullOrEmpty(searchCriteria.HomeTown))
                {
                    string searchHomeTown = RemoveDiacritics(searchCriteria.HomeTown.ToLower());
                    filteredQuery = filteredQuery.Where(x =>
                        x.HomeTown != null &&
                        RemoveDiacritics(x.HomeTown.ToLower()).Contains(searchHomeTown));
                }

                if (!string.IsNullOrEmpty(searchCriteria.YearOfBirth))
                {
                    filteredQuery = filteredQuery.Where(x =>
                        x.DateOfBirth != null &&
                        x.DateOfBirth.Contains(searchCriteria.YearOfBirth));
                }

                if (!string.IsNullOrEmpty(searchCriteria.YearOfSacrifice))
                {
                    filteredQuery = filteredQuery.Where(x =>
                        x.DateOfSacrifice != null &&
                        x.DateOfSacrifice.Contains(searchCriteria.YearOfSacrifice));
                }

                // Thêm tìm kiếm theo MartyrCode
                if (!string.IsNullOrEmpty(searchCriteria.MartyrCode))
                {
                    string searchCode = searchCriteria.MartyrCode.ToLower();
                    filteredQuery = filteredQuery.Where(x =>
                        x.MartyrGrave != null &&
                        x.MartyrGrave.MartyrCode.ToLower().Contains(searchCode));
                }

                // Tính toán phân trang
                var totalRecords = filteredQuery.Count();
                var totalPage = (int)Math.Ceiling(totalRecords / (double)pageSize);

                // Áp dụng phân trang
                var martyrGraves = filteredQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Chuyển đổi sang DTO và lấy images
                var dtoList = new List<MartyrGraveSearchDtoResponse>();
                foreach (var m in martyrGraves)
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
                        ImageUrls = new List<GraveImageDtoResponse>()
                    };

                    // Lấy images cho từng martyr
                    var graveImages = await _unitOfWork.GraveImageRepository
                        .GetAllAsync(g => g.MartyrId == m.MartyrId);

                    graveDto.ImageUrls = graveImages
                        .Where(i => !string.IsNullOrEmpty(i.UrlPath))
                        .Select(i => new GraveImageDtoResponse
                        {
                            Image = i.UrlPath
                        })
                        .ToList();

                    dtoList.Add(graveDto);
                }

                return (dtoList, totalPage);
            }
            catch (Exception ex)
            {
                // Log exception
                throw;
            }
        }

        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
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

                    return await System.Threading.Tasks.Task.FromResult(stringBuilder.ToString());
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
                var graves = await _unitOfWork.MartyrGraveRepository.GetAsync(g => g.AccountId == customerId, includeProperties: "Location");
                if (graves.Any())
                {
                    foreach (var grave in graves)
                    {
                        var graveView = _mapper.Map<MartyrGraveDtoResponse>(grave);
                        if (grave.Location != null)
                        {
                            graveView.RowNumber = grave.Location.RowNumber;
                            graveView.MartyrNumber = grave.Location.MartyrNumber;
                            graveView.AreaNumber = grave.Location.AreaNumber;
                        }

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

        private async Task<(bool status, string result, List<(string? phone, string? password)> outputList)> CreateMartyrGraveListAsyncV4(List<MartyrGraveDtoRequest> martyrGraveList)
        {
            var outputList = new List<(string? phone, string? password)>();
            var newAccounts = new List<Account>();
            var newMartyrGraves = new List<MartyrGrave>();
            var newMartyrGraveInfos = new List<MartyrGraveInformation>();
            var newGraveImages = new List<GraveImage>();

            // Pre-fetch areas, locations, and existing customers
            var areaNumbers = martyrGraveList.Select(m => m.AreaNumber).Distinct().ToList();
            var locationKeys = martyrGraveList
                .Select(m => (m.AreaNumber, m.RowNumber, m.MartyrNumber))
                .Distinct().ToList();
            var customerPhones = martyrGraveList.Select(m => m.Customer.Phone).Distinct().ToList();

            var areas = (await _unitOfWork.AreaRepository.GetAsync(a => areaNumbers.Contains(a.AreaNumber)))
                .ToDictionary(a => a.AreaNumber);

            var locations = (await _unitOfWork.LocationRepository.GetAsync(l =>
                areaNumbers.Contains(l.AreaNumber) &&
                locationKeys.Select(k => k.RowNumber).Contains(l.RowNumber) &&
                locationKeys.Select(k => k.MartyrNumber).Contains(l.MartyrNumber)))
                .ToDictionary(l => (l.AreaNumber, l.RowNumber, l.MartyrNumber));

            var existingCustomers = (await _unitOfWork.AccountRepository.FindAsync(c => customerPhones.Contains(c.PhoneNumber)))
                .ToDictionary(c => c.PhoneNumber);

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    foreach (var martyrGraveDto in martyrGraveList)
                    {
                        // Validate and look up area and location
                        if (!areas.TryGetValue(martyrGraveDto.AreaNumber, out var area) ||
                            !locations.TryGetValue((martyrGraveDto.AreaNumber, martyrGraveDto.RowNumber, martyrGraveDto.MartyrNumber), out var location))
                        {
                            // Log or handle missing area/location
                            continue;
                        }

                        var customerPhone = martyrGraveDto.Customer.Phone;
                        var customerUserName = martyrGraveDto.Customer.UserName;
                        if (string.IsNullOrEmpty(customerPhone) || string.IsNullOrEmpty(customerUserName))
                        {
                            // Log or skip missing mandatory data
                            continue;
                        }

                        string? password = null;
                        if (!existingCustomers.TryGetValue(customerPhone, out var existingCustomer))
                        {
                            // Create new account
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
                                CustomerCode = GenerateCustomerCode(customerUserName, customerPhone),
                                CreateAt = DateTime.Now,
                                HashedPassword = await HashPassword(password)
                            };
                            // Lưu vào database
                            await _unitOfWork.AccountRepository.AddAsync(account);
                            await _unitOfWork.SaveAsync(); // Lưu để AccountId được gán

                            // Thêm vào existingCustomers
                            existingCustomers[customerPhone] = account;
                        }

                        // Generate martyr code and skip if duplicate
                        var martyrCode = GenerateMartyrCode(location.AreaNumber, location.RowNumber, location.MartyrNumber);
                        if (newMartyrGraves.Any(m => m.MartyrCode == martyrCode) ||
                            (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrCode == martyrCode)).Any())
                        {
                            continue;
                        }

                        // Create martyr grave
                        var martyrGrave = new MartyrGrave
                        {
                            AreaId = area.AreaId,
                            LocationId = location.LocationId,
                            MartyrCode = martyrCode,
                            Status = 1,
                            AccountId = existingCustomers[customerPhone].AccountId
                        };
                        // Lưu vào database
                        await _unitOfWork.MartyrGraveRepository.AddAsync(martyrGrave);
                        await _unitOfWork.SaveAsync(); // Lưu để AccountId được gán

                        // Add martyr grave information
                        newMartyrGraveInfos.AddRange(martyrGraveDto.Informations.Select(info => new MartyrGraveInformation
                        {
                            MartyrId = martyrGrave.MartyrId, // Will be updated after insertion
                            Name = info.Name,
                            NickName = info.NickName,
                            Position = info.Position,
                            Medal = info.Medal,
                            HomeTown = info.HomeTown,
                            DateOfBirth = info.DateOfBirth,
                            DateOfSacrifice = info.DateOfSacrifice
                        }));

                        // Add grave images
                        newGraveImages.AddRange(martyrGraveDto.Image.Select(img => new GraveImage
                        {
                            MartyrId = martyrGrave.MartyrId, // Will be updated after insertion
                            UrlPath = img.UrlPath
                        }));

                        // Collect output details
                        outputList.Add((customerPhone, password));
                    }

                    // Bulk insert new data
                    if (newAccounts.Any()) await _unitOfWork.AccountRepository.AddRangeAsync(newAccounts);
                    if (newMartyrGraves.Any()) await _unitOfWork.MartyrGraveRepository.AddRangeAsync(newMartyrGraves);
                    if (newMartyrGraveInfos.Any()) await _unitOfWork.MartyrGraveInformationRepository.AddRangeAsync(newMartyrGraveInfos);
                    if (newGraveImages.Any()) await _unitOfWork.GraveImageRepository.AddRangeAsync(newGraveImages);

                    await _unitOfWork.SaveAsync();
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
                        // Validate and map data
                        if (int.TryParse(worksheet.Cells[row, 8].Text, out var areaNumber) &&
                            int.TryParse(worksheet.Cells[row, 9].Text, out var rowNumber) &&
                            int.TryParse(worksheet.Cells[row, 10].Text, out var martyrNumber) &&
                            !string.IsNullOrWhiteSpace(worksheet.Cells[row, 11].Text) &&
                            !string.IsNullOrWhiteSpace(worksheet.Cells[row, 12].Text))
                        {
                            var martyrGraveDto = new MartyrGraveDtoRequest
                            {
                                AreaNumber = areaNumber,
                                RowNumber = rowNumber,
                                MartyrNumber = martyrNumber,
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
                        else
                        {
                            // Log invalid row
                            Console.WriteLine($"Invalid data at row {row}");
                        }
                    }

                    // Process the entire list in batches
                    if (martyrGraveList.Any())
                    {
                        const int batchSize = 100; // Define batch size
                        for (int i = 0; i < martyrGraveList.Count; i += batchSize)
                        {
                            var batch = martyrGraveList.Skip(i).Take(batchSize).ToList();
                            var (status, result, phonePasswordPairs) = await CreateMartyrGraveListAsyncV4(batch);

                            if (status)
                            {
                                outputList.AddRange(phonePasswordPairs);
                            }
                            else
                            {
                                // Log issues for the current batch
                                Console.WriteLine($"Batch {i / batchSize + 1} failed: {result}");
                            }
                        }

                        // Export the phone and password data to an output Excel file
                        var outputFilePath = ExportPhoneAndPasswordsToExcel(outputList, filePath);
                        return (true, $"Data processed successfully. Exported to {outputFilePath}");
                    }
                    else
                    {
                        return (false, "No valid data found in the Excel file.");
                    }
                }
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

        public async Task<(List<MaintenanceHistoryDtoResponse> maintenanceHistory, int totalPage)> GetMaintenanceHistoryInMartyrGrave(int customerId, int martyrGraveId, int taskType, int pageIndex, int pageSize)
        {
            try
            {
                var maintenanceHistoryDtoResponses = new List<MaintenanceHistoryDtoResponse>();
                var martyrGrave = (await _unitOfWork.MartyrGraveRepository.GetAsync(m => m.MartyrId == martyrGraveId, includeProperties: "Account")).FirstOrDefault();
                if (martyrGrave == null || martyrGrave.Account.AccountId != customerId)
                {
                    return (maintenanceHistoryDtoResponses, 0);
                }
                int totalTask = 0;
                int totalPage = 0;
                if (taskType == 1)
                {
                    //int totalTask = (await _unitOfWork.TaskRepository.GetAsync(s => s.OrderDetail.MartyrGrave.MartyrId == martyrGraveId, includeProperties: "OrderDetail.MartyrGrave")).Count();
                    //totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);
                    IEnumerable<StaffTask> tasks = (await _unitOfWork.TaskRepository.GetAsync(t => t.OrderDetail.MartyrGrave.MartyrId == martyrGraveId, includeProperties: "OrderDetail.Service,OrderDetail.MartyrGrave,OrderDetail.Order.Account,Account")).OrderByDescending(t => t.StartDate);
                    // Tính tổng số Task và số trang
                    totalTask = tasks.Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                    // Phân trang sau khi sắp xếp
                    tasks = tasks.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    if (tasks != null)
                    {
                        foreach (var item in tasks)
                        {
                            var taskResponse = new MaintenanceHistoryDtoResponse
                            {
                                CustomerName = item.OrderDetail.Order.Account.FullName,
                                CustomerPhone = item.OrderDetail.Order.Account.PhoneNumber,
                                ServiceName = item.OrderDetail.Service.ServiceName,
                                MartyrCode = item.OrderDetail.MartyrGrave.MartyrCode,
                                EndDate = DateOnly.FromDateTime(item.EndDate),
                                Description = item.Description,
                                StaffName = item.Account.FullName,
                                StaffPhone = item.Account.PhoneNumber,
                                Status = item.Status
                            };
                            maintenanceHistoryDtoResponses.Add(taskResponse);
                        }
                    }
                }
                else if (taskType == 2)
                {
                    //int totalTask = (await _unitOfWork.AssignmentTaskRepository.GetAsync(t => t.Service_Schedule.MartyrGrave.MartyrId == martyrGraveId, includeProperties: "Service_Schedule.MartyrGrave")).Count();
                    //totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);
                    IEnumerable<AssignmentTask> tasks = (await _unitOfWork.AssignmentTaskRepository.GetAsync(t => t.Service_Schedule.MartyrGrave.MartyrId == martyrGraveId, includeProperties: "Service_Schedule.Service,Service_Schedule.MartyrGrave,Service_Schedule.Account,Account")).OrderByDescending(t => t.CreateAt);
                    // Tính tổng số Task và số trang
                    totalTask = tasks.Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                    // Phân trang sau khi sắp xếp
                    tasks = tasks.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    if (tasks != null)
                    {
                        foreach (var item in tasks)
                        {
                            var scheduleStaff = new MaintenanceHistoryDtoResponse
                            {
                                CustomerName = item.Service_Schedule.Account.FullName,
                                CustomerPhone = item.Service_Schedule.Account.PhoneNumber,
                                ServiceName = item.Service_Schedule.Service.ServiceName,
                                MartyrCode = item.Service_Schedule.MartyrGrave.MartyrCode,
                                EndDate = DateOnly.FromDateTime(item.EndDate),
                                Description = item.Description,
                                StaffName = item.Account.FullName,
                                StaffPhone = item.Account.PhoneNumber,
                                Status = item.Status
                            };
                            maintenanceHistoryDtoResponses.Add(scheduleStaff);
                        }
                    }
                }
                else if (taskType == 3)
                {
                    //int totalTask = (await _unitOfWork.RequestTaskRepository.GetAsync(t => t.RequestCustomer.MartyrGrave.MartyrId == martyrGraveId, includeProperties: "RequestCustomer.MartyrGrave")).Count();
                    //totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);
                    IEnumerable<RequestTask> tasks = (await _unitOfWork.RequestTaskRepository.GetAsync(t => t.RequestCustomer.MartyrGrave.MartyrId == martyrGraveId, includeProperties: "RequestCustomer.MartyrGrave,RequestCustomer.RequestType,Account,RequestCustomer.Account,Account")).OrderByDescending(t => t.CreateAt);
                    // Tính tổng số Task và số trang
                    totalTask = tasks.Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                    // Phân trang sau khi sắp xếp
                    tasks = tasks.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    if (tasks != null)
                    {
                        foreach (var item in tasks)
                        {
                            var scheduleStaff = new MaintenanceHistoryDtoResponse
                            {
                                CustomerName = item.RequestCustomer.Account.FullName,
                                CustomerPhone = item.RequestCustomer.Account.PhoneNumber,
                                ServiceName = item.RequestCustomer.RequestType.TypeName,
                                MartyrCode = item.RequestCustomer.MartyrGrave.MartyrCode,
                                EndDate = item.EndDate,
                                Description = item.Description,
                                StaffName = item.Account.FullName,
                                StaffPhone = item.Account.PhoneNumber,
                                Status = item.Status
                            };
                            maintenanceHistoryDtoResponses.Add(scheduleStaff);
                        }
                    }
                }
                else
                {
                    return (maintenanceHistoryDtoResponses, 0);
                }


                return (maintenanceHistoryDtoResponses, totalPage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
