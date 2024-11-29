using MartyrGraveManagement_BAL.ModelViews.BlogDTOs;
using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IBlogService
    {
        Task<List<DetailedBlogResponseDTO>> GetBlogByAccountId(int accountId);
        Task<string> CreateBlogAsync(CreateBlogDTORequest request, int accountId);
        Task<(IEnumerable<BlogDTO> blogList, int totalPage)> GetAllBlogsAsync(int managerId, int pageIndex, int pageSize, DateTime Date);
        Task<(List<BlogDTO> blogList, int totalPage)> GetAllBlogsWithStatusTrueAsync(int pageIndex = 1, int pageSize = 5);
        Task<BlogDTO> GetBlogByIdAsync(int blogId);
        Task<string> UpdateBlogAsync(int blogId, CreateBlogDTORequest request);
        Task<string> UpdateBlogStatusAsync(int blogId, bool status);
    }
}
