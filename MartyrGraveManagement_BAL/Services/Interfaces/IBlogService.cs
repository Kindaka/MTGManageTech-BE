using MartyrGraveManagement_BAL.ModelViews.BlogDTOs;
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
        Task<string> CreateBlogAsync(CreateBlogDTORequest request);
        Task<List<BlogDTO>> GetAllBlogsAsync();
        Task<BlogDTO> GetBlogByIdAsync(int blogId);
        Task<string> UpdateBlogAsync(int blogId, CreateBlogDTORequest request);
        Task<string> UpdateBlogStatusAsync(int blogId, bool status);
    }
}
