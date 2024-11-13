using MartyrGraveManagement_BAL.ModelViews.BlogCategoryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IBlogCategoryService
    {
        Task<(bool status, string message)> DeleteBlogCategoryAsync(int id);
        Task<(bool status, string message)> UpdateBlogCategoryAsync(int id, BlogCategoryDtoRequest blogCategoryDto);
        Task<List<BlogCategoryDtoResponse>> GetAllBlogCategoriesAsync();
        Task<BlogCategoryDtoResponse> GetBlogCategoryByIdAsync(int id);
        Task<(bool status, string message)> CreateBlogCategoryAsync(BlogCategoryDtoRequest blogCategoryDto);
        Task<List<BlogCategoryDtoResponse>> GetAllBlogCategoriesByStatusTrueAsync();
        Task<bool> UpdateBlogCategoryStatusAsync(int historyId, bool newStatus);


    }
}
