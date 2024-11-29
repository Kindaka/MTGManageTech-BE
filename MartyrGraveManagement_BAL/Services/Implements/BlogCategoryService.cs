using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.BlogCategoryDTOs;
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
    public class BlogCategoryService : IBlogCategoryService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BlogCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(bool status, string message)> CreateBlogCategoryAsync(BlogCategoryDtoRequest blogCategoryDto)
        {
            try
            {
                var blogCategory = new BlogCategory
                {
                    BlogCategoryName = blogCategoryDto.BlogCategoryName,
                    Description = blogCategoryDto.Description,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Status = true
                };

                await _unitOfWork.BlogCategoryRepository.AddAsync(blogCategory);
                await _unitOfWork.SaveAsync();
                return (true, "Tạo danh mục blog thành công.");
            }
            catch (Exception ex)
            {
                return (false, $"Có lỗi xảy ra khi tạo danh mục blog: {ex.Message}");
            }
        }

        public async Task<BlogCategoryDtoResponse> GetBlogCategoryByIdAsync(int id)
        {
            var blogCategories = await _unitOfWork.BlogCategoryRepository.GetAsync(
                filter: b => b.HistoryId == id,
                includeProperties: "Blogs,Blogs.Account"
            );

            var blogCategory = blogCategories.FirstOrDefault(); // Lấy kết quả đầu tiên từ danh sách

            if (blogCategory == null)
            {
                return null;
            }

            var blogCategoryResponse = new BlogCategoryDtoResponse
            {
                HistoryId = blogCategory.HistoryId,
                BlogCategoryName = blogCategory.BlogCategoryName,
                Description = blogCategory.Description,
                CreatedDate = blogCategory.CreatedDate,
                UpdatedDate = blogCategory.UpdatedDate,
                Status = blogCategory.Status,
                Blogs = blogCategory.Blogs?.Select(blog => new BlogDtoResponse
                {
                    BlogId = blog.BlogId,
                    AccountId = blog.AccountId,
                    HistoryId = blog.HistoryId,
                    FullName = blog.Account?.FullName,
                    BlogName = blog.BlogName,
                    CreateDate = blog.CreateDate,
                    UpdateDate = blog.UpdateDate,
                    Status = blog.Status
                }).ToList()
            };

            return blogCategoryResponse;
        }


        public async Task<List<BlogCategoryDtoResponse>> GetAllBlogCategoriesAsync()
        {
            var blogCategories = await _unitOfWork.BlogCategoryRepository.GetAllAsync(includeProperties: "Blogs,Blogs.Account");
            return blogCategories.Select(b => new BlogCategoryDtoResponse
            {
                HistoryId = b.HistoryId,
                BlogCategoryName = b.BlogCategoryName,
                Description = b.Description,
                CreatedDate = b.CreatedDate,
                UpdatedDate = b.UpdatedDate,
                Status = b.Status,
                Blogs = b.Blogs?.Select(blog => new BlogDtoResponse
                {
                    BlogId = blog.BlogId,
                    AccountId = blog.AccountId,
                    HistoryId = blog.HistoryId,
                    FullName = blog.Account?.FullName,
                    BlogName = blog.BlogName,
                    CreateDate = blog.CreateDate,
                    UpdateDate = blog.UpdateDate,
                    Status = blog.Status
                }).ToList()
            }).ToList();
        }



        public async Task<(bool status, string message)> UpdateBlogCategoryAsync(int id, BlogCategoryDtoRequest blogCategoryDto)
        {
            var blogCategory = await _unitOfWork.BlogCategoryRepository.GetByIDAsync(id);
            if (blogCategory == null)
            {
                return (false, "Không tìm thấy danh mục blog.");
            }

            try
            {
                blogCategory.BlogCategoryName = blogCategoryDto.BlogCategoryName;
                blogCategory.Description = blogCategoryDto.Description;
                blogCategory.UpdatedDate = DateTime.Now;
                blogCategory.Status = blogCategoryDto.Status;

                await _unitOfWork.BlogCategoryRepository.UpdateAsync(blogCategory);
                await _unitOfWork.SaveAsync();
                return (true, "Cập nhật danh mục blog thành công.");
            }
            catch (Exception ex)
            {
                return (false, $"Có lỗi xảy ra khi cập nhật danh mục blog: {ex.Message}");
            }
        }



        public async Task<(bool status, string message)> DeleteBlogCategoryAsync(int id)
        {
            var blogCategory = await _unitOfWork.BlogCategoryRepository.GetByIDAsync(id);
            if (blogCategory == null)
            {
                return (false, "Không tìm thấy danh mục blog.");
            }

            try
            {
               await _unitOfWork.BlogCategoryRepository.DeleteAsync(blogCategory);
                await _unitOfWork.SaveAsync();
                return (true, "Xóa danh mục blog thành công.");
            }
            catch (Exception ex)
            {
                return (false, $"Có lỗi xảy ra khi xóa danh mục blog: {ex.Message}");
            }
        }


        public async Task<(List<BlogCategoryDtoResponse> blogCategoryList, int totalPage)> GetAllBlogCategoriesByStatusTrueAsync(int pageIndex = 1, int pageSize = 5)
        {
            // Lấy tất cả các BlogCategory có Status = true với phân trang
            var blogCategories = await _unitOfWork.BlogCategoryRepository.GetAsync(
                filter: b => b.Status == true,
                includeProperties: "Blogs,Blogs.Account",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            // Đếm tổng số BlogCategory có Status = true
            var totalBlogCategories = (await _unitOfWork.BlogCategoryRepository.GetAsync(
                filter: b => b.Status == true)).Count();

            // Tính tổng số trang
            var totalPage = (int)Math.Ceiling(totalBlogCategories / (double)pageSize);

            // Chuyển đổi dữ liệu sang danh sách BlogCategoryDtoResponse
            var blogCategoryDtos = blogCategories.Select(b => new BlogCategoryDtoResponse
            {
                HistoryId = b.HistoryId,
                BlogCategoryName = b.BlogCategoryName,
                Description = b.Description,
                CreatedDate = b.CreatedDate,
                UpdatedDate = b.UpdatedDate,
                Status = b.Status,
                Blogs = b.Blogs?.Where(blog => blog.Status == true).Select(blog => new BlogDtoResponse
                {
                    BlogId = blog.BlogId,
                    AccountId = blog.AccountId,
                    HistoryId = blog.HistoryId,
                    FullName = blog.Account?.FullName,
                    BlogName = blog.BlogName,
                    CreateDate = blog.CreateDate,
                    UpdateDate = blog.UpdateDate,
                    Status = blog.Status
                }).ToList()
            }).ToList();

            return (blogCategoryDtos, totalPage);
        }


        public async Task<bool> UpdateBlogCategoryStatusAsync(int historyId, bool newStatus)
        {
            try
            {
                // Lấy BlogCategory theo HistoryId
                var blogCategory = await _unitOfWork.BlogCategoryRepository.GetByIDAsync(historyId);

                if (blogCategory == null)
                {
                    return false; // Không tìm thấy BlogCategory
                }

                // Cập nhật trạng thái
                blogCategory.Status = newStatus;

                // Lưu thay đổi
                await _unitOfWork.BlogCategoryRepository.UpdateAsync(blogCategory);

                return true;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (ghi log nếu cần)
                Console.WriteLine($"Lỗi khi cập nhật trạng thái: {ex.Message}");
                return false;
            }
        }


    }
}
